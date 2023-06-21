using CommandLine;

namespace Mindbox.ValidateVaultScheme;

public record CommandLineArguments
{
    [Option("token",
        Required = true,
        HelpText = "Github token")]
    public string Token { get; set; } = null!;

    [Option("organization",
        Required = true,
        HelpText = "Organization name (e.g. mindbox-cloud)")]
    public string OrgName { get; set; } = null!;

    [Option("repository",
        Required = true,
        HelpText = "Repository name (e.g. nexus)")]
    public string RepoName { get; set; } = null!;
    
    [Option("pull-request-id",
        Required = true,
        HelpText = "Pull request id")]
    public int PullRequestId { get; set; }

    [Option("oldfile-local-path",
        Required = true,
        HelpText = "Local path to old rendered file (yaml). If file does not exist, we treat secrets as created.")]
    public string OldFileLocalPath { get; set; } = null!;

    [Option("newfile-local-path",
        Required = true,
        HelpText = "Local path to new rendered file (yaml). If file does not exist, we treat secrets as deleted.")]
    public string NewFileLocalPath { get; set; } = null!;

    [Option("file-source-path",
        Required = true,
        HelpText = "Source path to template file (yaml.gotmpl)")]
    public string FileSourcePath { get; set; } = null!;

    /// <summary>
    ///     If we use bool, --allow-breaking-changes false evaluates to true (parameter is set, value is irrelevant)
    /// </summary>
    [Option("allow-breaking-changes",
        Required = true,
        HelpText = "Fail process (exit code 1) if any keys were deleted from new version of the file.")]
    public bool? AllowBreakingChanges { get; set; }
}