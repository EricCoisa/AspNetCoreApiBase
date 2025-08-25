using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoreApiBase.Application.DTOs;
using CoreApiBase.Services;
using CoreApiBase.Utils;
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
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public UserController(IUserService userService, IMapper mapper, AuthService authService)
        {
            _userService = userService;
            _mapper = mapper;
            _authService = authService;
        }

        /// <summary>
        /// Retorna todos os usuários (apenas para administradores).
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        [HttpGet("GetAll")]
        [Authorize(Policy = nameof(Policy.AdminOnly))]
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

        /// <summary>
        /// Retorna um usuário pelo ID. Usuário pode acessar seus próprios dados ou o administrador pode acessar qualquer usuário.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Dados do usuário.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = nameof(Policy.UserOrAdmin))]
        public async Task<ActionResult<UserDto>> Get(int id)
        {
            try
            {
                // Check if user is accessing their own data or is admin
                var currentUserId = TokenDataHandler.GetUserId(User);
                var isAdmin = TokenDataHandler.IsAdmin(User);

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

        /// <summary>
        /// Cria um novo usuário (apenas para administradores).
        /// </summary>
        /// <param name="userDto">Dados do usuário.</param>
        /// <returns>Usuário criado.</returns>
        [HttpPost]
        [Authorize(Policy = nameof(Policy.AdminOnly))]
        public async Task<ActionResult<UserDto>> Create(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                var created = await _userService.AddAsync(user);
                var createdDto = _mapper.Map<UserDto>(created);
                return CreatedAtAction(nameof(Get), new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error creating user: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualiza os dados de um usuário. Usuário pode atualizar seus próprios dados ou o administrador pode atualizar qualquer usuário.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <param name="userDto">Dados do usuário.</param>
        /// <returns>Usuário atualizado.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = nameof(Policy.UserOrAdmin))]
        public async Task<ActionResult<UserDto>> Update(int id, UserDto userDto)
        {
            try
            {
                // Check if user is updating their own data or is admin
                var currentUserId = TokenDataHandler.GetUserId(User);
                var isAdmin = TokenDataHandler.IsAdmin(User);

                if (currentUserId != id && !isAdmin)
                    return Forbid();

                if (id != userDto.Id)
                    return BadRequest();

                // Get current user from database
                var existingUser = await _userService.GetByIdAsync(id);
                if (existingUser == null)
                    return NotFound(new { message = "User not found" });

                var user = _mapper.Map<User>(userDto);

                // If role is being changed, refresh SecurityStamp to invalidate existing tokens
                if (existingUser.Role != user.Role)
                {
                    user.RefreshSecurityStamp();
                }

                var updated = await _userService.UpdateAsync(user);
                var updatedDto = _mapper.Map<UserDto>(updated);
                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error updating user: {ex.Message}" });
            }
        }

        /// <summary>
        /// Exclui um usuário pelo ID (apenas para administradores).
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Sem conteúdo se excluído com sucesso.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(Policy.AdminOnly))]
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

    // ...existing code...
    }
}
