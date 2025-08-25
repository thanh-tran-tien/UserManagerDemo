using FluentValidation;
using UserManagerDemo.Application.Users.Dtos;

namespace UserManagerDemo.Application.Users.Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().Length(1, 50);

        RuleFor(x => x.LastName)
            .NotEmpty().Length(1, 50);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9\s\-]{7,15}$")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.ZipCode)
            .Matches(@"^[A-Za-z0-9\- ]{3,10}$")
            .When(x => !string.IsNullOrEmpty(x.ZipCode));
    }
}