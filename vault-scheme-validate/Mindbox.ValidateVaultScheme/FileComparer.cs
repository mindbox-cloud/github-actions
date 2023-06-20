using System.Diagnostics;
using System.Text;
using YamlDotNet.Serialization;

namespace Mindbox.ValidateVaultScheme;

public static class FileComparer
{
    public static CompareResult Compare(string? oldFilePath, string? newFilePath, string fileSourcePath)
    {
        Debug.Assert(oldFilePath is not null || newFilePath is not null);
        IReadOnlyList<string> addedKeys;
        IReadOnlyList<string> deletedKeys;
        
        if (oldFilePath is null)
        {
            addedKeys = ReadKeyList(newFilePath!);
            return CompareResult.FileCreated(
                $"Secrets list file `{fileSourcePath}` created in pull request",
                addedKeys.ToArray());
        }

        if (newFilePath is null)
        {
            deletedKeys = ReadKeyList(oldFilePath);
            return CompareResult.FileDeleted(
                $"Secrets list file `{fileSourcePath}` deleted in pull request",
                deletedKeys);
        }
        
        var newKeys = ReadKeyList(newFilePath);
        var oldKeys = ReadKeyList(oldFilePath);

        addedKeys = newKeys.Except(oldKeys).ToList();
        deletedKeys = oldKeys.Except(newKeys).ToList();

        var hasBreakingChanges = deletedKeys.Any();

        var changes = new List<string>(2);
        if (addedKeys.Any())
        {
            changes.Add($"**{addedKeys.Count}** key(s) were added");
        }
        if (deletedKeys.Any())
        {
            changes.Add($"**{deletedKeys.Count}** key(s) were deleted");
        }

        var message = $"Secrets list file `{fileSourcePath}` changed in pull request. {string.Join(" and ", changes)}";

        return CompareResult.FileChanged(
            deletedKeys,
            addedKeys,
            hasBreakingChanges,
            message);
    }

    private static IReadOnlyList<string> ReadKeyList(string filePath)
    {
        using var file = File.OpenRead(filePath);
        
        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();
        var fileModel = deserializer.Deserialize<YamlModel>(new StreamReader(file, Encoding.UTF8));

        return fileModel
            .VaultAgentConfig
            .Secrets
            .Select(s => s.Path)
            .ToArray();
    }
    
    private class YamlModel
    {
        [YamlMember(Alias = "vaultAgentConfig")]
        public VaultAgentConfigModel VaultAgentConfig { get; set; } = null!;

        public class VaultAgentConfigModel
        {
            [YamlMember(Alias = "secrets")]
            public List<SecretModel> Secrets { get; set; } = null!;
        }

        public class SecretModel
        {
            [YamlMember(Alias = "path")]
            public string Path { get; set; } = null!;
            
            [YamlMember(Alias = "name")]
            public string? Name { get; set; } = null!;
        }
    }
}