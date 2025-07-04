using AutoMapper;
using MediatR;
using SignaliteWebAPI.Domain.DTOs.Users;
using SignaliteWebAPI.Infrastructure.Exceptions;
using SignaliteWebAPI.Infrastructure.Interfaces.Repositories;
using SignaliteWebAPI.Infrastructure.Interfaces.Services;

namespace SignaliteWebAPI.Application.Features.Users.DeleteBackgroundPhoto;

public class DeleteBackgroundPhotoHandler(
    IUserRepository userRepository,
    IPhotoRepository photoRepository,
    IFriendsRepository friendsRepository,
    IMediaService mediaService,
    INotificationsService notificationsService,
    IMapper mapper
    ): IRequestHandler<DeleteBackgroundPhotoCommand>
{
    public async Task Handle(DeleteBackgroundPhotoCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithBackgroundPhotoAsync(request.UserId);
        if (user?.BackgroundPhoto == null)
            throw new NotFoundException("Background photo is missing");

        // delete photo from cloud storage
        var deletionResult = await mediaService.DeleteMediaAsync(user.BackgroundPhoto.PublicId);
        if (deletionResult.Error != null) 
            throw new CloudinaryException(deletionResult.Error.Message);

        // remove photo from database
        await photoRepository.RemovePhotoAsync(user.BackgroundPhoto.Id);
        
        var friendsToMap = await friendsRepository.GetUserFriends(request.UserId);
        var usersToNotify = mapper.Map<List<UserBasicInfo>>(friendsToMap);
        
        var userDto = mapper.Map<UserDTO>(user);
        await notificationsService.UserUpdated(userDto, userDto.Username, usersToNotify);
    }
}