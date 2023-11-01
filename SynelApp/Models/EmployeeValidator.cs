using FluentValidation;

namespace SynelApp.Models
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.PayrollNumber).NotEmpty().MaximumLength(20);
            RuleFor(e => e.Forenames).NotEmpty().MaximumLength(50);
            RuleFor(e => e.DateOfBirth)
                .NotEmpty()
                .InclusiveBetween(DateTime.UtcNow.AddYears(-120), DateTime.UtcNow.AddYears(-16))
                .WithMessage("Employee's age must be between 16 and 120 years old");
            RuleFor(e => e.Telephone).NotEmpty().MaximumLength(20);
            RuleFor(e => e.Mobile).NotEmpty().MaximumLength(20);
            RuleFor(e => e.Address).NotEmpty().MaximumLength(255);
            RuleFor(e => e.Address2).MaximumLength(255);
            RuleFor(e => e.Postcode).NotEmpty().MaximumLength(20);
            RuleFor(e => e.EmailHome).EmailAddress().MaximumLength(100);
            RuleFor(e => e.StartDate).GreaterThan(DateTime.UtcNow.AddYears(-40))
                .WithMessage("Employee cannot be hired before the company was opened");
        }
    }
}
