using DataAccess;
using Microsoft.EntityFrameworkCore;
using Modals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Business
{
    public class UserService
    {
        private readonly HelparkDbContext _context;
        private readonly DBHelper _dbHelper; // DBHelper nesnesi eklendi

        public UserService(HelparkDbContext context, DBHelper dbHelper) // DBHelper parametresi eklendi
        {
            _context = context;
            _dbHelper = dbHelper; // DBHelper nesnesi enjekte edildi
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var dataTable = await _dbHelper.SelectAsync("SELECT * FROM TBL_USER"); // DBHelper kullanıldı
            var users = new List<User>();

            // DataTable'dan User listesine dönüştürme işlemi
            foreach (DataRow row in dataTable.Rows)
            {
                var user = new User
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Name = row["Name"].ToString(),
                    Surname = row["Surname"].ToString(),
                    Email = row["Email"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString()
                };
                users.Add(user);
            }

            return users;
        }

        public async Task<bool> GetUserPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

            var parameters = new SqlParameter[]
            {
        new SqlParameter("@PhoneNumber", phoneNumber)
            };

            var dataTable = await _dbHelper.SelectAsyncSafe("SELECT * FROM TBL_USER WHERE PHONE_NUMBER = @PhoneNumber", parameters);

            if (dataTable.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;

            }

        }

        public async Task<bool> UserPhoneNumberControl(string phoneNumber)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@PhoneNumber",phoneNumber)
            };

            var dataTable = await _dbHelper.SelectAsyncSafe("SELECT * FROM TBL_USER WHERE PHONE_NUMBER = @PhoneNumber", parameters);

            return dataTable.Rows.Count == 0;
        }


        public async Task<bool> UserEmailControl(string email)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Email",email)
            };

            var dataTable = await _dbHelper.SelectAsyncSafe("SELECT * FROM TBL_USER WHERE EMAIL = @Email", parameters);

            return dataTable.Rows.Count == 0;
        }

        public async Task AddUserAsync(User user)
        {
            string email = user.Email;
            string phoneNumber = user.PhoneNumber;

            // Telefon numarası veya e-posta zaten varsa false, yoksa true döner
            bool isPhoneAvailable = await UserPhoneNumberControl(phoneNumber);
            bool isEmailAvailable = await UserEmailControl(email);

            // Eğer telefon numarası veya e-posta zaten varsa, kayıt işlemini iptal et
            if (!isPhoneAvailable || !isEmailAvailable) // Eğer herhangi biri kullanılıyorsa
            {
                throw new Exception("User with given email or phone number already exists.");
            }

            var query = "INSERT INTO dbo.TBL_USER (NAME, SURNAME, EMAIL, PHONE_NUMBER) VALUES (@Name, @Surname, @Email, @PhoneNumber)";
            var parameters = new SqlParameter[]
            {
        new SqlParameter("@Name", user.Name),
        new SqlParameter("@Surname", user.Surname),
        new SqlParameter("@Email", user.Email),
        new SqlParameter("@PhoneNumber", user.PhoneNumber)
            };

            await _dbHelper.ExecuteNonQueryAsync(query, parameters);
        }


    }
}
