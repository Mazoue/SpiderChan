using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models.Chan;
using Models.Downloads;
using Services.Interfaces;
using System.Web;

namespace SpiderChan.Pages
{
    public partial class BoardOverview : ComponentBase
    {
        [Parameter]
        public string BoardId { get; set; }

        [Parameter]
        public string BoardName { get; set; }

        [Inject]
        private NavigationManager NavManager { get; set; }
        [Inject]
        private IBoardService BoardService { get; set; }        

        private DownloadManager downloadManager;
        private string BoardTitle { get; set; } = string.Empty;
        private bool IsLoading { get; set; } = true;

        private IEnumerable<Catalogue> Catalogues { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            if (BoardId != null)
            {
                Catalogues = await BoardService.GetBoardCatalogue(BoardId);
            }

            BoardTitle = !string.IsNullOrEmpty(BoardName) ? BoardName : BoardId;
            IsLoading = false;
        }

        protected void ExpandThreadPosts(ChanThread currentThread) => NavManager.NavigateTo(!string.IsNullOrEmpty(currentThread.Sub)
                ? $"/thread/{BoardId}/{currentThread.No}/{HttpUtility.UrlEncode(currentThread.Sub)}"
                : $"/thread/{BoardId}/{currentThread.No}");


        private void SelectAllThreads(object isChecked)
        {
            foreach (var catalog in Catalogues)
            {
                foreach (var currentThread in catalog.Threads)
                {

                    currentThread.Checked = (bool)isChecked;

                }
            }
            StateHasChanged();
        }

        protected async Task DownloadCatalogue(IEnumerable<Catalogue> catalogues)
        {
            var boardDownloadRequest = new BoardDownloadRequest()
            {
                BoardId = BoardId,
                Catalogues = new List<Catalogue?>()
            };
          
            boardDownloadRequest.Catalogues.AddRange(
                catalogues.Where(catalogue => catalogue.Threads.Any(thread => thread.Checked))
            );

            await downloadManager.DownloadBoard(boardDownloadRequest);
        }
    }
}