using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MajorProject.Business.Helpers;
using MajorProject.Business.Models;
using MajorProject.Business.Services;

namespace Major_Project.Core
{
    public static class UserSession
    {
        private static DeviceSessionService _sessions;
        private static DeviceSessionTicket _ticket;
        public static int StaffID => _ticket != null && _ticket.IsStaff ? _ticket.AccountID : 0;
        public static bool IsLoggedIn => _ticket != null;
        public static string FullName { get; private set; } = "";
        public static string RoleName => _ticket?.RoleName ?? "";
        internal static string ClientIp { get; private set; } = "unknown";
        internal static TimeSpan Remaining => _ticket == null ? TimeSpan.Zero : _ticket.ExpiresAtUtc - DateTime.UtcNow;

        internal static event EventHandler Expired;
        internal static void Begin(string connectionString, bool isStaff, int accountId, string fullName, string authenticatedRole)
        {
            ClearSession();
            var role = authenticatedRole?.Trim();
            if (!SupportsRole(isStaff, role))
                throw new UnauthorizedAccessException("This account role is unavailable.");
            string clientIp = GetClientIp();
            var sessions = new DeviceSessionService(connectionString);
            var ticket = sessions.Begin(isStaff, accountId, clientIp, "THY WILL Desktop / " + Environment.MachineName);
            if (!string.Equals(role, ticket.RoleName?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                End(sessions, ticket);
                throw new UnauthorizedAccessException("The account role changed. Sign in again.");
            }

            _sessions = sessions;
            _ticket = ticket;
            FullName = fullName ?? "";
            ClientIp = clientIp;
        }

        internal static bool Check(bool userActivity = true)
        {
            if (_ticket == null)
                return false;
            try
            {
                if (_sessions.Validate(_ticket, userActivity))
                    return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Desktop session validation failed.", ex);
            }

            ClearSession();
            Expired?.Invoke(null, EventArgs.Empty);
            return false;
        }

        public static void ClearSession()
        {
            var ticket = _ticket;
            var sessions = _sessions;
            _ticket = null;
            _sessions = null;
            FullName = "";
            ClientIp = "unknown";
            if (ticket != null)
                End(sessions, ticket);
        }

        private static void End(DeviceSessionService sessions, DeviceSessionTicket ticket)
        {
            try
            {
                sessions.End(ticket);
            }
            catch (Exception ex)
            {
                Logger.Error("Could not end the tracked desktop session.", ex);
            }
        }

        public static bool HasRole(string role)
        {
            return _ticket != null && string.Equals(RoleName.Trim(), role, StringComparison.OrdinalIgnoreCase);
        }

        private static bool SupportsRole(bool isStaff, string role)
        {
            var roles = new[]
            {
                "Administrator",
                "Admin",
                "Manager",
                "Technician",
                "SysAdmin"
            };
            return isStaff && roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        private static string GetClientIp()
        {
            try
            {
                return NetworkInterface.GetAllNetworkInterfaces().Where(adapter => adapter.OperationalStatus == OperationalStatus.Up && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback).SelectMany(adapter => adapter.GetIPProperties().UnicastAddresses).Select(address => address.Address).OrderBy(address => address.AddressFamily == AddressFamily.InterNetwork ? 0 : 1).Select(address => address.ToString()).FirstOrDefault(address => address.Length <= 45) ?? "unknown";
            }
            catch (NetworkInformationException)
            {
                return "unknown";
            }
        }
    }
}
