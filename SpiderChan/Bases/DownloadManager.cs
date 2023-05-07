using Microsoft.AspNetCore.Components;
using Models.Chan;
using Models.Downloads;
using Services;
using Services.Interfaces;
using Services.Services;
using System.Web;

namespace SpiderChan.Pages
{
    public partial class DownloadManager : ComponentBase
    {
        [Inject]
        private IPostService PostService { get; set; }

        [Inject]
        private IThreadService ThreadService { get; set; }

        [Inject]
        private DownloadManagerService DownloadManagerService { get; set; }

        public bool ShowDownloadManager => DownloadManagerService.Posts.Any();

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public async Task DownloadFiles(DownloadRequest downloadRequest)
        {
            var downloads = PostService.GenerateDownloads(downloadRequest);

            if(downloads == null || !downloads.Any())
                return;

            DownloadManagerService.AddDownloads(downloads.ToList());
            await InvokeAsync(() => StateHasChanged());

            if(DownloadManagerService.Posts.Any())
            {
                await foreach(var download in PostService.DownloadPostsAsync(downloads))
                {
                    DownloadManagerService.RemoveDownload(download);
                    await InvokeAsync(() => StateHasChanged());
                }
            }
        }

        public async Task DownloadCatalogue(Catalogue? catalogue, string boardId)
        {
            if(catalogue != null)
            {
                foreach(var thread in catalogue.Threads)
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
            foreach(var catalogue in boardDownloadRequest.Catalogues)
            {
                await DownloadCatalogue(catalogue, boardDownloadRequest.BoardId);
            }
        }
    }
}
