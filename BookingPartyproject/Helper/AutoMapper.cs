using AutoMapper;
using Model.DTO;
using Model.Entity;

namespace BookingPartyproject.Helper
{
    public class AutoMapper : Profile
    {
        public AutoMapper() 
        {
            CreateMap<ServiceDTO, Service>();
            CreateMap<Service, ServiceDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<RoomDTO, Room>();
            CreateMap<Room, RoomDTO>();
            CreateMap<DepositDTO, Deposit>();
            CreateMap<Deposit, DepositDTO>();
            CreateMap<TransactionCreatedDTO, TransactionHistory>();
            CreateMap<UserReponseDTO, User>();
            CreateMap<User, UserReponseDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();
            CreateMap<NotificationResponseDTO, Notification>();
            CreateMap<Notification, NotificationResponseDTO>();
            CreateMap<BookingDetail, BookingDetailDTO>();
            CreateMap<BookingDetailDTO, BookingDetail>();
            CreateMap<TransactionHistory, TransactionCreatedDTO>();
            CreateMap<Booking, BookingResponseDTO>();
            CreateMap<BookingResponseDTO, Booking>();
            CreateMap<ImageDTO, Image>();
            CreateMap<Image, ImageDTO>();
            CreateMap<Service, ServiceCreatedDTO>();
            CreateMap<ServiceCreatedDTO, Service>();
            CreateMap<Room, RoomCreatedDTO>();
            CreateMap<RoomCreatedDTO, Room>();
            CreateMap<RoleResponseDTO, Role>();
            CreateMap<Role, RoleResponseDTO>();
            CreateMap<Room, RoomResponse>();
            CreateMap<RoomResponse, Room>();
            CreateMap<RegisterDTO , User>();
            CreateMap<User , RegisterDTO>();
            CreateMap<Feedback , FeedbackReponseDTO>();
            CreateMap<FeedbackReponseDTO , Feedback>();
            CreateMap<Service , ServiceResponseDTO>();
            CreateMap<ServiceResponseDTO , Service>();
            CreateMap<WithDrawalReponseDTO , WithdrawalRequest>();
            CreateMap<WithdrawalRequest , WithDrawalReponseDTO>();
            CreateMap<WithdrawalRequest , WithdrawalCreatedDTO>();
            CreateMap<WithdrawalCreatedDTO , WithdrawalRequest>();
            CreateMap<FeedbackCreatedDTO , Feedback>();
            CreateMap<Feedback , FeedbackCreatedDTO>();
        }
    }
}
