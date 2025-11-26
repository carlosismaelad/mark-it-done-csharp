using System.Text.RegularExpressions;
using MarkItDoneApi.V1.Core.DomainExceptions;
using MarkItDoneApi.V1.User.Rest.DTO;

namespace MarkItDoneApi.V1.User.UserUtils;

public static class UserValidation
{
    // ======================== INDIVIDUAL VALIDATIONS ========================

    private static void ValidateUsername(string username, bool isRequired = false)
    {
        if (isRequired && (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username)))
        {
            throw new BusinessException("Campo 'Nome de usuário' deve estar preenchido.");
        }

        if (!string.IsNullOrEmpty(username) && string.IsNullOrWhiteSpace(username))
        {
            throw new BusinessException("Nome de usuário não pode estar em branco.");
        }

        if (!string.IsNullOrEmpty(username) && username.Length > 30)
        {
            throw new BusinessException("Nome de usuário não pode conter mais de 30 caracteres.");
        }
    }

    private static void ValidateEmail(string email, bool isRequired = false)
    {
        if (isRequired && (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email)))
        {
            throw new BusinessException("Campo 'e-mail' deve estar preenchido.");
        }

        if (!string.IsNullOrEmpty(email))
        {
            ValidateEmailFormat(email);
        }
    }

    private static void ValidatePassword(string password, bool isRequired = false)
    {
        if (isRequired && (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)))
        {
            throw new BusinessException("Campo 'Senha' deve estar preenchido.");
        }

        if (!string.IsNullOrEmpty(password))
        {
            ValidatePasswordStrength(password);
        }
    }

    // ======================== BUSINESS VALIDATIONS ========================

    public static void ValidateUserCreation(CreateUserRequestDto data)
    {
        ValidateUsername(data.Username, true);
        ValidateEmail(data.Email, true);
        ValidatePassword(data.Password, true);
    }

    public static void ValidateUserUpdate(UpdateUserRequestDto data)
    {
        if (data.Username is not null)
        {
            ValidateUsername(data.Username);
        }

        if (data.Email is not null)
        {
            ValidateEmail(data.Email);
        }

        if (data.Password is not null)
        {
            ValidatePassword(data.Password);
        }
    }

    // ======================== AUXILIARY FUNCTIONS ========================

    private static void ValidateEmailFormat(string email)
    {
        var errors = new List<string>();
        
        // why this regex? https://html.spec.whatwg.org/multipage/input.html#valid-e-mail-address
        const string emailPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
        var emailRegex = new Regex(emailPattern);

        if (!emailRegex.IsMatch(email))
            errors.Add("deve ser um e-mail válido pois enviaremos o código de autenticação para ele.");
        
        if (string.IsNullOrWhiteSpace(email))
            errors.Add("e-mail não pode estar em branco.");


        if (errors.Count > 0)
            throw new BusinessException($"Algo errado com o seu e-mail: {string.Join(", ", errors)}");
    }

    private static void ValidatePasswordStrength(string password)
    {
        var errors = new List<string>();

        if (password.Length < 8)
            errors.Add("pelo menos 8 caracteres");

        if (!password.Any(char.IsLower))
            errors.Add("pelo menos uma letra minúscula");

        if (!password.Any(char.IsUpper))
            errors.Add("pelo menos uma letra maiúscula");

        if (!password.Any(char.IsDigit))
            errors.Add("pelo menos um número");

        if (!password.Any(c => "!@#$%^&*(),.?\":{}|<>".Contains(c)))
            errors.Add("pelo menos um caractere especial");

        if (errors.Count > 0)
        {
          throw new BusinessException($"Senha deve conter: {string.Join(", ", errors)}.");
        }
    }

    // ======================== PUBLIC FUNCTIONS (COMPATIBILITY) ========================

    public static void UserCreationValidation(CreateUserRequestDto data)
    {
        ValidateUserCreation(data);
    }

    public static void UserUpdateValidation(UpdateUserRequestDto data)
    {
        ValidateUserUpdate(data);
    }
}