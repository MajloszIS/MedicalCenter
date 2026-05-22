using System;

namespace MedicalCenter.DTOs
{
    public class AcceptDeliveryDto
    {
        public Guid CourierId { get; set; }
    }

    public class UpdateDeliveryStatusDto
    {
        public string StatusName { get; set; }
    }
}