using Business;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Modals;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<AuthController> _logger; // Logger tipi güncellendi
        private readonly DBHelper _dbHelper;

        public AuthController(UserService userService, ILogger<AuthController> logger, DBHelper dbHelper)
        {
            _userService = userService;
            _logger = logger;
            _dbHelper = dbHelper;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Login için gerekli bilgileri bir form veya JSON olarak dönebilirsiniz.
            return Ok(new { message = "Please provide login credentials." });
        }

        [HttpPost("verifyPhoneNumber")]
        public async Task<IActionResult> VerifyPhoneNumberAsync(string phoneNumber)
        {
            bool isPhoneNumberRegistered = await _userService.GetUserPhoneNumberAsync(phoneNumber);

            if (!isPhoneNumberRegistered)
            {
                return NotFound(new { message = "Phone number is not registered." });
            }
            else
            {
                // API'de genellikle token veya benzeri bir şey döneriz.
                return Ok(new { message = "Phone number is verified." });
            }
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            // Kayıt formunun gerekliliklerini döneriz.
            return Ok(new { message = "Please provide registration details." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Doğrudan hatalı modeli döndürüyoruz.
            }

            try
            {
                await _userService.AddUserAsync(user);
                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error registering user: " + ex.Message);
                return StatusCode(500, new { message = ex.Message }); // İç sunucu hatası
            }
        }

        // SaveUserToDatabase metodunu kaldırabiliriz, zaten UserService üzerindeyiz.
    }
}
