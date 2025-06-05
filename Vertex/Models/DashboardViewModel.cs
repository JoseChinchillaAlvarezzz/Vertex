using System;
using System.Collections.Generic;

namespace Vertex.Models
{
    public class DashboardViewModel
    {
       
        public Dictionary<string, int> PriorityCounts { get; set; } = new();

        public Dictionary<string, int> ApplicationCountsLast30Days { get; set; } = new();

        public Dictionary<string, int> TechnicianResolvedCounts { get; set; } = new();
    }
}
