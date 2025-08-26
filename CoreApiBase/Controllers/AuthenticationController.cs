using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreApiBase.Application.DTOs;
using CoreApiBase.Services;
using CoreApiBase.Utils;
using CoreDomainBase.Entities;
using CoreDomainBase.Enums;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq;
using CoreDomainBase.Interfaces.Services;
using Microsoft.Extensions.Localization;
using CoreApiBase.Resources;

namespace CoreApiBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AuthenticationController(IUserService userService, IMapper mapper, AuthService authService, IStringLocalizer<SharedResource> localizer)
        {
            _userService = userService;
            _mapper = mapper;
            _authService = authService;
            _localizer = localizer;
        }

        /// <summary>
        /// Realiza o registro de um novo usuário.
        /// </summary>
        /// <param name="registerDto">Dados para registro do usuário.</param>
        /// <returns>Token e dados do usuário registrado.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            try
            {
                var existingUsers = await _userService.GetAllAsync();
                if (existingUsers.Any(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
                {
                    return BadRequest(new { message = _localizer["UserExistsAlready"] });
                }

                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = _authService.HashPassword(registerDto.Password),
                    Role = Roles.User
                };

                var createdUser = await _userService.AddAsync(user);
                var token = _authService.GenerateToken(createdUser);

                return Ok(new
                {
                    token,
                    user = _mapper.Map<UserDto>(createdUser),
                    message = _localizer["UserRegisteredSuccessfully"]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = _localizer["RegistrationFailed", ex.Message] });
            }
        }

        /// <summary>
        /// Realiza o login do usuário.
        /// </summary>
        /// <param name="loginDto">Dados de login.</param>
        /// <returns>Token e dados do usuário autenticado.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var users = await _userService.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Username == loginDto.Username || u.Email == loginDto.Username);

                if (user == null || !_authService.VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = _localizer["InvalidCredentials"] });
                }

                var token = _authService.GenerateToken(user);

                return Ok(new
                {
                    token,
                    user = _mapper.Map<UserDto>(user),
                    message = _localizer["LoginSuccessful"]
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = _localizer["LoginFailed", ex.Message] });
            }
        }

        /// <summary>
        /// Retorna o perfil do usuário autenticado.
        /// </summary>
        /// <returns>Dados do usuário autenticado.</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = TokenDataHandler.GetUserId(User);
                var user = await _userService.GetByIdAsync(userId);

                if (user == null)
                    return NotFound(new { message = _localizer["UserNotFound"] });

                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = _localizer["ErrorRetrievingProfile", ex.Message] });
            }
        }

        /// <summary>
        /// Retorna informações do token JWT do usuário autenticado.
        /// </summary>
        /// <returns>Informações do token.</returns>
        [HttpGet("token-info")]
        [Authorize]
        public ActionResult GetTokenInfo()
        {
            try
            {
                return Ok(new
                {
                    userId = TokenDataHandler.GetUserId(User),
                    username = TokenDataHandler.GetUsername(User),
                    email = TokenDataHandler.GetEmail(User),
                    securityStamp = TokenDataHandler.GetSecurityStamp(User),
                    isAdmin = TokenDataHandler.IsAdmin(User),
                    hasUserRole = TokenDataHandler.HasRole(User, "User"),
                    hasAdminRole = TokenDataHandler.HasRole(User, "Admin")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = _localizer["ErrorRetrievingTokenInfo", ex.Message] });
            }
        }

        /// <summary>
        /// Revoga os tokens do usuário autenticado (apenas para administradores).
        /// </summary>
        /// <returns>Confirmação da revogação.</returns>
        [HttpPost("revoke-token")]
        [Authorize(Policy = nameof(Policy.AdminOnly))]
        public async Task<ActionResult> RevokeToken()
        {
            try
            {
                var id = TokenDataHandler.GetUserId(User);
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = _localizer["UserNotFound"] });

                user.RefreshSecurityStamp();
                await _userService.UpdateAsync(user);

                return Ok(new { message = _localizer["TokensRevoked"], userId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = _localizer["ErrorRevokingTokens", ex.Message] });
            }
        }
    }
}
