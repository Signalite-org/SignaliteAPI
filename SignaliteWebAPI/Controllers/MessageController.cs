﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignaliteWebAPI.Application.Features.Messages.DeleteAttachment;
using SignaliteWebAPI.Application.Features.Messages.DeleteMessage;
using SignaliteWebAPI.Application.Features.Messages.GetMessageThread;
using SignaliteWebAPI.Application.Features.Messages.ModifyMessage;
using SignaliteWebAPI.Application.Features.Messages.SendMessage;
using SignaliteWebAPI.Application.Helpers;
using SignaliteWebAPI.Domain.DTOs.Messages;
using SignaliteWebAPI.Domain.Models;
using SignaliteWebAPI.Infrastructure.Extensions;
using SignaliteWebAPI.Infrastructure.Interfaces.Services;

namespace SignaliteWebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessageController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SendMessageResult>> SendMessage([FromForm] SendMessageDTO messageDto)
    {
        var command = new SendMessageCommand
        {
            SendMessageDto = messageDto,
            SenderId = User.GetUserId(),
        };
        var sendMessageResult = await mediator.Send(command);
        return Ok(sendMessageResult);
    }

    [HttpGet("{groupId}")]
    public async Task<ActionResult<List<MessageDTO>>> GetMessageThread([FromRoute] int groupId, [FromQuery] PaginationQuery paginationQuery)
    {
        var query = new GetMessageThreadQuery
        {
            GroupId = groupId,
            UserId = User.GetUserId(),
            PaginationQuery = paginationQuery
        };
        var messages = await mediator.Send(query);
        return Ok(messages);
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage([FromRoute] int messageId)
    {
        var command = new DeleteMessageCommand
        {
            MessageId = messageId,
            SenderId = User.GetUserId(),
        };
        await mediator.Send(command);
        return NoContent();
    }
    
    [HttpDelete("{messageId}/attachment")]
    public async Task<IActionResult> DeleteAttachment([FromRoute] int messageId)
    {
        var command = new DeleteAttachmentCommand
        {
            MessageId = messageId,
            SenderId = User.GetUserId(),
        };
        await mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{messageId}")]
    public async Task<IActionResult> ModifyMessage([FromRoute] int messageId, string messageContent)
    {
        var command = new ModifyMessageCommand
        {
            MessageId = messageId,
            MessageContent = messageContent,
            SenderId = User.GetUserId(),
        };
        await mediator.Send(command);
        return Created();
    }
}