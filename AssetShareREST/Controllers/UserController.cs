using Microsoft.AspNetCore.Mvc;

namespace AssetShareREST.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
