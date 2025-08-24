using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Payment
{
   public enum PaymentType
    {
        PA, //Preauthorization
        DB, //Debit
        CD, //Credit
        CP, //Capture
        RV, //Reversal
        RF //Refund
    }
}
