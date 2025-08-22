﻿using MediatR;
using SharedKernel.Primitives;

namespace Application.Abstractions.Messaging;
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;

