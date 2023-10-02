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

    [Verb("new",HelpText = "Creates new empty template in the templates root directory")]
    public class NewOptions : IVerb
    {
        [Value(0,
            HelpText = "Template Name",
            MetaName = "name",
            Default = "")]
        public string? TemplateName { get; set; }

        [Option(Default = false, HelpText = "The order in which this template will be applied, 0 or Shared, 1 or AfterShared, 2 or BeforeShared, 3 or WithoutShared")]
        public string ApplyOrder { get; set; } = "1";


        public int Start()
        {
            if (string.IsNullOrEmpty(TemplateName))
                Helper.ExitError("Invalid template name");
            var manager = new Manager();
            Helper.Output($"Creating:'{TemplateName}'");
            manager.CreateNewTemplate(TemplateName!);

            return 0;
        }
    }


    public interface IVerb
    {
        int Start();
    }
}
