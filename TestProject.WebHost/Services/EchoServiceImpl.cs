using AutoMapper;
using Grpc.Core;
using TestProject.Grpc.Contracts;
using TestProject.WebHost.Services.Internal.Models;

namespace TestProject.WebHost.Services;

public class EchoServiceImpl
    : EchoService.EchoServiceBase
{
    private readonly ILogger<EchoServiceImpl> _logger;

    private readonly IMapper _mapper;

    public EchoServiceImpl(ILogger<EchoServiceImpl> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public override Task<EchoMessage> EchoAsync (EchoMessage request, ServerCallContext context)
    {
        var echoModel = _mapper.Map<EchoModel>(request);

        _logger.LogDebug($"Полученное сообщение: {echoModel}");

        var echoMessage = _mapper.Map<EchoMessage>(request);

        return Task.FromResult(echoMessage);
    }
}
