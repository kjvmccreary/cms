using FluentValidation;
using ContractService.DTOs;
using ContractService.Models;

namespace ContractService.Validators
{
    public class CreateContractDtoValidator : AbstractValidator<CreateContractDto>
    {
        public CreateContractDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.ContractTypeId)
                .NotEmpty().WithMessage("Contract type is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(BeValidDate).WithMessage("Start date must be a valid date");

            RuleFor(x => x.EndDate)
                .Must((dto, endDate) => !endDate.HasValue || endDate > dto.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 4).WithMessage("Priority must be between 1 (High) and 4 (Low)");

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(0).When(x => x.Value.HasValue)
                .WithMessage("Contract value must be greater than or equal to 0");

            RuleFor(x => x.Currency)
                .MaximumLength(3).WithMessage("Currency code cannot exceed 3 characters")
                .Must(BeValidCurrency).When(x => !string.IsNullOrEmpty(x.Currency))
                .WithMessage("Currency must be a valid ISO currency code (USD, EUR, GBP, etc.)");

            RuleFor(x => x.BillingFrequency)
                .MaximumLength(20).WithMessage("Billing frequency cannot exceed 20 characters")
                .Must(BeValidBillingFrequency).When(x => !string.IsNullOrEmpty(x.BillingFrequency))
                .WithMessage("Billing frequency must be one of: Monthly, Quarterly, Annually, One-time");

            RuleFor(x => x.RenewalReminderDays)
                .InclusiveBetween(1, 365).WithMessage("Renewal reminder days must be between 1 and 365");

            RuleFor(x => x.AutoRenewalDurationDays)
                .InclusiveBetween(1, 3650).When(x => x.AutoRenewalDurationDays.HasValue)
                .WithMessage("Auto-renewal duration must be between 1 and 3650 days");

            RuleFor(x => x.InternalNotes)
                .MaximumLength(2000).WithMessage("Internal notes cannot exceed 2000 characters");

            RuleFor(x => x.Tags)
                .MaximumLength(500).WithMessage("Tags cannot exceed 500 characters");

            RuleFor(x => x.Department)
                .MaximumLength(100).WithMessage("Department cannot exceed 100 characters");

            RuleFor(x => x.ProjectCode)
                .MaximumLength(50).WithMessage("Project code cannot exceed 50 characters");

            RuleFor(x => x.Parties)
                .NotNull().WithMessage("Parties list cannot be null")
                .Must(HaveAtLeastOneExternalParty).WithMessage("Contract must have at least one external party");

            RuleForEach(x => x.Parties).SetValidator(new CreateContractPartyDtoValidator());

            // Custom rule: Auto-renewal requires end date
            RuleFor(x => x)
                .Must(x => !x.AutoRenewal || x.EndDate.HasValue)
                .WithMessage("Auto-renewal contracts must have an end date");
        }

        private static bool BeValidDate(DateTime date)
        {
            return date > DateTime.MinValue && date <= DateTime.MaxValue;
        }

        private static bool BeValidCurrency(string currency)
        {
            var validCurrencies = new[] { "USD", "EUR", "GBP", "CAD", "AUD", "JPY", "CHF", "CNY", "INR", "SEK", "NOK", "DKK" };
            return validCurrencies.Contains(currency.ToUpper());
        }

        private static bool BeValidBillingFrequency(string frequency)
        {
            var validFrequencies = new[] { "Monthly", "Quarterly", "Semi-annually", "Annually", "One-time", "Custom" };
            return validFrequencies.Contains(frequency, StringComparer.OrdinalIgnoreCase);
        }

        private static bool HaveAtLeastOneExternalParty(List<CreateContractPartyDto> parties)
        {
            return parties.Any(p => p.PartyType == (int)PartyType.External);
        }
    }

    public class UpdateContractDtoValidator : AbstractValidator<UpdateContractDto>
    {
        public UpdateContractDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.ContractTypeId)
                .NotEmpty().WithMessage("Contract type is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(BeValidDate).WithMessage("Start date must be a valid date");

            RuleFor(x => x.EndDate)
                .Must((dto, endDate) => !endDate.HasValue || endDate > dto.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 4).WithMessage("Priority must be between 1 (High) and 4 (Low)");

            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(0).When(x => x.Value.HasValue)
                .WithMessage("Contract value must be greater than or equal to 0");

            RuleFor(x => x.Currency)
                .MaximumLength(3).WithMessage("Currency code cannot exceed 3 characters")
                .Must(BeValidCurrency).When(x => !string.IsNullOrEmpty(x.Currency))
                .WithMessage("Currency must be a valid ISO currency code (USD, EUR, GBP, etc.)");

            RuleFor(x => x.BillingFrequency)
                .MaximumLength(20).WithMessage("Billing frequency cannot exceed 20 characters")
                .Must(BeValidBillingFrequency).When(x => !string.IsNullOrEmpty(x.BillingFrequency))
                .WithMessage("Billing frequency must be one of: Monthly, Quarterly, Annually, One-time");

            RuleFor(x => x.RenewalReminderDays)
                .InclusiveBetween(1, 365).WithMessage("Renewal reminder days must be between 1 and 365");

            RuleFor(x => x.AutoRenewalDurationDays)
                .InclusiveBetween(1, 3650).When(x => x.AutoRenewalDurationDays.HasValue)
                .WithMessage("Auto-renewal duration must be between 1 and 3650 days");

            RuleFor(x => x.InternalNotes)
                .MaximumLength(2000).WithMessage("Internal notes cannot exceed 2000 characters");

            RuleFor(x => x.Tags)
                .MaximumLength(500).WithMessage("Tags cannot exceed 500 characters");

            RuleFor(x => x.Department)
                .MaximumLength(100).WithMessage("Department cannot exceed 100 characters");

            RuleFor(x => x.ProjectCode)
                .MaximumLength(50).WithMessage("Project code cannot exceed 50 characters");

            // Custom rule: Auto-renewal requires end date
            RuleFor(x => x)
                .Must(x => !x.AutoRenewal || x.EndDate.HasValue)
                .WithMessage("Auto-renewal contracts must have an end date");
        }

        private static bool BeValidDate(DateTime date)
        {
            return date > DateTime.MinValue && date <= DateTime.MaxValue;
        }

        private static bool BeValidCurrency(string currency)
        {
            var validCurrencies = new[] { "USD", "EUR", "GBP", "CAD", "AUD", "JPY", "CHF", "CNY", "INR", "SEK", "NOK", "DKK" };
            return validCurrencies.Contains(currency.ToUpper());
        }

        private static bool BeValidBillingFrequency(string frequency)
        {
            var validFrequencies = new[] { "Monthly", "Quarterly", "Semi-annually", "Annually", "One-time", "Custom" };
            return validFrequencies.Contains(frequency, StringComparer.OrdinalIgnoreCase);
        }
    }

    public class CreateContractPartyDtoValidator : AbstractValidator<CreateContractPartyDto>
    {
        public CreateContractPartyDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Party name is required")
                .MaximumLength(200).WithMessage("Party name cannot exceed 200 characters");

            RuleFor(x => x.LegalName)
                .MaximumLength(200).WithMessage("Legal name cannot exceed 200 characters");

            RuleFor(x => x.PartyType)
                .Must(BeValidPartyType).WithMessage("Party type must be a valid value (0=Internal, 1=External, 2=Witness, 3=LegalRepresentative)");

            RuleFor(x => x.ContactPersonName)
                .MaximumLength(100).WithMessage("Contact person name cannot exceed 100 characters");

            RuleFor(x => x.ContactPersonTitle)
                .MaximumLength(100).WithMessage("Contact person title cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Contact email must be a valid email address")
                .MaximumLength(255).WithMessage("Contact email cannot exceed 255 characters");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Contact phone cannot exceed 20 characters")
                .Matches(@"^[\d\s\-\+\(\)\.]+$").When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Contact phone contains invalid characters");

            RuleFor(x => x.AddressLine1)
                .MaximumLength(200).WithMessage("Address line 1 cannot exceed 200 characters");

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200).WithMessage("Address line 2 cannot exceed 200 characters");

            RuleFor(x => x.City)
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.State)
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters");

            RuleFor(x => x.PostalCode)
                .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");

            RuleFor(x => x.Country)
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

            RuleFor(x => x.TaxId)
                .MaximumLength(50).WithMessage("Tax ID cannot exceed 50 characters");

            RuleFor(x => x.RegistrationNumber)
                .MaximumLength(50).WithMessage("Registration number cannot exceed 50 characters");

            RuleFor(x => x.Website)
                .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.Website))
                .WithMessage("Website must be a valid URL")
                .MaximumLength(255).WithMessage("Website cannot exceed 255 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
        }

        private static bool BeValidPartyType(int partyType)
        {
            return Enum.IsDefined(typeof(PartyType), partyType);
        }

        private static bool BeValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }

    public class CreateContractTypeDtoValidator : AbstractValidator<CreateContractTypeDto>
    {
        public CreateContractTypeDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required")
                .Matches(@"^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex color code (e.g., #007bff)");

            RuleFor(x => x.Icon)
                .MaximumLength(50).WithMessage("Icon name cannot exceed 50 characters");

            RuleFor(x => x.DefaultDurationDays)
                .InclusiveBetween(1, 3650).When(x => x.DefaultDurationDays.HasValue)
                .WithMessage("Default duration must be between 1 and 3650 days");

            RuleFor(x => x.DefaultReminderDays)
                .InclusiveBetween(1, 365).WithMessage("Default reminder days must be between 1 and 365");
        }
    }

    public class ContractStatusChangeDtoValidator : AbstractValidator<ContractStatusChangeDto>
    {
        public ContractStatusChangeDtoValidator()
        {
            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("New status is required")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters")
                .Must(BeValidStatus).WithMessage("Invalid status value");

            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");
        }

        private static bool BeValidStatus(string status)
        {
            return Enum.TryParse<ContractStatus>(status, true, out _);
        }
    }

    public class ContractSignatureDtoValidator : AbstractValidator<ContractSignatureDto>
    {
        public ContractSignatureDtoValidator()
        {
            RuleFor(x => x.PartyId)
                .NotEmpty().WithMessage("Party ID is required");

            RuleFor(x => x.SignedByName)
                .NotEmpty().WithMessage("Signer name is required")
                .MaximumLength(100).WithMessage("Signer name cannot exceed 100 characters");

            RuleFor(x => x.SignedByTitle)
                .MaximumLength(100).WithMessage("Signer title cannot exceed 100 characters");
        }
    }
}