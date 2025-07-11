using Grpc.Core;
using Grpc.Core.Interceptors;
using TestProject.WebHost.Services.Internal;

namespace TestProject.WebHost.Interceptors;

public class RequestCounterInterceptor(RequestCounterService counterService, ILogger<RequestCounterInterceptor> logger = null) : Interceptor
{
    private readonly RequestCounterService _counterService = counterService;

    private readonly ILogger<RequestCounterInterceptor> _logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _counterService.AddNewRequest();

        _logger.LogInformation($"Запрос на метод: {context.Method}, текущее кол-во запросов: {_counterService.RequestCounter}");

        var response = await continuation(request, context);

        return response;
    }
}
