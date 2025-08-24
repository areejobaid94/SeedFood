using Infoseed.MessagingPortal.Storage;
using Infoseed.MessagingPortal.Web.Sunshine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowFileController : MessagingPortalControllerBase
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        IDBService _liteDBService;
        private IHubContext<TeamInboxHub> _hub;

        public ShowFileController(
          IDBService liteDBService,
          IHubContext<TeamInboxHub> hub, IBinaryObjectManager binaryObjectManager)
        {
            _binaryObjectManager = binaryObjectManager;
            _liteDBService = liteDBService;
            _hub = hub;
        }

        /// <summary>
        /// api to get the id of attachment  from data table (dbo.AppBinaryObjects) and return the file 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("ShowFile")]
        public async Task<IActionResult> ShowFileAsync(Guid Id)
        {
            try
            {
                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    var fileObject = await _binaryObjectManager.GetOrNullAsync(Id);
                    if (fileObject == null)
                    {
                        return StatusCode((int)HttpStatusCode.NotFound);
                    }
                    return File(fileObject.Bytes, fileObject.MimeType, fileObject.FileName);
                }    

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}
