using CommandLine;
using Mindbox.ValidateVaultScheme;

var commandLineArgs = Parser.Default.ParseArguments<CommandLineArguments>(args).Value;

// var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
// var commandLineArgs = new CommandLineArguments
// {
//     AllowBreakingChanges = true,
//     OldFileLocalPath = "TestOldFile.yaml",
//     FileSourcePath = "helmfile/values/vault-secrets.yaml.gotmpl",
//     NewFileLocalPath = "TestNewFile.yaml",
//     OrgName = "mindbox-cloud",
//     RepoName = "DirectCRM",
//     Token = token,
//     PullRequestId = 41466 // https://github.com/mindbox-cloud/DirectCRM/pull/41466
// };

var result = FileComparer.Compare(
    commandLineArgs.OldFileLocalPath, 
    commandLineArgs.NewFileLocalPath,
    commandLineArgs.FileSourcePath);
Console.WriteLine("Files compared");
    
await GithubNotifier.SendResultAsync(commandLineArgs, result);
Console.WriteLine("GitHub comment sent");

var exitCode = result.HasBreakingChanges && !commandLineArgs.AllowBreakingChanges
    ? 1
    : 0;

Console.WriteLine($"Exiting with code {exitCode}");

Environment.Exit(exitCode);