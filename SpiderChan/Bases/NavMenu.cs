using Microsoft.AspNetCore.Components;
using Models.Chan;
using Services.Interfaces;

namespace SpiderChan.Shared
{
    public partial class NavMenu : ComponentBase
    {
        [Inject] private IBoardService BoardDataService { get; set; }
        [Inject] private IGeneralConfigService GeneralConfigService { get; set; }

        public List<Board> AllBoardDetails { get; set; }

        protected override async Task OnInitializedAsync() => AllBoardDetails = await BoardDataService.FetchDetailsForAllBoards();
    }
}
