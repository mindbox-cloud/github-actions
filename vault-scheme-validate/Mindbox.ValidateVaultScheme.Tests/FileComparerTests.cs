using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.ValidateVaultScheme.Tests;

[TestClass]
public class FileComparerTests
{
    [TestMethod]
    public void Compare_WhenOldFileIsNull_ReturnsFileCreated()
    {
        using var newFile = TempFile.Create(CreateFileContent(new [] {"key1", "key2"}));

        var result = FileComparer.Compare(null, newFile.FilePath, newFile.FilePath);

        Assert.IsTrue(result.AddedKeys.SequenceEqual(new [] {"key1", "key2"}));
        Assert.IsFalse(result.DeletedKeys.Any());
        Assert.IsFalse(result.HasBreakingChanges);
        Assert.IsTrue(result.HasChanges);
        StringAssert.Contains(result.PullRequestMessage, "created");
    }

    [TestMethod]
    public void Compare_WhenNewFileIsNull_ReturnsFileDeleted()
    {
        using var oldFile = TempFile.Create(CreateFileContent(new [] {"key1", "key2"}));

        var result = FileComparer.Compare(oldFile.FilePath, null, oldFile.FilePath);

        Assert.IsTrue(result.DeletedKeys.SequenceEqual(new [] {"key1", "key2"}));
        Assert.IsFalse(result.AddedKeys.Any());
        Assert.IsTrue(result.HasBreakingChanges);
        Assert.IsTrue(result.HasChanges);
        StringAssert.Contains(result.PullRequestMessage, "deleted");
    }

    [TestMethod]
    public void Compare_WhenFileContentIsChanged_ReturnsFileChanged()
    {
        using var oldFile = TempFile.Create(CreateFileContent(new [] {"key1", "key2"}));
        using var newFile = TempFile.Create(CreateFileContent(new [] {"key2", "key3"}));

        var result = FileComparer.Compare(oldFile.FilePath, newFile.FilePath, newFile.FilePath);

        Assert.IsTrue(result.DeletedKeys.SequenceEqual(new [] {"key1"}));
        Assert.IsTrue(result.AddedKeys.SequenceEqual(new [] {"key3"}));
        Assert.IsTrue(result.HasBreakingChanges);
        Assert.IsTrue(result.HasChanges);
        StringAssert.Contains(result.PullRequestMessage, "changed");
    }

    private string CreateFileContent(string[] keys)
    {
        var secretModel = new StringBuilder();
        foreach(var key in keys)
        {
            secretModel.AppendLine($"  - path: {key}");
            secretModel.AppendLine($"    name: name");
        }

        var fileContent = $"vaultAgentConfig:\n  secrets:\n{secretModel}";
        return fileContent;
    }
}