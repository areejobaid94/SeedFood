using Abp.Runtime.Validation;
using Infoseed.MessagingPortal.Common;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.Dto
{
    public class GetPaymentHistoryInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }

            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                return s.Replace("editionDisplayName", "Edition.DisplayName");
            });
        }
    }
}
