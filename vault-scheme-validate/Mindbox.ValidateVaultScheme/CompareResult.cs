using System.Diagnostics;

namespace Mindbox.ValidateVaultScheme;

public record CompareResult
{
    private CompareResult(
        IReadOnlyList<string> deletedKeys, 
        IReadOnlyList<string> addedKeys,
        bool hasBreakingChanges,
        string pullRequestMessage)
    {
        DeletedKeys = deletedKeys;
        AddedKeys = addedKeys;
        HasBreakingChanges = hasBreakingChanges;
        PullRequestMessage = pullRequestMessage;
    }

    public IReadOnlyList<string> DeletedKeys { get; }
    
    public IReadOnlyList<string> AddedKeys { get; }
    
    public bool HasBreakingChanges { get; }
    
    public string PullRequestMessage { get; }

    public static CompareResult FileCreated(string message, IReadOnlyList<string> addedKeys)
    {
        return new CompareResult(
            Array.Empty<string>(),
            addedKeys,
            false,
            message);
    }

    public static CompareResult FileDeleted(string message, IReadOnlyList<string> deletedKeys)
    {
        return new CompareResult(
            deletedKeys,
            Array.Empty<string>(),
            true,
            message);
    }

    public static CompareResult FileChanged(
        IReadOnlyList<string> deletedKeys,
        IReadOnlyList<string> addedKeys,
        bool hasBreakingChanges,
        string pullRequestMessage)
    {
        Debug.Assert(hasBreakingChanges == deletedKeys.Any());
        
        return new CompareResult(
            deletedKeys,
            addedKeys,
            hasBreakingChanges,
            pullRequestMessage);
    }

    public bool HasChanges => DeletedKeys.Any() || AddedKeys.Any();
}