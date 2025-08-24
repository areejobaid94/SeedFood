using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;
using NPOI.SS.UserModel;

namespace Infoseed.MessagingPortal.Authorization.Users.Exporting
{
    public class UserLoginExcelExporter : NpoiExcelExporterBase, IUserLoginExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserLoginExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UserLoginAttemptDto> userListDtos)
        {
            return CreateExcelPackage(
                "Users.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.CreateSheet(L("Users"));

                    AddHeader(
                        sheet,
                        L("UserNameOrEmail"),
                        L("ClientIpAddress"),
                        L("BrowserInfo"),
                        L("CreationDate"),
                        L("CreationTime")
                        );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.UserNameOrEmail,
                        _ => _.ClientIpAddress,
                        _ => _.BrowserInfo,
                        _ => _.formattedDate,
                        _ => _.formattedTime                      
                        );
                    
                    //for (var i = 1; i <= userListDtos.Count; i++)
                    //{
                    //    //Formatting cells
                    //    SetCellDataFormat(sheet.GetRow(i).Cells[8], "yyyy-mm-dd");
                    //}
                    
                    //for (var i = 0; i < 9; i++)
                    //{
                    //    sheet.AutoSizeColumn(i);
                    //}
                });
        }
    }
}
