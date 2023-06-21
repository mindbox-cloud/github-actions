using System.Text;
using System.Text.Json;

namespace Mindbox.ValidateVaultScheme;

public static class GithubNotifier
{
    public static async Task SendResultAsync(CommandLineArguments commandLineArguments, CompareResult result)
    {
        var org = commandLineArguments.OrgName;
        var repo = commandLineArguments.RepoName;
        var issue = commandLineArguments.PullRequestId;
        var url = $"https://api.github.com/repos/{org}/{repo}/issues/{issue}/comments";

        var commentBody = CreateCommentBody(commandLineArguments, result);
        
        var payload = new
        {
            body = commentBody
        };

        var serializedPayload = JsonSerializer.Serialize(payload);

        var httpContent = new StringContent(serializedPayload, Encoding.UTF8, "application/json");

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "My-App-Name");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {commandLineArguments.Token}");

        var response = await client.PostAsync(url, httpContent);

        response.EnsureSuccessStatusCode();
    }

    public static string CreateCommentBody(CommandLineArguments commandLineArguments,CompareResult result)
    {
        var commentBodyBuilder = new StringBuilder();
        
        if (!result.HasChanges)
        {
            commentBodyBuilder.AppendLine(
                $"Secrets list file {commandLineArguments.FileSourcePath} is effectively unchanged");
        }
        else
        {
            commentBodyBuilder.AppendLine(result.PullRequestMessage);

            commentBodyBuilder.AppendLine($@"
```diff
{string.Join("\n", result.DeletedKeys.Select(key => $"-{key}"))}
{string.Join("\n", result.AddedKeys.Select(key => $"+{key}"))}
```
");
            
            if (result.HasBreakingChanges)
            {
                commentBodyBuilder.AppendLine("This PR contains breaking changes");
                if (commandLineArguments.AllowBreakingChanges)
                {
                    commentBodyBuilder.AppendLine("This check **DOES NOT** stop PR from being merged");
                }
                else
                {
                    commentBodyBuilder.AppendLine(
                        "Please add `allow vault breaking changes` label to this PR if you are sure these changes are OK");
                }
            }
        }

        return commentBodyBuilder.ToString();
    }
}