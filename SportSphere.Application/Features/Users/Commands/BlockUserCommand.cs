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
    public class BlockUserCommand : IRequest<ApiResponse<bool>>
    {
        public string UserIdToBlock { get; set; } = null!;

    }
    public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
    {
        public BlockUserCommandValidator()
        {
            RuleFor(x => x.UserIdToBlock)
                .NotEmpty().WithMessage("UserId to block is required");
        }
    }
    public class BlockUserHandler : IRequestHandler<BlockUserCommand, ApiResponse<bool>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<UserBlock> blockRepo;
        private readonly UserManager<ApplicationUser> userManager;

        public BlockUserHandler(IHttpContextAccessor httpContextAccessor, IGenericRepository<UserBlock> blockRepo, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            this.blockRepo = blockRepo;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<bool>> Handle(BlockUserCommand request, CancellationToken cancellationToken)
        {
            string? currentUserId = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return ApiResponse<bool>.Error(ErrorCode.Unauthorized, "You must login first");
            }

            var UserToblock = await userManager.FindByIdAsync(request.UserIdToBlock);

            if (UserToblock == null)
            {
                return ApiResponse<bool>.Error(ErrorCode.NotFound, "Please check the user and try again");

            }

            if (currentUserId == request.UserIdToBlock)
                return ApiResponse<bool>.Error(ErrorCode.BadRequest, "You cannot block yourself");

            var existingBlock = await blockRepo.GetFilteredFirstOrDefaultAsync(b => b.BlockerId == currentUserId && b.BlockedId == request.UserIdToBlock);

            if (existingBlock != null)
                return ApiResponse<bool>.Error(ErrorCode.Conflict, "User is already blocked");

            var block = new UserBlock
            {
                BlockerId = currentUserId,
                BlockedId = request.UserIdToBlock,
                CreatedAt = DateTime.UtcNow
            };

            await blockRepo.AddAsync(block);
            await blockRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "User blocked successfully");
        }
    }



}
