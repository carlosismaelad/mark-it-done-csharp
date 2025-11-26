using MarkItDoneApi.V1.User.Repository;
using MarkItDoneApi.V1.User.Rest.DTO;
using MarkItDoneApi.V1.User.Service;
using MarkItDoneApi.V1.User.UserUtils;
using Microsoft.AspNetCore.Mvc;

namespace MarkItDoneApi.V1.User.Rest.Controller;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto user)
    {
        UserValidation.UserCreationValidation(user);
        var response = await _userService.CreateAsync(user);

        var createdUser = UserResponseDto.FromEntity(response);        

        return Created(string.Empty, createdUser);
    }

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByUsername([FromRoute] string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);

        var userFound = UserResponseDto.FromEntity(user);
        
        return Ok(userFound);
    }

    [HttpPatch("{username}")]
    public async Task<IActionResult> UpdateUser([FromRoute] string username, [FromBody] UpdateUserRequestDto user) 
    {
        UserValidation.UserUpdateValidation(user);
        var updatedUser = await _userService.UpdateUserAsync(username, user);

        var updatedUserResponse = UserResponseDto.FromEntity(updatedUser);
        
        return Ok(updatedUserResponse);
    }

}