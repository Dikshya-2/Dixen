using Dixen.Repo.DTOs;
using Dixen.Repo.DTOs.Auth;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Services;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dixen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly ITwoFAService _twoFactorAuthService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            IJWTService authService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            ITwoFAService twoFactorAuthService,
            RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _config = configuration;
            _twoFactorAuthService = twoFactorAuthService;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (request.Age < 18)
                return BadRequest("Registration only allowed for users 18 or 18+");
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Age = request.Age,
                Gender = request.Gender
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // TODO: Maybe i should think of role assign
            var roleName = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role;

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                    return BadRequest("Failed to create role.");
            }

            await _userManager.AddToRoleAsync(user, roleName);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationUrl = $"http://localhost:4200/conform-email?email={user.Email}&token={encodedToken}";

            await _emailSender.SendEmailAsync(
                 user.Email!,
                "Confirm your email",
                 $@"Click here to confirm: <a href='{confirmationUrl}'>Click Here</a>");

            return Ok(new
            {
                Message = "User registered. Confirm email using the link.",
                ConfirmationLink = confirmationUrl
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return BadRequest("Email or token is missing.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("User not found.");

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
                return BadRequest("Email confirmation failed.");

            await _userManager.SetTwoFactorEnabledAsync(user, true);

            if (string.IsNullOrEmpty(await _userManager.GetAuthenticatorKeyAsync(user)))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
            }

            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);

            var qrCodeImage = _twoFactorAuthService.GenerateQrCodeImage(user.Email!, authenticatorKey!);

            var response = new ConfirmEmailResponseDto
            {
                Message = "Scan QR with Google Authenticator.",
                QrCodeImage = qrCodeImage,
                ManualKey = authenticatorKey
            };
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials.Please check your email and password.");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Email not confirmed.");

            if (await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return Ok(new
                {
                    Message = "Email and password correct! Now enter your 6-digit code from Google Authenticator.\"",
                    Requires2FA = true,
                });
            }
            var jwt = await _authService.GenerateJwtToken(user);
            return Ok(new { token = jwt });
        }

        [HttpPost("verify2fa")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized();

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                model.Code);

            if (!isValid)
                return BadRequest("Invalid or expired code.");

            var jwt = await _authService.GenerateJwtToken(user);
            return Ok(new { token = jwt });
        }    
    }
}
