using Microsoft.AspNetCore.Mvc;
using Business; // OtpService sınıfının bulunduğu namespace
using System.Threading.Tasks;

namespace UserAPI.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class GenerateOtpController : Controller
    {
        private readonly OtpService _otpService;

        public GenerateOtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        // GET işlemi için bu kısmı kullanabilirsiniz.
        [HttpGet]
        public IActionResult SendOtp()
        {
            // Oluşturulan kullanıcı nesnesini view'e gönder
            return View("SendOtp");
        }

        // POST işlemi için bu kısmı kullanabilirsiniz.
        [HttpPost]
        public async Task<IActionResult> SendOtp([FromForm] PhoneNumberModel model)
        {
            var phoneNumber = model.PhoneNumber;

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return BadRequest("Telefon numarası boş olamaz.");
            }

            var otp = await _otpService.GenerateOtpAsync(phoneNumber);

            if (otp == "Hata")
            {
                return BadRequest("OTP oluşturulamadı veya telefon numarası kayıtlı değil.");
            }

            return Ok($"OTP başarıyla gönderildi: {otp}");
        }
    }

    public class PhoneNumberModel
    {
        public string PhoneNumber { get; set; }
    }
}
