using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Users.Commands
{
    public class UnblockUserCommand : IRequest<ApiResponse<bool>>
    {
        public string UserIDtoUnBlocked { get; set; } = null!;
    }
    public class UnblockUserCommandValidator : AbstractValidator<UnblockUserCommand>
    {
        public UnblockUserCommandValidator()
        {
            RuleFor(x => x.UserIDtoUnBlocked).NotEmpty().WithMessage("UserIDtoUnBlocked is required");
        }
    }
    public class UnblockUserHandler : IRequestHandler<UnblockUserCommand, ApiResponse<bool>>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IValidator<UnblockUserCommand> _validator;
        private readonly IGenericRepository<UserBlock> blockRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public UnblockUserHandler(IHttpContextAccessor httpContextAccessor, IValidator<UnblockUserCommand> validator, IGenericRepository<UserBlock> blockRepo, UserManager<ApplicationUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            _validator = validator;
            this.blockRepo = blockRepo;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<bool>> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }


            string? currentUserId = httpContextAccessor.HttpContext?.User?
.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You must login first");
            }


            var UserToblock = await userManager.FindByIdAsync(request.UserIDtoUnBlocked);

            if (UserToblock == null)
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Please check the user and try again");

            }

            if (currentUserId == request.UserIDtoUnBlocked)
                return ApiResponse<bool>.Error(ErrorCode.BadRequest, "You cannot block yourself");








            // check if block exists
            var block = await blockRepo.GetFilteredFirstOrDefaultAsync(b => b.BlockerId == currentUserId && b.BlockedId == request.UserIDtoUnBlocked);

            if (block == null)
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "This User is already unblocked");

            // remove block
            blockRepo.ActualDelete(block);
            await blockRepo.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "User unblocked successfully");
        }
    }
}
