using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace TestProject.WebHost.Mappers;


public class TimestampProfile : Profile
{
    public TimestampProfile()
    {
        CreateMap<DateTime, Timestamp>().ConvertUsing(src => Timestamp.FromDateTime(src.ToUniversalTime()));

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        CreateMap<DateTime?, Timestamp>().ConvertUsing(src =>
            src.HasValue 
            ? Timestamp.FromDateTime(src.Value.ToUniversalTime()) 
            : null);
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.

        CreateMap<Timestamp, DateTime>().ConvertUsing(src => src.ToDateTime().ToLocalTime());

        CreateMap<Timestamp, DateTime?>().ConvertUsing(src => src == null 
            ? null 
            : src.ToDateTime().ToUniversalTime());
    }
}