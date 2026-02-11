using Dixen.API.Controllers;
using Dixen.Repo.DTOs;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DixenXUnitTest.ControllerTests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IJWTService> _jwtServiceMock;
        private readonly Mock<ITwoFAService> _twoFAMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IConfiguration> _configMock;

        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Setup mocks for UserManager and SignInManager
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _emailSenderMock = new Mock<IEmailSender>();
            _jwtServiceMock = new Mock<IJWTService>();
            _twoFAMock = new Mock<ITwoFAService>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _configMock = new Mock<IConfiguration>();

            _controller = new AuthController(
                _jwtServiceMock.Object,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _emailSenderMock.Object,
                _configMock.Object,
                _twoFAMock.Object,
                _roleManagerMock.Object
            );
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@test.com", Age = 20, FullName = "Dikshya" };

            _userManagerMock.Setup(u => u.FindByEmailAsync(user.Email))
                            .ReturnsAsync(user);

            _userManagerMock.Setup(u => u.ConfirmEmailAsync(user, "token123"))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.SetTwoFactorEnabledAsync(user, true))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.GetAuthenticatorKeyAsync(user))
                            .ReturnsAsync("auth-key");

            _twoFAMock.Setup(t => t.GenerateQrCodeImage(user.Email, "auth-key"))
                      .Returns("dummy-qrcode");

            var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("token123"));
            // Act
            var result = await _controller.ConfirmEmail(user.Email, tokenEncoded);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ConfirmEmailResponseDto>(okResult.Value);

            Assert.Contains("Scan QR", value.Message);
            Assert.Equal("dummy-qrcode", value.QrCodeImage);
            Assert.Equal("auth-key", value.ManualKey);
        }

    }
}
