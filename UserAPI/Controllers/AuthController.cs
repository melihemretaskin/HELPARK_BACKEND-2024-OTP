using Business;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Modals;

namespace UserAPI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        private readonly Result _result;
        private readonly HelparkDbContext _context;
        private readonly ILogger<AccountController> _logger; // ILogger eklenmiş
        private readonly DBHelper _dbHelper;

        // Inject the UserService into the constructor
        public AuthController(HelparkDbContext context, ILogger<AccountController> logger, DBHelper dbHelper,UserService userService)
        {
            _userService = userService;
            _context = context;
            _logger = logger; // ILogger eklenmiş
            _dbHelper = dbHelper;
        }

        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPhoneNumberAsync(string phoneNumber)
        {

            bool isPhoneNumberRegistered = await _userService.GetUserPhoneNumberAsync(phoneNumber);

            if (!isPhoneNumberRegistered)
            {
                // Telefon numarası kayıtlı değilse kayıt sayfasına yönlendir
                return RedirectToAction("Fail", "ErrorModel", new { area = "" });

            }
            else
            {
                Response.Headers.Add("PhoneNumber", phoneNumber);

                return RedirectToAction("Success", "Result", new { area = "" });
            }
        }

        public IActionResult Register()
        {

            // Oluşturulan kullanıcı nesnesini view'e gönder
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {


            if (!ModelState.IsValid)
            {
                try
                {
                    await SaveUserToDatabase(user);

                    return RedirectToAction("Success", "Result", new { area = "" });
                }
                catch (Exception ex)
                {

                    _result.message = ex.Message;
                    _result.timeStamp = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
                    _result.statusCode = //http status code 

                    ModelState.AddModelError("", "Kullanıcı kaydederken bir hata oluştu. Lütfen tekrar deneyin.");
                    _logger.LogError("Kullanıcı kaydederken bir hata oluştu: " + ex.Message);

                    return RedirectToAction("Fail", "ErrorModel", new { area = "" });
                }
            }


            return View(user);
        }

        private async Task SaveUserToDatabase(User user)
        {
            try
            {
                // Kullanıcıyı veritabanına eklemek için UserService kullanın
                var userService = new UserService(_context, _dbHelper);

                // Kullanıcıyı veritabanına eklemek için AddAsync metodunu çağırın
                await userService.AddUserAsync(user);

                //// Değişiklikleri kaydet
                //await _context.SaveChangesAsync();

                // Başarılı kayıt olduğunda bir işlem yapabilirsiniz, örneğin bir loglama işlemi yapabilirsiniz.
                // Örnek: Logger.Log("Kullanıcı başarıyla kaydedildi.");
            }
            catch (Exception ex)
            {
                // Hata durumunda bir işlem yapabilirsiniz, örneğin hata mesajını loglayabilirsiniz.
                // Örnek: 
                _logger.LogError("Kullanıcı kaydederken bir hata oluştu: " + ex.Message); // ILogger kullanımı düzeltilmiş
                throw; // Hata yukarıya fırlatılır, çağıran kod tarafından ele alınabilir.
            }
        }

    }
}
