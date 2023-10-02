using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.Models
{
    public class BuildServer
    {
        public string Name { get; set; } = "";
        public string Enviroment_Variable { get; set; } = "";
        public string Git_User_Email { get; set; } = "";
        public string Git_User_Name { get; set; } = "";



        // consts
        public const string AzurePipelines = nameof(AzurePipelines);
        public const string AzurePipelines_Enviroment_Variable = "TF_BUILD";
        public const string AzurePipelines_Build_Service_Name = "Azure Build Service";
        public const string AzurePipelines_Build_Service_Email = "AzureBuildService@azure.com";
    }
}
