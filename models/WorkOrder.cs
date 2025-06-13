using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

#pragma warning disable CS8618

namespace CustomerApi.Models
{
    public class WorkOrder
    {
        [Key]
        public int id { get; set; }
        public DateTime requestDate { get; set; }
        public string requestedBy { get; set; }
        public string details { get; set; }
        
        // Navigation property for OData $expand
        public virtual ICollection<InspectionStatus> InspectionStatus { get; set; }
    }
}

