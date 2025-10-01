using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.WebApi.Responses;
using SportSphere.Application.Features.Reports.DTO;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using SportSphere.Domain.Interfaces;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Features.Reports.Commands
{
    public class ReportContentCommand : IRequest<ApiResponse<bool>>
    {
        public ReportContentCommandDTO ReportContentCommandData { get; set; }
    }
    public class ReportContentCommandValidator : AbstractValidator<ReportContentCommand>
    {
        public ReportContentCommandValidator()
        {
            RuleFor(x => x.ReportContentCommandData.ReporterId).NotEmpty().WithMessage("ReporterId is required");
            RuleFor(x => x.ReportContentCommandData.Reason).NotEmpty().WithMessage("Reason is required");

            RuleFor(x => new { x.ReportContentCommandData.ReportedUserId, x.ReportContentCommandData.ReportedPostId, x.ReportContentCommandData.ReportedCommentId })
                .Must(x => !string.IsNullOrEmpty(x.ReportedUserId) || x.ReportedPostId.HasValue || x.ReportedCommentId.HasValue)
                .WithMessage("At least one target (user, post, or comment) must be provided");
        }
    }
    public class ReportContentHandler : IRequestHandler<ReportContentCommand, ApiResponse<bool>>
    {
        private readonly IValidator<ReportContentCommand> _validator;
        private readonly IGenericRepository<Report> reportRepo;

        public ReportContentHandler(IValidator<ReportContentCommand> validator, IGenericRepository<Report> reportRepo)
        {
            _validator = validator;
            this.reportRepo = reportRepo;
        }

        public async Task<ApiResponse<bool>> Handle(ReportContentCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<bool>.Error(ErrorCode.ValidationError, errors);
            }

            var report = new Report
            {
                ReporterId = request.ReportContentCommandData.ReporterId,
                ReportedUserId = request.ReportContentCommandData.ReportedUserId,
                ReportedPostId = request.ReportContentCommandData.ReportedPostId,
                ReportedCommentId = request.ReportContentCommandData.ReportedCommentId,
                Reason = request.ReportContentCommandData.Reason,
                Details = request.ReportContentCommandData.Details,
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await reportRepo.AddAsync(report);
            await reportRepo.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Report submitted successfully");
        }
    }



}
