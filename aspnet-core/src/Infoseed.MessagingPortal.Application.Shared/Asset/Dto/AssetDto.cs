using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Asset.Dto
{
    public class AssetDto
    {
        public long Id { get; set; }
        public string AssetNameAr { get; set; }
        public string AssetNameEn { get; set; }
        public string AssetDescriptionAr { get; set; }
        public string AssetDescriptionEn { get; set; }
        public int TenantId { get; set; }
        public long AssetLevelOneId { get; set; }
        public long? AssetLevelTwoId { get; set; }
        public long? AssetLevelThreeId { get; set; }
        public long AssetLevelFourId { get; set; }
        public int AssetTypeId { get; set; }

        public DateTime  CreatedOn { get; set; }
        public string CreatedBy { get; set; } 
        
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public bool IsOffer { get; set; }


        public AssetStatus AssetStatus { get; set; }
        public int AssetStatusId
        {
            get { return (int)this.AssetStatus; }
            set { this.AssetStatus = (AssetStatus)value; }
        }
        public string CreatedOnString { get; set; }
        //public string CreatedOnString
        //{
        //    get { return (String)this.CreatedOn.ToString(); }
        //    set { }
        //}

        public List<AssetAttachmentDto> lstAssetAttachmentDto { get; set; }

    }
}
