namespace ATM.Models;
public class Config
{
    public string TemplatesRoot { get; set; } = "";

    [JsonIgnore]
    public DirectoryInfo TemplatesRootInfo => new DirectoryInfo(TemplatesRoot);
}