using AutoMapper;
using TestProject.Core.Entities;
using TestProject.Grpc.Contracts;
using static TestProject.Grpc.Contracts.AddSumResponse.Types;
using static TestProject.Grpc.Contracts.GetAllTestEntitiesResponse.Types;
using static TestProject.Grpc.Contracts.GetAllTestEntitiesResponse.Types.GetAllTestEntitiesDto.Types;
using static TestProject.Grpc.Contracts.GetByIdTestEntityResponse.Types;

namespace TestProject.WebHost.Mappers;

public class AppProfile : Profile
{
    public AppProfile() : base()
    {
        CreateMap<CreateTestEntityRequest, TestEntity>();

        CreateMap<TestEntity, CreateTestEntityResponse>();

        CreateMap<SubTestEntity, GetByIdSubTestEntityDto>();
        CreateMap<TestEntity, GetByIdTestEntityResponse>();  

        CreateMap<SubTestEntity, GetAllSubTestEntitiesDto>();
        CreateMap<TestEntity, GetAllTestEntitiesDto>();

        CreateMap<SubTestEntity, AddSumSubTestEntityDto>();
        CreateMap<TestEntity, AddSumResponse>();

        CreateMap<CreateSubTestEntityRequest, SubTestEntity>();
        CreateMap<SubTestEntity, CreateSubTestEntityResponse>();
    }
}
