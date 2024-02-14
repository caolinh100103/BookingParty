using Model.DTO;

namespace BusinessLogicLayer.Interfaces;

public interface IBookingService
{
    Task<ResultDTO<BookingResponseDTO>> CreateBooking(BookingCreateDTO bookingDto, string token);
}