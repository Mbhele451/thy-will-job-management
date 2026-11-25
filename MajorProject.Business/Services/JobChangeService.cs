using System;
using System.Data;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public sealed class JobChangeService
    {
        private readonly JobChangeDataAccess data;
        public JobChangeService(string cs)
        {
            data = new JobChangeDataAccess(cs);
        }

        public static decimal CancellationFee(decimal originalTotal)
        {
            if (originalTotal < 0)
                throw new ArgumentOutOfRangeException(nameof(originalTotal));
            return Math.Round(originalTotal * 0.05m, 2, MidpointRounding.AwayFromZero);
        }

        public DataTable Materials(int job, int actor, bool manager = false) => data.Materials(job, actor, manager);
        public int RequestCustomerCancellation(int job, int customer, string reason) => data.RequestCancellation(job, customer, true, reason);
        public int RequestStaffCancellation(int job, int actor, string reason) => data.RequestCancellation(job, actor, false, reason);
        public DataTable TechnicianMaterialReports(int actor) => data.TechnicianMaterialReports(actor);
        public void SubmitMaterials(int request, int actor, DataTable used) => data.SubmitMaterials(request, actor, used);
        public byte[] GetFeeInvoicePdf(int customer, int request)
        {
            var rows = CustomerFees(customer).Select("RequestID=" + request);
            if (rows.Length != 1)
                throw new InvalidOperationException("This fee invoice is unavailable for this account.");
            return InvoicePdfService.GenerateFeeInvoicePdf(rows[0]);
        }
        public int Request(int job, int actor, string type, string reason, DateTime? date, DataTable used, bool manager = false) => data.Request(job, actor, type, reason, date, used, manager);
        public int RequestCustomerReschedule(int job, int customer, DateTime date, string reason) => data.RequestCustomerReschedule(job, customer, date, reason);
        public DataTable CustomerRequests(int customer) => data.CustomerRequests(customer);
        public DataTable Requests(int actor, bool manager) => data.Requests(actor, manager);
        public DataTable RequestMaterials(int actor, int request) => data.RequestMaterials(actor, request);
        public DataTable ReassignmentQueue(int actor, bool allOpen = false) => data.ReassignmentQueue(actor, allOpen);
        public void Decide(int request, int actor, bool approve, string note, int technician = 0, DateTime? date = null) => data.Decide(request, actor, approve, note, technician, date ?? DateTime.Today);
        public void Reassign(int job, int actor, int technician, DateTime date, string note, int? expectedStaff = null) => data.Reassign(job, actor, technician, date, note, expectedStaff);
        public DataTable Notifications(int actor) => data.Notifications(actor);
        public void ReadNotification(int actor, long id) => data.ReadNotification(actor, id);
        public DataTable CustomerFees(int customer) => data.CustomerFees(customer);
        public void PayDemoFee(int customer, int request) => data.PayDemoFee(customer, request);
    }
}
