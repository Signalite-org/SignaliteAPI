using FluentValidation;

namespace SignaliteWebAPI.Application.Features.Users.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        const int minLengthPassword = 8;

        RuleFor(x => x.ChangePasswordDto.OldPassword).NotNull().WithMessage("Old password is required.")
            .NotEmpty().WithMessage("Old password is empty.");
        
        RuleFor(x => x.ChangePasswordDto.NewPassword).NotNull().WithMessage("New password is required.")
            .NotEmpty().WithMessage("New password is empty.")
            .MinimumLength(minLengthPassword).WithMessage("New password must be at least 8 characters long.")
            .Matches(@"[a-z]").WithMessage("New password  must contain at least one lowercase letter.")
            .Matches(@"[A-Z]").WithMessage("New password  must contain at least one uppercase letter.")
            .Matches(@"[@$!%*?&\.\^]").WithMessage("New password  must contain at least one special character.")
            .Matches(@"\d").WithMessage("New password must contain at least one number.");
    }
}