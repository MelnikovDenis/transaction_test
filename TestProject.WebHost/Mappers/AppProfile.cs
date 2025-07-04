using AutoMapper;
using TestProject.Core.Entities;
using TestProject.Grpc.Contracts;
using static TestProject.Grpc.Contracts.GetAllTestEntitiesResponse.Types;
using static TestProject.Grpc.Contracts.GetAllTestEntitiesResponse.Types.GetAllTestEntitiesDto.Types;

namespace TestProject.WebHost.Mappers;

public class AppProfile : Profile
{
    public AppProfile() : base()
    {
        CreateMap<CreateTestEntityRequest, TestEntity>();

        CreateMap<TestEntity, CreateTestEntityResponse>();

        CreateMap<TestEntity, GetByIdTestEntityResponse>();

        CreateMap<SubTestEntity, GetAllSubTestEntitiesDto>();

        CreateMap<TestEntity, GetAllTestEntitiesDto>();
    }
}
