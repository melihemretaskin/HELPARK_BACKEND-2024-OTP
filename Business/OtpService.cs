using DataAccess;
using Modals; // Bu namespace modellerinizi içeriyor olmalı
using OtpNet;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Business
{
    public class OtpService
    {
        private readonly HelparkDbContext _context;
        private readonly DBHelper _dbHelper;

        public OtpService(HelparkDbContext context, DBHelper dbHelper)
        {
            _context = context;
            _dbHelper = dbHelper;
        }

        private static string GenerateRandomOtp()
        {
            var key = KeyGeneration.GenerateRandomKey(20); // OTP.NET anahtar boyutu genellikle bayt cinsinden verilir.
            var totp = new Totp(key);
            return totp.ComputeTotp();
        }

        private async Task SaveOtpAsync(string phoneNumber, string otp, DateTime expiryTime)
        {
            var query = "UPDATE dbo.TBL_USER SET OTP = @OTP, EXPIRY_TIME = @EXPIRY_TIME WHERE PHONE_NUMBER = @PHONE_NUMBER";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@OTP", otp),
                new SqlParameter("@EXPIRY_TIME", expiryTime),
                new SqlParameter("@PHONE_NUMBER", phoneNumber),
            };

            await _dbHelper.ExecuteNonQueryAsync(query, parameters);
        }

        private async Task<(string otp, DateTime expiryTime)> GetOtpAsync(string phoneNumber)
        {
            var query = "SELECT OTP, EXPIRY_TIME FROM TBL_USER WHERE PHONE_NUMBER = @PhoneNumber";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PhoneNumber", phoneNumber),
            };

            var dataTable = await _dbHelper.SelectAsyncSafe(query, parameters);

            if (dataTable.Rows.Count == 1)
            {
                var row = dataTable.Rows[0];
                return (otp: row["OTP"].ToString(), expiryTime: Convert.ToDateTime(row["EXPIRY_TIME"]));
            }
            else
            {
                return (otp: "", expiryTime: DateTime.MinValue);
            }
        }

        public async Task<string> GenerateOtpAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

            var otp = GenerateRandomOtp();
            var expiryTime = DateTime.UtcNow.AddMinutes(1);

            await SaveOtpAsync(phoneNumber, otp, expiryTime);

            return otp;
        }

        public async Task<bool> ValidateOtpAsync(string phoneNumber, string otpToValidate)
        {
            var (otp, expiryTime) = await GetOtpAsync(phoneNumber);

            if (DateTime.UtcNow > expiryTime || otp != otpToValidate)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RequestNewOtpAsync(string phoneNumber)
        {
            // Bu metod, bir kullanıcı için yeni bir OTP isteği oluşturur
            // Kullanıcının mevcut OTP'sini geçersiz kıl (isterseniz bu adımı atlayabilirsiniz)
            // Yeni bir OTP oluştur ve kaydet
            return !string.IsNullOrEmpty(await GenerateOtpAsync(phoneNumber));
        }

        // Not: `ExpireOtpAsync` metodu burada gösterilmemiştir çünkü 
        // mevcut lojikte yeni OTP üretimi eski OTP'yi otomatik olarak geçersiz kılacaktır.
        // Eğer eski OTP'yi açıkça geçersiz kılmak istiyorsanız, 
        // bu işlevi `GenerateOtpAsync` metodunun bir parçası olarak gerçekleştirebilirsiniz.
    }
}
