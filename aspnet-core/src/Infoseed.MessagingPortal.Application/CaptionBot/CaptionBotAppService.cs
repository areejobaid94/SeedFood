using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;
using System.Linq;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using System.Data.SqlClient;
using Infoseed.MessagingPortal.MultiTenancy;
using Newtonsoft.Json;
using Framework.Data;

namespace Infoseed.MessagingPortal.CaptionBot
{
   public  class CaptionBotAppService : MessagingPortalAppServiceBase, ICaptionBotAppService
    {
        private readonly IRepository<Caption, long> _captionRepository;
        private readonly IRepository<LanguageBot, long> _languageRepository;
        private readonly IRepository<TextResource, long> _textResourceRepository;


        public CaptionBotAppService(IRepository<Caption, long> captionRepository, IRepository<LanguageBot, long> languageRepository, IRepository<TextResource, long> textResourceRepository)
        {
            _captionRepository = captionRepository;
            _languageRepository = languageRepository;
            _textResourceRepository = textResourceRepository;

        }

        public async Task CreateCaption(int tenantId, int localID, TenantTypeEunm tenantType)
        {
            try
            {

                if (tenantType==TenantTypeEunm.Booking)
                {
                    List<Caption> captionslistar = LocalArBooking(tenantId);

                    foreach (var item in captionslistar)
                    {


                        CreateCaption(item);
                    }

                    List<Caption> captionslisten = LocalEnBooking(tenantId);
                    foreach (var item in captionslisten)
                    {


                        CreateCaption(item);
                    }
                }
                else
                {
                    List<Caption> captionslistar = LocalAr(tenantId);

                    foreach (var item in captionslistar)
                    {


                        CreateCaption(item);
                    }

                    List<Caption> captionslisten = LocalEn(tenantId);
                    foreach (var item in captionslisten)
                    {


                        CreateCaption(item);
                    }
                    //if (localID == -1)
                    //{


                    //    List<Caption> captionslistar = LocalArDelivery(tenantId);

                    //    foreach (var item in captionslistar)
                    //    {


                    //        CreateCaption(item);
                    //    }


                    //}
                    //else if (localID == 0)
                    //{
                    //    List<Caption> captionslistar = LocalAr(tenantId);

                    //    foreach (var item in captionslistar)
                    //    {


                    //        CreateCaption(item);
                    //    }

                    //    List<Caption> captionslisten = LocalEn(tenantId);
                    //    foreach (var item in captionslisten)
                    //    {


                    //        CreateCaption(item);
                    //    }

                    //}
                    //else if (localID == 1)
                    //{

                    //    List<Caption> captionslistar = LocalAr(tenantId);

                    //    foreach (var item in captionslistar)
                    //    {


                    //        CreateCaption(item);
                    //    }
                    //}
                    //else if (localID == 2)
                    //{

                    //    List<Caption> captionslisten = LocalEn(tenantId);
                    //    foreach (var item in captionslisten)
                    //    {


                    //        CreateCaption(item);
                    //    }
                    //}
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            


            try {

                //create   Conservation Measurements
                List<ConversationMeasurementsTenantModel> lstconservation = new List<ConversationMeasurementsTenantModel>();



                for (int i = 1; i <= 12; i++)
                {
                    ConversationMeasurementsTenantModel conservationMeasurementsModel = new ConversationMeasurementsTenantModel();

                    conservationMeasurementsModel.TenantId = tenantId;
                    conservationMeasurementsModel.Year = DateTime.Now.Year;
                    conservationMeasurementsModel.Month = i;
                    conservationMeasurementsModel.BusinessInitiatedCount = 0;
                    conservationMeasurementsModel.UserInitiatedCount = 0;
                    conservationMeasurementsModel.ReferralConversionCount = 0;
                    conservationMeasurementsModel.TotalFreeConversation = 1000;
                    conservationMeasurementsModel.LastUpdatedDate = DateTime.Now;
                    conservationMeasurementsModel.CreationDate = DateTime.Now;

                    conservationMeasurementsModel.TotalFreeConversationWA = 1000;
                    conservationMeasurementsModel.TotalUsageFreeConversationWA = 0;
                    conservationMeasurementsModel.TotalUsageFreeUIWA = 0;
                    conservationMeasurementsModel.TotalUsageFreeBIWA = 0;
                    conservationMeasurementsModel.TotalUsagePaidConversationWA = 0;
                    conservationMeasurementsModel.TotalUsagePaidUIWA = 0;
                    conservationMeasurementsModel.TotalUsagePaidBIWA = 0;
                    conservationMeasurementsModel.TotalUsageFreeConversation = 0;
                    conservationMeasurementsModel.TotalUIConversation = 0;
                    conservationMeasurementsModel.TotalUsageUIConversation = 0;
                    conservationMeasurementsModel.TotalBIConversation = 0;
                    conservationMeasurementsModel.TotalUsageBIConversation = 0;


                    lstconservation.Add(conservationMeasurementsModel);

                }
                CreateTenantsConversations(lstconservation);
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        public async Task<PagedResultDto<GetCaptionForViewDto>> GetAll(GetAllCaptionInput input)
        {
            try
            {
                var filteredCaption = _captionRepository.GetAll()
                     .Include(e => e.TextResource)
                     .Include(e => e.LanguageBot)
                     .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Text == input.NameFilter);


                var caption = from o in filteredCaption

                              select new GetCaptionForViewDto()
                              {
                                  CaptionDto = new CaptionDto
                                  {
                                      Id = o.Id,
                                      LanguageBotId = o.LanguageBotId,
                                      Text = o.Text,
                                      TenantId = o.TenantId,
                                      TextResourceId = o.TextResourceId


                                  }
                              };


                var totalCount = filteredCaption.Count();
                var list = filteredCaption.ToList();

                return new PagedResultDto<GetCaptionForViewDto>(
                    totalCount,
                     caption.ToList()
                );
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }


        public List<CaptionDto> GetCaption(int tenantId, int? lang = null)
        {
            return getCaption( tenantId, lang);
        }
        private void CreateCaption(Caption  caption)
        {

            if (caption.HeaderText==null)
            {
                caption.HeaderText="";

            }
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "INSERT INTO Caption "
                            + " (CreationTime ,IsDeleted ,TenantId ,Text , LanguageBotId, TextResourceId,HeaderText) VALUES "
                            + " (@CreationTime ,@IsDeleted ,@TenantId ,@Text ,@LanguageBotId, @TextResourceId ,@HeaderText) ";

                        command.Parameters.AddWithValue("@CreationTime", caption.CreationTime);
                        command.Parameters.AddWithValue("@IsDeleted", caption.IsDeleted);
                        command.Parameters.AddWithValue("@TenantId", caption.TenantId);
                        command.Parameters.AddWithValue("@Text", caption.Text);
                        command.Parameters.AddWithValue("@LanguageBotId", caption.LanguageBotId);
                        command.Parameters.AddWithValue("@TextResourceId", caption.TextResourceId);
                        command.Parameters.AddWithValue("@HeaderText", caption.HeaderText);





                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

        }

        private static List<Caption> LocalAr(int tenantId)
        {
            List<Caption> captionslist = new List<Caption>();
            try
            {
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "مرحبا بك  *{0}* \r\n اهلا وسهلا بكم في *info-seed Bot* 😊",
                    LanguageBotId = 1,
                    TextResourceId = 1
                });


                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء الضغط على الرابط \r\n 👇👇👇👇👇👇 \r\n https://menu.info-seed.com/?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3}&LanguageBot={4}&lang={5}&OrderType={6}  \r\n 👆👆👆👆👆👆",
                    LanguageBotId = 1,
                    TextResourceId = 2,
                    HeaderText="ChatStepLink"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء ادخال رقم الـ Order الطلب",
                    LanguageBotId = 1,
                    TextResourceId = 3,
                    HeaderText="ChatStepTriggersCancel"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "رقم الطلب خطأ",
                    LanguageBotId = 1,
                    TextResourceId = 4
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "تم الغاء الطلب رقم {0} من قبل صاحب الطلب {1} \r\n هل هناك اي  شيء يمكنني مساعدتك به",
                    LanguageBotId = 1,
                    TextResourceId = 5
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء كتابة اقتراحكم او شكواكم 😊😊😊",
                    LanguageBotId = 1,
                    TextResourceId = 6,
                    HeaderText="ChatStepInquiries"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء تقيم الخدمة من 1 الى 5 ",
                    LanguageBotId = 1,
                    TextResourceId = 7,
                    HeaderText="ChatStepTriggersEvaluationQuestionl"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "لقد تم استلام الشكوى أو الإقتراح وسوف يتم الرد عليكم إذا اقتضت الضرورة. ",
                    LanguageBotId = 1,
                    TextResourceId = 8,
                    HeaderText="LiveChat"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء ارسال المــوقـــع (ـLocation) 📌",
                    LanguageBotId = 1,
                    TextResourceId = 9,
                    HeaderText="ChatStepDelivery"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "كلفة التوصيل لمنطقتك هي  {0} دينار",
                    LanguageBotId = 1,
                    TextResourceId = 10,
                    HeaderText="ChatStepDelivery"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "تم ايقاف الطلب,يمكنك الطلب من جديد",
                    LanguageBotId = 1,
                    TextResourceId = 11,
                    HeaderText="ChatStepCancelOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء اختيار احد الفروع التالية",
                    LanguageBotId = 1,
                    TextResourceId = 12,
                    HeaderText="ChatStepPickup"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء اختيار احد الفروع التالية",
                    LanguageBotId = 1,
                    TextResourceId = 1118
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "نحن لا ندعم التوصيل لمنطقتك ",
                    LanguageBotId = 1,
                    TextResourceId = 13,
                    HeaderText="ChatStepDelivery"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "طريقة تسليم الطلب ",
                    LanguageBotId = 1,
                    TextResourceId = 14,
                    HeaderText="ChatStart"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "هل تريد الاستمرار",
                    LanguageBotId = 1,
                    TextResourceId = 15
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "نحن لا ندعم التوصيل لمنطقتك {0}",
                    LanguageBotId = 1,
                    TextResourceId = 16
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "عذرا .. لا يمكن الغاء الطلب الان ",
                    LanguageBotId = 1,
                    TextResourceId = 17,
                    HeaderText="ChatStepCancelOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "العدد:",
                    LanguageBotId = 1,
                    TextResourceId = 18,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الاضافات ",
                    LanguageBotId = 1,
                    TextResourceId = 19,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "المجموع:   ",
                    LanguageBotId = 1,
                    TextResourceId = 20,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "السعر الكلي للصنف: ",
                    LanguageBotId = 1,
                    TextResourceId = 21,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ شكرا لك معلومات الطلب رقم الطلب :  {0} قيمة التوصيل :  {1} من الموقع :  {2} من الفرع : {3} السعر الإجمالي للطلب: {4} ------------------ سوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم",
                    LanguageBotId = 1,
                    TextResourceId = 22,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "رقم الطلب :  ",
                    LanguageBotId = 1,
                    TextResourceId = 23
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ شكرا لك معلومات الطلب رقم الطلب: {0} من الفرع : {1} السعر الإجمالي للطلب: {2} ------------------ سوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم",
                    LanguageBotId = 1,
                    TextResourceId = 24,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ شكرا لك معلومات الطلب رقم الطلب :  {0} قيمة التوصيل :  {1} من الموقع :  {2} من الفرع : {3} السعر الإجمالي للطلب: {4} السعر بنقاط: {5} ------------------ سوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم",
                    LanguageBotId = 1,
                    TextResourceId = 25,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ شكرا لك معلومات الطلب رقم الطلب: {0} من الفرع : {1} السعر الإجمالي للطلب: {2} السعر بنقاط: {3} ------------------ سوف يتم اعلامكم عند بدا تجهيز طلبكم.. شكرا لكم",
                    LanguageBotId = 1,
                    TextResourceId = 26,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "من الموقع :",
                    LanguageBotId = 1,
                    TextResourceId = 27
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "من الفرع :",
                    LanguageBotId = 1,
                    TextResourceId = 28
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "طلبكم قيد التجهيز ...",
                    LanguageBotId = 1,
                    TextResourceId = 29
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "تم الغاء الطلب من قبل المطعم  ...",
                    LanguageBotId = 1,
                    TextResourceId = 30
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "طلبكم قيد التجهيز ...",
                    LanguageBotId = 1,
                    TextResourceId = 31
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "الرجاء إدخال الإضافات.",
                    LanguageBotId = 1,
                    TextResourceId = 33
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "هل ترغب باضافة أي *ملاحظات* اخرى ",
                    LanguageBotId = 1,
                    TextResourceId = 34
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "وقت تسلم الطلب ؟",
                    LanguageBotId = 1,
                    TextResourceId = 35
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Pending Template AR",
                    LanguageBotId = 1,
                    TextResourceId = 1253
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Pending Template EN",
                    LanguageBotId = 2,
                    TextResourceId = 1253
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Reminder_Template AR",
                    LanguageBotId = 1,
                    TextResourceId = 1254
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Reminder_Template EN",
                    LanguageBotId = 2,
                    TextResourceId = 1254
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Cancel_Template AR",
                    LanguageBotId = 1,
                    TextResourceId = 1255
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Cancel_Template EN",
                    LanguageBotId = 2,
                    TextResourceId = 1255
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Delete_Template AR",
                    LanguageBotId = 1,
                    TextResourceId = 1256
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Delete_Template EN",
                    LanguageBotId = 2,
                    TextResourceId = 1256
                });


                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "سيتم التواصل معك بأقل من 24 ساعة\r\nيرجى الانتظار ",
                    LanguageBotId = 1,
                    TextResourceId = 1028,
                    HeaderText="LiveChat"
                });

                return captionslist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private static List<Caption> LocalArDelivery(int tenantId)
        {
            List<Caption> captionslist = new List<Caption>();

            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "شكرا لك معلومات الطلب: رقم الطلب : {0} من الموقع : {1} الى الموقع : {2} معلومات عن الموقع : {3} قيمة التوصيل : {4}",
                LanguageBotId = 1,
                TextResourceId = 53
            });

            return captionslist;
        }
        private static List<Caption> LocalEn(int tenantId)
        {
            List<Caption> captionslist = new List<Caption>();
            try
            {
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Welcome *{0}* \r\n Welcome to *info-seed Bot* 😊",
                    LanguageBotId = 2,
                    TextResourceId = 1
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please click on the link \r\n 👇👇👇👇👇👇 \r\n https://menu.info-seed.com/?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3 }&LanguageBot={4}&lang={5}&OrderType={6}  \r\n 👆👆👆👆👆👆",
                    LanguageBotId = 2,
                    TextResourceId = 2,
                    HeaderText="ChatStepLink"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please enter the order number",
                    LanguageBotId = 2,
                    TextResourceId = 3,
                    HeaderText="ChatStepTriggersCancel"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "The order number is wrong",
                    LanguageBotId = 2,
                    TextResourceId = 4
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "The request number {0} has been canceled by the applicant {1} \r\n Is there anything I can help you with",
                    LanguageBotId = 2,
                    TextResourceId = 5
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please write your suggestion or complaint",
                    LanguageBotId = 2,
                    TextResourceId = 6,
                    HeaderText="ChatStepInquiries"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please rate the service from 1 to 5",
                    LanguageBotId = 2,
                    TextResourceId = 7,
                    HeaderText="ChatStepTriggersEvaluationQuestionl"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "The complaint or suggestion has been received and we will respond to you if necessary.",
                    LanguageBotId = 2,
                    TextResourceId = 8,
                    HeaderText="LiveChat"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please send the location (Location) 📌",
                    LanguageBotId = 2,
                    TextResourceId = 9,
                    HeaderText="ChatStepDelivery"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "The cost of delivery to your area is {0} dinars",
                    LanguageBotId = 2,
                    TextResourceId = 10,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "The order has been suspended, you can order again",
                    LanguageBotId = 2,
                    TextResourceId = 11,
                    HeaderText="ChatStepCancelOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please choose one of the following branches",
                    LanguageBotId = 2,
                    TextResourceId = 1118
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please choose one of the following branches",
                    LanguageBotId = 2,
                    TextResourceId = 12,
                    HeaderText="ChatStepPickup"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "We do not support delivery to your area",
                    LanguageBotId = 2,
                    TextResourceId = 13,
                    HeaderText="ChatStepDelivery"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "How to deliver the order",
                    LanguageBotId = 2,
                    TextResourceId = 14,
                    HeaderText="ChatStart"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "do you want to continue",
                    LanguageBotId = 2,
                    TextResourceId = 15
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "We do not support delivery to your area {0}",
                    LanguageBotId = 2,
                    TextResourceId = 16
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Sorry, it is not possible to cancel the order now",
                    LanguageBotId = 2,
                    TextResourceId = 17,
                    HeaderText="ChatStepCancelOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "the number:",
                    LanguageBotId = 2,
                    TextResourceId = 18,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Additions",
                    LanguageBotId = 2,
                    TextResourceId = 19,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Total:",
                    LanguageBotId = 2,
                    TextResourceId = 20,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Total item price:",
                    LanguageBotId = 2,
                    TextResourceId = 21,
                    HeaderText="ChatStepOrderDetails"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ Thank you for ordering information Order number: {0} Delivery value: {1} From the site: {2} From the branch: {3} Total order price: {4} ------------------ You will be notified when your request is processed. Thank you",
                    LanguageBotId = 2,
                    TextResourceId = 22,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "order number :",
                    LanguageBotId = 2,
                    TextResourceId = 23
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ Thank you for ordering information Order number: {0} From the branch: {1} Total order price: {2} ------------------ You will be notified when your request is processed. Thank you",
                    LanguageBotId = 2,
                    TextResourceId = 24,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ Thank you for ordering information Order number: {0} Delivery value: {1} From the site: {2} From the branch: {3} Total order price: {4} Price in pips: {5} ------------------ You will be notified when your request is processed. Thank you",
                    LanguageBotId = 2,
                    TextResourceId = 25,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "------------------ Thank you for ordering information Order number: {0} From the branch: {1} Total order price: {2} Price in pips: {3} ------------------ You will be notified when your request is processed. Thank you",
                    LanguageBotId = 2,
                    TextResourceId = 26,
                    HeaderText="ChatStepCreateOrder"
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "From the site:",
                    LanguageBotId = 2,
                    TextResourceId = 27
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "From the branch:",
                    LanguageBotId = 2,
                    TextResourceId = 28
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Your request is being processed...",
                    LanguageBotId = 2,
                    TextResourceId = 29
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "The order was canceled by the restaurant...",
                    LanguageBotId = 2,
                    TextResourceId = 30
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Your request is being processed...",
                    LanguageBotId = 2,
                    TextResourceId = 31
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please enter plugins.",
                    LanguageBotId = 2,
                    TextResourceId = 33
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Would you like to add any other *notes*?",
                    LanguageBotId = 2,
                    TextResourceId = 34
                });
                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "When to receive the request?",
                    LanguageBotId = 2,
                    TextResourceId = 35
                });

                captionslist.Add(new Caption
                {
                    CreationTime = DateTime.Now,
                    TenantId = tenantId,
                    IsDeleted = false,
                    Text = "Please wait a bit\r\n",
                    LanguageBotId = 2,
                    TextResourceId = 1028,
                    HeaderText="LiveChat"

                });
                return captionslist;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private static List<Caption> LocalArBooking(int tenantId)
        {
            List<Caption> captionslist = new List<Caption>();

            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "عيادات الشرق الاوسط ترحب بكم. بنحب نخبرك انو صار بامكانك تحجز موعد عن طريق واتساب",
                LanguageBotId = 1,
                TextResourceId = 1257
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "لحظات وسوف يتم تأكيد حجزك..... تحملنا شوي.",
                LanguageBotId = 1,
                TextResourceId = 1258
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "يرجى اختيار وقت اخر",
                LanguageBotId = 1,
                TextResourceId = 1259
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "لا يوجد مواعيد لك",
                LanguageBotId = 1,
                TextResourceId = 1260
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "تم الغاء الموعد",
                LanguageBotId = 1,
                TextResourceId = 1261
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "يرجى تحديد اليوم؟؟؟\r\n",
                LanguageBotId = 1,
                TextResourceId = 1262
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "يرجى اختيار الوقت الي بناسبك.؟؟؟",
                LanguageBotId = 1,
                TextResourceId = 1263
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "{0}\r\nتود _اسم العيادة _ تذكيركم بموعدكم {2} ,{1} - \r\nيرجى الحضور قبل الموعد بنصف ساعة  ",
                LanguageBotId = 1,
                TextResourceId = 1254,
                HeaderText="Reminder Template"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "CancelTemplate",
                LanguageBotId = 1,
                TextResourceId = 1255,
                HeaderText="CancelTemplate"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "{0}\r\nتود -اسم العيادة- اعلامكم بأنه تم الغاء موعدكم \r\nبموعدكم {2} ,{1} \r\nبسبب الظروف الجوية \r\nen",
                LanguageBotId = 1,
                TextResourceId = 1256,
                HeaderText="Delete Template"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "سيد{0}\r\nبحب اخبرك انو تم تأكيد موعدك\r\n{1} ,{2}\r\nموقع العيادة هو : الشميساني - شارع عبدالله غوشة عمارة 15\r\n\r\nhttps://www.google.com/maps/search/31.958024,+35.855996\r\n",
                LanguageBotId = 1,
                TextResourceId = 1253,
                HeaderText="Pending Template"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "لا يوجد مواعيد لهذا الفرع",
                LanguageBotId = 1,
                TextResourceId = 1264
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "الرجاء ادخال الاسم ",
                LanguageBotId = 1,
                TextResourceId = 1173
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "الرجاء اختيار الطبيب",
                LanguageBotId = 1,
                TextResourceId = 1265
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "يا ريت تخبرنا اذا عندك اي ملاحظة حاب تحكيها للدكتور قبل ما تيجي ....\r\n",
                LanguageBotId = 1,
                TextResourceId = 1266
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "الرجاء اختيار الفرع ",
                LanguageBotId = 1,
                TextResourceId = 12
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "لا يوجد مواعيد للطبيب",
                LanguageBotId = 1,
                TextResourceId = 3001
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "لحظات ويتم التواصل معك ",
                LanguageBotId = 1,
                TextResourceId = 1043
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "لحظات ويتم التواصل معك ",
                LanguageBotId = 1,
                TextResourceId = 1028
            });
            return captionslist;
        }
        private static List<Caption> LocalEnBooking(int tenantId)
        {
            List<Caption> captionslist = new List<Caption>();

            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Please Choose",
                LanguageBotId = 2,
                TextResourceId = 1257
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Thank you ,Your appointment will be confirmed as soon as possible",
                LanguageBotId = 2,
                TextResourceId = 1258
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Please choose another time",
                LanguageBotId = 2,
                TextResourceId = 1259
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "There are no appointments for you",
                LanguageBotId = 2,
                TextResourceId = 1260
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "The appointment has been cancelled",
                LanguageBotId = 2,
                TextResourceId = 1261
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Please select a date",
                LanguageBotId = 2,
                TextResourceId = 1262
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Please choose time",
                LanguageBotId = 2,
                TextResourceId = 1263
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "{0}\r\nتود _اسم العيادة _ تذكيركم بموعدكم {2} ,{1} - \r\nيرجى الحضور قبل الموعد بنصف ساعة  ",
                LanguageBotId = 2,
                TextResourceId = 1254,
                HeaderText="Reminder Template"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "CancelTemplate",
                LanguageBotId = 2,
                TextResourceId = 1255,
                HeaderText="CancelTemplate"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "{0}\r\nتود -اسم العيادة- اعلامكم بأنه تم الغاء موعدكم \r\nبموعدكم {2} ,{1} \r\nبسبب الظروف الجوية \r\nen",
                LanguageBotId = 2,
                TextResourceId = 1256,
                HeaderText="Delete Template"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "سيد{0}\r\nبحب اخبرك انو تم تأكيد موعدك\r\n{1} ,{2}\r\nموقع العيادة هو : الشميساني - شارع عبدالله غوشة عمارة 15\r\n\r\nhttps://www.google.com/maps/search/31.958024,+35.855996\r\n",
                LanguageBotId = 2,
                TextResourceId = 1253,
                HeaderText="Pending Template"
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "There are no appointments for this branch",
                LanguageBotId = 2,
                TextResourceId = 1264
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Please enter the name",
                LanguageBotId = 2,
                TextResourceId = 1173
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Choose doctor",
                LanguageBotId = 2,
                TextResourceId = 1265
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "enter your note",
                LanguageBotId = 2,
                TextResourceId = 1266
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Please select a branch",
                LanguageBotId = 2,
                TextResourceId = 12
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "There are no doctor's appointments",
                LanguageBotId = 2,
                TextResourceId = 3001
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Moments and you are contacted",
                LanguageBotId = 2,
                TextResourceId = 1043
            });
            captionslist.Add(new Caption
            {
                CreationTime = DateTime.Now,
                TenantId = tenantId,
                IsDeleted = false,
                Text = "Moments and you are contacted",
                LanguageBotId = 2,
                TextResourceId = 1028
            });
            return captionslist;
        }
        private void CreateTenantsConversations(List<ConversationMeasurementsTenantModel> lstconservation)
        {
            try
            {
                //string item = JsonConvert.SerializeObject(lstconservation);

                

                var SP_Name = Constants.Tenant.SP_ConversationMeasurementsAdd;

                var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@ConversationMeasurementsJson",JsonConvert.SerializeObject(lstconservation))
            };
                

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
       




        private List<CaptionDto> getCaption(int tenantId,int? lang)
        {
            try
            {
                List<CaptionDto> captionDtos = new List<CaptionDto>();

                 var SP_Name = Constants.Caption.SP_CaptionGet;
                 var sqlParameters = new List<SqlParameter> {
                 new SqlParameter("@TenantId",tenantId)
                ,new SqlParameter("@Lang",lang)
 
                };
                captionDtos = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                DataReaderMapper.ConvertCaptionDto, AppSettingsModel.ConnectionStrings).ToList();

                return captionDtos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}