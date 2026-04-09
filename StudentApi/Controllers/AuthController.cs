using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

// [ApiController] - указывает, что этот класс - API контроллер
// [Route("api/v1/[controller]")] - маршрут: /api/v1/auth
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // POST /api/v1/auth/register - регистрация
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] StudentRegisterDto dto)
    {
        // Проверяем валидность данных (атрибуты [Required] и т.д.)
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(dto);

        if (result == null)
            return Conflict(new { message = "Пользователь с таким логином уже существует" });

        // 201 Created - успешное создание
        return CreatedAtAction(nameof(Register), new { id = result.IdStudent }, result);
    }

    // POST /api/v1/auth/login - вход
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] StudentLoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized(new { message = "Неверный логин или пароль" });

        return Ok(result);
    }
}