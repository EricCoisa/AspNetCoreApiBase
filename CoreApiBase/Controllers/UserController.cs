using Microsoft.AspNetCore.Mvc;
using CoreApiBase.Application.DTOs;
using CoreDomainBase.Entities;
using CoreDomainBase.Interfaces.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApiBase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IServicesBase<User> _userService;
        private readonly IMapper _mapper;

        public UserController(IServicesBase<User> userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var created = await _userService.AddAsync(user);
            var createdDto = _mapper.Map<UserDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, UserDto userDto)
        {
            if (id != userDto.Id) return BadRequest();
            var user = _mapper.Map<User>(userDto);
            var updated = await _userService.UpdateAsync(user);
            var updatedDto = _mapper.Map<UserDto>(updated);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
