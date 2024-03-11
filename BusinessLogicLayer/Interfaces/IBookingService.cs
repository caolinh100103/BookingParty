using Model.DTO;
using Model.Entity;

namespace BusinessLogicLayer.Interfaces;

public interface IBookingService
{
    Task<ResultDTO<BookingResponseDTO>> CreateBooking(BookingCreateDTO bookingDto, string token);
    Task<ResultDTO<bool>> CancelByCustomer(int BookingId);
    Task<ResultDTO<ICollection<BookingResponseDTO>>> GetAllBooking();
    Task<ResultDTO<ICollection<BookingDetailDTO>>> GetAllBookingDetailByBookingId(int bookingId);
    Task<ResultDTO<int>> FinishBooking(int bookingId);
    Task<ResultDTO<ICollection<BookingResponseDTO>>> GetAllBookingByUserId(int userId);
    Task<ResultDTO<bool>> CancelByPartyHost(int BookingId);
}