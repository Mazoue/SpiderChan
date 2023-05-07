using Models.Downloads;
using Services.Interfaces;

namespace Services.Services
{
    public class DownloadManagerService 
    {
        public List<FileDownloadRequest> Posts { get; private set; } = new List<FileDownloadRequest>();
        public int ProcessedFilesCount { get; private set; }
        public int TotalFilesCount { get; private set; }

        public void AddDownloads(List<FileDownloadRequest> downloads)
        {
            if(downloads != null && downloads.Any())
            {
                TotalFilesCount += downloads.Count;
                Posts.AddRange(downloads);
            }
        }

        public void RemoveDownload(FileDownloadRequest download)
        {
            if(download != null)
            {
                Posts.Remove(download);
                ProcessedFilesCount++;
            }
        }

        public void ResetDownloadCounts()
        {
            ProcessedFilesCount = 0;
            TotalFilesCount = 0;
        }
    }
}
