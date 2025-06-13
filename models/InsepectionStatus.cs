using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace CustomerApi.Models
{
    public class InspectionStatus
    {
        [Key]
        public int id { get; set; }
        public int workOrderId { get; set; }
        public string status { get; set; }
        public DateTime inspectionDate { get; set; }
        public string inspector { get; set; }
        public string comments { get; set; }
    }
}

