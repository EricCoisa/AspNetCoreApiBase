using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreApiBase.Application.DTOs;
using CoreApiBase.Services;
using CoreDomainBase.Entities;
using CoreDomainBase.Interfaces.Services;
using CoreDomainBase.Enums;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApiBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IServicesBase<User> _userService;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public UserController(IServicesBase<User> userService, IMapper mapper, AuthService authService)
        {
            _userService = userService;
            _mapper = mapper;
            _authService = authService;
        }

        // Authentication endpoints
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] UserRegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUsers = await _userService.GetAllAsync();
                if (existingUsers.Any(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
                {
                    return BadRequest(new { message = "Username or email already exists" });
                }

                // Create new user
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = _authService.HashPassword(registerDto.Password),
                    Role = Roles.User // Default role
                };

                var createdUser = await _userService.AddAsync(user);
                var token = _authService.GenerateToken(createdUser);

                return Ok(new 
                { 
                    token,
                    user = _mapper.Map<UserDto>(createdUser),
                    message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Registration failed: {ex.Message}" });
            }
        }

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
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                var token = _authService.GenerateToken(user);

                return Ok(new 
                { 
                    token,
                    user = _mapper.Map<UserDto>(user),
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Login failed: {ex.Message}" });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value ?? "0");
                var user = await _userService.GetByIdAsync(userId);
                
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving profile: {ex.Message}" });
            }
        }

        // Admin-only endpoints
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving users: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            try
            {
                // Check if user is accessing their own data or is admin
                var currentUserId = int.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value ?? "0");
                var isAdmin = User.IsInRole("Admin");
                
                if (currentUserId != id && !isAdmin)
                    return Forbid();

                var user = await _userService.GetByIdAsync(id);
                if (user == null) 
                    return NotFound(new { message = "User not found" });
                
                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving user: {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UserDto>> Create(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                var created = await _userService.AddAsync(user);
                var createdDto = _mapper.Map<UserDto>(created);
                return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error creating user: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<UserDto>> Update(int id, UserDto userDto)
        {
            try
            {
                // Check if user is updating their own data or is admin
                var currentUserId = int.Parse(User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value ?? "0");
                var isAdmin = User.IsInRole("Admin");
                
                if (currentUserId != id && !isAdmin)
                    return Forbid();

                if (id != userDto.Id) 
                    return BadRequest();
                
                var user = _mapper.Map<User>(userDto);
                var updated = await _userService.UpdateAsync(user);
                var updatedDto = _mapper.Map<UserDto>(updated);
                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating user: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeleteAsync(id);
                if (!result) 
                    return NotFound(new { message = "User not found" });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error deleting user: {ex.Message}" });
            }
        }
    }
}
