using AutoMapper;
using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer.Helper;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<BookingCreateDTO, Booking>();
        CreateMap<Booking, BookingCreateDTO>();
        CreateMap<Room, RoomDTO>();
        CreateMap<RoomDTO, Room>();
    }
}