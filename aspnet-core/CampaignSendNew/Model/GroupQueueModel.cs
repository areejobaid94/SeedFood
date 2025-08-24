using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignSendNew.Model
{
    public class GroupQueueModel
    {
        public long rowId { get; set; } = 0;
        public int tenantId { get; set; }
        public long groupId { get; set; }
        public string contactJson { get; set; }
        public DateTime createdDate { get; set; }
        public bool IsExternalContact { get; set; }
        public bool IsCreated { get; set; }
        public List<MembersDto> membersDto { get; set; }=new List<MembersDto>(){ };
    }



    public class MembersDto
    {
        public int id { get; set; }
        public string phoneNumber { get; set; }
        public string displayName { get; set; }
        public int failedId { get; set; }
        public bool isFailed { get; set; }
        public Dictionary<string, string> variables { get; set; }
        public int customeropt { get; set; }
    }

    //public class MembersDto
    //{
    //    public int id { get; set; }
    //    public string phoneNumber { get; set; }
    //    public string displayName { get; set; }
    //    public int failedId { get; set; }
    //    public bool isFailed { get; set; }
    //}
}
