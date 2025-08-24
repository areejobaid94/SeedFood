using Framework.Payment;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Services
{
    //This is a sample with Request/Result classes (Simply implement as you see fit)
    public static class MyBackgroundWorker
    {
        private static ConcurrentQueue<KeyValuePair<Guid, Request>> _queue = new ConcurrentQueue<KeyValuePair<Guid, Request>>();
        public static ConcurrentDictionary<Guid, Request> Results = new ConcurrentDictionary<Guid, Request>();

        static MyBackgroundWorker()
        {
            var thread = new Thread(ProcessQueue);
            thread.Start();
        }

        private static void ProcessQueue()
        {
            KeyValuePair<Guid, Request> req;
            while (_queue.TryDequeue(out req))
            {
                var x= req.Value;
                //Do processing here (Make sure to do it in a try/catch block)
                Results.TryAdd(req.Key, req.Value);
            }
        }

        public static Guid AddItem(Request req)
        {
            var guid = new Guid();
            KeyValuePair < Guid, Request > keyValuePair = new KeyValuePair<Guid, Request>(guid, req);
            
            _queue.Enqueue(keyValuePair);
            return guid;
        }
    }
}
