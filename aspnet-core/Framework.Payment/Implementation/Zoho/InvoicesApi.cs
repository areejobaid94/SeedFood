
using Framework.Data;
using Framework.Payment.Interfaces.Zoho;
using Framework.Payment.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Framework.Payment.Implementation.Zoho
{
    public class InvoicesApi : APIBase, IInvoices
    {
        private IConfiguration _configuration;
        public InvoicesApi(IConfiguration configuration) : base(configuration)
        {
            _configuration=configuration;

        }
        public string GenerateAccessToken(string code)
        {
            var client = new RestClient("https://accounts.zoho.com/oauth/v2/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "_zcsr_tmp=f3bac8d5-a5c5-4013-96c9-b599113bc4ea; b266a5bf57=a711b6da0e6cbadb5e254290f114a026; iamcsr=f3bac8d5-a5c5-4013-96c9-b599113bc4ea");
            request.AlwaysMultipartFormData = true;
            request.AddParameter("client_id", this._configuration["Zoho:ClientID"]);
            request.AddParameter("client_secret", this._configuration["Zoho:ClientSecret"]);
            request.AddParameter("redirect_uri", this._configuration["Zoho:RedirectUri"]);
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "authorization_code");
            IRestResponse response = client.Execute(request);

            var rez = JsonConvert.DeserializeObject<AccessTokenModel>(response.Content);

            access_token=rez.access_token;
            refresh_token=rez.refresh_token;
            basicAuthToken = "Bearer "+access_token;

            return response.Content;
        }

        public string GetContactsByPhoneNumber(string PhoneNumber)
        {
            var result = HttpGet("https://invoice.zoho.com/api/v3/contacts?phone="+PhoneNumber);

            return result;
        }

        public string GetInvoices(long customerId,int pageNumber = 1, int pageSize = 100)
        {
            

            var result = HttpGet("https://invoice.zoho.com/api/v3/invoices?customer_id="+customerId+"&&per_page="+pageSize+"&&page="+pageNumber+"&&response_option=1");

            return result;
        }

        public string GetInvoicesByInvoiceId(string invoicenumber)
        {
            var result = HttpGet("https://invoice.zoho.com/api/v3/invoices?invoice_number="+invoicenumber);

            return result;
        }

        public string GetInvoicesOverdue(long customerId)
        {
            var result = HttpGet("https://invoice.zoho.com/api/v3/invoices?customer_id="+customerId+"&&status=overdue");

            return result;
        }

        public string GetInvoicesSearch(string url)
        {
            var result = HttpGet(url);

            return result;
        }

        public  string GetInvoicesStatus(string ZohoCustomerId, string status)
        {
     
            var result = HttpGet("https://invoice.zoho.com/api/v3/invoices?customer_id="+ZohoCustomerId+"&&status="+status);

            return result;


        }
        public string GetStatements(string ZohoCustomerId, string filter)
        {
            var start = "";
            var end = "";

            FunDate(filter, ref start, ref end);

            var result = HttpGet("https://invoice.zoho.com/api/v3/customers/"+ZohoCustomerId+"/statements?from_date="+start+"&to_date="+end);

            return result;


        }

        public string GetInvoicesByCustomerId(string CustomerId)
        {
            var result = HttpGet("https://invoice.zoho.com/api/v3/invoices?customer_id="+CustomerId);

            return result;
        }

        private static void FunDate(string filter, ref string start, ref string end)
        {
            switch (filter)
            {
                case "today":
                    start = DateTime.UtcNow.ToString("yyyy-MM-dd"); 
                    end  = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    break;
                case "this_week":
                    start = DateTime.UtcNow.ToString("yyyy-MM-dd"); //DateTime.UtcNow.Year+"-"+DateTime.UtcNow.Month+"-"+DateTime.UtcNow.DayOfWeek;
                    end  = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd");
                    break;
                case "this_month":
                    start = DateTime.UtcNow.Year+"-"+DateTime.UtcNow.ToString("MM")+"-01";
                    end = DateTime.UtcNow.Year+"-"+DateTime.UtcNow.ToString("MM")+"-"+DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month) ;
                    break;
                case "this_year":
                    start = DateTime.UtcNow.Year+"-01-01";
                    end = DateTime.UtcNow.Year+"-12-"+DateTime.DaysInMonth(DateTime.UtcNow.Year, 12);
                    break;
                case "yesterday":
                    start = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
                    end  = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    break;
                case "previous_week":
                    start = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
                    end  = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    break;
                case "previous_month":
                    var dat= DateTime.UtcNow.AddMonths(-1);
                    start =dat.Year+"-"+dat.ToString("MM")+"-01";
                    end =dat.Year+"-"+dat.ToString("MM")+"-"+DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
                    break;
                case "previous_quarter":
                    start = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    end  = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    break;

            }
        }

        public string GetContactsList()
        {
       
            var result = HttpGet("https://invoice.zoho.com/api/v3/contacts");

            return result;
        }
    }
}
