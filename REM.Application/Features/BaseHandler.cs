namespace REM.Application.Features;

public abstract class BaseHandler<TRequest, TResponse>() : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected bool IsArabic => CultureHelper.IsArabic;

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
