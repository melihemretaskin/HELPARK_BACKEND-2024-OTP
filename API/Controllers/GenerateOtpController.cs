using Business;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateOtpController : ControllerBase
    {
        private readonly OtpService _otpService;

        public GenerateOtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        // GET işlemi Swagger'da anlamlı olmayacağından ve kullanıcı arabirimine özgü olduğundan kaldırıldı.

        // POST işlemi için bu kısmı kullanabilirsiniz.
        [HttpPost]
        public async Task<ActionResult> SendOtp([FromForm] PhoneNumberModel model)
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

            // HTTP 200 OK durumu ile birlikte OTP bilgisini JSON formatında döndür
            return Ok(new { Message = $"OTP başarıyla gönderildi: {otp}" });
        }
    }

    public class PhoneNumberModel
    {
        public string PhoneNumber { get; set; }
    }
}
