using Abp.Domain.Repositories;
using AutoMapper.Configuration;
using Framework.Data;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{
    public class UpdateContactSync
    {

        private readonly IDocumentClient _IDocumentClient;

        //  private readonly IRepository<Contact> _contactRepository;
        //   IDBService _dbService;
        public UpdateContactSync(IDocumentClient iDocumentClient)
        {
            _IDocumentClient = iDocumentClient;

            //this._contactRepository = contactRepos;
            //_dbService = dbService;
        }
        public async Task UpdateConversationExpired()
        {

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customers = await itemsCollection.GetItemsAsync( a =>a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.D360Key== "qB5lYe6Leadbi6ccmiFYj3ONAK", null, int.MaxValue ,0);


            foreach (var item in customers.Item1)
            {
                item.D360Key = "zJJfZCtP8UcWZmZQCEesN9TBAK";
                
                await itemsCollection.UpdateItemAsync(item._self, item);



                //TimeSpan timeSpan = DateTime.Now - item.LastMessageData;
                //int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);
                //if (totalHours <= 24)
                //{
                //    item.IsConversationExpired = false;


                //}
                //else
                //{
                //    //if(item.IsConversationExpired && item.IsOpen)
                //    //{
                //    //    item.IsConversationExpired = true;
                //    //    item.IsOpen = false;
                //    //    await itemsCollection.UpdateItemAsync(item._self, item);

                //    //}

                //    item.IsConversationExpired = true;
                //    item.IsOpen = false;
                //    await itemsCollection.UpdateItemAsync(item._self, item);


                //}




            }


        }
    }
}
