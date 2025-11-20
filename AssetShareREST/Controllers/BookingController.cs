using Microsoft.AspNetCore.Mvc;

namespace AssetShareREST.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
