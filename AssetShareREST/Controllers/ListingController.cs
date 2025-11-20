using Microsoft.AspNetCore.Mvc;

namespace AssetShareREST.Controllers
{
    public class ListingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
