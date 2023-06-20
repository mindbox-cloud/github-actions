using CommandLine;

namespace Mindbox.ValidateVaultScheme;

public class CommandLineArguments
{
    [Option("token",
        Required = true,
        HelpText = "Github token")]
    public string Token { get; set; } = null!;

    [Option("organization",
        Required = true,
        HelpText = "Orgranization name (e.g. mindbox-cloud)")]
    public string OrgName { get; set; }
    
    [Option("repository",
        Required = true,
        HelpText = "Orgranization name (e.g. nexus)")]
    public string RepoName { get; set; }
    
    [Option("oldfile-local-path",
        Required = false,
        HelpText = "Local path to old rendered file (yaml)")]
    public string? OldFileLocalPath { get; set; }
    
    [Option("newfile-local-path",
        Required = false,
        HelpText = "Local path to new rendered file (yaml)")]
    public string? NewFileLocalPath { get; set; }
    
    [Option("file-source-path",
        Required = false,
        HelpText = "Source path to template file (yaml.gotmpl)")]
    public string FileSourcePath { get; set; }

    [Option("allow-breaking-changes",
        Required = true,
        HelpText = "New file version path (local)")]
    public bool AllowBreakingChanges { get; set; }
    
    [Option("pull-request-id",
        Required = true,
        HelpText = "Pull request id")]
    public int PullRequestId { get; set; }
}