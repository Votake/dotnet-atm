using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM
{
    [Verb("init", HelpText = "setup ATM ")]
    public class InitOptions:IVerb
    {
        [Value(0,
            HelpText = "The path to your templates root directory",
            MetaName = "TemplatesRoot")]
        public string TemplatesRoot { get; set; } = Manager.DefaultTemplatesRoot;

        public int Start() 
        {
            TemplatesRoot = Helper.ToFullPath(TemplatesRoot);

            var manager = new Manager(TemplatesRoot,true);
            Helper.Output($"ATM is initiated at the following path:'{TemplatesRoot}'",ConsoleColor.Green);
            return 0;
        }
    }

    [Verb("new",HelpText = "Creates new template in the templates root directory")]
    public class NewOptions : IVerb
    {
        [Value(0,
            HelpText = "Template Name",
            MetaName = "name",
            Default = "")]
        public string? TemplateName { get; set; }
        public int Start()
        {
            if (string.IsNullOrEmpty(TemplateName))
                Helper.ExitError("Invalid template name");

            return 0;
        }
    }


    public interface IVerb
    {
        int Start();
    }
}
