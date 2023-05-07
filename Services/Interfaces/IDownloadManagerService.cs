using Models.Downloads;

namespace Services.Interfaces
{
    public interface IDownloadManagerService
    {
        void AddDownloads(List<FileDownloadRequest> downloads);
        void RemoveDownload(FileDownloadRequest download);
        void ResetDownloadCounts();

    }
}
