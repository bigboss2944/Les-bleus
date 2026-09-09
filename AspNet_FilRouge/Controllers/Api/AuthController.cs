using AspNet_FilRouge.Models.Api;
using AspNet_FilRouge.Services;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNet_FilRouge.Controllers.Api
{
    /// <summary>
    /// API REST d'authentification — délivre un jeton JWT à l'application MAUI
    /// à partir des identifiants Identity existants (vendeur ou administrateur).
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISellerService _sellerService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ISellerService sellerService,
            IJwtTokenService jwtTokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _sellerService = sellerService;
            _jwtTokenService = jwtTokenService;
        }

        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] MobileLoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName)
                ?? await _userManager.FindByEmailAsync(request.UserName);
            if (user == null)
                return Unauthorized(new { error = "Identifiants invalides." });

            var checkResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!checkResult.Succeeded)
                return Unauthorized(new { error = "Identifiants invalides." });

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user, roles);

            string? sellerId = null;
            if (roles.Contains(AppConstants.Roles.Vendeur))
            {
                var seller = await _sellerService.GetByIdAsync(user.Id);
                sellerId = seller?.Id;
            }

            return Ok(new MobileLoginResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                UserId = user.Id,
                UserName = user.UserName ?? user.Id,
                Roles = roles.ToArray(),
                SellerId = sellerId
            });
        }
    }
}
