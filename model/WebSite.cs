﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace model
{
    public class WebSite
    {
        public int Id { get; set; }
        public string WebName { get; set; }
        public string Host { get; set; }
        public string WebToken { get; set; }
        public string Manager { get; set; }
        public string ManagerPhone { get; set; }
        public string ManagerEmail { get; set; }
        public string CheckUrl { get; set; }
        public int Enable { get; set; }

    }
}
