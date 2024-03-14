using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers
{
    public class ResultController : Controller
    {
        public IActionResult Success()
        {

            // Oluşturulan kullanıcı nesnesini view'e gönder
            return View("Success");
        }

    }
}
