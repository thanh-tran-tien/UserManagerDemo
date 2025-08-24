using FluentValidation;
using UserManagerDemo.Application.Auth.Dtos;
using UserManagerDemo.Application.Users.Validators;

namespace UserManagerDemo.Application.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.User)
            .SetValidator(new UserDtoValidator());

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);
    }
}