namespace ATM.Models;
public class Template
{

    public string ApplyOrder { get; set; } = "";
    public bool ReApplyAllOnPreBuild { get; set; }

    public string BuildServerEnviromentVariable = "CI";

    public string BranchesRegexFile { get; set; } = VersioningManager.GitVersionConfigFile;

    public string VersioningTool { get; set; } = VersioningManager.GitVersion;


    [JsonIgnore]
    public DirectoryInfo? TemplateDir { get; set; }
    [JsonIgnore]
    public string Name => TemplateDir?.Name ?? "";

    [JsonIgnore]
    public const string SettingFile = "atm.json";


}