using Microsoft.AspNetCore.Mvc;

namespace AlturStudyCase.Models.ViewComponents
{
    public class Header : ViewComponent
    {
        public Header()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
