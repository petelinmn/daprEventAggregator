﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard
{
    public class OnSelectObjectEventArgs : EventArgs
    {
        public Guid SelectedGuid { get; set; }
    }
}
