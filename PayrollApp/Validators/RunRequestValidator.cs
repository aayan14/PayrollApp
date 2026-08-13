using FluentValidation;
using PayrollApp.Models;

namespace PayrollApp.Validators
{
    public class RunRequestValidator: AbstractValidator<PayrollRunRequest>
    {
        public RunRequestValidator() 
        {
            RuleFor(x => x.Month).InclusiveBetween(1, 12).WithMessage("Month must be a valid value.");
            RuleFor(x => x.Year).InclusiveBetween(2000, 2100).WithMessage($"Year must be between 2000 and {DateTime.Now.Year}.");

            RuleFor(x => x).Must(NotBeInFuture).WithMessage("Selected Month and Year cannot be ahead current Month and Year");
        }

        private bool NotBeInFuture(PayrollRunRequest request)
        {
            var requestDate = new DateTime(request.Year, request.Month, 1);
            var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return requestDate <= currentDate;
        }
    }
}
