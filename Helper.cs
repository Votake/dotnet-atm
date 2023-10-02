using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Reflection;

namespace ATM
{
    public static class Helper
    {
        public static string Combine(DirectoryInfo? baseDir = null, params string[] combine)
        {
            string result = baseDir?.FullName ?? string.Empty;

            if (string.IsNullOrEmpty(result)) result = Directory.GetCurrentDirectory();

            foreach (var dir in combine)
            {
                result = Path.Combine(result, dir.TrimStart('\\', '/'));
            }
            return result;
        }

        public static string ToFullPath(string path)
        {
            path = ReplaceSlashes(path);
            path = GetEnviromentVar(path);

            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }
            return path;
        }

        private static string ReplaceSlashes(string path)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            return path;

        }

        public static void CreateDir(DirectoryInfo parent, params string[] combines)
        {
            foreach (string dirName in combines)
            {
                parent = new DirectoryInfo(Combine(parent, dirName));
                if (!parent.Exists) { parent.Create(); }
                FileInfo atmFile = new FileInfo(Path.Combine(parent.FullName, ".atm"));
                if (!atmFile.Exists) { atmFile.Create(); }
                atmFile.Attributes = FileAttributes.Hidden;
            }
        }

        private static string GetEnviromentVar(string pathWithEnvVar)
        {
            // Check if the path starts with "%" or "$"
            if (pathWithEnvVar.StartsWith("%") || pathWithEnvVar.StartsWith("$"))
            {
                // Find the end of the environment variable
                int end = pathWithEnvVar.IndexOfAny(new[] { '%', '/', '\\' }, 1);

                if (end >= 0)
                {
                    // Extract the environment variable
                    string envVar = pathWithEnvVar.Substring(0, end);

                    // Remove any initial "\" or "/" in position 0 and 1 of the envVar
                    envVar = envVar.TrimStart('\\', '/');

                    // Get the remaining part of the path
                    string remainingPath = pathWithEnvVar.Substring(end + 1);

                    // Remove any initial "\" or "/" in position 0 and 1 of the remainingPath
                    remainingPath = remainingPath.TrimStart('\\', '/');

                    // Convert the environment variable to the full path
                    string fullPath = Environment.ExpandEnvironmentVariables(envVar);

                    // Combine the full path with the remaining part of the path
                    pathWithEnvVar = Path.Combine(fullPath, remainingPath);

                }
            }

            return pathWithEnvVar;
        }

        private static IEnumerable<string> SearchFiles(string searchPattern, string path)
        {
            path = ToFullPath(path);
            return Directory.EnumerateFiles(Path.GetDirectoryName(path) ?? path, Path.GetFileName(path), SearchOption.AllDirectories);
        }


        public static string? FindGitDirectory(string startDirectory)
        {
            string? currentDirectory = ToFullPath(startDirectory);

            while (currentDirectory != null)
            {
                string gitDirectory = Path.Combine(currentDirectory, ".git");

                if (Directory.Exists(gitDirectory))
                {
                    return currentDirectory;
                }

                currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            }

            return null;
        }

        public static void CreateScriptFiles(string directoryPath)
        {
            string toolName = Assembly.GetExecutingAssembly().GetName().Name?? "";
            string toolCommand = "dotnet-atm";

            if (string.IsNullOrEmpty(toolName))
                ExitError("Error creating init script files");

            directoryPath = ToFullPath(directoryPath);

            // Create init.bat file
            string batFilePath = Path.Combine(directoryPath, "init.bat");
            string[] batLines = {
        "@echo off",
        $"dotnet tool list -g | findstr /C:\"{toolName}\" 1>nul",
        $"if %errorlevel% neq 0 (",
        $"    echo {toolName} is not installed. Installing...",
        $"    dotnet tool install --global {toolName}",
        ")",
        $"echo Running {toolCommand} init...",
        $"{toolCommand} init %~dp0",
        "pause"
    };
            File.WriteAllLines(batFilePath, batLines);

            // Create init.sh file
            string shFilePath = Path.Combine(directoryPath, "init.sh");
            string[] shLines = {
        "#!/bin/bash",
        $"if ! dotnet tool list -g | grep -q '{toolName}'; then",
        $"    echo \"{toolName} is not installed. Installing...\"",
        $"    dotnet tool install --global {toolName}",
        "fi",
        $"echo \"Running {toolCommand} init...\"",
        $"{toolCommand} init \"$(dirname \"$(readlink -f \"$0\")\")\"",
        "read -p \"Press enter to continue\""
    };
            File.WriteAllLines(shFilePath, shLines);

            Output("Init script files created");
        }


        public static void CopyDirectoryFiles(string source, string target)
        {
            source = ToFullPath(source);
            target = ToFullPath(target);

            var directories = Directory.GetDirectories(source, "*", SearchOption.AllDirectories)
                                       .Where(dir => !dir.EndsWith(".atm"));

            foreach (string dirPath in directories)
            {
                Directory.CreateDirectory(dirPath.Replace(source, target));
            }

            var files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories)
                                 .Where(file => Path.GetExtension(file) != ".atm");

            foreach (string filePath in files)
            {
                string targetPath = filePath.Replace(source, target);
                if (!File.Exists(targetPath) || !IsEqual(filePath, targetPath))
                {
                    File.Copy(filePath, targetPath, true);
                }
            }
        }

        public static bool IsEqual(string filePath1, string filePath2)
        {
            filePath1 = ToFullPath(filePath1);
            filePath2 = ToFullPath(filePath2);

            byte[] file1 = File.ReadAllBytes(filePath1);
            byte[] file2 = File.ReadAllBytes(filePath2);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash1 = sha256.ComputeHash(file1);
                byte[] hash2 = sha256.ComputeHash(file2);

                return hash1.SequenceEqual(hash2);
            }
        }

        public static Dictionary<string, string> GetPackageSources(string nugetConfigFilePath)
        {
            nugetConfigFilePath = GetEnviromentVar(nugetConfigFilePath);

            // Load the XML document
            XDocument doc = XDocument.Load(nugetConfigFilePath);

            // Get all package sources
            var packageSources = doc.Root?.Element("packageSources");

            // Create a dictionary to store the package sources
            Dictionary<string, string> packageSourceDictionary = new Dictionary<string, string>();

            if (packageSources == null) { return packageSourceDictionary; }

            // Loop through each add element in packageSources
            foreach (var addElement in packageSources.Elements("add"))
            {
                // Get the key and value attributes
                var key = addElement?.Attribute("key")?.Value;
                var value = addElement?.Attribute("value")?.Value;

                // Add the key and value to the dictionary
                if (key != null && value != null)
                {
                    packageSourceDictionary.Add(key, value);
                }

            }

            return packageSourceDictionary;
        }

        public static T? ReadJson<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<T>(json);
                return config;
            }
            else return default;

        }

        public static void WriteJson<T>(T config, string filePath)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static T? ReadYAML<T>(string filePath)
        {
            // Read from YAML file
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<T>(File.ReadAllText(filePath));
        }

        public static void WriteYAML<T>(T config, string filePath)
        {
            // Write to YAML file
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(config);
            File.WriteAllText(filePath, yaml);
        }

        public static void ExitError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("\t" + error);
            Console.WriteLine();
            Console.ResetColor();
            Environment.Exit(1);
        }
        public static void Output(string text, ConsoleColor consoleColor = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine("\t" + text.Replace(":'", ":" + Environment.NewLine + "'"));
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
