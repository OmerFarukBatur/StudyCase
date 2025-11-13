using Microsoft.AspNetCore.Mvc;

namespace AlturStudyCase.Models.ViewComponents
{
    public class Footer : ViewComponent
    {
        public Footer()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}