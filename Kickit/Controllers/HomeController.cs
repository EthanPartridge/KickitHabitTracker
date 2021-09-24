using Kickit.Models;
using Kickit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace Kickit.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        
        public IActionResult SignUp()
        {
            return View();
        }
        
        //POST SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User obj)
        {            
            
            if (ModelState.IsValid)
            {
                var dbEmail = _db.Users.Where(u => u.Email == obj.Email).
                    Select(e => e.Email);
                
                if (dbEmail.Any())
                {
                    TempData["Error"] = "Error: An account has already been created for this email.";
                    return View("SignUp");
                }

                obj.Password = BCrypt.Net.BCrypt.HashPassword(obj.Password);
                _db.Users.Add(obj);
                _db.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim("email", obj.Email),
                    new Claim(ClaimTypes.NameIdentifier, obj.Email),
                    new Claim(ClaimTypes.Name, obj.First_Name),
                };

                var claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrinciple);
                return RedirectToAction("Index", "Habit");
            }
            return View(obj);
        }

        //GET Login
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        //POST Login
        [HttpPost("Login")]
        public async Task<IActionResult> Validate(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var dbEmail = _db.Users.Where(u => u.Email == email).
                    Select(e => e.Email);
                var dbPassword = _db.Users.Where(p => p.Email == email).
                    Select(a => a.Password);

                if (!dbEmail.Any() || !dbPassword.Any())
                {
                    TempData["Error"] = "Error: Email or Password is invalid.";
                    return View("Login");
                }

                bool isValidPassword = BCrypt.Net.BCrypt.
                    Verify(password, dbPassword.Single());

                if (dbEmail.Single().Equals(email) && isValidPassword)
                {
                    var dbFirstName = _db.Users.Where(u => u.Email == email).
                        Select(n => n.First_Name).Single();

                    var claims = new List<Claim>
                {
                    new Claim("email", email),
                    new Claim(ClaimTypes.NameIdentifier, email),
                    new Claim(ClaimTypes.Name, dbFirstName),
                };

                    var claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrinciple);
                    return RedirectToAction("Index", "Habit");
                }
                else
                {
                    TempData["Error"] = "Error: Email or Password is invalid.";
                    return View("Login");
                }
            }
            return View();
        }
        
        //Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        //GET ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //POST ForgotPassword
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var dbEmail = _db.Users.Where(u => u.Email == obj.Email).
                    Select(e => e.Email);

                if (!dbEmail.Any())
                {
                    return View("ForgotPasswordConfirmation");
                }

                //To get reset email function working, substitute "[Your email here]" with an actual email and
                //substitute "[Your email's password here]" with that email's password

                string resetCode = Guid.NewGuid().ToString();
                var link = "kickithabittracker.azurewebsites.net/Home/ResetPassword?email=" + obj.Email+"&token=" + resetCode;
                var fromEmail = new MailAddress("[Your email here]", "Kickit Habit Tracker");
                var toEmail = new MailAddress(obj.Email);
                var fromEmailPassword = "[Your email's password here]";
                string subject = "Reset Password";
                string body = "Hello,<br/><br/>We received a request to reset your account password. " +
                    "Please click the link below to reset your password:" +
                    "<br/><br/><a href=" + link + ">Reset Password Link</a>";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
                var temp = _db.Users.Find(obj.Email);
                temp.ResetPasswordCode = resetCode;
                _db.SaveChanges();

                return View("ForgotPasswordConfirmation");
            }
            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //GET ResetPassword
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return NotFound();
            }

            var user = _db.Users.Where(r => r.ResetPasswordCode == token).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel obj = new ResetPasswordModel();
                obj.ResetCode = token;
                return View(obj);
            }
            else
            {
                return NotFound();
            }
        }

        //POST ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword (ResetPasswordModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.Where(r => r.ResetPasswordCode == obj.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    bool oldPassword = BCrypt.Net.BCrypt.Verify(obj.NewPassword, user.Password);
                    if (oldPassword)
                    {
                        ViewBag.Message = "Your new password must be different from your previous password.";
                        return View();
                    }

                    user.Password = BCrypt.Net.BCrypt.HashPassword(obj.NewPassword);
                    user.ResetPasswordCode = null;
                    _db.SaveChanges();

                    var claims = new List<Claim>
                {
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(ClaimTypes.Name, user.First_Name),
                };

                    var claimsIdentity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrinciple);
                }
            }
            else
            {
                return View();
            }
            
            return RedirectToAction("ResetPasswordConfirmation");
        }

        //GET ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation ()
        {
            return View();
        }
    }
}