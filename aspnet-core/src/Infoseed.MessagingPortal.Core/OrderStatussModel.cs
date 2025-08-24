using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class OrderStatussModel
    {
		public enum OrderStatuss
		{
			NoN = 0,
			Pending = 1,
			Done = 2,
			Delete = 3,
			Draft = 4
		}
	}
}
