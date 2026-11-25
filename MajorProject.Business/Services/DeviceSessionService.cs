using System;
using System.Security.Cryptography;
using System.Text;
using MajorProject.Business.Models;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    // Desktop integration hook. Call Begin only after a successful shared AuthenticationService login.
    // The desktop must retain its ticket in memory and check it before protected actions and on a heartbeat.
    public sealed class DeviceSessionService
    {
        private readonly SystemAdminDataAccess data;
        public DeviceSessionService(string cs)
        {
            data = new SystemAdminDataAccess(cs);
        }

        private static byte[] Hash(string value)
        {
            using (var sha = SHA256.Create())
                return sha.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        public DeviceSessionTicket Begin(bool staff, int accountID, string clientIP, string clientDescription)
        {
            if (accountID <= 0)
                throw new ArgumentException("Authenticate first.");
            clientIP = SystemAdminService.Text(clientIP, 45, "Client IP");
            clientDescription = SystemAdminService.Text(clientDescription, 256, "Client description");
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            var ticket = new DeviceSessionTicket
            {
                RecordID = Guid.NewGuid(),
                Secret = Convert.ToBase64String(bytes),
                AccountID = accountID,
                IsStaff = staff
            };
            int minutes = int.Parse(data.Setting("SessionIdleMinutes", "20"));
            data.StartSession(staff, accountID, ticket.RecordID, Hash(ticket.Secret), clientIP, clientDescription, "Desktop", minutes);
            var result = data.ValidateSession(staff, accountID, ticket.RecordID, Hash(ticket.Secret), minutes);
            if (result.Rows.Count != 1)
                throw new UnauthorizedAccessException("The session is unavailable.");
            ticket.RoleName = Convert.ToString(result.Rows[0]["RoleName"]);
            ticket.ExpiresAtUtc = DateTime.SpecifyKind(Convert.ToDateTime(result.Rows[0]["ExpiresAtUtc"]), DateTimeKind.Utc);
            return ticket;
        }

        public bool Validate(DeviceSessionTicket ticket, bool userActivity = true, string currentClientIP = null)
        {
            if (ticket == null || string.IsNullOrEmpty(ticket.Secret))
                return false;
            int minutes = int.Parse(data.Setting("SessionIdleMinutes", "20"));
            var result = data.ValidateSession(ticket.IsStaff, ticket.AccountID, ticket.RecordID, Hash(ticket.Secret), minutes, currentClientIP == null ? null : SystemAdminService.Text(currentClientIP, 45, "Client IP"), userActivity);
            if (result.Rows.Count != 1)
                return false;
            ticket.ExpiresAtUtc = DateTime.SpecifyKind(Convert.ToDateTime(result.Rows[0]["ExpiresAtUtc"]), DateTimeKind.Utc);
            return string.Equals(ticket.RoleName, Convert.ToString(result.Rows[0]["RoleName"]), StringComparison.OrdinalIgnoreCase);
        }

        public void End(DeviceSessionTicket ticket)
        {
            if (ticket != null && !string.IsNullOrEmpty(ticket.Secret))
                data.EndSession(ticket.RecordID, Hash(ticket.Secret));
        }
    }
}
