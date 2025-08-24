namespace NewFunctionApp
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class DocumentDBHelper<T> where T : class
    {
        private DocumentClient client;
        private string DocumentCollection = string.Empty;

        public DocumentDBHelper(CollectionTypes collectionTypes)
        {
            switch (collectionTypes)
            {
                case CollectionTypes.ItemsCollection:
                    this.DocumentCollection = Constants.ItemsCollection;
                    break;
                case CollectionTypes.ConversationsCollection:
                    this.DocumentCollection = Constants.ConversationsCollection;
                    break;
                case CollectionTypes.CustomersCollection:
                    this.DocumentCollection = Constants.CustomersCollection;
                    break;
                default:
                    throw new Exception("Please Define the Collection of Database to be Created, Document Database Collection is missing");
            }

            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Initialize()
        {
            client = new DocumentClient(new Uri(Constants.EndPoint), Constants.AuthKey, new ConnectionPolicy { EnableEndpointDiscovery = false });
            //Task.Run(() => CreateDatabaseIfNotExistsAsync()).Wait();
            //Task.Run(() => CreateCollectionIfNotExistsAsync()).Wait();
        }

        public async Task<T> GetItemAsync(string id, string storedProcedure = "", string subscriptionId = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(storedProcedure))
                {
                    Uri uri = UriFactory.CreateStoredProcedureUri(Constants.Database
                        , Constants.ItemsCollection, storedProcedure);

                    RequestOptions options = new RequestOptions { PartitionKey = new PartitionKey(subscriptionId) };

                    var result = await client.ExecuteStoredProcedureAsync<string>(uri, options, new[] { id });
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result.Response);

                    return obj;
                }
                else
                {
                    Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(Constants.Database, DocumentCollection, id));
                    return (T)(dynamic)document;
                }
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Error Occured While Trying to Get the Subscription Document", e);
                }
            }
        }

        public async Task<T> ExecuteStoredProcedureAsync(string storedProcedure, string partitionKey, params string[] parameters)
        {
            try
            {
                Uri uri = UriFactory.CreateStoredProcedureUri(Constants.Database, DocumentCollection, storedProcedure);

                RequestOptions options = new RequestOptions { PartitionKey = new PartitionKey(partitionKey) };

                var result = await client.ExecuteStoredProcedureAsync<T>(uri, options, parameters);
                //var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result.Response);

                return result;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Error Occured While Trying to Get the Subscription Document", e);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> GetItemAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                
                var result = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)

                .AsEnumerable()
                .FirstOrDefault();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IEnumerable<T>> GetItemListAsync(Expression<Func<T, bool>> predicate)
        {
            var result = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)

                .AsEnumerable()
                .ToList();

            return result;
        }

        public async Task<T> GetItemOrderDescAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> order)
        {
            var result = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)
                .OrderByDescending(order)
                .AsEnumerable()
                .FirstOrDefault();

            return result;
        }


        public async Task<Tuple<IEnumerable<T>, string>> GetItemsAsync(Expression<Func<T, bool>> predicate, string continuationToken, int pageSize,int pageNumber, Expression<Func<T, bool>> extraPredicate = null)
        {
            //if (pageNumber > 0 )
            //{
            //    pageNumber = pageNumber - 1;
            //}

            //if (pageSize <= 0)
            //{
            //    pageSize = 20;// int.MaxValue;
            //}

            if (extraPredicate == null)
            {
                extraPredicate = c => true;
            }

            var feedOptions = new FeedOptions
            {
                MaxItemCount = -1,//pageSize,
                EnableCrossPartitionQuery = true,

                // IMPORTANT: Set the continuation token (NULL for the first ever request/page)
                RequestContinuation = continuationToken
            };

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection);
            IQueryable<T> filter = client.CreateDocumentQuery<T>(collectionUri, feedOptions).Where(predicate).Where(extraPredicate).Skip(pageSize * pageNumber).Take(pageSize);
            IDocumentQuery<T> query = filter.AsDocumentQuery();

            try
            {
                
                FeedResponse<T> feedRespose = await query.ExecuteNextAsync<T>();

                List<T> documents = feedRespose.ToList();
                string nextResult = feedRespose.ResponseContinuation;
                
                var response = new Tuple<IEnumerable<T>, string>(documents, nextResult);

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public async Task<Tuple<IEnumerable<T>, string>> GetItemsRAsync(Expression<Func<T, bool>> predicate, string continuationToken, int pageSize, int pageNumber, Expression<Func<T, object>> order, Expression<Func<T, bool>> extraPredicate = null)
        {
            //if (pageNumber > 0 )
            //{
            //    pageNumber = pageNumber - 1;
            //}

            //if (pageSize <= 0)
            //{
            //    pageSize = 20;// int.MaxValue;
            //}

            if (extraPredicate == null)
            {
                extraPredicate = c => true;
            }

            var feedOptions = new FeedOptions
            {
                MaxItemCount = -1,//pageSize,
                EnableCrossPartitionQuery = true,
               
                // IMPORTANT: Set the continuation token (NULL for the first ever request/page)
                RequestContinuation = continuationToken
            };

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection);
            IQueryable<T> filter = client.CreateDocumentQuery<T>(collectionUri, feedOptions).Where(predicate).Where(extraPredicate).OrderByDescending(order).Skip(pageSize * pageNumber).Take(pageSize);
            IDocumentQuery<T> query = filter.AsDocumentQuery();

            try
            {

                FeedResponse<T> feedRespose = await query.ExecuteNextAsync<T>();

                List<T> documents = feedRespose.ToList();
                string nextResult = feedRespose.ResponseContinuation;

                var response = new Tuple<IEnumerable<T>, string>(documents, nextResult);

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public async Task<Tuple<IEnumerable<T>, string>> GetAllItemsAsync( string continuationToken, int pageSize, Expression<Func<T, bool>> extraPredicate = null)
        {

            if (pageSize <= 0)
            {
                pageSize = int.MaxValue;
            }

            if (extraPredicate == null)
            {
                extraPredicate = c => true;
            }

            var feedOptions = new FeedOptions
            {
                MaxItemCount = pageSize,
                EnableCrossPartitionQuery = true,

                // IMPORTANT: Set the continuation token (NULL for the first ever request/page)
                RequestContinuation = continuationToken
            };

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection);

            IQueryable<T> filter = client.CreateDocumentQuery<T>(collectionUri, feedOptions).Where(extraPredicate);
            IDocumentQuery<T> query = filter.AsDocumentQuery();

            try
            {
                FeedResponse<T> feedRespose = await query.ExecuteNextAsync<T>();

                List<T> documents = feedRespose.ToList();
                string nextResult = feedRespose.ResponseContinuation;

                var response = new Tuple<IEnumerable<T>, string>(documents, nextResult);

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task<DocumentResult> CreateItemAsync(T item)
        {
            try
            {
                RequestOptions options = new RequestOptions { PostTriggerInclude = new List<string> { "updateMetadata" } };
               // var result = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DocumentDBConfiguration.Database, DocumentCollection), item, options);
                //RequestOptions options = new RequestOptions { PostTriggerInclude = new List<string> { "updateMetadata" } };
                var result = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection), item);
                return new DocumentResult()
                {
                    ID = result.Resource.Id,
                    Uri = result.Resource.AttachmentsLink
                };
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<DocumentResult> CreateItemTestAsync(T item)
        {
            try
            {
                RequestOptions options = new RequestOptions { PostTriggerInclude = new List<string> { "updateMetadata" } };
                 var result = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection), item, options);
                //RequestOptions options = new RequestOptions { PostTriggerInclude = new List<string> { "updateMetadata" } };
               // var result = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DocumentDBConfiguration.Database, DocumentCollection), item);
                return new DocumentResult()
                {
                    ID = result.Resource.Id,
                    Uri = result.Resource.AttachmentsLink
                };
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<DocumentResult> UpdateTenantItemAsync(T item)
        {
            try
            {

                var result = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection), item);
                return new DocumentResult()
                {
                    ID = result.Resource.Id,
                    Uri = result.Resource.AttachmentsLink
                };
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<DocumentResult> UpdateItemAsync(string url, T item)
        {
            try
            {

                var result = await client.ReplaceDocumentAsync(url, item);
                return new DocumentResult()
                {
                    ID = result.Resource.Id,
                    Uri = result.Resource.AttachmentsLink
                };
            }
            catch (Exception ex )
            {
             
                throw;
            }
        
        }

        public async Task DeleteItemAsync(string id, string partitionKey = "")
        {
            await client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(Constants.Database, DocumentCollection, id),
                new RequestOptions()
                {
                    PartitionKey = new PartitionKey(partitionKey)
                }
                );
        }


        public async Task DeleteItem(string userId, int? TenantId)
        {
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = int.MaxValue,
                EnableCrossPartitionQuery = true
            };

            var queryString = "SELECT * FROM c WHERE c.TenantId=" + TenantId + " and c.userId='" + userId + "'";

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection);

            var queryInSql = client.CreateDocumentQuery<Document>(
                                   collectionUri,
                                   queryString,
                                   queryOptions).AsDocumentQuery();
            var document = await queryInSql.ExecuteNextAsync<Document>().ConfigureAwait(false);

            try
            {
                //Delete a document using its selfLink property
                //To get the documentLink you would have to query for the Document, using CreateDocumentQuery(),  and then refer to its .SelfLink property
                await client.DeleteDocumentAsync(document.AsEnumerable().FirstOrDefault().SelfLink, new RequestOptions { PartitionKey = new PartitionKey(document.AsEnumerable().FirstOrDefault().Id) });
            }
            catch (Exception e)
            {

            }
       
        }

        public async Task DeleteChatItem(string queryString)
        {
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = int.MaxValue,
                EnableCrossPartitionQuery = true
            };

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection);

            var queryInSql = client.CreateDocumentQuery<Document>(
                                   collectionUri,
                                   queryString,
                                   queryOptions).AsDocumentQuery();
            var document = await queryInSql.ExecuteNextAsync<Document>().ConfigureAwait(false);

            try
            {
                foreach(var item in document.AsEnumerable())
                {
                    var json= item.ToString();

                    if (!json.Contains("\"displayName\""))
                    {
                            //Delete a document using its selfLink property
                            //To get the documentLink you would have to query for the Document, using CreateDocumentQuery(),  and then refer to its .SelfLink property
                            await client.DeleteDocumentAsync(item.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(item.Id) });
                        }                  
                  
                }
              
            }
            catch (Exception e)
            {

            }

        }
        public class response
        {
            public string id { get; set; }
            public string deviceid { get; set; }
            public string _rid { get; set; }
            public string _self { get; set; }
            public string _etag { get; set; }
            public string _attachments { get; set; }
            public long _ts { get; set; }
        }
   


        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(Constants.Database));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = Constants.Database });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, DocumentCollection));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(Constants.Database),
                        new DocumentCollection { Id = DocumentCollection },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}