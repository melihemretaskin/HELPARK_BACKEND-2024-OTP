using Business;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OTPVerificationController : ControllerBase
    {
        private readonly OtpService _otpService;

        public OTPVerificationController(OtpService otpService)
        {
            _otpService = otpService;
        }

        // Bu metod Swagger'da anlamlı olmadığı ve kullanıcı arabirimine özgü olduğu için kaldırıldı.

        [HttpPost("Verify")]
        public async Task<ActionResult> VerifyOtpAsync([FromForm] string phoneNumber, [FromForm] string otp)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(otp))
            {
                return BadRequest("Telefon numarası ve OTP boş olamaz.");
            }

            var isOtpValid = await _otpService.ValidateOtpAsync(phoneNumber, otp);

            if (!isOtpValid)
            {
                return Unauthorized("OTP geçersiz veya süresi dolmuş.");
            }

            // Doğrulama başarılı olduğunda, ilgili bilgiyi HTTP 200 OK durumu ile döndür
            return Ok("OTP başarıyla doğrulandı.");
        }

        // Token sayfası kullanıcı arabirimine özgü olduğu için ve API'de anlamlı olmadığı için kaldırıldı.
    }
}
