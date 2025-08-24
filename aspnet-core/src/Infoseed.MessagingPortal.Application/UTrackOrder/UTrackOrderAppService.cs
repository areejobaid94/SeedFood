using Infoseed.MessagingPortal.UTracOrder;
using Infoseed.MessagingPortal.UTracOrder.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.UTrackOrder
{
    public class UTrackOrderAppService : MessagingPortalAppServiceBase, IUTrackOrderAppService
    {
        public async Task<UTracCreateOrderResponseModel> CreateUTracOrder(UTracCreateOrderModel model)
        {
            return await createUtracOrderAsync(model);
        }

        public async Task<UTracPriceResponseModel> GetUTracPriceList(string integrator_id)
        {
            return await getUTracPriceList(integrator_id);
        }
        public async Task<UTracOrderResponseModel> GetUTracOrderList(UTracSearchEntity model)
        {
            return await getUTracOrderList(model);
        }
        public async Task<UTracCancelOrderResponseModel> CancelUtracOrder(UTracCancelOrderModel model)
        {
            return await cancelUtracOrder(model);
        }












        #region Private Methods
        private async Task<UTracCreateOrderResponseModel> createUtracOrderAsync(UTracCreateOrderModel model)
        {
            try
            {
                var httpClient = new HttpClient();

                var url = Constants.UTrac.UTracTestBaseUrl + "create_integrator_order";

                var jsonBody = JsonConvert.SerializeObject(model);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, content);

                var result = await response.Content.ReadAsStringAsync();

                UTracCreateOrderResponseModel _responseModel = JsonConvert.DeserializeObject<UTracCreateOrderResponseModel>(result);

                return _responseModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<UTracPriceResponseModel> getUTracPriceList(string integrator_id)
        {
            try
            {
                var httpClient = new HttpClient();

                var url = Constants.UTrac.UTracTestBaseUrl + "get_price_list";

                var response = await httpClient.GetAsync(url + "?" + integrator_id);

                var result = await response.Content.ReadAsStringAsync();

                UTracPriceResponseModel _responseModel = JsonConvert.DeserializeObject<UTracPriceResponseModel>(result);

                return _responseModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<UTracOrderResponseModel> getUTracOrderList(UTracSearchEntity model)
        {
            try
            {
                var httpClient = new HttpClient();

                var url = Constants.UTrac.UTracTestBaseUrl + "get_integrator_order_list";

                var jsonBody = JsonConvert.SerializeObject(model);


                var queryStringParams = new Dictionary<string, string>
                {
                    { "search", JsonConvert.SerializeObject(model.SearchModel) },
                    { "pagination", JsonConvert.SerializeObject(model.PaginationModel) }
                };

                var query = string.Join("&", queryStringParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

                var response = await httpClient.GetAsync(url + "?" + query);

                var result = await response.Content.ReadAsStringAsync();

                UTracOrderResponseModel _responseModel = JsonConvert.DeserializeObject<UTracOrderResponseModel>(result);

                return _responseModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private async Task<UTracCancelOrderResponseModel> cancelUtracOrder(UTracCancelOrderModel model)
        {
            try
            {
                var httpClient = new HttpClient();

                var url = Constants.UTrac.UTracTestBaseUrl + "cancel_integrator_order";

                var jsonBody = JsonConvert.SerializeObject(model);

                var request = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();

                UTracCancelOrderResponseModel _responseModel = JsonConvert.DeserializeObject<UTracCancelOrderResponseModel>(result);

                return _responseModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
    }
