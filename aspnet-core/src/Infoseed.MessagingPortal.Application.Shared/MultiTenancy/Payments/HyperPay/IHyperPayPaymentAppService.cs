using Abp.Application.Services;
using Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.MultiTenancy.Payments.HyperPay
{
   public interface IHyperPayPaymentAppService: IApplicationService
    {
        Task ConfirmPayment(long paymentId, string hyperPayOrderId);

        HyperPayConfigurationDto GetConfiguration();
    }
}
