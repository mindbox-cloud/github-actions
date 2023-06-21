using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.ValidateVaultScheme.Tests;

[TestClass]
public class GithubNotifierTests
{
    [TestMethod]
    public void CreateCommentBody_WhenNoChanges_AppliesCorrectTemplate()
    {
        var args = new CommandLineArguments
        {
            FileSourcePath = "test/path",
            AllowBreakingChanges = false
        };

        var result = CompareResult.FileCreated("PullRequestMessage", new List<string>());

        var body = GithubNotifier.CreateCommentBody(args, result);
        StringAssert.Contains(body, "Secrets list file test/path is effectively unchanged");
    }

    [TestMethod]
    public void CreateCommentBody_WhenHasBreakingChanges_AppliesCorrectTemplate()
    {
        var args = new CommandLineArguments
        {
            FileSourcePath = "test/path",
            AllowBreakingChanges = false
        };

        var result = CompareResult.FileChanged(
            new[] { "deletedKey" }, 
            new[] { "addedKey" }, 
            true, 
            "PullRequestMessage");

        var body = GithubNotifier.CreateCommentBody(args, result);
        StringAssert.Contains(body, "PullRequestMessage");
        StringAssert.Contains(body, "-deletedKey");
        StringAssert.Contains(body, "+addedKey");
        StringAssert.Contains(body, "This PR contains breaking changes");
        StringAssert.Contains(body, "Please add `allow vault breaking changes` label");
    }

    [TestMethod]
    public void CreateCommentBody_WhenHasBreakingChangesButAllowed_AppliesCorrectTemplate()
    {
        var args = new CommandLineArguments
        {
            FileSourcePath = "test/path",
            AllowBreakingChanges = true
        };

        var result = CompareResult.FileChanged(
            new[] { "deletedKey" }, 
            new[] { "addedKey" }, 
            true, 
            "PullRequestMessage");

        var body = GithubNotifier.CreateCommentBody(args, result);
        StringAssert.Contains(body, "PullRequestMessage");
        StringAssert.Contains(body, "-deletedKey");
        StringAssert.Contains(body, "+addedKey");
        StringAssert.Contains(body, "This PR contains breaking changes");
        StringAssert.Contains(body, "This check **DOES NOT** stop PR from being merged");
    }
}