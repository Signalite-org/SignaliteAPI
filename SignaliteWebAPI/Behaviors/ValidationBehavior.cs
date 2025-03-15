﻿using FluentValidation;
using MediatR;
using SignaliteWebAPI.Exceptions;

namespace SignaliteWebAPI.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(e => e != null).ToList();
            if (failures.Count != 0)
            {
                var errorList = failures.Select(f => f.ErrorMessage).ToList();
                throw new ValidatorException("One or more validation errors occured")
                {
                    Errors = errorList
                };
            }
        }

        return await next();
    }
}