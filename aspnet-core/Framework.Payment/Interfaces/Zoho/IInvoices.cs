using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Payment.Interfaces.Zoho
{
    public interface IInvoices
    {
        //  void CreateInvoices(string model);
        string GetInvoicesByInvoiceId(string invoicenumber);
        string GetInvoicesOverdue(long customerId);
        string GetInvoices(long customerId,int pageNumber , int pageSize );
        string GenerateAccessToken(string code);

        string GetInvoicesSearch(string url);

        string GetInvoicesStatus(string ZohoCustomerId, string status);
        string GetContactsByPhoneNumber(string PhoneNumber);
        string GetContactsList();

        string GetStatements(string ZohoCustomerId, string filter);
        string GetInvoicesByCustomerId(string CustomerId);
    }
}
