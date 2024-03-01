﻿using AutoMapper;
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
        }
    }
}
