using Application.Services;
using Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly IConfiguration _config;

        public AuthServiceTests()
        {
            _userManagerMock = CreateUserManagerMock();
            _signInManagerMock = CreateSignInManagerMock(_userManagerMock.Object);

            // Config fake para JWT
            var inMemory = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-test-key-1234567890-super-secret-test-key",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();
        }

        private AuthService CreateSut()
            => new AuthService(_userManagerMock.Object, _signInManagerMock.Object, _config);

        [Fact]
        public async Task Registrar_QuandoCreateFalhar_DeveRetornarErroComDescricoes()
        {
            // Arrange
            var sut = CreateSut();

            var request = new RegistroRequest
            {
                Email = "teste@teste.com",
                Senha = "Senha@123"
            };

            var identityErrors = new[]
            {
                new IdentityError { Description = "Erro 1" },
                new IdentityError { Description = "Erro 2" },
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), request.Senha))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            // Act
            var response = await sut.Registrar(request);

            Assert.False(response.Sucesso);
            Assert.NotNull(response.Erros);
            Assert.Equal(2, response.Erros.Count);
            Assert.Contains("Erro 1", response.Erros);
            Assert.Contains("Erro 2", response.Erros);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Cliente"), Times.Never);
        }

        [Fact]
        public async Task Registrar_QuandoCreateOk_DeveAdicionarRoleCliente_E_RetornarSucesso()
        {
            var sut = CreateSut();

            var request = new RegistroRequest
            {
                Email = "cliente@teste.com",
                Senha = "Senha@123"
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), request.Senha))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Cliente"))
                .ReturnsAsync(IdentityResult.Success);

            var response = await sut.Registrar(request);

            Assert.True(response.Sucesso);
            Assert.Empty(response.Erros);

            _userManagerMock.Verify(
                x => x.CreateAsync(It.Is<IdentityUser>(u => u.Email == request.Email && u.UserName == request.Email), request.Senha),
                Times.Once);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Cliente"), Times.Once);
        }

        [Fact]
        public async Task Login_QuandoUsuarioNaoExiste_DeveRetornarErroCredenciaisNaoEncontradas()
        {
            var sut = CreateSut();

            var request = new LoginRequest { Email = "naoexiste@teste.com", Senha = "Senha@123" };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((IdentityUser?)null);

            var response = await sut.Login(request);

            Assert.False(response.Sucesso);
            Assert.Single(response.Erros);
            Assert.Equal("Credenciais não encontradas", response.Erros[0]);
            Assert.Equal(default, response.Data);
        }

        [Fact]
        public async Task Login_QuandoSenhaInvalida_DeveRetornarErroCredenciaisNaoEncontradas()
        {

            var sut = CreateSut();

            var user = new IdentityUser { Id = "user-1", Email = "user@teste.com", UserName = "user@teste.com" };
            var request = new LoginRequest { Email = user.Email!, Senha = "SenhaErrada" };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(user, request.Senha, false))
                .ReturnsAsync(SignInResult.Failed);

            var response = await sut.Login(request);


            Assert.False(response.Sucesso);
            Assert.Single(response.Erros);
            Assert.Equal("Credenciais não encontradas", response.Erros[0]);
            Assert.Equal(default, response.Data);
        }

        [Fact]
        public async Task Login_QuandoOk_DeveRetornarTokenJWT_ComClaimsEsperadas()
        {

            var sut = CreateSut();

            var user = new IdentityUser { Id = "user-123", Email = "user@teste.com", UserName = "user@teste.com" };
            var request = new LoginRequest { Email = user.Email!, Senha = "Senha@123" };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(user, request.Senha, false))
                .ReturnsAsync(SignInResult.Success);

            _userManagerMock
                .Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Cliente" });

            var response = await sut.Login(request);

            Assert.True(response.Sucesso);
            Assert.NotNull(response.Data);
            Assert.NotEmpty(response.Data);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(response.Data);

            Assert.Equal("test-issuer", jwt.Issuer);
            Assert.Contains("test-audience", jwt.Audiences);

            Assert.Equal(user.Email, jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);

            Assert.Equal("Cliente", jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value);

            Assert.Equal(user.Id, jwt.Claims.First(c => c.Type == "userId").Value);

            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            Assert.False(string.IsNullOrWhiteSpace(jti));
            Assert.True(jwt.ValidTo > DateTime.UtcNow.AddHours(5.9));
            Assert.True(jwt.ValidTo <= DateTime.UtcNow.AddHours(6.1));
        }


        private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<IdentityUser>>();

            return new Mock<UserManager<IdentityUser>>(
                store.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object,
                Array.Empty<IUserValidator<IdentityUser>>(),
                Array.Empty<IPasswordValidator<IdentityUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<IdentityUser>>>().Object
            );
        }

        private static Mock<SignInManager<IdentityUser>> CreateSignInManagerMock(UserManager<IdentityUser> userManager)
        {
            return new Mock<SignInManager<IdentityUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<IdentityUser>>().Object
            );
        }
    }
}
