using Microsoft.AspNetCore.Mvc;

namespace AssetShareREST.Controllers
{
    public class MachineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
