﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Guid CompanyId { get; set; }
    }
}
