using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer.Interfaces;

public interface IBookingService
{
    Task<ResultDTO<BookingResponseDTO>> CreateBooking(BookingCreateDTO bookingDto, string token);
    Task<bool> CancelByPH(int BookingId);
    Task<ResultDTO<ICollection<BookingResponseDTO>>> GetAllBooking();
    Task<ResultDTO<ICollection<BookingDetailDTO>>> GetAllBookingDetailByBookingId(int bookingId);
}