﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Db.Models
{
    public interface IIdentity
    {
        int Id { get; set; }
    }
}
