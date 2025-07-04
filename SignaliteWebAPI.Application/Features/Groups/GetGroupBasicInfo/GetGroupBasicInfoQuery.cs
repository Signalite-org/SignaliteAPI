﻿using MediatR;
using SignaliteWebAPI.Domain.DTOs.Groups;

namespace SignaliteWebAPI.Application.Features.Groups.GetGroupBasicInfo;

public class GetGroupBasicInfoQuery : IRequest<GroupBasicInfoDTO>
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
}
