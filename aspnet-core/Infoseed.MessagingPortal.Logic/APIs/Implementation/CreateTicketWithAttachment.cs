
using Infoseed.MessagingPortal.MgSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Infoseed.MessagingPortal.Logic
{
    internal class CreateTicketWithAttachment
    {
        #region private
        private static void writeCRLF(Stream o)
        {
            byte[] crLf = Encoding.ASCII.GetBytes("\r\n");
            o.Write(crLf, 0, crLf.Length);
        }

        private static void writeBoundaryBytes(Stream o, string b, bool isFinalBoundary)
        {
            string boundary = isFinalBoundary == true ? "--" + b + "--" : "--" + b + "\r\n";
            byte[] d = Encoding.ASCII.GetBytes(boundary);
            o.Write(d, 0, d.Length);
        }

        private static void writeContentDispositionFormDataHeader(Stream o, string name)
        {
            string data = "Content-Disposition: form-data; name=\"" + name + "\"\r\n\r\n";
            byte[] b = Encoding.ASCII.GetBytes(data);
            o.Write(b, 0, b.Length);
        }

        private static void writeContentDispositionFileHeader(Stream o, string name, string fileName, string contentType)
        {
            string data = "Content-Disposition: form-data; name=\"" + name + "\"; filename=\"" + fileName + "\"\r\n";
            data += "Content-Type: " + contentType + "\r\n\r\n";
            byte[] b = Encoding.ASCII.GetBytes(data);
            o.Write(b, 0, b.Length);
        }

        private static void writeString(Stream o, string data)
        {
            //UTF8
            byte[] b = Encoding.UTF8.GetBytes(data);
            o.Write(b, 0, b.Length);
        }

        #endregion

        //internal static JObject Create(CreateTicket ticket, string url, string authToken)
        //{
        //    // Define boundary:
        //    string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

        //    // Web Request:
        //    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);

        //    wr.Headers.Clear();

        //    // Method and headers:
        //    wr.ContentType = "application/json; boundary=" + boundary;
        //    wr.Method = "POST";
        //    wr.KeepAlive = true;

        //    // Basic auth:
        //    wr.Headers[HttpRequestHeader.Authorization] = authToken;

        //    // Body:
        //    using (var rs = wr.GetRequestStream())
        //    {
        //        if (!string.IsNullOrWhiteSpace(ticket.subject))
        //        {
        //            // Email:
        //            writeBoundaryBytes(rs, boundary, false);
        //            writeContentDispositionFormDataHeader(rs, "subject");
        //            writeString(rs, ticket.subject);
        //            writeCRLF(rs);
        //        }


        //        // Subject:
        //        writeBoundaryBytes(rs, boundary, false);
        //        writeContentDispositionFormDataHeader(rs, "content");
        //        writeString(rs, ticket.content);
        //        writeCRLF(rs);

        //        // Name:
        //        writeBoundaryBytes(rs, boundary, false);
        //        writeContentDispositionFormDataHeader(rs, "hs_pipeline");
        //        writeString(rs, ticket.hs_pipeline);
        //        writeCRLF(rs);

        //        // Phone:
        //        writeBoundaryBytes(rs, boundary, false);
        //        writeContentDispositionFormDataHeader(rs, "hs_pipeline_stage");
        //        writeString(rs, ticket.hs_pipeline_stage);
        //        writeCRLF(rs);

        //        //// Description:
        //        //writeBoundaryBytes(rs, boundary, false);
        //        //writeContentDispositionFormDataHeader(rs, "description");
        //        //writeString(rs, ticket.Description);
        //        //writeCRLF(rs);

        //        //// Status:
        //        //writeBoundaryBytes(rs, boundary, false);
        //        //writeContentDispositionFormDataHeader(rs, "status");
        //        //writeString(rs, ticket.Status);
        //        //writeCRLF(rs);

        //        //// Priority:
        //        //writeBoundaryBytes(rs, boundary, false);
        //        //writeContentDispositionFormDataHeader(rs, "priority");
        //        //writeString(rs, ticket.Priority);
        //        //writeCRLF(rs);

        //        //// Type:
        //        //writeBoundaryBytes(rs, boundary, false);
        //        //writeContentDispositionFormDataHeader(rs, "type");
        //        //writeString(rs, "Tickets");
        //        //writeCRLF(rs);

        //        //// Category:
        //        //if (ticket.Category != null)
        //        //{
        //        //    writeBoundaryBytes(rs, boundary, false);
        //        //    writeContentDispositionFormDataHeader(rs, "custom_fields[cf_customer_sub_category_1]");
        //        //    writeString(rs, ticket.Category.cf_customer_sub_category_1);
        //        //    writeCRLF(rs);

        //        //    writeBoundaryBytes(rs, boundary, false);
        //        //    writeContentDispositionFormDataHeader(rs, "custom_fields[cf_customer_sub_category_2]");
        //        //    writeString(rs, ticket.Category.cf_customer_sub_category_2);
        //        //    writeCRLF(rs);
        //        //}

        //        //// OrderNumber:
        //        //if (!string.IsNullOrWhiteSpace(ticket.OrderNumber))
        //        //{
        //        //    writeBoundaryBytes(rs, boundary, false);
        //        //    writeContentDispositionFormDataHeader(rs, "custom_fields[cf_order_number906294]");
        //        //    writeString(rs, ticket.OrderNumber);
        //        //    writeCRLF(rs);
        //        //}

        //        //// Attachment:
        //        //if (ticket.Attachments != null)
        //        //{
        //        //    foreach (var attachment in ticket.Attachments)
        //        //    {
        //        //        writeBoundaryBytes(rs, boundary, false);
        //        //        writeContentDispositionFileHeader(rs, "attachments[]", attachment.Filename, attachment.FileType);
        //        //        rs.Write(attachment.Base64, 0, attachment.Base64.Length);
        //        //        writeCRLF(rs);
        //        //    }
        //        //}
        //        // End marker:
        //        writeBoundaryBytes(rs, boundary, true);

        //        rs.Close();

        //        try
        //        {
        //            var response = (HttpWebResponse)wr.GetResponse();
        //            Stream resStream = response.GetResponseStream();
        //            string Response = new StreamReader(resStream, Encoding.ASCII).ReadToEnd();

        //            return JsonConvert.DeserializeObject<JObject>(Response);
        //        }
        //        catch (WebException ex)
        //        {
        //            throw ex;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //}



    }
}
