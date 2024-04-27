using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Db.Models
{
    public enum PaymentStatus
    {
        InProgress,
        Success,
        Canceled,
        Failed
    }
}
