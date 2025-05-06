using JG.Code.Catalog.Domain.Enum;

namespace JG.Code.Catalog.Domain.Entity;

public class Media
{
    public string FilePath { get; private set; }
    public string EncodedPath { get; private set; }
    public MediaStatus Status { get; private set; }

    public Media(string filePath)
    {
        FilePath = filePath;
        Status = MediaStatus.Pending;
    }

    public void UpdateAsSentToEncode()
    {
        Status = MediaStatus.Processing;
    }

    public void UpdateAsEncoded(string encodedPath)
    {
        Status = MediaStatus.Completed;
        EncodedPath = encodedPath;
    }
}