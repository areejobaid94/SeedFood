using Framework.Data;
using Infoseed.MessagingPortal.General.Dto;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.General
{
    public class CosmosAppService : MessagingPortalAppServiceBase, ICosmosAppService
    {

        private readonly IDocumentClient _IDocumentClient;
        public CosmosAppService(IDocumentClient iDocumentClient)
        {
            _IDocumentClient = iDocumentClient;
        }
        public async Task<TenantsModel> GetTenant(int tenantId)
        {
            TenantsModel tenantsModel = new TenantsModel();
            return tenantsModel;
            //    var itemsCollection = new DocumentCosmoseDB<TenantsModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            //    TenantsModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == tenantId);
            //    return tenant;
            //}
        }
    }
}
