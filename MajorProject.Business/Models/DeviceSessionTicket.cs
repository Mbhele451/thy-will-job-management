using System;

namespace MajorProject.Business.Models
{
    [Serializable]
    public sealed class DeviceSessionTicket
    {
        public DateTime ExpiresAtUtc { get; set; }
        public Guid RecordID { get; set; }
        public string Secret { get; set; }
        public int AccountID { get; set; }
        public bool IsStaff { get; set; }
        public string RoleName { get; set; }
    }
}
