using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Data;
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
        var id = request.Id;

        var subTestEntity = await _uow.TestSubEntityRepo.GetByIdAsync(id, context.CancellationToken);

        if (subTestEntity == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Не найдена сущность {typeof(SubTestEntity).Name} с id: {id}"));
        }

        return _mapper.Map<GetSubTestEntityByIdResponse>(subTestEntity);
    }

    public override async Task<UpdateSubTestEntityNameResponse> UpdateSubTestEntityNameAsync(UpdateSubTestEntityNameRequest request, ServerCallContext context)
    {
        var id = request.Id;
        var newName = request.NewName;

        await _uow.BeginTransactionAsync(IsolationLevel.ReadCommitted, context.CancellationToken);

        try
        {
            var rowsAffected = await _uow.TestSubEntityRepo.UpdateNameAsync(id, newName, context.CancellationToken);

            if (rowsAffected == 0)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Не удалось обновить поле {nameof(SubTestEntity.Name)} у сущности {typeof(SubTestEntity).Name} с id: {id}"));
            }

            var subTestEntity = await _uow.TestSubEntityRepo.GetByIdAsync(id, context.CancellationToken);

            if (subTestEntity == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Не найдена сущность {typeof(SubTestEntity).Name} с id: {id}"));
            }

            await _uow.CommitTransactionAsync(context.CancellationToken);

            return _mapper.Map<UpdateSubTestEntityNameResponse>(subTestEntity);
        }
        catch(Exception)
        {
            await _uow.RollbackTransactionAsync(CancellationToken.None);

            throw;
        }             
    }

    public override async Task<Empty> DeleteSubTestEntityAsync(DeleteSubTestEntityRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}
