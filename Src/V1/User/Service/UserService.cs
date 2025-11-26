using MarkItDoneApi.V1.Core.Security;
using MarkItDoneApi.V1.User.Entity;
using MarkItDoneApi.V1.User.Repository;
using MarkItDoneApi.V1.User.Rest.DTO;

namespace MarkItDoneApi.V1.User.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserEntity> CreateAsync(CreateUserRequestDto request)
    {
        await _userRepository.ValidateUniqueUsernameAsync(request.Username);
        await _userRepository.ValidateUniqueEmailAsync(request.Email);
        var hashedPassword = PasswordService.Hash(request.Password);

        var userToCreate = new CreateUserRequestDto(
            Username: request.Username,
            Email: request.Email,
            Password: hashedPassword
        );

        var newUser = await _userRepository.CreateAsync(userToCreate);

        return newUser;
    }

    public async Task<UserEntity> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetOneByUsername(username);

        return user;
    }
    
    public async Task<UserEntity> UpdateUserAsync(string username, UpdateUserRequestDto user) 
    {
        var currentUser = await _userRepository.GetOneByUsername(username);

        if (!string.IsNullOrEmpty(user.Username) && user.Username != currentUser.Username)
            await _userRepository.ValidateUniqueUsernameAsync(user.Username);

        if (!string.IsNullOrEmpty(user.Email) && user.Email != currentUser.Email)
            await _userRepository.ValidateUniqueEmailAsync(user.Email);

        var userToUpdate = new UpdateUserRequestDto(
            Username: user.Username ?? currentUser.Username,
            Email: user.Email ?? currentUser.Email,
            Password: !string.IsNullOrEmpty(user.Password) 
                ? PasswordService.Hash(user.Password) 
                : currentUser.Password
        );
            
        var updatedUser = await _userRepository.UpdateUser(username, userToUpdate);
        
        return updatedUser;
    }
}