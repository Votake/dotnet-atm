
namespace ATM.Models;
public class VersioningManager
{
    /// <summary>
    /// Generates gitversion.yml (if not exist) into the given directory
    /// </summary>
    /// <param name="dirInfo">the directory to save the gitversion.yml in it</param>
    public void GenerateGitVersionYaml(DirectoryInfo dirInfo)
    {
        string filePath = Helper.Combine(dirInfo, GitVersionConfigFile);
        if (!File.Exists(filePath))
        {
            Helper.WriteYAML(new GitVersionConfigYaml(), filePath);
        }
        
    }


    // constants
    public const string GitVersion = nameof(GitVersion);
    public const string GitVersionConfigFile = "gitversion.yml";


}