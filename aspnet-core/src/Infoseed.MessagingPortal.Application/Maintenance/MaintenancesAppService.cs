using Abp.Application.Services.Dto;
using Framework.Data;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Maintenance.Dtos;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Orders;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Maintenance
{
    public class MaintenancesAppService : MessagingPortalAppServiceBase, IMaintenancesAppService
	{


		private IHubContext<TeamInboxHub> _hub2;
		private IHubContext<MaintenancesHub> _hub;
		private readonly IDocumentClient _IDocumentClient;

		public MaintenancesAppService(IHubContext<TeamInboxHub> hub2, IHubContext<MaintenancesHub> hub
						, IDocumentClient iDocumentClient


			)
		{
	
			_hub2 = hub2;
			_hub = hub;
			_IDocumentClient = iDocumentClient;


		}

		public PagedResultDto<GetMaintenancesForViewDto> GetAll(GetAllMaintenancesInput input)
        {

              var list = getMaintenances((int)AbpSession.TenantId, input.SkipCount, input.MaxResultCount);

			list.Reverse();



			return new PagedResultDto<GetMaintenancesForViewDto>(list.Count(), list);
        }

		public async Task Lock(EntityDto<long> input, int agentId, string agentName, string stringTotall)
		{
			var maintenanc = getMaintenancesByID(input.Id);

			maintenanc.isLockByAgent = true;
			maintenanc.AgentId = agentId;
			maintenanc.LockByAgentName = agentName;

			UpdateMaintenanc(maintenanc);


            var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), maintenanc.OrderStatus);

            GetMaintenancesForViewDto getOrderForViewDto = new GetMaintenancesForViewDto();
   
              getOrderForViewDto.orderStatusName = orderStatusName;
			getOrderForViewDto.TenantID = maintenanc.TenantID;

			getOrderForViewDto.isLockByAgent = true;
			getOrderForViewDto.AgentId = agentId;
			getOrderForViewDto.LockByAgentName = agentName;
			getOrderForViewDto.Id = maintenanc.Id;
			//await _hub.Clients.All.SendAsync("MaintenancesAgentOrder", getOrderForViewDto);
		}
		public async Task UnLock(EntityDto<long> input, string stringTotall)
		{
			var maintenanc = getMaintenancesByID(input.Id);

			maintenanc.isLockByAgent = false;
			maintenanc.AgentId = -1;
			maintenanc.LockByAgentName = "";

			UpdateMaintenanc(maintenanc);


			var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), maintenanc.OrderStatus);

			GetMaintenancesForViewDto getOrderForViewDto = new GetMaintenancesForViewDto();

			getOrderForViewDto.orderStatusName = orderStatusName;
			getOrderForViewDto.TenantID = maintenanc.TenantID;
			getOrderForViewDto.isLockByAgent = false;
			getOrderForViewDto.AgentId = -1;
			getOrderForViewDto.LockByAgentName = "";
			getOrderForViewDto.Id = maintenanc.Id;
			await _hub.Clients.All.SendAsync("MaintenancesAgentOrder", getOrderForViewDto);
		}
		public async Task DeleteOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName)
		{
			var maintenanc = getMaintenancesByID(input.Id);

			maintenanc.isLockByAgent = true;
			maintenanc.AgentId = agentId;
			maintenanc.LockByAgentName = agentName;
			maintenanc.OrderStatus = 3; //Delete
			UpdateMaintenanc(maintenanc);
			
			var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), maintenanc.OrderStatus);
			

		


			var contact = GetCustomer(maintenanc.ContactId);
			var user = GetCustomerAzuer(contact.UserId);

			var captionDeleteOrderText = GetCaptionFormat("BackEnd_Text_DeleteOrder", "ar-JO", maintenanc.TenantID, "", "");//تم الغاء الطلب من قبل المطعم  ...

			if (maintenanc.OrderLocal == "ar" || maintenanc.OrderLocal == null)
			{

				captionDeleteOrderText = GetCaptionFormat("BackEnd_Text_DeleteOrder", "ar-JO", maintenanc.TenantID, "", "");//تم الغاء الطلب من قبل المطعم  ...


			}
			else
			{
				captionDeleteOrderText = "the Order has been canceled ";// GetCaptionFormat("BackEnd_Text_DeleteOrder", "ar-JO", order.TenantId, "", "");//تم الغاء الطلب من قبل المطعم  ...

			}



			SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
			{
				text = captionDeleteOrderText,
				type = "text",
				agentName = maintenanc.LockByAgentName,
				agentId = maintenanc.AgentId.ToString()


			};


			if (string.IsNullOrEmpty(user.SunshineAppID))
			{
				Send2WhatsAppD360Model masseges = new Send2WhatsAppD360Model
				{
					to = user.phoneNumber,
					type = "text",
					text = new Send2WhatsAppD360Model.Text
					{
						body = captionDeleteOrderText
					}
				};

				//Todo Make the connector as a Service and return status in this method
				var result = await WhatsAppDialogConnector2.PostMsgToSmooch(user.D360Key, masseges);
				if (result == HttpStatusCode.Created)
				{
					var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
					user.customerChat = CustomerChat;
					await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", user);
				}

			}
			else
			{

				var tenant = GetTenantByAppId(user.SunshineAppID).Result;

				var result = await SunshineConnectorModel.PostMsgToSmooch(user.SunshineAppID, user.SunshineConversationId, message, tenant.SmoochAPIKeyID, tenant.SmoochSecretKey);

				if (result == HttpStatusCode.Created)
				{
					var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
					user.customerChat = CustomerChat;
					await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", user);

				}


			}




			GetMaintenancesForViewDto getOrderForViewDto = new GetMaintenancesForViewDto();

			getOrderForViewDto.orderStatusName = orderStatusName;
			getOrderForViewDto.TenantID = maintenanc.TenantID;

			getOrderForViewDto.isLockByAgent = true;
			getOrderForViewDto.AgentId = agentId;
			getOrderForViewDto.LockByAgentName = agentName;
			getOrderForViewDto.OrderStatus = 3; //Delete
			getOrderForViewDto.Id = maintenanc.Id;
			await _hub.Clients.All.SendAsync("MaintenancesAgentOrder", getOrderForViewDto);


		}
		public async Task DoneOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName)
		{
			var maintenanc = getMaintenancesByID(input.Id);

			maintenanc.isLockByAgent = true;
			maintenanc.AgentId = agentId;
			maintenanc.LockByAgentName = agentName;
			maintenanc.OrderStatus = 2; //Done
			UpdateMaintenanc(maintenanc);


			var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), maintenanc.OrderStatus);
			



			var contact = GetCustomer(maintenanc.ContactId);
			var user = GetCustomerAzuer(contact.UserId);


			string captionDoneOrderText = "طلبكم قيد التجهيز ...";
		
				if (maintenanc.OrderLocal == "ar" || maintenanc.OrderLocal == null)
				{
					captionDoneOrderText = GetCaptionFormat("BackEnd_Text_DoneOrder_Delivery", "ar-JO", maintenanc.TenantID, "", "");//طلبكم قيد التجهيز ... بحاجة الى 45 دقيقة

				}
				else
				{

					captionDoneOrderText = "Your Order is Being Processed ";// GetCaptionFormat("BackEnd_Text_DoneOrder_Delivery", "ar-JO", order.TenantId, "", "");//طلبكم قيد التجهيز ... بحاجة الى 45 دقيقة
				}





			SunshinePostMsgBotModel.Content message = new SunshinePostMsgBotModel.Content
			{
				text = captionDoneOrderText,
				type = "text",
				agentName = maintenanc.LockByAgentName,
				agentId = maintenanc.AgentId.ToString()

			};



			if (string.IsNullOrEmpty(user.SunshineAppID))
			{
				Send2WhatsAppD360Model masseges = new Send2WhatsAppD360Model
				{
					to = user.phoneNumber,
					type = "text",
					text = new Send2WhatsAppD360Model.Text
					{
						body = captionDoneOrderText
					}
				};

				//Todo Make the connector as a Service and return status in this method
				var result = await WhatsAppDialogConnector2.PostMsgToSmooch(user.D360Key, masseges);
				if (result == HttpStatusCode.Created)
				{
					var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
					user.customerChat = CustomerChat;
					await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", user);
				}

			}
			else
			{

				var tenant = GetTenantByAppId(user.SunshineAppID).Result;



				var result = await SunshineConnectorModel.PostMsgToSmooch(user.SunshineAppID, user.SunshineConversationId, message, tenant.SmoochAPIKeyID, tenant.SmoochSecretKey);

				if (result == HttpStatusCode.Created)
				{
					var CustomerChat = UpdateCustomerChat(user.TenantId, message, user.userId, user.SunshineConversationId);
					user.customerChat = CustomerChat;
					await _hub2.Clients.All.SendAsync("brodCastEndUserMessage", user);

				}


			}





			GetMaintenancesForViewDto getOrderForViewDto = new GetMaintenancesForViewDto();

			getOrderForViewDto.orderStatusName = orderStatusName;
			getOrderForViewDto.TenantID = maintenanc.TenantID;



			getOrderForViewDto.isLockByAgent = true;
			getOrderForViewDto.AgentId = agentId;
			getOrderForViewDto.LockByAgentName = agentName;
			getOrderForViewDto.OrderStatus = 2; //Done
			getOrderForViewDto.Id = maintenanc.Id;



			await _hub.Clients.All.SendAsync("MaintenancesAgentOrder", getOrderForViewDto);
		}
		public async Task CloseOrder(EntityDto<long> input, string stringTotall)
		{
			var maintenanc = getMaintenancesByID(input.Id);
			maintenanc.OrderRemarks = "CancelByAgent";
			UpdateMaintenanc(maintenanc);


			var orderStatusName = Enum.GetName(typeof(OrderStatusEunm), maintenanc.OrderStatus);

			GetMaintenancesForViewDto getOrderForViewDto = new GetMaintenancesForViewDto();

			getOrderForViewDto.orderStatusName = orderStatusName;
			getOrderForViewDto.TenantID = maintenanc.TenantID;
			getOrderForViewDto.Id = maintenanc.Id;
			getOrderForViewDto.OrderRemarks = "CancelByAgent";

			await _hub.Clients.All.SendAsync("MaintenancesAgentOrder", getOrderForViewDto);
		}


		private List<GetMaintenancesForViewDto> getMaintenances(int TenantID, int PageNumber,int PageSize)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[Maintenance] where TenantID=" + TenantID+ "ORDER BY Id  OFFSET " + (PageNumber * PageSize) + " ROWS FETCH NEXT " + PageSize + " ROWS ONLY";


            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<GetMaintenancesForViewDto>  getMaintenancesForViewDtos = new List<GetMaintenancesForViewDto>();

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                //  var IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString());


                try
                {
                    getMaintenancesForViewDtos.Add(new GetMaintenancesForViewDto
                    {
						UserId= dataSet.Tables[0].Rows[i]["UserId"].ToString() ?? "",
						DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString() ?? "",
						phoneNumber = dataSet.Tables[0].Rows[i]["phoneNumber"].ToString() ?? "",
						Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
                        Address = dataSet.Tables[0].Rows[i]["Address"].ToString() ?? "",
                        CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? ""),
                        ContactId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactId"].ToString() ?? ""),
                        OrderStatus = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? ""),
                        DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? ""),
                        OrderLocal = dataSet.Tables[0].Rows[i]["OrderLocal"].ToString() ?? "",
                        CustomerName = dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? "",
                        PhoneNumber1 = dataSet.Tables[0].Rows[i]["PhoneNumber1"].ToString() ?? "",
                        PhoneNumber2 = dataSet.Tables[0].Rows[i]["PhoneNumber2"].ToString() ?? "",
                        PhoneType = dataSet.Tables[0].Rows[i]["PhoneType"].ToString() ?? "",
                        Damage = dataSet.Tables[0].Rows[i]["Damage"].ToString() ?? "",
                         StringTotal=(Math.Round(Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "") * 100) / 100).ToString("N2")+" JD",
                         CreationDateString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd/yyyy"),
                          CreationTimeString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd hh:mm tt"),
                           isLockByAgent = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["isLockByAgent"].ToString() ?? ""),
                           LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString() ?? "",
                            orderStatusName = Enum.GetName(typeof(OrderStatusEunm), Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? "")),
                            AgentId= Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
						   OrderRemarks = dataSet.Tables[0].Rows[i]["OrderRemarks"].ToString() ?? "",


					});

                }catch(Exception )
                {
					try
					{
						getMaintenancesForViewDtos.Add(new GetMaintenancesForViewDto
						{
							UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString() ?? "",
							DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString() ?? "",
							phoneNumber = dataSet.Tables[0].Rows[i]["phoneNumber"].ToString() ?? "",
							Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
							Address = dataSet.Tables[0].Rows[i]["Address"].ToString() ?? "",
							CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? ""),
							ContactId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactId"].ToString() ?? ""),
							OrderStatus = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? ""),
							DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? ""),
							OrderLocal = dataSet.Tables[0].Rows[i]["OrderLocal"].ToString() ?? "",
							CustomerName = dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? "",
							PhoneNumber1 = dataSet.Tables[0].Rows[i]["PhoneNumber1"].ToString() ?? "",
							PhoneNumber2 = dataSet.Tables[0].Rows[i]["PhoneNumber2"].ToString() ?? "",
							PhoneType = dataSet.Tables[0].Rows[i]["PhoneType"].ToString() ?? "",
							Damage = dataSet.Tables[0].Rows[i]["Damage"].ToString() ?? "",
							StringTotal = (Math.Round(Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "") * 100) / 100).ToString("N2") + " JD",
							CreationDateString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd/yyyy"),
							CreationTimeString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd hh:mm tt"),
							isLockByAgent = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["isLockByAgent"].ToString() ?? ""),
							LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString() ?? "",
							orderStatusName = Enum.GetName(typeof(OrderStatusEunm), Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? "")),
							AgentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
							OrderRemarks= dataSet.Tables[0].Rows[i]["OrderRemarks"].ToString() ?? "",


						});
					}
					catch
					{
						getMaintenancesForViewDtos.Add(new GetMaintenancesForViewDto
						{
							UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString() ?? "",
							DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString() ?? "",
							phoneNumber = dataSet.Tables[0].Rows[i]["phoneNumber"].ToString() ?? "",
							Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
							Address = dataSet.Tables[0].Rows[i]["Address"].ToString() ?? "",
							CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? ""),
							ContactId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactId"].ToString() ?? ""),
							OrderStatus = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? ""),
							DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? ""),
							OrderLocal = dataSet.Tables[0].Rows[i]["OrderLocal"].ToString() ?? "",
							CustomerName = dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? "",
							PhoneNumber1 = dataSet.Tables[0].Rows[i]["PhoneNumber1"].ToString() ?? "",
							PhoneNumber2 = dataSet.Tables[0].Rows[i]["PhoneNumber2"].ToString() ?? "",
							PhoneType = dataSet.Tables[0].Rows[i]["PhoneType"].ToString() ?? "",
							Damage = dataSet.Tables[0].Rows[i]["Damage"].ToString() ?? "",
							StringTotal = (Math.Round(Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "") * 100) / 100).ToString("N2") + " JD",
							CreationDateString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd/yyyy"),
							CreationTimeString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd hh:mm tt"),
							isLockByAgent = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["isLockByAgent"].ToString() ?? ""),
							LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString() ?? "",
							orderStatusName = Enum.GetName(typeof(OrderStatusEunm), Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? "")),
							AgentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
							OrderRemarks =  "",


						});


					}
				}
              


                

            }

            conn.Close();
            da.Dispose();

            return getMaintenancesForViewDtos;

        }
		private GetMaintenancesForViewDto getMaintenancesByID(long id)
		{
			string connString = AppSettingsModel.ConnectionStrings;
			string query = "select * from [dbo].[Maintenance] where Id=" + id;


			SqlConnection conn = new SqlConnection(connString);
			SqlCommand cmd = new SqlCommand(query, conn);
			conn.Open();

			// create the DataSet 
			DataSet dataSet = new DataSet();

			// create data adapter
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			// this will query your database and return the result to your datatable
			da.Fill(dataSet);

			GetMaintenancesForViewDto getMaintenancesForViewDtos = new GetMaintenancesForViewDto();

			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{
				//  var IsAvailableBranch = bool.Parse(dataSet.Tables[0].Rows[i]["IsAvailableBranch"].ToString());


				try
				{
					getMaintenancesForViewDtos=new GetMaintenancesForViewDto
					{
						UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString() ?? "",
						TenantID = int.Parse(dataSet.Tables[0].Rows[i]["TenantID"].ToString() ?? "0"),
						DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString() ?? "",
						phoneNumber = dataSet.Tables[0].Rows[i]["phoneNumber"].ToString() ?? "",
						Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
						Address = dataSet.Tables[0].Rows[i]["Address"].ToString() ?? "",
						CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? ""),
						ContactId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactId"].ToString() ?? ""),
						OrderStatus = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? ""),
						DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? ""),
						OrderLocal = dataSet.Tables[0].Rows[i]["OrderLocal"].ToString() ?? "",
						CustomerName = dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? "",
						PhoneNumber1 = dataSet.Tables[0].Rows[i]["PhoneNumber1"].ToString() ?? "",
						PhoneNumber2 = dataSet.Tables[0].Rows[i]["PhoneNumber2"].ToString() ?? "",
						PhoneType = dataSet.Tables[0].Rows[i]["PhoneType"].ToString() ?? "",
						Damage = dataSet.Tables[0].Rows[i]["Damage"].ToString() ?? "",
						StringTotal = (Math.Round(Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "") * 100) / 100).ToString("N2") + " JD",
						CreationDateString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd/yyyy"),
						CreationTimeString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd hh:mm tt"),
						isLockByAgent = Convert.ToBoolean(dataSet.Tables[0].Rows[i]["isLockByAgent"].ToString() ?? ""),
						LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString() ?? "",
						orderStatusName = Enum.GetName(typeof(OrderStatusEunm), Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? "")),
						AgentId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AgentId"]),
						OrderRemarks= dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? ""

					};

				}
				catch (Exception )
				{


                    try
                    {
						getMaintenancesForViewDtos = new GetMaintenancesForViewDto
						{
							UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString() ?? "",
							TenantID = int.Parse(dataSet.Tables[0].Rows[i]["TenantID"].ToString() ?? "0"),
							DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString() ?? "",
							phoneNumber = dataSet.Tables[0].Rows[i]["phoneNumber"].ToString() ?? "",
							Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
							Address = dataSet.Tables[0].Rows[i]["Address"].ToString() ?? "",
							CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? ""),
							ContactId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactId"].ToString() ?? ""),
							OrderStatus = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? ""),
							DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? ""),
							OrderLocal = dataSet.Tables[0].Rows[i]["OrderLocal"].ToString() ?? "",
							CustomerName = dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? "",
							PhoneNumber1 = dataSet.Tables[0].Rows[i]["PhoneNumber1"].ToString() ?? "",
							PhoneNumber2 = dataSet.Tables[0].Rows[i]["PhoneNumber2"].ToString() ?? "",
							PhoneType = dataSet.Tables[0].Rows[i]["PhoneType"].ToString() ?? "",
							Damage = dataSet.Tables[0].Rows[i]["Damage"].ToString() ?? "",
							StringTotal = (Math.Round(Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "") * 100) / 100).ToString("N2") + " JD",
							CreationDateString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd/yyyy"),
							CreationTimeString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd hh:mm tt"),
							isLockByAgent = false,
							LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString() ?? "",
							orderStatusName = Enum.GetName(typeof(OrderStatusEunm), Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? "")),
							AgentId = 0,
							OrderRemarks = dataSet.Tables[0].Rows[i]["OrderRemarks"].ToString() ?? ""



						};
					}
                    catch
					{
						getMaintenancesForViewDtos = new GetMaintenancesForViewDto
						{
							UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString() ?? "",
							TenantID = int.Parse(dataSet.Tables[0].Rows[i]["TenantID"].ToString() ?? "0"),
							DisplayName = dataSet.Tables[0].Rows[i]["DisplayName"].ToString() ?? "",
							phoneNumber = dataSet.Tables[0].Rows[i]["phoneNumber"].ToString() ?? "",
							Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
							Address = dataSet.Tables[0].Rows[i]["Address"].ToString() ?? "",
							CreationTime = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? ""),
							ContactId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["ContactId"].ToString() ?? ""),
							OrderStatus = Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? ""),
							DeliveryCost = Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? ""),
							OrderLocal = dataSet.Tables[0].Rows[i]["OrderLocal"].ToString() ?? "",
							CustomerName = dataSet.Tables[0].Rows[i]["CustomerName"].ToString() ?? "",
							PhoneNumber1 = dataSet.Tables[0].Rows[i]["PhoneNumber1"].ToString() ?? "",
							PhoneNumber2 = dataSet.Tables[0].Rows[i]["PhoneNumber2"].ToString() ?? "",
							PhoneType = dataSet.Tables[0].Rows[i]["PhoneType"].ToString() ?? "",
							Damage = dataSet.Tables[0].Rows[i]["Damage"].ToString() ?? "",
							StringTotal = (Math.Round(Convert.ToDecimal(dataSet.Tables[0].Rows[i]["DeliveryCost"].ToString() ?? "") * 100) / 100).ToString("N2") + " JD",
							CreationDateString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd/yyyy"),
							CreationTimeString = Convert.ToDateTime(dataSet.Tables[0].Rows[i]["CreationTime"].ToString() ?? "").ToString("MM/dd hh:mm tt"),
							isLockByAgent = false,
							LockByAgentName = dataSet.Tables[0].Rows[i]["LockByAgentName"].ToString() ?? "",
							orderStatusName = Enum.GetName(typeof(OrderStatusEunm), Convert.ToInt32(dataSet.Tables[0].Rows[i]["OrderStatus"].ToString() ?? "")),
							AgentId = 0,
							OrderRemarks =  ""



						};


					}
				
				}

			}

			conn.Close();
			da.Dispose();

			return getMaintenancesForViewDtos;

		}

		private void UpdateMaintenanc(GetMaintenancesForViewDto model)
		{
			var timeAdd = DateTime.Now.AddHours(AppSettingsModel.AddHour);

			string connString = AppSettingsModel.ConnectionStrings;
			using (SqlConnection connection = new SqlConnection(connString))
				try
				{

					using (SqlCommand command = connection.CreateCommand())
					{

						command.CommandText = "Update Maintenance set "
                               + "OrderStatus = @OrderStatus ,"
                                + "isLockByAgent = @isLockByAgent ,"
								 + "OrderRemarks = @OrderRemarks ,"
								 + "LockByAgentName = @LockByAgentName ,"
								  + "AgentId = @AgentId "
								  + "where Id= @Id";


						command.Parameters.AddWithValue("@Id", model.Id);
						command.Parameters.AddWithValue("@OrderStatus", model.OrderStatus);
						command.Parameters.AddWithValue("@isLockByAgent", model.isLockByAgent);
						command.Parameters.AddWithValue("@OrderRemarks", model.OrderRemarks);
						command.Parameters.AddWithValue("@LockByAgentName", model.LockByAgentName);
						command.Parameters.AddWithValue("@AgentId", model.AgentId);


						connection.Open();
						command.ExecuteNonQuery();
						connection.Close();
					}
				}
				catch (Exception )
				{


				}

		}

		private Contact GetCustomer(int? CustomerId)
		{
			string connString = AppSettingsModel.ConnectionStrings;
			string query = "select * from [dbo].[Contacts] where Id=" + CustomerId;


			SqlConnection conn = new SqlConnection(connString);
			SqlCommand cmd = new SqlCommand(query, conn);
			conn.Open();

			// create the DataSet 
			DataSet dataSet = new DataSet();

			// create data adapter
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			// this will query your database and return the result to your datatable
			da.Fill(dataSet);

			Contact contact = new Contact();

			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{

				contact = new Contact
				{
					Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
					UserId = dataSet.Tables[0].Rows[i]["UserId"].ToString(),
					TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
					PhoneNumber = dataSet.Tables[0].Rows[i]["PhoneNumber"].ToString()
				};
			}

			conn.Close();
			da.Dispose();

			return contact;
		}
		private CustomerModel GetCustomerAzuer(string UserId)
		{
			string result = string.Empty;
			var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
			var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && UserId != null && a.userId == UserId);
			if (customerResult.IsCompletedSuccessfully)
			{
				return customerResult.Result;

			}


			return null;
		}
		private async Task<TenantModel> GetTenantByAppId(string id)
		{
			if (string.IsNullOrEmpty(id))
				return null;
			var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

			TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.SmoochAppID == id);
			return tenant;
		}
		private string GetCaptionFormat(string key, string lang, int? TenantID, string parm1, string parm2)
		{
			var langg = GetLanguageBot(TenantID, lang);
			var testR = GetTextResource(TenantID, key);
			var caption = GetAllCaption(TenantID, langg.Id, testR.Id);

			if (caption != null && testR.Category == "Welcome_Text")
			{
				var text = string.Format(caption.Text, parm1.Trim());

				return text.Replace("\\r\\n", "\r\n");
			}
			if (caption != null && testR.Category == "PlaceOrder_Text")
			{
				var text = string.Format(caption.Text, TenantID.ToString(), parm1.Trim());

				return text.Replace("\\r\\n", "\r\n");
			}

			if (caption != null && testR.Category == "CancelOrder_SeccesMassege")
			{
				var text = string.Format(caption.Text, parm1.ToString(), parm2.Trim());

				return text.Replace("\\r\\n", "\r\n");
			}

			if (caption != null && testR.Category == "NextBranch_Text")
			{
				var text = string.Format(caption.Text, parm1.ToString());

				return text.Replace("\\r\\n", "\r\n");
			}
			if (caption != null && testR.Category == "Select_Text")
			{
				var text = string.Format(caption.Text, parm1.ToString());

				return text.Replace("\\r\\n", "\r\n");
			}

			if (caption != null)
			{

				return caption.Text.Replace("\\r\\n", "\r\n");
			}

			return "";
		}

		private Caption GetAllCaption(int? TenantID, int langId, int testRId)
		{

			try
			{
				string connString = AppSettingsModel.ConnectionStrings;
				string query = "select * from [dbo].[Caption] where TenantID=" + TenantID + "and LanguageBotId =  " + langId + " and TextResourceId=" + testRId;


				SqlConnection conn = new SqlConnection(connString);
				SqlCommand cmd = new SqlCommand(query, conn);
				conn.Open();

				// create the DataSet 
				DataSet dataSet = new DataSet();

				// create data adapter
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				// this will query your database and return the result to your datatable
				da.Fill(dataSet);

				Caption captions = new Caption();

				for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
				{

					captions = new Caption
					{
						Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
						Text = dataSet.Tables[0].Rows[i]["Text"].ToString(),
						TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),
						LanguageBotId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["LanguageBotId"]),
						TextResourceId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TextResourceId"]),

					};
				}

				conn.Close();
				da.Dispose();

				return captions;

			}
			catch
			{
				return null;

			}

		}
		private LanguageBot GetLanguageBot(int? TenantID, string lang)
		{

			try
			{
				string connString = AppSettingsModel.ConnectionStrings;
				string query = "select * from [dbo].[LanguageBot] where  ISO ='" + lang + "'";


				SqlConnection conn = new SqlConnection(connString);
				SqlCommand cmd = new SqlCommand(query, conn);
				conn.Open();

				// create the DataSet 
				DataSet dataSet = new DataSet();

				// create data adapter
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				// this will query your database and return the result to your datatable
				da.Fill(dataSet);

				LanguageBot languageBots = new LanguageBot();

				for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
				{

					languageBots = new LanguageBot
					{
						Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
						ISO = dataSet.Tables[0].Rows[i]["ISO"].ToString(),
						Name = dataSet.Tables[0].Rows[i]["Name"].ToString(),
						TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

					};
				}

				conn.Close();
				da.Dispose();

				return languageBots;

			}
			catch
			{
				return null;

			}

		}
		private TextResource GetTextResource(int? TenantID, string key)
		{

			try
			{
				string connString = AppSettingsModel.ConnectionStrings;
				string query = "select * from [dbo].[TextResource] where Category = '" + key + "'";


				SqlConnection conn = new SqlConnection(connString);
				SqlCommand cmd = new SqlCommand(query, conn);
				conn.Open();

				// create the DataSet 
				DataSet dataSet = new DataSet();

				// create data adapter
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				// this will query your database and return the result to your datatable
				da.Fill(dataSet);

				TextResource textResources = new TextResource();

				for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
				{

					textResources = new TextResource
					{
						Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["Id"]),
						Key = dataSet.Tables[0].Rows[i]["Key"].ToString(),
						Category = dataSet.Tables[0].Rows[i]["Category"].ToString(),
						TenantId = Convert.ToInt32(dataSet.Tables[0].Rows[i]["TenantId"]),

					};
				}

				conn.Close();
				da.Dispose();

				return textResources;

			}
			catch (Exception )
			{
				return null;

			}

		}
		private CustomerChat UpdateCustomerChat(int? tenantId, SunshinePostMsgBotModel.Content model, string userId, string conversationID)
		{
			var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

			// Create your new conversation instance
			var CustomerChat = new CustomerChat()
			{
				messageId = Guid.NewGuid().ToString(),
				userId = userId,
				SunshineConversationId = conversationID,
				text = model.text,
				type = model.type,
				fileName = model.fileName,
				CreateDate = DateTime.Now,
				status = 1,
				sender = Tenants.Contacts.MessageSenderType.TeamInbox,
				mediaUrl = model.mediaUrl,
				agentName = model.agentName,
				agentId = model.agentId,
				TenantId = tenantId,

			};
			var result = itemsCollection.CreateItemAsync(CustomerChat).Result;

			return CustomerChat;
		}
	}
}
