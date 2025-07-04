using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TestProject.App.Infra.Contracts;
using TestProject.Core.Entities;
using TestProject.Grpc.Contracts;
using static TestProject.Grpc.Contracts.GetAllTestEntitiesResponse.Types;

namespace TestProject.WebHost.Services;

public class TestEntityServiceImpl(IUnitOfWork uow, IMapper mapper) : TestEntityService.TestEntityServiceBase
{
    private readonly IUnitOfWork _uow = uow;

    private readonly IMapper _mapper = mapper;

    public override async Task<CreateTestEntityResponse> CreateTestEntityAsync(CreateTestEntityRequest request, ServerCallContext context)
    {
        var testEntity = _mapper.Map<TestEntity>(request);

        testEntity.Id = await _uow.TestEntityRepo.CreateAsync(testEntity, context.CancellationToken);

        var createTestEntityResponse = _mapper.Map<CreateTestEntityResponse>(testEntity);

        return createTestEntityResponse;
    }

    public override async Task<GetByIdTestEntityResponse> GetByIdTestEntityAsync(GetByIdTestEntityRequest request, ServerCallContext context)
    {
        var id = request.Id;

        var testEntity = await _uow.TestEntityRepo.GetByIdAsync(id, context.CancellationToken);

        if(testEntity == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Не найдена сущность {typeof(TestEntity).Name} с id: {id}"));
        }

        return _mapper.Map<GetByIdTestEntityResponse>(testEntity);
    }

    public override async Task<GetAllTestEntitiesResponse> GetAllTestEntitiesAsync(Empty request, ServerCallContext context)
    {
        var testEntities = await _uow.TestEntityRepo.GetAllAsync(context.CancellationToken);

        var response = new GetAllTestEntitiesResponse();

        response.TestEntities.AddRange(testEntities.Select(_mapper.Map<GetAllTestEntitiesDto>));

        return  response;
    }
}
