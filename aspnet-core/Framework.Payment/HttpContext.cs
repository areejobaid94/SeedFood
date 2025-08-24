using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Payment
{

    public static class HttpContext
    {
        private static Microsoft.AspNetCore.Http.IHttpContextAccessor m_httpContextAccessor;


        public static void Configure(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
        }


        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                if(m_httpContextAccessor != null)
                return m_httpContextAccessor.HttpContext;
                else
                {
                    return null;
                }
            }
        }

    }
}

