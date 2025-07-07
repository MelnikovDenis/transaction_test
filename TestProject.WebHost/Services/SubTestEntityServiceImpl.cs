using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TestProject.App.Infra.Contracts;
using TestProject.Core.Entities;
using TestProject.Grpc.Contracts;

namespace TestProject.WebHost.Services;

public class SubTestEntityServiceImpl(IUnitOfWork uow, IMapper mapper) : SubTestEntityService.SubTestEntityServiceBase
{
    private readonly IUnitOfWork _uow = uow;

    private readonly IMapper _mapper = mapper;

    public override async Task<CreateSubTestEntityResponse> CreateSubTestEntityAsync(CreateSubTestEntityRequest request, ServerCallContext context)
    {
        var subTestEntity = _mapper.Map<SubTestEntity>(request);

        subTestEntity.Id = await _uow.TestSubEntityRepo.CreateAsync(subTestEntity, context.CancellationToken);

        return _mapper.Map<CreateSubTestEntityResponse>(subTestEntity);
    }

    public override async Task<GetSubTestEntityByIdResponse> GetSubTestEntityByIdAsync(GetSubTestEntityByIdRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override async Task<UpdateSubTestEntityNameResponse> UpdateSubTestEntityNameAsync(UpdateSubTestEntityNameRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override async Task<Empty> DeleteSubTestEntityAsync(DeleteSubTestEntityRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}
