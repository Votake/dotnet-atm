using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ATM.Models;

public class GitVersionConfigYaml
{
    public string Mode { get; set; } = "ContinuousDeployment";

    [YamlMember(Alias = "continuous-delivery-fallback-tag",ApplyNamingConventions =false )]
    public string ContinuousDeliveryFallbackTag { get; set; } = "";

    public Dictionary<string, GitVersionBranchConfigs> branches { get; set; } = new Dictionary<string, GitVersionBranchConfigs>();

    public GitVersionConfigYaml()
    {
        branches.Add(main,new GitVersionBranchConfigs(mainRegex,mainTag));
        branches.Add(develop,new GitVersionBranchConfigs(developRegex,developTag));
        branches.Add(release, new GitVersionBranchConfigs(releaseRegex,releaseTag));
        branches.Add(feature, new GitVersionBranchConfigs(featureRegex,featureTag));
        branches.Add(hotfix, new GitVersionBranchConfigs(hotfixRegex,hotfixTag));
        branches.Add(pullRequest, new GitVersionBranchConfigs(pullRequestRegex,pullRequestTag));
        branches.Add(support, new GitVersionBranchConfigs(supportRegex,supportTag));

    }




    // constants
    public const string main = "main";
    public const string mainRegex = "main";
    public const string mainTag = "";

    public const string develop = "develop";
    public const string developRegex = "dev(elop(ment)?)?";
    public const string developTag = "2-alpha";

    public const string release = "release";
    public const string releaseRegex = "^r(eleases?)?[/-]";
    public const string releaseTag = "4-beta";

    public const string feature = "feature";
    public const string featureTag = "1-alpha.{BranchName}";
    public const string featureRegex = "^f(eatures?)?[/-]";

    public const string hotfix = "hotfix";
    public const string hotfixTag = "3-beta";
    public const string hotfixRegex = "^(hot)?fix(es)?[/-]";

    public const string pullRequest = "pull-request";
    public const string pullRequestTag = "PullRequest";
    public const string pullRequestRegex = @"^(pull|pull\-requests|pr)[/-]";

    public const string support = "support";
    public const string supportTag = "";
    public const string supportRegex = "^support[/-]";

}

public class GitVersionBranchConfigs:GitBranch
{
    public GitVersionBranchConfigs(string brachRegex = "",string preReleaseTag="") : base(brachRegex)
    {
        tag = preReleaseTag;
    }
    public string tag { get; set; } = string.Empty;
}



