using AutoMapper;
using Grpc.Core;
using TestProject.Grpc.Contracts;
using TestProject.WebHost.Services.Internal;
using TestProject.WebHost.Services.Internal.Models;

namespace TestProject.WebHost.Services;

public class AuthGrpcServiceImpl(IMapper mapper, AuthService authService) : AuthGrpcService.AuthGrpcServiceBase
{
    private readonly IMapper _mapper = mapper;

    private readonly AuthService _authService = authService;

    public override Task<AuthResponse> AuthAsync(AuthRequest request, ServerCallContext context)
    {
        var authData = _mapper.Map<AuthData>(request);

        if (!_authService.TryGetToken(authData, out var token))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Неверное имя пользователя или пароль"));
        }

        return Task.FromResult<AuthResponse>(new AuthResponse { Token = token });
    }
}
