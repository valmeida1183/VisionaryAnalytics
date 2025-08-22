using MediatR;
using SharedKernel.Primitives;

namespace Application.Abstractions.Messaging;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
