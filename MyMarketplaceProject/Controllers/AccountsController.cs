using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMarketplaceProject.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace MyMarketplaceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller
    {
        private readonly MarketplaceContext context;
        private static Dictionary<string, TempUser> tempUsers = new Dictionary<string, TempUser>();
        private readonly IConfiguration configuration;
        public AccountsController(MarketplaceContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            try
            {
                var loginClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
                if (loginClaim == null)
                {
                    return Unauthorized("Пользователь не аутентифицирован");
                }

                var login = loginClaim.Value;

                var account = context.Accounts
                    .Include(a => a.User) // Включаем связанный объект User
                    .FirstOrDefault(a => a.Login == login);
                if (account == null)
                {
                    return NotFound("Пользователь не найден");
                }

                var user = account.User;

                var profileData = new
                {
                    name = user.Name,
                    surname = user.Surname,
                    patronym = user.Patronum,
                    email = account.Login,
                    dateOfBirth = user.DateOfBirth.ToString("yyyy-MM-dd")
                };

                return Ok(profileData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }

        // PUT: api/Accounts/updateProfile
        [HttpPut("updateProfile")]
        public IActionResult UpdateProfile(UserProfileUpdateModel model)
        {
            try
            {
                var account = context.Accounts.FirstOrDefault(a => a.Login == model.Email);
                if (account == null)
                {
                    return NotFound("Аккаунт не найден");
                }

                var user = context.Users.FirstOrDefault(u => u.UserId == account.UserId);
                if (user == null)
                {
                    return NotFound("Пользователь не найден");
                }

                // Обновляем данные профиля
                user.Name = model.Name;
                user.Patronum = model.Patronym;
                user.Surname = model.Surname;
                user.DateOfBirth = DateTime.ParseExact(model.DateOfBirth, "yyyy-MM-dd", null);

                context.SaveChanges();

                return Ok("Данные профиля успешно обновлены");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message} - {ex.InnerException?.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверка наличия пользователя с таким же email
            if (context.Accounts.Any(a => a.Login == model.Email))
            {
                return BadRequest("Пользователь с такой почтой уже существует.");
            }

            int verificationCode = GenerateVerificationCode();

            var tempUser = new TempUser
            {
                Email = model.Email,
                Password = HashPassword(model.Email, model.Password),
                Name = model.Name,
                Patronum = model.Patronum,
                Surname = model.Surname,
                DateOfBirth = model.DateOfBirth,
                VerificationCode = verificationCode,
                VerificationCodeExpires = DateTime.UtcNow.AddMinutes(30) // Код действует 30 минут
            };

            // Сохранение временного пользователя
            tempUsers[model.Email] = tempUser;

            await SendVerificationEmail(tempUser.Email, verificationCode);

            return Ok("Код подтверждения был отправлен на вашу почту.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = context.Accounts.FirstOrDefault(a => a.Login == model.Email);
            string hashedPassword = HashPassword(model.Email, model.Password);
            if (account == null || (hashedPassword != account.Password))
            {
                return Unauthorized("Неправильная почта или пароль");
            }

            // Генерация JWT-токена
            JwtManager jwtManager = new JwtManager(configuration);
            var token = jwtManager.GenerateJwtToken(account);

            return Ok(new { Token = token });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyModel model)
        {
            if (!tempUsers.ContainsKey(model.Email))
            {
                return BadRequest("Пользователь не найден или код подтверждения истек.");
            }

            var tempUser = tempUsers[model.Email];

            if (tempUser.VerificationCode != model.Code || tempUser.VerificationCodeExpires < DateTime.UtcNow)
            {
                return BadRequest("Неправильный или истекший код подтверждения.");
            }

            var user = new User
            {
                Name = tempUser.Name,
                Patronum = tempUser.Patronum,
                Surname = tempUser.Surname,
                DateOfBirth = tempUser.DateOfBirth,
                IsModerator = 0
            };

            var account = new Account
            {
                Login = tempUser.Email,
                Password = tempUser.Password,
                VerificationCode = 0,
                User = user
            };

            // Сохранение пользователя и аккаунта в базу данных
            context.Users.Add(user);
            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            // Удаление временных данных
            tempUsers.Remove(model.Email);

            // Генерация JWT-токена
            JwtManager jwtManager = new JwtManager(configuration);
            var token = jwtManager.GenerateJwtToken(account);

            return Ok(new { Token = token });
        }
        private async Task SendVerificationEmail(string email, int verificationCode)
        {
            var smtpUsername = configuration["Smtp:Username"];
            var smtpPassword = configuration["Smtp:Password"];
            if (string.IsNullOrWhiteSpace(smtpUsername) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                throw new InvalidOperationException("Set Smtp__Username and Smtp__Password.");
            }

            // Настройка данных smtp сервера
            var smtpClient = new SmtpClient("smtp.yandex.ru")
            {
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword)
            };
            //создание сообщения
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUsername),
                Subject = "Ваш код подтверждения для регистрации",
                Body = $@"
                    <html>
                        <body>
                            <h2>Код подтверждения для регистрации. Внимание! Он действует 30 минут с момента отправки сообщения</h2>
                            <p>Ваш код подтверждения: <strong style='font-size: 1.5em; color: #007bff;'>{verificationCode}</strong></p>
                            <p>Если вы получили это сообщение по ошибке, пожалуйста, проигнорируйте его.</p>
                        </body>
                    </html>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Обработка ошибки отправки письма
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
                throw;
            }
        }
        private int GenerateVerificationCode()
        {
            Random seed = new Random();
            Random rnd = new Random(seed.Next(1000, 10000));
            return rnd.Next(100000, 1000000);//100000-999999 - 6 цифр
        }
        //генерация пароля SHA-256
        private string HashPassword(string email, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                string tmp = email + password;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(email + password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
