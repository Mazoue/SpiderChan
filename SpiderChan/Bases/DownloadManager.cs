using Microsoft.AspNetCore.Components;
using Models.Chan;
using Models.Downloads;
using Services.Interfaces;
using System.Web;

namespace SpiderChan.Pages
{
    public partial class DownloadManager : ComponentBase
    {
        [Inject]
        private IPostService PostService { get; set; }

        [Inject]
        private IThreadService ThreadService { get; set; }

        private List<FileDownloadRequest> Posts { get; set; }

        public bool ShowDownloadManager { get; set; }

        private int ProcessedFilesCount { get; set; }
        private int TotalFilesCount { get; set; }
        protected override void OnInitialized()
        {
            ShowDownloadManager = false;
        }
        public async Task DownloadFiles(DownloadRequest downloadRequest)
        {
            var downloads = PostService.GenerateDownloads(downloadRequest);

            if (downloads == null || !downloads.Any())
                return;

            Posts ??= new List<FileDownloadRequest>();
            TotalFilesCount = downloads.Count(); // Set the total files count

            Posts = downloads.ToList();
            ShowDownloadManager = Posts.Any(); // Update the visibility based on the presence of posts
            await InvokeAsync(() => StateHasChanged()); // Inform the parent component about the state change

            if (Posts != null && Posts.Any())
            {
                await foreach (var download in PostService.DownloadPostsAsync(downloads))
                {
                    Posts.Remove(download);
                    ProcessedFilesCount++; // Increment the processed files count

                    ShowDownloadManager = Posts.Any(); // Update the visibility based on the presence of posts
                    await InvokeAsync(() => StateHasChanged()); // Inform the parent component about the state change
                }
            }
        }


        public async Task DownloadCatalogue(Catalogue? catalogue, string boardId)
        {
            if (catalogue != null)
            {
                foreach (var thread in catalogue.Threads)
                {
                    var posts = await ThreadService.GetPostsInThreads(boardId, thread.No);
                    await DownloadFiles(new DownloadRequest()
                    {
                        Thread = new DownloadRequestThreadDetails()
                        {
                            ThreadTitle = HttpUtility.UrlEncode(thread.Sub),
                            BoardId = boardId,
                            Posts = posts.PostCollection
                        }
                    });
                }
            }
        }

        public async Task DownloadBoard(BoardDownloadRequest boardDownloadRequest)
        {
            foreach (var catalogue in boardDownloadRequest.Catalogues)
            {
                await DownloadCatalogue(catalogue, boardDownloadRequest.BoardId);
            }
        }
    }
}
