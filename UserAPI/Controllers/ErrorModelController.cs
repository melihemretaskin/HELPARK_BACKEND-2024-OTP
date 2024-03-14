using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers
{
    public class ErrorModelController : Controller
    {
        public IActionResult Fail()
        {
            return View("Fail");
        }
    }
}
