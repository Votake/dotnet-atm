namespace ATM.Models;
public class Manager
{
    public Manager(string? templatesRoot = "", bool init = false)
    {
        if (init) Init(templatesRoot);

        Config ??= new();
        if (string.IsNullOrEmpty(templatesRoot))
        {
            if (File.Exists(ConfigFilePath))
            {
                Config = Helper.ReadJson<Config>(ConfigFilePath)?? new Config();
            }
            else
            {
                Helper.ExitError("Please use Init to set the Templates Root Path");
            }
        }
        else
        {
            if (Directory.Exists(templatesRoot))
            {
                Config.TemplatesRoot = templatesRoot;
            }
            else
            {
                Helper.ExitError($"The path '{templatesRoot}' doesn't exist");
            }
        }
        

        LoadTemplates();
    }



    public List<Template> Templates { get; set; } = new List<Template>();
    public Config Config { get; set; }



    public static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
    public static readonly string DefaultTemplatesRoot = ".\\dotnet-atm";


    public const string ToSoultionDir = nameof(ToSoultionDir);
    public const string ToProjectDir = nameof(ToProjectDir);
    public const string ToRepoRoot = nameof(ToRepoRoot);
    public const string PreBuildScripts = nameof(PreBuildScripts);
    public const string PostBuildScripts = nameof(PostBuildScripts);

    public const string Shared = nameof(Shared);
    public const string AfterShared = nameof(AfterShared);



    public void ApplyTemplate(string templateName)
    {
        // find the template
        var template = Templates.FirstOrDefault(x => x.TemplateDir?.Name.ToLower() == templateName.ToLower());
        if (template == null)
        {
            Helper.ExitError($"A template with the name '{templateName}' doesn't exist");
        }
        // Helper.CopyDirectoryFiles(baseDirectory(ToSoultionDir))

    }

    public void CreateNewTemplate(string templateName = Shared, string applyOrder = Shared, bool reApplyAllOnPreBuild = true)
    {
        if (applyOrder == Shared) { templateName = Shared; }
        var newTemplate = new Template()
        {
            TemplateDir = new DirectoryInfo(Helper.Combine(Config.TemplatesRootInfo, templateName)),
            ApplyOrder = applyOrder,
            ReApplyAllOnPreBuild = reApplyAllOnPreBuild
        };

        Helper.CreateDir(Config.TemplatesRootInfo, templateName, ToSoultionDir);
        Helper.CreateDir(Config.TemplatesRootInfo, templateName, ToRepoRoot);
        Helper.CreateDir(Config.TemplatesRootInfo, templateName, ToProjectDir);
        Helper.CreateDir(Config.TemplatesRootInfo, templateName, PreBuildScripts);
        Helper.CreateDir(Config.TemplatesRootInfo, templateName, PostBuildScripts);

        Helper.WriteJson(newTemplate, Helper.Combine(newTemplate.TemplateDir, Template.SettingFile));
        Helper.Output($"'{templateName}' created at:'{Config.TemplatesRoot}'");
    }


    private void LoadTemplates()
    {
        var templates = Config.TemplatesRootInfo.GetDirectories();
        foreach (var templateDir in templates)
        {
            var jsonFile = Path.Combine(templateDir.FullName, Template.SettingFile);
            var template = Helper.ReadJson<Template>(jsonFile);
            if (template == null) continue;
            template.TemplateDir = templateDir;
            Templates.Add(template);
        }
    }

    private void Init(string? templatesRoot = "")
    {
        Config ??= new();

        if(string.IsNullOrEmpty(templatesRoot?.Trim()))
            Helper.ExitError("Must specifiy a valid Templates Root Path");

        if (Helper.FindGitDirectory(templatesRoot!) == null)
            Helper.ExitError("The Templates Directory must be inside a git repository");

        Config.TemplatesRoot = templatesRoot!;

        Helper.WriteJson(Config, ConfigFilePath);
        Helper.Output($"config file updated at:'{ConfigFilePath}'");

        DirectoryInfo atmFolder = new DirectoryInfo(Helper.Combine(Config.TemplatesRootInfo, ".atm"));
        if (!atmFolder.Exists)
        {
            atmFolder.Create();
            atmFolder.Attributes = FileAttributes.Hidden;
            Helper.Output($".atm folder created at:'{atmFolder.FullName}'");
        }
        Helper.CreateScriptFiles(templatesRoot??"");

        CreateNewTemplate();

    }





}
