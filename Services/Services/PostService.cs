using Models.Downloads;
using Services.Interfaces;
using System.Threading.Channels;

namespace Services.Services
{
    public class PostService : IPostService
    {
        private readonly IDownloadService _downloadService;     

        private readonly int RequestsPerSecond = 1;

        public PostService(IDownloadService downloadService)
        {
            _downloadService = downloadService;           
        }
        public async Task<Stream> GetImageThumbnailAsync(string boardId, string imageId) => await _downloadService.GetImageThumbnailAsync(boardId, imageId);

        public IEnumerable<FileDownloadRequest> GenerateDownloads(DownloadRequest downloadRequest)
        {
            var fileDownloads = new List<FileDownloadRequest>();

            Parallel.ForEach(downloadRequest.Thread.Posts, post =>
            {
                if (!string.IsNullOrEmpty(post.filename) && !string.IsNullOrEmpty(post.ext))
                {
                    var baseFolder = BuildFolderStructure(downloadRequest.Thread.ThreadTitle, downloadRequest.Thread.BoardId).Trim();

                    var filePath = GenerateFilePath(baseFolder, post.filename, post.ext);

                    var fileUrl = $"{downloadRequest.Thread.BoardId}/{post.tim}{post.ext}";

                    fileDownloads.Add(new FileDownloadRequest()
                    {
                        PostNumber = post.no,
                        FileExtension = post.ext,
                        FileName = post.filename,
                        FileSize = post.fsize,
                        FilePath = filePath,
                        FileUrl = fileUrl
                    });
                }
            });

            return fileDownloads;
        }

        public async IAsyncEnumerable<FileDownloadRequest> DownloadPostsAsync(IEnumerable<FileDownloadRequest> downloadRequests)
        {
            var channel = Channel.CreateUnbounded<FileDownloadRequest>();
            var consumerTask = ConsumeDownloadsAsync(channel.Reader);

            foreach (var downloadRequest in downloadRequests)
            {
                if (downloadRequest != null && !string.IsNullOrEmpty(downloadRequest.FileUrl) && !string.IsNullOrEmpty(downloadRequest.FilePath))
                {
                    await channel.Writer.WriteAsync(downloadRequest);
                }
            }

            channel.Writer.Complete();
            await foreach (var completedDownload in consumerTask)
            {
                yield return completedDownload;
            }
        }

        private async IAsyncEnumerable<FileDownloadRequest> ConsumeDownloadsAsync(ChannelReader<FileDownloadRequest> reader)
        {
            var rateLimit = TimeSpan.FromSeconds(1.0 / RequestsPerSecond);
            while (await reader.WaitToReadAsync())
            {
                if (reader.TryRead(out var downloadRequest))
                {
                    var downloadStartTime = DateTime.UtcNow;

                    await _downloadService.DownloadFileAsync(downloadRequest.FileUrl, downloadRequest.FilePath);

                    var downloadElapsedTime = DateTime.UtcNow - downloadStartTime;
                    if (downloadElapsedTime < rateLimit)
                    {
                        await Task.Delay(rateLimit - downloadElapsedTime);
                    }

                    yield return downloadRequest;
                }
            }
        }


        private string BuildFolderStructure(string? threadTitle, string boardId)
        {
            var threadName = !string.IsNullOrEmpty(threadTitle) ? _downloadService.CleanInput(threadTitle) : "No Title";
            var baseFolder = _downloadService.CreateFileDestination(boardId, threadName);
            return baseFolder;
        }

        private string GenerateFilePath(string baseFolder, string fileName, string fileExtension)
        {
            var postName = _downloadService.CleanInput(fileName);
            var filePath = _downloadService.GenerateFilePath(baseFolder, postName, fileExtension);
            return filePath;
        }
    }
}
