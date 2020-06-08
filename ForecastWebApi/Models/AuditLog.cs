using System;
using System.Collections.Generic;

namespace ForecastWebApi.Models
{
    public partial class AuditLog
    {
        public int Id { get; set; }
        public string SearchName { get; set; }
        public string SearchTime { get; set; }
    }
}
