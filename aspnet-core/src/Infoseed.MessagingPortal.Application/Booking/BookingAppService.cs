using Abp.Application.Services.Dto;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Framework.Data;
using Infoseed.MessagingPortal.Areas;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Booking;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.General.Dto;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;

namespace Infoseed.MessagingPortal.booking
{
    public class BookingAppService : MessagingPortalAppServiceBase, IBookingAppService
    {
        private readonly IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        private readonly IAreasAppService _areasAppService;
        private readonly IUserAppService _iUserAppService;
        public BookingAppService(IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService, IAreasAppService areasAppServic, IUserAppService iUserAppService)
        {
            _whatsAppMessageTemplateAppService= whatsAppMessageTemplateAppService;
            _areasAppService=areasAppServic;
            _iUserAppService=iUserAppService;
        }
        public BookingEntity GetBooking(string statusIds, string startdate, string endDate, string branchesId, int? tenantId = null, long? userId = null, string userIds = null)
        {
            return getBooking(statusIds, startdate, endDate, branchesId, tenantId, userId, userIds);
        }

        public BookingModel GetBookingById(long bookingId)
        {
            return getBookingById(bookingId);
        }

        public async Task<string> CreateBookingAsync(BookingModel booking)
        {
            return await createBookingAsync(booking);
        }
        public async Task<string> UpdateBooking(BookingModel booking)
        {

            return await updateBookingAsync(booking);
        }
        public string UpdateBookingIsNew(int bookingId)
        {
            return updateBookingIsNew(bookingId);
        }


        public bool DeleteBooking(long bookingId)
        {
            return deleteBooking(bookingId);
        }

        public async Task<string> SendBookingTemplateAsync(long bookingId)
        {
            return await sendBookingTemplateAsync(bookingId);
        }
        public async Task GenerateBookingTemplatesAsync(int tenantId)
        {
            await generateBookingTemplatesAsync(tenantId);
        }
        public bool CheckBookingCapacity(int tenantId, string bookingDateTime, long userId)
        {
            return checkBookingCapacity(tenantId, bookingDateTime, userId);
        }

        public void CreateBookingContact(BookingContact bookingContact)
        {
            createBookingContact(bookingContact);
        }

        public List<BookingModel> GetContactBooking(int contactId, int tenantId, int languageId)
        {
            return getContactBooking(contactId, tenantId, languageId);
        }
        public string GetContactBookingTime(DateTime bookingDateTime, int languageId)
        {
            return getContactBookingTime(bookingDateTime, languageId);
        }
        public bool CheckIfNotExistBookingTemplate(int tenantId, string name)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingTemplateCheckGet;
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@TenantId",tenantId),
                    new SqlParameter("@TemplateName",name),
                };
                var OutputParameter = new SqlParameter();
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                OutputParameter.SqlDbType = SqlDbType.Bit;
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<string, string> GetBookingDay(long userId, int languageId)
        {
            return getBookingDay(userId, languageId);
        }
        public List<string> GetBookingTime(string date, int tenantId, long userId)
        {
            return getBookingTime(date, tenantId, userId);
        }

        public List<CaptionDto> GetBookingCaption(int textResourceId, int tenantId)
        {
            return getBookingCaption(textResourceId, tenantId);
        }



        public List<UserListDto> GetBookingUser(int tenantId, string userIds = null)
        {

            List<UserListDto> userListDtos = new List<UserListDto>();

            var users = _iUserAppService.GetBookingUsers(tenantId, userIds).Result;


            foreach (var us in users)
            {
                var role = GetRole(tenantId);
                var userRole = GetUserRole(tenantId, us.Id.ToString()).Distinct();

                foreach (var item in userRole)
                {
                    var uR = role.Where(x => x.Id==item.RoleId).FirstOrDefault();
                    if (uR!=null)
                    {
                        if (uR.Name!="Admin")
                        {
                            userListDtos.Add(us);
                        }

                    }

                }



            }

            return userListDtos;

        }

        public CreateOrEditTemplateMessageDto GetTemplateMessageById(int id)
        {
            return getTemplateMessageById(id);
        }

        public List<AreaDto> GetBranchesByUserId(int UserId, bool isAdmin)
        {
            return getBranchesByUserId(UserId, isAdmin);
        }

        public List<UserListDto> GetUserListByUserIds(string UserIds)
        {
            List<UserListDto> userListDtos = new List<UserListDto>();
            var users = _iUserAppService.GetUsersBot(AbpSession.TenantId, UserIds).Result;


            foreach (var us in users)
            {
                var role = GetRole(AbpSession.TenantId.Value);
                var userRole = GetUserRole(AbpSession.TenantId.Value, us.Id.ToString()).Distinct();

                foreach (var item in userRole)
                {
                    var uR = role.Where(x => x.Id==item.RoleId).FirstOrDefault();
                    if (uR!=null)
                    {
                        if (uR.Name!="Admin")
                        {
                            userListDtos.Add(us);
                        }

                    }

                }



            }

            return userListDtos;
        }

        public List<BookingOffDays> GetBookingOffDays(long userId)
        {
            return getBookingOffDays(userId);
        }
        public bool CreateBookingOffDays(BookingOffDaysEntity bookingOffDaysEntity)
        {
            return createBookingOffDays(bookingOffDaysEntity);
        }



        #region Private Methods
        private BookingEntity getBooking(string statusIds, string startdate, string endDate, string branchesId, int? tenantId = null, long? userId = null, string userIds = null)
        {
            try
            {
                tenantId ??= AbpSession.TenantId.Value;
                userId ??=  AbpSession.UserId.Value;
                string AreaIds = "";
                bool isAdmin = false;
                bool isBot = false;
                if (AbpSession.TenantId!=null)
                {
                    NullableIdDto<long> input = new NullableIdDto<long>();
                    input.Id=userId.Value;
                    var userm = _iUserAppService.GetUserForEdit(input).Result;
                    isAdmin = ISRoleAdmin(tenantId.Value, userId.ToString());
                    AreaIds = userm.User.AreaIds;



                }
                else
                {
                    isBot=true;
                }


                if (isAdmin)
                {
                    userId=0;

                    if (branchesId!=null)
                    {
                        AreaIds=branchesId;
                    }

                }






                DateTime start = new DateTime();
                DateTime end = new DateTime();

                if (DateTime.TryParseExact(startdate, "d MMM yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out start) &&
                    DateTime.TryParseExact(endDate, "d MMM yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out end))
                {

                }
                else
                {
                    start = DateTime.ParseExact(startdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    end = DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    //var x = new DateTime(2023, 3, 26);
                }


                start = start.AddHours(AppSettingsModel.DivHour);
                end = end.AddHours(AppSettingsModel.DivHour);

                BookingEntity Booking = new BookingEntity();
                var SP_Name = Constants.Booking.SP_BookingsGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@TenantId",tenantId),
                    new SqlParameter("@StatusIds",statusIds),
                    new SqlParameter("@StartDate",start),
                    new SqlParameter("@EndDate",end),
                    new SqlParameter("@UserId",userId),
                    new SqlParameter("@UserIds",userIds),
                    new SqlParameter("@AreaIds",AreaIds),
                    new SqlParameter("@isAdmin",isAdmin),
                    new SqlParameter("@isBot",isBot),
                };

                var OutputTotalPending = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalPending",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputTotalPending);

                var OutputTotalConfirmed = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalConfirmed",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputTotalConfirmed);

                var OutputTotalBooked = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalBooked",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputTotalBooked);

                var OutputTotalCanceled = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCanceled",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputTotalCanceled);

                var OutputTotalDelete = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalDelete",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputTotalDelete);

                var OutputTotalCount = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputTotalCount);

                Booking.lstBookingModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBooking, AppSettingsModel.ConnectionStrings).ToList();
                Booking.TotalPending = (int)OutputTotalPending.Value;
                Booking.TotalConfirmed = (int)OutputTotalConfirmed.Value;
                Booking.TotalBooked = (int)OutputTotalBooked.Value;
                Booking.TotalCanceled = (int)OutputTotalCanceled.Value;
                Booking.TotalDelete = (int)OutputTotalDelete.Value;
                Booking.TotalCount = (int)OutputTotalCount.Value;

                return Booking;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private BookingModel getBookingById(long bookingId)
        {
            try
            {
                BookingModel Booking = new BookingModel();
                var SP_Name = Constants.Booking.SP_BookingByIdGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@BookingId",bookingId)
                };

                Booking = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBooking, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return Booking;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<BookingModel> getContactBooking(int contactId, int tenantId, int languageId)
        {
            try
            {
                List<BookingModel> Booking = new List<BookingModel>();
                var SP_Name = Constants.Booking.SP_BookingContactGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@TenantId",tenantId),
                    new SqlParameter("@ContactId",contactId),
                };

                Booking = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBooking, AppSettingsModel.ConnectionStrings).ToList();
                foreach (var item in Booking)
                {
                    item.ContactBookingTime = getContactBookingTime(item.BookingDateTime, languageId);
                }

                return Booking;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string getContactBookingTime(DateTime bookingDateTime, int languageId)
        {
            string date = bookingDateTime.ToString("d/MM");
            string time = bookingDateTime.ToString("h:mm");
            string period = bookingDateTime.ToString("tt", new CultureInfo("en-US"));
            string dayName = bookingDateTime.ToString("ddd");


            if (languageId == 1)
            {
                period = bookingDateTime.ToString("tt", new CultureInfo("ar-AE"));
                switch (bookingDateTime.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        dayName = "احد";
                        break;
                    case DayOfWeek.Monday:
                        dayName = "اثنين";
                        break;
                    case DayOfWeek.Tuesday:
                        dayName = "ثلاثاء";
                        break;
                    case DayOfWeek.Wednesday:
                        dayName = "اربعاء";
                        break;
                    case DayOfWeek.Thursday:
                        dayName = "خميس";
                        break;
                    case DayOfWeek.Friday:
                        dayName = "جمعة";
                        break;
                    case DayOfWeek.Saturday:
                        dayName = "سبت";
                        break;
                    default:
                        break;
                }

            }

            string result = dayName + " " + date + " " + time+period;
            return result;

        }
        private async Task<string> createBookingAsync(BookingModel booking)
        {
            try
            {
                if (booking.TenantId == 0)
                {
                    booking.TenantId = AbpSession.TenantId.Value;
                }

                if (booking.BookingTypeId == 2)
                {
                    booking.BookingDateTime = DateTime.ParseExact(booking.BookingDate + " " + booking.BookingTime, "d MMM yy h:mm tt", CultureInfo.InvariantCulture);
                }
                else
                {
                    booking.BookingDateTime = DateTime.Parse(booking.BookingDateTimeString, CultureInfo.InvariantCulture);
                }

                if (booking.BookingDateTime.AddHours(AppSettingsModel.DivHour) > DateTime.UtcNow && IsAvailableBookingDate(booking.BookingDateTime.ToString(), booking.UserId))
                {
                    booking.BookingDateTime = booking.BookingDateTime.AddHours(AppSettingsModel.DivHour);
                    booking.BookingDateTimeString = booking.BookingDateTime.ToString();
          
                        if (checkBookingCapacity(booking.TenantId, booking.BookingDateTimeString, booking.UserId))
                    {
                        if (_whatsAppMessageTemplateAppService.checkPhoneNumber(booking.PhoneNumber))
                        {
                            if (booking.TenantId == 0)
                            {
                                booking.TenantId = AbpSession.TenantId.Value;
                            }
                            if (booking.CreatedBy == 0)
                            {
                                booking.CreatedBy = booking.ContactId;
                            }
                            booking.CustomerId = booking.TenantId.ToString() + "_" + booking.PhoneNumber;
                            var SP_Name = Constants.Booking.SP_BookingAdd;
                            var sqlParameters = new List<SqlParameter> {

                                new SqlParameter("@TenantId",booking.TenantId),
                                new SqlParameter("@CustomerName",booking.CustomerName),
                                new SqlParameter("@PhoneNumber",booking.PhoneNumber),
                                new SqlParameter("@BookingDateTime",booking.BookingDateTime),//.AddHours(AppSettingsModel.DivHour)),
                                new SqlParameter("@StatusId",booking.StatusId),
                                new SqlParameter("@AreaId",booking.AreaId),
                                new SqlParameter("@CreatedBy",booking.CreatedBy),
                                new SqlParameter("@ContactId",booking.ContactId),
                                new SqlParameter("@LanguageId",booking.LanguageId),
                                new SqlParameter("@BookingTypeId",booking.BookingTypeId),
                                new SqlParameter("@CustomerId",booking.CustomerId),
                                new SqlParameter("@Note",booking.Note),
                                new SqlParameter("@UserName",booking.UserName),
                                new SqlParameter("@IsNew",booking.IsNew),
                                new SqlParameter("@UserId",booking.UserId),
                            };
                            var OutputParameter = new SqlParameter
                            {
                                SqlDbType = SqlDbType.BigInt,
                                ParameterName = "@BookingId",
                                Direction = ParameterDirection.Output
                            };

                            sqlParameters.Add(OutputParameter);
                            SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                            if (OutputParameter.Value != DBNull.Value)
                            {
                                BookingModel model = getBookingById((long)OutputParameter.Value);
                                SocketIOManager.SendBooking(model, model.TenantId);
                                if (model.StatusId == (int)BookingStatusEnum.Confirmed)
                                {
                                    return await sendBookingTemplateAsync((long)OutputParameter.Value);
                                }
                                else
                                {
                                    return "booking_success";
                                }
                            }
                            else
                            {
                                return "booking_failed";
                            }
                        }
                        else
                        {
                            return "phonenumber_failed";
                        }
                    }
                    else
                    {
                        return "capacity_failed";
                    }
                }
                else
                {
                    return "time_failed";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<string> updateBookingAsync(BookingModel booking)
        {
            try
            {
                booking.BookingDateTime = DateTime.Parse(booking.BookingDateTimeString,CultureInfo.InvariantCulture);

                if (booking.BookingDateTime.AddHours(AppSettingsModel.DivHour) > DateTime.UtcNow && IsAvailableBookingDate(booking.BookingDateTime.ToString(), booking.UserId) || booking.StatusId == 4 || booking.StatusId == 5)
                {
                    booking.BookingDateTime = booking.BookingDateTime.AddHours(AppSettingsModel.DivHour);

                    booking.BookingDateTimeString = booking.BookingDateTime.ToString();

                    if (checkBookingCapacity(booking.TenantId, booking.BookingDateTimeString, booking.UserId) || booking.StatusId == 3 || booking.StatusId == 4 || booking.StatusId == 5)
                    {

                        var SP_Name = Constants.Booking.SP_BookingUpdate;
                        var sqlParameters = new List<SqlParameter>
                        {
                            new SqlParameter("@TenantId",booking.TenantId),
                            new SqlParameter("@BookingDateTime",booking.BookingDateTime),
                            new SqlParameter("@StatusId",booking.StatusId),
                            new SqlParameter("@AreaId",booking.AreaId),
                            new SqlParameter("@BookingNumber",booking.BookingNumber),
                            new SqlParameter("@ContactId",booking.ContactId),
                            new SqlParameter("@LanguageId",booking.LanguageId),
                            new SqlParameter("@DeletionReasonId",booking.DeletionReasonId),
                            new SqlParameter("@Note",booking.Note),
                            new SqlParameter("@IsNew",booking.IsNew),
                        };

                        var OutputParameter = new SqlParameter
                        {
                            ParameterName = "@Id",
                            Direction = ParameterDirection.Output,
                            SqlDbType = SqlDbType.BigInt
                        };
                        sqlParameters.Add(OutputParameter);


                        SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                        if (OutputParameter.Value != DBNull.Value)
                        {
                            //booking.Id = (long)OutputParameter.Value;
                            BookingModel model = getBookingById((long)OutputParameter.Value);
                            SocketIOManager.SendBooking(model, model.TenantId);
                            if (model.StatusId == (int)BookingStatusEnum.Confirmed || model.StatusId == (int)BookingStatusEnum.Deleted)
                            {
                                return await sendBookingTemplateAsync((long)OutputParameter.Value);
                            }
                            else
                            {
                                return "update_success";
                            }
                        }
                        else
                        {
                            return "update_failed";
                        }
                    }
                    else
                    {
                        return "capacity_failed";
                    }
                }
                else
                {
                    return "time_failed";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string updateBookingIsNew(int bookingID)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingUpdateIsNew;

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId",AbpSession.TenantId),
                    new SqlParameter("@BookingID",bookingID),
                    new SqlParameter("@IsNew",false),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return "Done";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool deleteBooking(long bookingId)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingDelete;
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("BookingId",bookingId)
                };
                var OutputParameter = new SqlParameter
                {
                    ParameterName = "Result",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Bit
                };
                sqlParameters.Add(OutputParameter);

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task generateBookingTemplatesAsync(int tenantId)
        {
            try
            {
                if (CheckIfNotExistBookingTemplate(tenantId, Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_19)))
                {
                    MessageTemplateModel messageTemplateModel = new MessageTemplateModel
                    {
                        name = Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_19),
                        category = Enum.GetName(typeof(WhatsAppCategoryEnum), WhatsAppCategoryEnum.MARKETING),
                        language = Enum.GetName(typeof(WhatsAppLanguageEnum), WhatsAppLanguageEnum.en),
                        VariableCount = 1,
                        isDeleted = false
                    };
                    List<WhatsAppComponentModel> components = new List<WhatsAppComponentModel>();

                    WhatsAppComponentModel componentBody = new WhatsAppComponentModel
                    {
                        text = "Hello" + " {{1}} " + "Thank You",
                        type = Enum.GetName(typeof(WhatsAppComponentTypeEnum), WhatsAppComponentTypeEnum.BODY),
                    };

                    if (messageTemplateModel.VariableCount > 0)
                    {
                        WhatsAppExampleModel exampleModel = new WhatsAppExampleModel
                        {
                            body_text = new string[1][]
                        };
                        List<string> vars = new List<string>();
                        for (int index = 0; index < messageTemplateModel.VariableCount; index++)
                        {
                            vars.Add("Variable" + index.ToString());
                        }
                        exampleModel.body_text[0] = vars.ToArray();
                        componentBody.example = exampleModel;
                    }
                    components.Add(componentBody);
                    messageTemplateModel.components = components;

                    await _whatsAppMessageTemplateAppService.AddWhatsAppMessageTemplateAsync(messageTemplateModel, tenantId);
                }
                if (CheckIfNotExistBookingTemplate(tenantId, Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_ar_19)))
                {
                    MessageTemplateModel messageTemplateModel = new MessageTemplateModel
                    {
                        name = Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_ar_19),
                        category = Enum.GetName(typeof(WhatsAppCategoryEnum), WhatsAppCategoryEnum.MARKETING),
                        language = Enum.GetName(typeof(WhatsAppLanguageEnum), WhatsAppLanguageEnum.en),
                        VariableCount = 1,
                        isDeleted = false
                    };
                    List<WhatsAppComponentModel> components = new List<WhatsAppComponentModel>();

                    WhatsAppComponentModel componentBody = new WhatsAppComponentModel
                    {
                        text = "مرحبا" + " {{1}} " + "شكرا لك",
                        type = Enum.GetName(typeof(WhatsAppComponentTypeEnum), WhatsAppComponentTypeEnum.BODY),
                    };

                    if (messageTemplateModel.VariableCount > 0)
                    {
                        WhatsAppExampleModel exampleModel = new WhatsAppExampleModel
                        {
                            body_text = new string[1][]
                        };
                        List<string> vars = new List<string>();
                        for (int index = 0; index < messageTemplateModel.VariableCount; index++)
                        {
                            vars.Add("Variable" + index.ToString());
                        }
                        exampleModel.body_text[0] = vars.ToArray();
                        componentBody.example = exampleModel;
                    }
                    components.Add(componentBody);

                    messageTemplateModel.components = components;
                    await _whatsAppMessageTemplateAppService.AddWhatsAppMessageTemplateAsync(messageTemplateModel, tenantId);
                }
                if (CheckIfNotExistBookingTemplate(tenantId, Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.reminder_booking_19)))
                {
                    MessageTemplateModel messageTemplateModel = new MessageTemplateModel
                    {
                        name = Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.reminder_booking_19),
                        category = Enum.GetName(typeof(WhatsAppCategoryEnum), WhatsAppCategoryEnum.MARKETING),
                        language = Enum.GetName(typeof(WhatsAppLanguageEnum), WhatsAppLanguageEnum.en),
                        VariableCount = 1,
                        isDeleted = false
                    };
                    List<WhatsAppComponentModel> components = new List<WhatsAppComponentModel>();

                    WhatsAppComponentModel componentBody = new WhatsAppComponentModel
                    {
                        text = "Reminder Message :" + " {{1}} " + "Have A Good Day",
                        type = Enum.GetName(typeof(WhatsAppComponentTypeEnum), WhatsAppComponentTypeEnum.BODY),
                    };

                    if (messageTemplateModel.VariableCount > 0)
                    {
                        WhatsAppExampleModel exampleModel = new WhatsAppExampleModel
                        {
                            body_text = new string[1][]
                        };
                        List<string> vars = new List<string>();
                        for (int index = 0; index < messageTemplateModel.VariableCount; index++)
                        {
                            vars.Add("Variable" + index.ToString());
                        }
                        exampleModel.body_text[0] = vars.ToArray();
                        componentBody.example = exampleModel;
                    }
                    components.Add(componentBody);

                    WhatsAppComponentModel componentButton = new WhatsAppComponentModel
                    {
                        type = Enum.GetName(typeof(WhatsAppComponentTypeEnum), WhatsAppComponentTypeEnum.BUTTONS),
                        buttons = new List<WhatsAppButtonModel>()
                    };

                    WhatsAppButtonModel buttonOne = new WhatsAppButtonModel
                    {
                        text = "CONFIRM",
                        type = Enum.GetName(typeof(WhatsAppButtonTypeEnum), WhatsAppButtonTypeEnum.QUICK_REPLY)
                    };
                    componentButton.buttons.Add(buttonOne);

                    WhatsAppButtonModel buttonTwo = new WhatsAppButtonModel
                    {
                        text = "CANCEL",
                        type = Enum.GetName(typeof(WhatsAppButtonTypeEnum), WhatsAppButtonTypeEnum.QUICK_REPLY)
                    };
                    componentButton.buttons.Add(buttonTwo);

                    //WhatsAppButtonModel buttonThree = new WhatsAppButtonModel
                    //{
                    //    text = "Modify",
                    //    type = Enum.GetName(typeof(WhatsAppButtonTypeEnum), WhatsAppButtonTypeEnum.QUICK_REPLY)
                    //};
                    //componentButton.buttons.Add(buttonThree);

                    components.Add(componentButton);
                    messageTemplateModel.components = components;
                    await _whatsAppMessageTemplateAppService.AddWhatsAppMessageTemplateAsync(messageTemplateModel, tenantId);
                }
                if (CheckIfNotExistBookingTemplate(tenantId, Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.reminder_booking_ar_19)))
                {
                    MessageTemplateModel messageTemplateModel = new MessageTemplateModel
                    {
                        name = Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.reminder_booking_ar_19),
                        category = Enum.GetName(typeof(WhatsAppCategoryEnum), WhatsAppCategoryEnum.MARKETING),
                        language = Enum.GetName(typeof(WhatsAppLanguageEnum), WhatsAppLanguageEnum.en),
                        VariableCount = 1,
                        isDeleted = false
                    };
                    List<WhatsAppComponentModel> components = new List<WhatsAppComponentModel>();

                    WhatsAppComponentModel componentBody = new WhatsAppComponentModel
                    {
                        text = "رسالة تذكير :" + " {{1}} " + "يومك سعيد",
                        type = Enum.GetName(typeof(WhatsAppComponentTypeEnum), WhatsAppComponentTypeEnum.BODY),
                    };

                    if (messageTemplateModel.VariableCount > 0)
                    {
                        WhatsAppExampleModel exampleModel = new WhatsAppExampleModel
                        {
                            body_text = new string[1][]
                        };
                        List<string> vars = new List<string>();
                        for (int index = 0; index < messageTemplateModel.VariableCount; index++)
                        {
                            vars.Add("Variable" + index.ToString());
                        }
                        exampleModel.body_text[0] = vars.ToArray();
                        componentBody.example = exampleModel;
                    }
                    components.Add(componentBody);

                    WhatsAppComponentModel componentButton = new WhatsAppComponentModel
                    {
                        type = Enum.GetName(typeof(WhatsAppComponentTypeEnum), WhatsAppComponentTypeEnum.BUTTONS),
                        buttons = new List<WhatsAppButtonModel>()
                    };

                    WhatsAppButtonModel buttonOne = new WhatsAppButtonModel
                    {
                        text = "CONFIRM",
                        type = Enum.GetName(typeof(WhatsAppButtonTypeEnum), WhatsAppButtonTypeEnum.QUICK_REPLY)
                    };
                    componentButton.buttons.Add(buttonOne);

                    WhatsAppButtonModel buttonTwo = new WhatsAppButtonModel
                    {
                        text = "CANCEL",
                        type = Enum.GetName(typeof(WhatsAppButtonTypeEnum), WhatsAppButtonTypeEnum.QUICK_REPLY)
                    };
                    componentButton.buttons.Add(buttonTwo);

                    //WhatsAppButtonModel buttonThree = new WhatsAppButtonModel
                    //{
                    //    text = "Modify",
                    //    type = Enum.GetName(typeof(WhatsAppButtonTypeEnum), WhatsAppButtonTypeEnum.QUICK_REPLY)
                    //};
                    //componentButton.buttons.Add(buttonThree);

                    components.Add(componentButton);
                    messageTemplateModel.components = components;
                    await _whatsAppMessageTemplateAppService.AddWhatsAppMessageTemplateAsync(messageTemplateModel, tenantId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private bool checkBookingCapacity(int tenantId, string bookingDateTime, long userId)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(bookingDateTime);
                //dateTime = dateTime.AddHours(AppSettingsModel.DivHour);
                // Create an English (US) culture
                CultureInfo englishCulture = new CultureInfo("en-US");
                // Format the date and time in English
                bookingDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", englishCulture);

                var SP_Name = Constants.Booking.SP_BookingCapacityCheck;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@TenantId",tenantId),
                    new SqlParameter("@BookingDateTime",bookingDateTime),
                    new SqlParameter("@UserId",userId)
                };
                var OutputParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                if (OutputParameter.Value != null)
                {
                    return (bool)OutputParameter.Value;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<string> sendBookingTemplateAsync(long bookingId)
        {
            try
            {
                BookingModel booking = getBookingById(bookingId);
                var statistics = _whatsAppMessageTemplateAppService.GetStatistics(booking.TenantId);
                decimal? remainingAds = statistics.RemainingBIConversation + statistics.RemainingFreeConversation;

                if (remainingAds > 0)
                {
                    int textResourceId = 0;
                    switch (booking.StatusId)
                    {
                        case (int)BookingStatusEnum.Confirmed:
                            {
                                if (booking.LanguageId == 1)
                                {
                                    booking.TemplateId = _whatsAppMessageTemplateAppService.GetTemplateIdByName(Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_ar_19));
                                }
                                else
                                {
                                    booking.TemplateId = _whatsAppMessageTemplateAppService.GetTemplateIdByName(Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_19));
                                }
                                textResourceId = (int)BookingTemplateCaptionEnum.Confirmation;
                                break;
                            }
                        case (int)BookingStatusEnum.Deleted:
                            {
                                if (booking.LanguageId == 1)
                                {
                                    booking.TemplateId = _whatsAppMessageTemplateAppService.GetTemplateIdByName(Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_ar_19));
                                }
                                else
                                {
                                    booking.TemplateId = _whatsAppMessageTemplateAppService.GetTemplateIdByName(Enum.GetName(typeof(BookingTemplateNameEnum), BookingTemplateNameEnum.booking_template_19));
                                }
                                textResourceId = (int)BookingTemplateCaptionEnum.Delete;
                                break;
                            }
                    }
                    var templateCaption = getBookingTemplate(booking.TenantId, booking.LanguageId, textResourceId);

                    if (templateCaption != null)
                    {
                        //{0} => Name
                        //{1} => Time
                        //{2} => PhoneNumber



                        if (booking.StatusId == (int)BookingStatusEnum.Deleted)
                        {
                            //templateCaption.Text = booking.DeletionReason;
                            templateCaption.Text = getTemplateMessageById(booking.DeletionReasonId.Value).MessageText;
                        }
                        else
                        {
                            templateCaption.Text = templateCaption.Text.Replace("\n", "\\n");
                            templateCaption.Text = string.Format(templateCaption.Text, booking.CustomerName, booking.BookingDateTime.ToString("dd/MM/yyyy"), booking.BookingDateTime.ToString("h:mm tt")).Replace("\r\n", "\\n");
                        }

                        BookingContact bookingContact = await _whatsAppMessageTemplateAppService.SendBookingTemplatesAsync(booking, templateCaption);
                        if (bookingContact != null)
                        {
                            createBookingContact(bookingContact);
                            if (bookingContact.IsSent)
                            {
                                return "send_success";
                            }
                            else
                            {
                                return "send_failed";
                            }
                        }
                        else
                        {
                            return "template_failed";
                        }
                    }
                    else
                    {
                        return "template_caption_failed";
                    }
                }
                else
                {
                    return "bundle_failed";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CaptionDto getBookingTemplate(int tenantId, int language, int textResourceId)
        {
            try
            {
                CaptionDto template = new CaptionDto();
                var SP_Name = Constants.Booking.SP_BookingTemplateGet;
                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@TenantId",tenantId),
                    new SqlParameter("@LanguageId",language),
                    new SqlParameter("@TextResourceId",textResourceId),
                };

                template = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertCaptionDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return template;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void createBookingContact(BookingContact bookingContact)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingContactAdd;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@TenantId",bookingContact.TenantId),
                    new SqlParameter("@PhoneNumber",bookingContact.PhoneNumber),
                    new SqlParameter("@MessageId",bookingContact.MessageId),
                    new SqlParameter("@IsSent",bookingContact.IsSent),
                    new SqlParameter("@MessageRate",bookingContact.MessageRate),
                    new SqlParameter("@TemplateTypeId",bookingContact.TemplateTypeId),
                    new SqlParameter("@IsReminderSent",false),
                    new SqlParameter("@BookingId",bookingContact.BookingId),
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int getCapacity(int tenantId)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingCapacityGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@TenantId",tenantId)
                };
                var OutputParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBooking, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                int result = 0;
                if (OutputParameter.Value != DBNull.Value)
                {
                    result = (int)OutputParameter.Value;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private Dictionary<string, string> getBookingDay(long userId, int languageId)
        {
            Dictionary<string, string> msDays = new Dictionary<string, string>();
            Dictionary<string, string> days = new Dictionary<string, string>();
            WorkModel workModel = _iUserAppService.GetUserSetting(userId);

            var bookingOffDays = GetBookingOffDays(userId).Where(x=>x.IsOffDayBooking).ToList();
            int forstart = 0;
            int forend = 9;

            string straingdays = "";

            var isoffweek=false;
            if(!workModel.IsWorkActiveSat && !workModel.IsWorkActiveSun
                && !workModel.IsWorkActiveMon && !workModel.IsWorkActiveTues 
                && !workModel.IsWorkActiveWed && !workModel.IsWorkActiveThurs 
                && !workModel.IsWorkActiveFri)
            {

                isoffweek=true;
                return days;
            }


            foreach (var day in bookingOffDays)
            {

                straingdays+=day.Day;
            }


            var listdaystraing = straingdays.Split(",").Distinct().ToList();

            var countlist = 0;

            try
            {
                for (int i = forstart; countlist<10; i++)
                {
                    countlist=days.Count();

                    if (days.Count()==10)
                    {
                        return days;
                    }
                    var day = DateTime.Now.AddDays(i);
                    DayOfWeek nameOfDay = day.DayOfWeek;
                    var dayasstaring = day.ToString("MM/dd/yyyy");


                    if (forend>=1000)
                    {
                        return days; //end if loop
                    }

                    switch (nameOfDay)
                    {
                        case DayOfWeek.Saturday:
                            { 
                                if (!listdaystraing.Contains(dayasstaring))
                                {
                                    if (workModel.IsWorkActiveSat)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName =  day.ToString("yyyy d MM") + " " + "سبت";
                                        }
                                        else
                                        {
                                            dayName = "Sat" + " " + day.ToString("MM d yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);
                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;
                                }
                                else
                                {
                                    forend++;
                                    break;
                                }
                            }
                        case DayOfWeek.Sunday:
                            {
                                if (!listdaystraing.Contains(dayasstaring))
                                {

                                    if (workModel.IsWorkActiveSun)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName = day.ToString("yyyy d MM") + " " + "احد";
                                        }
                                        else
                                        {
                                            dayName = "Sun" + " " + day.ToString("MM d yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);
                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;
                                }
                                else
                                {
                                    forend++;
                                    break;
                                }
                               
                            }
                        case DayOfWeek.Monday:
                            {
                                if (!listdaystraing.Contains(dayasstaring))
                                {

                                    if (workModel.IsWorkActiveMon)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName = day.ToString("yyyy d MM") + " " + "اثنين";
                                        }
                                        else
                                        {
                                            dayName = "Mon" + " " + day.ToString("MM d yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);
                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;
                                }
                                else
                                {
                                    forend++;
                                    break;

                                }
      
                            }
                        case DayOfWeek.Tuesday:
                            {
                                if (!listdaystraing.Contains(dayasstaring))
                                {
                                    if (workModel.IsWorkActiveTues)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName = day.ToString("yyyy d MM") + " " + "ثلاثاء";
                                        }
                                        else
                                        {
                                            dayName = "Tues" + " " + day.ToString("MM d yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);
                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;
                                }
                                else
                                {
                                    forend++;
                                    break;
                                }
                                 
                            }
                        case DayOfWeek.Wednesday:
                            {
                                if (!listdaystraing.Contains(dayasstaring))
                                {
                                    if (workModel.IsWorkActiveWed)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName = day.ToString("yyyy d MM") + " " + "اربعاء";
                                        }
                                        else
                                        {
                                            dayName = "Wed" + " " + day.ToString("MM d yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);
                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;

                                }
                                else
                                {
                                    forend++;
                                    break;

                                }
                                  
                            }
                        case DayOfWeek.Thursday:
                            {
                                if (!listdaystraing.Contains(dayasstaring))
                                {
                                    if (workModel.IsWorkActiveThurs)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName = day.ToString("yyyy d MM") + " " + "خميس";
                                        }
                                        else
                                        {
                                            dayName = "Thur" + " " + day.ToString("MM d yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);
                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;
                                }
                                else
                                {
                                    forend++;
                                    break;

                                }
                                   
                            }
                        case DayOfWeek.Friday:
                            {
                                if (!listdaystraing.Contains(dayasstaring))
                                {
                                    if (workModel.IsWorkActiveFri)
                                    {
                                        string dayName;
                                        if (languageId == 1)
                                        {
                                            dayName = day.ToString("yyyy d MM") + " " + "جمعة";
                                        }
                                        else
                                        {
                                            dayName = "Fri" + " " + day.ToString("d MM yyyy");
                                        }
                                        string date = day.ToString("d MMM yy");
                                        days.Add(dayName, date);

                                    }
                                    if (!isoffweek)
                                    {
                                        forend++;
                                    }
                                    break;
                                }
                                else
                                {
                                    forend++;
                                    break;
                                }
                                   
                            }
                    }

                }
                 

             







                ////}//end for

                //foreach (var offday in bookingOffDays)
                //{
                //    var lisOffDay = offday.Day.Split(",").ToList();

                //    foreach (var offdays in lisOffDay)
                //    {
                //        var d = DateTime.ParseExact(offdays, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None);

                //        foreach (var item in days.Where(kvp => kvp.Value == d.ToString("d MMM yy")).ToList())
                //        {
                //            days.Remove(item.Key);
                //        }

                //    }

                //}

            }
            catch(Exception ex)
            {

            }
           


                return days;
        }
        private List<string> getBookingTime(string date, int tenantId, long userId)
        {
            List<string> times = new List<string>();
            DateTime day = DateTime.ParseExact(date, "d MMM yy", CultureInfo.InvariantCulture);
            DayOfWeek nameOfDay = day.DayOfWeek;
            WorkModel workModel = _iUserAppService.GetUserSetting(userId);
            List<BookingModel> bookings = getBooking(null, day.ToString("d MMM yy"), day.AddDays(1).ToString("d MMM yy"), null, tenantId, userId).lstBookingModel;
            int capacity = int.Parse(workModel.WorkTextAR);
            switch (nameOfDay)
            {
                case DayOfWeek.Saturday:
                    {
                        DateTime startFP = day.Date + TimeSpan.Parse(workModel.StartDateSat);
                        DateTime endFP = day.Date + TimeSpan.Parse(workModel.EndDateSat);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }
                        if (workModel.HasSPSat)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateSatSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateSatSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);
                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }

                        break;
                    }
                case DayOfWeek.Sunday:
                    {
                        DateTime startFP = day.Date  + TimeSpan.Parse(workModel.StartDateSun);
                        DateTime endFP = day.Date  + TimeSpan.Parse(workModel.EndDateSun);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }
                        if (workModel.HasSPSun)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateSunSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateSunSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);
                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }

                        break;
                    }
                case DayOfWeek.Monday:
                    {
                        DateTime startFP = day.Date  + TimeSpan.Parse(workModel.StartDateMon);
                        DateTime endFP = day.Date  + TimeSpan.Parse(workModel.EndDateMon);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }
                        if (workModel.HasSPMon)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateMonSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateMonSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);

                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }

                        break;
                    }
                case DayOfWeek.Tuesday:
                    {
                        DateTime startFP = day.Date  + TimeSpan.Parse(workModel.StartDateTues);
                        DateTime endFP = day.Date  + TimeSpan.Parse(workModel.EndDateTues);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }
                        if (workModel.HasSPTues)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateTuesSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateTuesSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);

                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }

                        break;
                    }
                case DayOfWeek.Wednesday:
                    {
                        DateTime startFP = day.Date  + TimeSpan.Parse(workModel.StartDateWed);
                        DateTime endFP = day.Date  + TimeSpan.Parse(workModel.EndDateWed);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }

                        if (workModel.HasSPWed)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateWedSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateWedSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);

                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }
                        break;
                    }
                case DayOfWeek.Thursday:
                    {
                        DateTime startFP = day.Date  + TimeSpan.Parse(workModel.StartDateThurs);
                        DateTime endFP = day.Date  + TimeSpan.Parse(workModel.EndDateThurs);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }
                        if (workModel.HasSPThurs)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateThursSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateThursSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);

                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }
                        break;
                    }
                case DayOfWeek.Friday:
                    {
                        DateTime startFP = day.Date  + TimeSpan.Parse(workModel.StartDateFri);
                        DateTime endFP = day.Date  + TimeSpan.Parse(workModel.EndDateFri);
                        int totalHoursFP = Math.Abs((int)(startFP - endFP).TotalHours);


                        for (int i = 0; i < totalHoursFP; i++)
                        {
                            if (checkBookingTime(bookings, startFP, userId, capacity))
                            {
                                if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                {
                                    if (DateTime.UtcNow < startFP.AddHours(AppSettingsModel.DivHour))
                                    {
                                        times.Add(startFP.ToString("h:mm tt"));
                                    }
                                }
                                else
                                {
                                    times.Add(startFP.ToString("h:mm tt"));
                                }
                            }
                            startFP = startFP.AddHours(1);

                        }
                        if (workModel.HasSPFri)
                        {
                            DateTime startSP = day.Date  + TimeSpan.Parse(workModel.StartDateFriSP);
                            DateTime endSP = day.Date  + TimeSpan.Parse(workModel.EndDateFriSP);
                            int totalHoursSP = Math.Abs((int)(startSP - endSP).TotalHours);

                            for (int i = 0; i < totalHoursSP; i++)
                            {
                                if (checkBookingTime(bookings, startSP, userId, capacity))
                                {
                                    if (DateTime.UtcNow.ToString("d MMM yy") == day.ToString("d MMM yy"))
                                    {
                                        if (DateTime.UtcNow < startSP.AddHours(AppSettingsModel.DivHour))
                                        {
                                            times.Add(startSP.ToString("h:mm tt"));
                                        }
                                    }
                                    else
                                    {
                                        times.Add(startSP.ToString("h:mm tt"));
                                    }
                                }
                                startSP = startSP.AddHours(1);
                            }
                        }

                        break;
                    }
            }





            var bookingOffDays = GetBookingOffDays(userId);
            if (bookingOffDays.Count>0)
            {
                var listremove = new List<string>();


                foreach (var Boff in bookingOffDays)
                {
                    foreach (var time in times)
                    {

                        var Tim = DateTime.ParseExact(time, "h:mm tt", null, System.Globalization.DateTimeStyles.None);
                        var STime = DateTime.ParseExact(Boff.StartTime, "H:mm", null, System.Globalization.DateTimeStyles.None);
                        var ETime = DateTime.ParseExact(Boff.EndTime, "H:mm", null, System.Globalization.DateTimeStyles.None);


                        var ListdayOff = Boff.Day.Split(",").ToList();


                        foreach (var LoffDay in ListdayOff)
                        {
                            var dayOff = DateTime.ParseExact(LoffDay, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None);

                            if (Tim>=STime && Tim<=ETime && dayOff==day)
                            {
                                // times.Remove(time);
                                listremove.Add(time);
                            }

                        }




                    }


                }


                foreach (var time in listremove)
                {

                    times.Remove(time);

                }

            }


            return times;

        }
        private bool checkBookingTime(List<BookingModel> booking, DateTime bookingDateTime, long userId, int capacity)
        {
            int bookingCount = 0;
            foreach (var item in booking)
            {
                if (item.BookingDateTime == bookingDateTime && item.UserId == userId && item.BookingStatus!=BookingStatusEnum.Deleted&&item.BookingStatus!=BookingStatusEnum.Canceled)
                {
                    bookingCount++;
                }
            }
            if (capacity <= bookingCount)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private List<CaptionDto> getBookingCaption(int textResourceId, int tenantId)
        {
            try
            {

                List<CaptionDto> captions = new List<CaptionDto>();
                var SP_Name = Constants.Booking.SP_CaptionByTextResourceIdGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@TextResourceId",textResourceId),
                    new SqlParameter("@TenantId",tenantId),
                };

                captions = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertCaptionDto, AppSettingsModel.ConnectionStrings).ToList();

                return captions;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool IsAvailableBookingDate(string bookingdate, long userId)
        {
            var bookingOffDays = GetBookingOffDays(userId);

            var BTim = DateTime.Parse(bookingdate);// DateTime.ParseExact(bookingdate, "d MMM yy h:mm tt", null, System.Globalization.DateTimeStyles.None);

            if (bookingOffDays.Count>0)
            {
                foreach (var Boff in bookingOffDays)
                {
                    if (Boff.Day != null && Boff.Day != "")
                    {
                        char[] delimiterChars = { ',' };
                        string[] datesArray = Boff.Day.Split(delimiterChars);
                        bool isValidBookingdate = DateTime.TryParse(bookingdate, out DateTime bookingDate);

                        if (isValidBookingdate)
                        {
                            foreach (string item in datesArray)
                            {
                                DateTime BoffDate = DateTime.Parse(item, CultureInfo.InvariantCulture);
                                bool isValidDate = DateTime.TryParse(BoffDate.ToString(), out DateTime dateItem);
                                
                                if (isValidDate)
                                {
                                    if (Boff.IsOffDayBooking && (bookingDate.Year == dateItem.Year && bookingDate.Month == dateItem.Month && bookingDate.Day == dateItem.Day))
                                    {
                                        return false;
                                    }
                                    else if(Boff.IsOffDayBooking== false  && (bookingDate.Year == dateItem.Year && bookingDate.Month == dateItem.Month && bookingDate.Day == dateItem.Day) )
                                    {
                                        var STime = DateTime.ParseExact(Boff.StartTime, "H:mm", null, System.Globalization.DateTimeStyles.None);
                                        var ETime = DateTime.ParseExact(Boff.EndTime, "H:mm", null, System.Globalization.DateTimeStyles.None);

                                        if ((bookingDate.Hour >= STime.Hour && BTim.Minute >= STime.Minute) && (bookingDate.Hour <= ETime.Hour && bookingDate.Minute <= ETime.Minute))
                                        {
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    //// var DD= DateTime.ParseExact(Boff.Day, "H:mm", null, System.Globalization.DateTimeStyles.None);
                    //var STime = DateTime.ParseExact(Boff.StartTime, "H:mm", null, System.Globalization.DateTimeStyles.None);
                    //var ETime = DateTime.ParseExact(Boff.EndTime, "H:mm", null, System.Globalization.DateTimeStyles.None);
                    //if (BTim>=STime && BTim<=ETime)
                    //{
                    //    return false;
                    //}
                }
            }
            return true;
        }
        private TenantsModel getTenantById(int tenantId)
        {
            try
            {
                TenantsModel tenant = new TenantsModel();
                var SP_Name = Constants.Tenant.SP_TenantsGetById;
                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@Id",tenantId)
                };

                tenant = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTenant, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return tenant;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private CreateOrEditTemplateMessageDto getTemplateMessageById(int id)
        {
            try
            {
                CreateOrEditTemplateMessageDto templateMessage = new CreateOrEditTemplateMessageDto();
                var SP_Name = Constants.TemplateMessage.SP_TemplateMessageByIdGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@Id",id)
                };

                templateMessage = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTemplateMessage, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return templateMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool ISRoleAdmin(int TenantId, string UserIds)
        {

            List<string> vs = new List<string>();

            var role = GetRole(TenantId);
            var userRole = GetUserRole(TenantId, UserIds).Distinct();

            foreach (var item in userRole)
            {
                var uR = role.Where(x => x.Id==item.RoleId).FirstOrDefault();
                if (uR!=null)
                {
                    if (uR.Name=="Admin")
                    {
                        return true;
                    }

                }

            }

            return false;
        }
        private List<RoleModelDto> GetRole(int TenantId)
        {

            var SP_Name = Constants.Role.SP_GetRole;

            var sqlParameters = new List<SqlParameter>(){
                new SqlParameter("@TenantId", TenantId)
            };

            List<RoleModelDto> roles = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToRoleDto, AppSettingsModel.ConnectionStrings).ToList();
            return roles;
        }
        private List<UserRoleModelDto> GetUserRole(int TenantId, string UserIds)
        {

            var SP_Name = Constants.Role.SP_GetUserRole;

            var sqlParameters = new List<SqlParameter>(){
            new SqlParameter("@TenantId", TenantId),
                new SqlParameter("@UserIds", UserIds)
            };

            List<UserRoleModelDto> userroles = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertToUserRoleDto, AppSettingsModel.ConnectionStrings).ToList();
            return userroles;
        }
        private List<AreaDto> getBranchesByUserId(int userId, bool isAdmin)
        {
            var area = _areasAppService.GetAllAreas(AbpSession.TenantId.Value, true);


            if (isAdmin)
            {
                return area;
            }

            return area.Where(x => x.UserIds.Contains(userId.ToString())).ToList();


        }

        private List<BookingOffDays> getBookingOffDays(long userId)
        {
            try
            {
                List<BookingOffDays> bookingOffDays = new List<BookingOffDays>();
                var SP_Name = Constants.Booking.SP_BookingOffDaysGet;

                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@UserId",userId)
                };

                bookingOffDays = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBookingOffDays, AppSettingsModel.ConnectionStrings).ToList();

                return bookingOffDays;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool createBookingOffDays(BookingOffDaysEntity bookingOffDaysEntity)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingOffDaysCreate;
                deleteBookingOffDays(bookingOffDaysEntity.UserId);

                DataTable bookingOffDaysTbl = prepareBookingOffDays(bookingOffDaysEntity.bookingOffDays);

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@tblBookingOffDays",bookingOffDaysTbl),
                    new SqlParameter("@UserId",bookingOffDaysEntity.UserId),
                    new SqlParameter("@TenantId",bookingOffDaysEntity.TenantId),
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void deleteBookingOffDays(long userId)
        {
            try
            {
                var SP_Name = Constants.Booking.SP_BookingOffDaysDelete;
                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@UserId",userId)

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DataTable prepareBookingOffDays(List<BookingOffDays> bookingOffDays)
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add(new DataColumn("Day", typeof(string)));
            tbl.Columns.Add(new DataColumn("StartTime", typeof(string)));
            tbl.Columns.Add(new DataColumn("EndTime", typeof(string)));
            tbl.Columns.Add(new DataColumn("IsOffDay", typeof(bool)));
            foreach (var item in bookingOffDays)
            {

                DataRow dr = tbl.NewRow();

                dr["Day"] = item.Day;
                dr["StartTime"] = item.StartTime;
                dr["EndTime"] = item.EndTime;
                dr["IsOffDay"] = item.IsOffDayBooking;
                tbl.Rows.Add(dr);

            }
            return tbl;
        }

        #endregion
    }
}
