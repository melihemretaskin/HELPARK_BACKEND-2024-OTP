using Business;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UserAPI.Controllers
{
    public class OTPVerificationController : Controller
    {
        private readonly OtpService _otpService;

        public OTPVerificationController(OtpService otpService)
        {
            _otpService = otpService;
        }

        public IActionResult OTP()
        {
            // OTP giriş sayfasını göster
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtpAsync(string phoneNumber, string otp)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(otp))
            {
                // Hata mesajını uygun bir şekilde kullanıcıya göster
                return View("Error", new { message = "Telefon numarası ve OTP boş olamaz." });
            }

            var isOtpValid = await _otpService.ValidateOtpAsync(phoneNumber, otp);

            if (!isOtpValid)
            {
                // Doğrulama başarısız oldu, hata mesajını kullanıcıya göster
                return View("Error", new { message = "OTP geçersiz veya süresi dolmuş." });
            }

            // Doğrulama başarılı, kullanıcıyı Token sayfasına yönlendir
            return RedirectToAction("Token");
        }

        public IActionResult Token()
        {
            // Token sayfasını göster
            return View("Token");
        }
    }
}
