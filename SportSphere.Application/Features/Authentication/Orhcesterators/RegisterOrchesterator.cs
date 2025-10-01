using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiXSquad.Application.Authentication.DTOs;
using MagiXSquad.Application.Services;
using MagiXSquad.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SportSphere.Application.Features.Authentication.Commands;
using SportSphere.Application.Features.Authentication.DTOs;
using SportSphere.Application.Features.Profile.Commands;
using SportSphere.Application.Features.Profile.DTOs;
using SportSphere.Domain.Entities;
using SportSphere.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportSphere.Application.Features.Authentication.Orhcesterators
{
    public class RegisterOrchesterator : IRequest<ApiResponse<RegisterRetDto>>
    {
        public RegisterOrchesteratorDTO RegisterOrchesteratorData { get; set; }

    }


    public class RegisterOrchesteratorHandler : IRequestHandler<RegisterOrchesterator, ApiResponse<RegisterRetDto>>
    {
        private readonly IMediator mediator;
        private readonly UserManager<ApplicationUser> userManager;

        public RegisterOrchesteratorHandler(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            this.mediator = mediator;
            this.userManager = userManager;
        }

        public async Task<ApiResponse<RegisterRetDto>> Handle(RegisterOrchesterator request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await mediator.Send(new RegisterCommand { RegisterData = request.RegisterOrchesteratorData.RegisterData });
                if (result.Success)
                {
                    var profileCommand = request.RegisterOrchesteratorData.Profile.MapOnto(new AddProfileDto());
                    // Assuming RegisterOrchesteratorData.Profile is of type RegisterOrchesteratorProfile
                    //var profileCommand = request.RegisterOrchesteratorData.Profile.Map<AddProfileDto>();

                    profileCommand.UserId = result.Data.UserId;
                    //request.RegisterOrchesteratorData.Profile.UserId = result.Data.UserId;
                    var response = await mediator.Send(new AddProfileCommand { Profile = profileCommand });
                    if (response.Success)
                    {
                        var command = new UploadProfilePictureCommand
                        {
                            File = request.RegisterOrchesteratorData.File,
                            UserId = result.Data.UserId
                        };

                        var mediaId = await mediator.Send(command);
                        if (mediaId.Success)
                        {
                            var FinalResponse = new RegisterRetDto();
                            var sendMailres = await mediator.Send(new EmailVerificationCommand { Email = result.Data.Email! });
                            if (sendMailres.Success)
                            {
                                FinalResponse = new RegisterRetDto
                                {
                                    UserId = result.Data.UserId,
                                    Email = result.Data.Email!,
                                    Role = result.Data.Role,
                                    Message = "Registration successful. Please check your email to verify your account.",
                                };
                            }
                            else
                            {
                                FinalResponse = new RegisterRetDto
                                {
                                    UserId = result.Data.UserId,
                                    Email = result.Data.Email!,
                                    Role = result.Data.Role,
                                    Message = "Registration successful. Verification email failed to send try to login and resend it.",
                                };
                            }


                            return ApiResponse<RegisterRetDto>.Ok(FinalResponse, FinalResponse.Message);
                        }
                    }
                }
                else
                {
                    //userManager.Users.Where(u => u.Id.Equals(result.Data.UserId)).ExecuteDelete();
                    if (result.ErrorCode != ErrorCode.Conflict)
                    {
                        var user = userManager.Users.FirstOrDefault(u => u.Id.Equals(result.Data.UserId));
                        if (user != null)
                        {

                            var res = await userManager.DeleteAsync(user);
                        }

                    }
                }
                return ApiResponse<RegisterRetDto>.Error(ErrorCode.UnknownError, result.Message);


            }
            catch (Exception e)
            {

                return ApiResponse<RegisterRetDto>.Error(ErrorCode.UnknownError, $"Unexpected error!=> {e.Message}");
            }

        }
    }




}
