using Abp.Application.Services;
using Castle.Core;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Booking
{
    public interface IBookingAppService : IApplicationService
    {
        BookingEntity GetBooking(string statusIds, string startdate, string endDate, string branchesId, int? tenantId = null, long? userId = null, string userIds = null);
        BookingModel GetBookingById(long bookingId);
        Task<string> CreateBookingAsync(BookingModel booking);
        Task<string> UpdateBooking(BookingModel booking);
        bool DeleteBooking(long bookingId);
        Task<string> SendBookingTemplateAsync(long bookingId);
        Task GenerateBookingTemplatesAsync(int tenantId);
        bool CheckBookingCapacity(int tenantId, string bookingDateTime, long userId);
        void CreateBookingContact(BookingContact bookingContact);
        List<BookingModel> GetContactBooking(int contactId, int tenantId, int languageId);
        string GetContactBookingTime(DateTime bookingDateTime, int languageId);
        Dictionary<string, string> GetBookingDay(long userId, int tenantId);
        List<string> GetBookingTime(string date, int tenantId, long userId);
        List<CaptionDto> GetBookingCaption(int textResourceId, int tenantId);
        string UpdateBookingIsNew(int bookingId);
        List<UserListDto> GetBookingUser(int tenantId, string userIds = null);
        bool IsAvailableBookingDate(string bookingdate, long userId);
        CreateOrEditTemplateMessageDto GetTemplateMessageById(int id);
        List<AreaDto> GetBranchesByUserId(int UserId, bool isAdmin);
        List<UserListDto> GetUserListByUserIds(string UserIds);

        List<BookingOffDays> GetBookingOffDays(long userId);
        bool CreateBookingOffDays(BookingOffDaysEntity bookingOffDaysEntity);
    }
}
