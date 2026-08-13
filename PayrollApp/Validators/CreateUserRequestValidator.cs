using FluentValidation;
using PayrollApp.Models;

namespace PayrollApp.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x).Must(PasswordValidation).WithMessage("Password must follow the rules");

            RuleFor(x => x.FullName).NotEmpty().WithMessage("Fullname is required");

            RuleFor(x => x.Role).Must(r => r.Trim().ToLower() == "superadmin" || r.Trim().ToLower() == "associatehr");
        }



        private bool PasswordValidation(CreateUserRequest req)
        {
            return true;

            // implementation left for regex
        }
    }
}
