using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class CoutryTelCodeModel
    {
        public string Pfx { get; set; }
        public string Iso { get; set; }
        public decimal Rate { get; set; }

        public CoutryTelCodeModel(string pfx, string iso, decimal rate)
        {
            Pfx = pfx;
            Iso = iso;
            Rate = rate;
        }

    }
}
