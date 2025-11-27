
using Application.Interfaces;
using Application.ViewModels.Requests;
using Application.ViewModels.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<CommonResponse> Registrar(RegistroRequest request)
        {
            IdentityUser user = new()
            {
                UserName = request.Email,
                Email = request.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Senha);

            if (!result.Succeeded)
            {
                List<string> erros = result.Errors.Select(x => x.Description).ToList();
                return CommonResponse.ErroBuilder(erros);
            }

            // adiciona role
            await _userManager.AddToRoleAsync(user, "Cliente");

            return CommonResponse.SucessoBuilder();
        }

        public async Task<CommonGenericResponse<string>> Login(LoginRequest request)
        {
            IdentityUser? user = await _userManager.FindByEmailAsync(request.Email);

            const string msgErroCredenciais = "Credenciais não encontradas";

            if (user == null)
            {
                return CommonGenericResponse<string>.ErroBuilder(msgErroCredenciais);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Senha, false);

            if (!result.Succeeded)
            {
                return CommonGenericResponse<string>.ErroBuilder(msgErroCredenciais);
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();

            var token = CriarToken(user, role);

            return CommonGenericResponse<string>.SucessoBuilder(token);
        }

        private string CriarToken(IdentityUser user, string role)
        {
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString())
            ];

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(
                _config["Jwt:Key"]
            ));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(6),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

