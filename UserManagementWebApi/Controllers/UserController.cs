using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagementWebApi.Models;

namespace UserManagementWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Nome de usuário e senha são obrigatórios.");
            }

            var user = new User { Name = request.UserName, PasswordHash = request.Password };

            try
            {
                await _unitOfWork.UserRepository.AddUserAsync(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno no servidor.", Details = ex.Message });
            }
            var response = new UserRegistrationResponse(user.Name, "Usuário registrado com sucesso.");

            return CreatedAtAction(nameof(Register), new { id = user.Id }, response);
        }

    }
}
