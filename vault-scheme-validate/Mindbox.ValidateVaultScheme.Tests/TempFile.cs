namespace Mindbox.ValidateVaultScheme.Tests;

public sealed class TempFile : IDisposable
{
    public static TempFile Create(string content) => new(content);
    
    public string FilePath { get; }

    private TempFile(string content)
    {
        FilePath = Path.GetTempFileName();
        File.WriteAllText(FilePath, content);
    }

    public void Dispose()
    {
        try
        {
            File.Delete(FilePath);
        }
        catch (IOException)
        {
        }
    }
}