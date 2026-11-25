using MajorProject.Business.Models;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace MajorProject.Business.Services
{
    public class ChatbotService
    {
        private JobService _jobService;
        private InvoiceService _invoiceService;
        private InventoryService _inventoryService;
        private CustomerService _customerService;
        private StaffService _staffService;
        private JobTypeService _jobTypeService;
        public ChatbotService()
        {
        }

        public ChatbotService(string connectionString)
        {
            _jobService = new JobService(connectionString);
            _invoiceService = new InvoiceService(connectionString);
            _inventoryService = new InventoryService(connectionString);
            _customerService = new CustomerService(connectionString);
            _staffService = new StaffService(connectionString);
            _jobTypeService = new JobTypeService(connectionString);
        }

        // ============================================================
        // HTML ENCODING
        // ============================================================
        private string EncodeHtml(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return WebUtility.HtmlEncode(value);
        }

        // ============================================================
        // PUBLIC CHATBOT
        // ============================================================
        public ChatResponse ProcessMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return CreateFallbackResponse();
            }

            message = message.Trim().ToLowerInvariant();
            if (IsEmergencyMessage(message))
            {
                return CreateEmergencyResponse();
            }

            if (IsServiceDetailMessage(message))
            {
                return CreateServiceDetailResponse(message);
            }

            if (IsServicesMessage(message))
            {
                return CreateServicesResponse();
            }

            if (IsServiceAreaMessage(message))
            {
                return CreateServiceAreaResponse();
            }

            if (IsBookingMessage(message))
            {
                return CreateBookingResponse();
            }

            if (IsContactMessage(message))
            {
                return CreateContactResponse();
            }

            if (IsAboutMessage(message))
            {
                return CreateAboutResponse();
            }

            return CreateFallbackResponse();
        }

        // ============================================================
        // CUSTOMER CHATBOT
        // ============================================================
        public ChatResponse ProcessCustomerMessage(string message, int customerID)
        {
            if (customerID <= 0)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "authentication",
                    Message = "I could not identify your customer account. Please sign in again."
                };
            }

            if (_jobService == null || _invoiceService == null)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "configuration",
                    Message = "Customer chatbot services are currently unavailable."
                };
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return CreateCustomerFallbackResponse();
            }

            message = message.Trim().ToLowerInvariant();
            if (IsEmergencyMessage(message))
            {
                return CreateEmergencyResponse();
            }

            if (message.Contains("cancel") || message.Contains("reschedul") || message.Contains("change my booking") || message.Contains("booking fee"))
            {
                return new ChatResponse
                {
                    Success = true,
                    Intent = "booking_changes",
                    Message = "Open Booking changes & fees, choose your booking and submit the reason for your request. You can follow the decision in your request history and see the reason and amount for each approved charge.",
                    ActionText = "Booking changes & fees",
                    ActionUrl = "~/Secure/Customer/CancellationFees.aspx"
                };
            }

            if (IsMyJobsMessage(message))
            {
                return CreateMyJobsResponse(customerID);
            }

            if (IsJobStatusMessage(message))
            {
                return CreateJobStatusResponse(message, customerID);
            }

            if (IsMyInvoicesMessage(message))
            {
                return CreateMyInvoicesResponse(customerID);
            }

            if (IsInvoiceDetailsMessage(message))
            {
                return CreateInvoiceDetailsResponse(message, customerID);
            }

            if (IsProfileMessage(message))
            {
                return CreateProfileResponse();
            }

            if (IsServiceDetailMessage(message))
            {
                return CreateServiceDetailResponse(message);
            }

            if (IsServicesMessage(message))
            {
                return CreateServicesResponse();
            }

            if (IsServiceAreaMessage(message))
            {
                return CreateServiceAreaResponse();
            }

            if (IsBookingMessage(message))
            {
                return CreateBookingResponse();
            }

            if (IsContactMessage(message))
            {
                return CreateContactResponse();
            }

            if (IsAboutMessage(message))
            {
                return CreateAboutResponse();
            }

            return CreateCustomerFallbackResponse();
        }

        // ============================================================
        // TECHNICIAN CHATBOT
        // ============================================================
        public ChatResponse ProcessTechnicianMessage(string message, int staffID)
        {
            if (staffID <= 0)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "authentication",
                    Message = "I could not identify your technician account. Please sign in again."
                };
            }

            if (_jobService == null || _inventoryService == null)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "configuration",
                    Message = "Technician chatbot services are currently unavailable."
                };
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return CreateTechnicianFallbackResponse();
            }

            message = message.Trim().ToLowerInvariant();
            if (IsEmergencyMessage(message))
            {
                return CreateEmergencyResponse();
            }

            if (IsMyJobsMessage(message))
            {
                return CreateTechnicianJobsResponse(staffID);
            }

            if (IsJobStatusMessage(message))
            {
                return CreateTechnicianJobStatusResponse(message, staffID);
            }

            if (IsLowStockMessage(message))
            {
                return CreateTechnicianLowStockResponse();
            }

            if (IsProfileMessage(message))
            {
                return CreateTechnicianProfileResponse();
            }

            if (IsServiceDetailMessage(message))
            {
                return CreateServiceDetailResponse(message);
            }

            if (IsServicesMessage(message))
            {
                return CreateServicesResponse();
            }

            if (IsServiceAreaMessage(message))
            {
                return CreateServiceAreaResponse();
            }

            if (IsContactMessage(message))
            {
                return CreateContactResponse();
            }

            if (IsAboutMessage(message))
            {
                return CreateAboutResponse();
            }

            return CreateTechnicianFallbackResponse();
        }

        // ============================================================
        // ADMINISTRATOR CHATBOT
        //
        // Administrator = clerk/receptionist.
        //
        // Allowed:
        // - Customers
        // - Jobs
        // - Work orders
        // - Operational job information
        // - Own profile
        // - General electrical information
        //
        // Restricted:
        // - Finance
        // - Invoice amounts
        // - Reports
        // - Salaries
        // - Staff management
        // - Direct database mutations
        // ============================================================
        public ChatResponse ProcessAdministratorMessage(string message, int staffID)
        {
            if (staffID <= 0)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "authentication",
                    Message = "I could not identify your administrator account. Please sign in again."
                };
            }

            if (_jobService == null || _customerService == null)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "configuration",
                    Message = "Administrator chatbot services are currently unavailable."
                };
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return CreateAdministratorFallbackResponse();
            }

            message = message.Trim().ToLowerInvariant();
            if (IsEmergencyMessage(message))
            {
                return CreateEmergencyResponse();
            }

            if (IsRestrictedAdministratorMessage(message))
            {
                return CreateAdministratorRestrictedResponse();
            }

            if (IsAdministratorStaffManagementMessage(message))
            {
                return CreateAdministratorRestrictedResponse();
            }

            if (IsAdministratorCreateJobMessage(message))
            {
                return CreateAdministratorCreateJobResponse(message);
            }

            if (IsAdministratorBookingMessage(message))
            {
                return CreateAdministratorCreateJobResponse(message);
            }

            if (IsAdministratorWorkOrderMessage(message))
            {
                return CreateAdministratorCreateJobResponse(message);
            }

            if (IsAdministratorRescheduleMessage(message))
            {
                return CreateAdministratorRescheduleResponse(message);
            }

            if (IsAdministratorCancelJobMessage(message))
            {
                return CreateAdministratorCancelJobResponse(message);
            }

            if (IsAdministratorJobStatusMessage(message))
            {
                return CreateAdministratorJobStatusResponse(message);
            }

            if (IsAdministratorJobsMessage(message))
            {
                return CreateAdministratorJobsResponse();
            }

            if (IsAdministratorCustomerLookupMessage(message))
            {
                return CreateAdministratorCustomerLookupResponse(message);
            }

            if (IsProfileMessage(message))
            {
                return CreateAdministratorProfileResponse();
            }

            if (IsServiceDetailMessage(message))
            {
                return CreateServiceDetailResponse(message);
            }

            if (IsServicesMessage(message))
            {
                return CreateServicesResponse();
            }

            if (IsServiceAreaMessage(message))
            {
                return CreateServiceAreaResponse();
            }

            if (IsContactMessage(message))
            {
                return CreateContactResponse();
            }

            if (IsAboutMessage(message))
            {
                return CreateAboutResponse();
            }

            return CreateAdministratorFallbackResponse();
        }

        // ============================================================
        // MANAGER CHATBOT
        //
        // Manager can access:
        // - Jobs
        // - Customers
        // - Technician/staff operational information
        // - Inventory
        // - Services
        // - Finance
        // - Invoices
        // - Reports navigation
        // - Own profile
        //
        // The chatbot does not directly mutate records.
        // ============================================================
        public ChatResponse ProcessManagerMessage(string message, int staffID)
        {
            if (staffID <= 0)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "authentication",
                    Message = "I could not identify your manager account. Please sign in again."
                };
            }

            if (_jobService == null || _invoiceService == null || _inventoryService == null || _customerService == null || _staffService == null || _jobTypeService == null)
            {
                return new ChatResponse
                {
                    Success = false,
                    Intent = "configuration",
                    Message = "Manager chatbot services are currently unavailable."
                };
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return CreateManagerFallbackResponse();
            }

            message = message.Trim().ToLowerInvariant();
            if (IsEmergencyMessage(message))
            {
                return CreateEmergencyResponse();
            }

            if (IsManagerCreateJobMessage(message))
            {
                return CreateManagerCreateJobResponse();
            }

            if (IsManagerRescheduleMessage(message))
            {
                return CreateManagerRescheduleResponse();
            }

            if (IsManagerCancelJobMessage(message))
            {
                return CreateManagerCancelJobResponse();
            }

            /*
             * Finance and invoice details must be checked before
             * service-cost intent so that messages such as
             * "transport cost for Job 1042" are treated as finance.
             */
            if (IsManagerInvoiceDetailsMessage(message))
            {
                return CreateManagerInvoiceDetailsResponse(message);
            }

            if (IsManagerInvoicesMessage(message))
            {
                return CreateManagerInvoicesResponse();
            }

            if (IsManagerJobStatusMessage(message))
            {
                return CreateManagerJobStatusResponse(message);
            }

            if (IsManagerCustomerJobsMessage(message))
            {
                return CreateManagerCustomerJobsResponse(message);
            }

            if (IsManagerCustomerLookupMessage(message))
            {
                return CreateManagerCustomerLookupResponse(message);
            }

            if (IsManagerTechnicianMessage(message))
            {
                return CreateManagerTechnicianResponse(message);
            }

            if (IsManagerInventoryMessage(message))
            {
                return CreateManagerInventoryResponse(message);
            }

            if (IsManagerServiceMessage(message))
            {
                return CreateManagerServiceResponse(message);
            }

            if (IsManagerReportsMessage(message))
            {
                return CreateManagerReportsResponse();
            }

            if (IsProfileMessage(message))
            {
                return CreateManagerProfileResponse();
            }

            if (IsManagerJobsMessage(message))
            {
                return CreateManagerJobsResponse(message);
            }

            if (IsServiceDetailMessage(message))
            {
                return CreateServiceDetailResponse(message);
            }

            if (IsServicesMessage(message))
            {
                return CreateServicesResponse();
            }

            if (IsServiceAreaMessage(message))
            {
                return CreateServiceAreaResponse();
            }

            if (IsContactMessage(message))
            {
                return CreateContactResponse();
            }

            if (IsAboutMessage(message))
            {
                return CreateAboutResponse();
            }

            return CreateManagerFallbackResponse();
        }

        // ============================================================
        // CUSTOMER INTENT DETECTION
        // ============================================================
        private bool IsMyJobsMessage(string message)
        {
            return message.Contains("my jobs") || message.Contains("my job") || message.Contains("jobs assigned to me") || message.Contains("my bookings") || message.Contains("my appointments");
        }

        private bool IsJobStatusMessage(string message)
        {
            return message.Contains("job status") || message.Contains("status of job") || message.Contains("status for job") || message.Contains("what is the status") || message.Contains("where is my job") || message.Contains("check my job");
        }

        private bool IsMyInvoicesMessage(string message)
        {
            return message.Contains("my invoices") || message.Contains("my invoice") || message.Contains("show invoices") || message.Contains("show my invoices");
        }

        private bool IsInvoiceDetailsMessage(string message)
        {
            return message.Contains("invoice details") || message.Contains("invoice detail") || message.Contains("details of invoice") || message.Contains("invoice for job");
        }

        private bool IsProfileMessage(string message)
        {
            return message.Contains("my profile") || message.Contains("profile") || message.Contains("my account") || message.Contains("account details") || message.Contains("change my password");
        }

        // ============================================================
        // TECHNICIAN INTENT DETECTION
        // ============================================================
        private bool IsLowStockMessage(string message)
        {
            return message.Contains("low stock") || message.Contains("low inventory") || message.Contains("items running low") || message.Contains("stock running low") || message.Contains("what is low in stock") || message.Contains("which items are low");
        }

        // ============================================================
        // ADMINISTRATOR INTENT DETECTION
        // ============================================================
        private bool IsAdministratorCreateJobMessage(string message)
        {
            return message.Contains("create job") || message.Contains("create a job") || message.Contains("new job") || message.Contains("add job") || message.Contains("create work order") || message.Contains("create a work order") || message.Contains("new work order") || message.Contains("add work order");
        }

        private bool IsAdministratorBookingMessage(string message)
        {
            return message.Contains("book a job") || message.Contains("book job") || message.Contains("make a booking") || message.Contains("create booking") || message.Contains("create a booking") || message.Contains("book a service") || message.Contains("book service");
        }

        private bool IsAdministratorWorkOrderMessage(string message)
        {
            return message.Contains("work order") || message.Contains("workorder") || message.Contains("work orders");
        }

        private bool IsAdministratorRescheduleMessage(string message)
        {
            return message.Contains("reschedule job") || message.Contains("reschedule a job") || message.Contains("reschedule booking") || message.Contains("change appointment") || message.Contains("change the appointment") || message.Contains("change service date") || message.Contains("change the service date") || message.Contains("change promise date") || message.Contains("move the job date");
        }

        private bool IsAdministratorCancelJobMessage(string message)
        {
            return message.Contains("cancel job") || message.Contains("cancel a job") || message.Contains("cancel booking") || message.Contains("cancel a booking") || message.Contains("cancel work order") || message.Contains("cancel the work order");
        }

        private bool IsAdministratorJobsMessage(string message)
        {
            return message.Contains("view jobs") || message.Contains("show jobs") || message.Contains("search jobs") || message.Contains("find jobs") || message.Contains("job list") || message.Contains("list of jobs") || message.Contains("manage jobs") || message.Contains("all jobs") || message.Contains("open jobs");
        }

        private bool IsAdministratorJobStatusMessage(string message)
        {
            return message.Contains("job status") || message.Contains("status of job") || message.Contains("status for job") || message.Contains("job details") || message.Contains("details of job") || message.Contains("check job") || message.Contains("check the job") || message.Contains("what is happening with job") || message.Contains("where is job");
        }

        private bool IsAdministratorCustomerLookupMessage(string message)
        {
            bool containsCustomer = message.Contains("customer") || message.Contains("client");
            bool containsLookupAction = message.Contains("find") || message.Contains("search") || message.Contains("lookup") || message.Contains("look up") || message.Contains("show");
            return containsCustomer && containsLookupAction;
        }

        private bool IsRestrictedAdministratorMessage(string message)
        {
            return message.Contains("finance") || message.Contains("finances") || message.Contains("financial") || message.Contains("report") || message.Contains("reports") || message.Contains("revenue") || message.Contains("profit") || message.Contains("income") || message.Contains("invoice") || message.Contains("invoices") || message.Contains("billing") || message.Contains("bill") || message.Contains("tax") || message.Contains("labour cost") || message.Contains("labor cost") || message.Contains("material cost") || message.Contains("transport cost") || message.Contains("grand total") || message.Contains("salary") || message.Contains("salaries");
        }

        private bool IsAdministratorStaffManagementMessage(string message)
        {
            return message.Contains("staff management") || message.Contains("manage staff") || message.Contains("staff members") || message.Contains("employee records") || message.Contains("employee information");
        }

        // ============================================================
        // MANAGER INTENT DETECTION
        // ============================================================
        private bool IsManagerCreateJobMessage(string message)
        {
            return message.Contains("create job") || message.Contains("create a job") || message.Contains("new job") || message.Contains("create work order") || message.Contains("create a work order") || message.Contains("new work order") || message.Contains("add work order") || message.Contains("book a job") || message.Contains("create booking");
        }

        private bool IsManagerRescheduleMessage(string message)
        {
            return message.Contains("reschedule job") || message.Contains("reschedule a job") || message.Contains("reschedule booking") || message.Contains("change appointment") || message.Contains("change the appointment") || message.Contains("change service date") || message.Contains("change the service date") || message.Contains("change promise date") || message.Contains("move the job date");
        }

        private bool IsManagerCancelJobMessage(string message)
        {
            return message.Contains("cancel job") || message.Contains("cancel a job") || message.Contains("cancel booking") || message.Contains("cancel a booking") || message.Contains("cancel work order") || message.Contains("cancel the work order");
        }

        private bool IsManagerJobStatusMessage(string message)
        {
            return message.Contains("job status") || message.Contains("status of job") || message.Contains("status for job") || message.Contains("job details") || message.Contains("details of job") || message.Contains("check job") || message.Contains("check the job") || message.Contains("what is happening with job") || message.Contains("where is job") || message.Contains("job #") || message.Contains("job number");
        }

        private bool IsManagerJobsMessage(string message)
        {
            return message.Contains("view jobs") || message.Contains("show jobs") || message.Contains("search jobs") || message.Contains("find jobs") || message.Contains("job list") || message.Contains("list of jobs") || message.Contains("manage jobs") || message.Contains("all jobs") || message.Contains("open jobs") || message.Contains("pending jobs") || message.Contains("in progress jobs") || message.Contains("completed jobs") || message.Contains("cancelled jobs") || message.Contains("canceled jobs");
        }

        private bool IsManagerCustomerLookupMessage(string message)
        {
            bool containsCustomer = message.Contains("customer") || message.Contains("client");
            bool containsLookupAction = message.Contains("find") || message.Contains("search") || message.Contains("lookup") || message.Contains("look up") || message.Contains("show");
            return containsCustomer && containsLookupAction;
        }

        private bool IsManagerCustomerJobsMessage(string message)
        {
            return message.Contains("customer jobs") || message.Contains("customer history") || message.Contains("customer job history") || message.Contains("jobs for customer") || message.Contains("jobs for client");
        }

        private bool IsManagerTechnicianMessage(string message)
        {
            return message.Contains("technician") || message.Contains("technicians") || message.Contains("staff availability") || message.Contains("technician availability") || message.Contains("available technician") || message.Contains("available technicians") || message.Contains("technician workload") || message.Contains("technician jobs") || message.Contains("staff workload");
        }

        private bool IsManagerInventoryMessage(string message)
        {
            return message.Contains("inventory") || message.Contains("stock") || message.Contains("low stock") || message.Contains("low inventory") || message.Contains("items in stock") || message.Contains("what is in stock") || message.Contains("stock levels");
        }

        private bool IsManagerServiceMessage(string message)
        {
            return message.Contains("service types") || message.Contains("job types") || message.Contains("service costs") || message.Contains("service cost") || message.Contains("service materials") || message.Contains("materials for service") || message.Contains("material requirements") || message.Contains("labour for service") || message.Contains("labor for service") || message.Contains("tax percentage");
        }

        private bool IsManagerInvoicesMessage(string message)
        {
            return message.Contains("all invoices") || message.Contains("invoices") || message.Contains("invoice list") || message.Contains("invoice status") || message.Contains("billing") || message.Contains("billed jobs");
        }

        private bool IsManagerInvoiceDetailsMessage(string message)
        {
            return message.Contains("invoice details") || message.Contains("invoice detail") || message.Contains("details of invoice") || message.Contains("invoice for job") || message.Contains("invoice amount") || message.Contains("invoice total") || message.Contains("grand total") || message.Contains("labour cost") || message.Contains("labor cost") || message.Contains("material cost") || message.Contains("transport cost") || message.Contains("tax amount");
        }

        private bool IsManagerReportsMessage(string message)
        {
            return message.Contains("report") || message.Contains("reports") || message.Contains("power bi") || message.Contains("powerbi") || message.Contains("business report") || message.Contains("management report");
        }

        // ============================================================
        // CUSTOMER RESPONSES
        // ============================================================
        private ChatResponse CreateMyJobsResponse(int customerID)
        {
            DataTable jobs = _jobService.GetJobsByCustomerID(customerID);
            if (jobs == null || jobs.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "my_jobs",
                    Message = "You currently do not have any jobs on your account.",
                    ActionText = "Book a Service",
                    ActionUrl = "~/Secure/Customer/BookJob.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("Here are your recent jobs:<br /><br />");
            int count = 0;
            foreach (DataRow row in jobs.Rows)
            {
                if (count >= 5)
                {
                    break;
                }

                string jobID = GetColumnValue(row, "JobID");
                string status = GetColumnValue(row, "JobStatus");
                string promiseDate = GetColumnValue(row, "PromiseDate");
                response.Append("<strong>Job #");
                response.Append(EncodeHtml(jobID));
                response.Append("</strong>");
                if (!string.IsNullOrWhiteSpace(status))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(status));
                }

                if (!string.IsNullOrWhiteSpace(promiseDate))
                {
                    response.Append(" — ");
                    response.Append(FormatDate(promiseDate));
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "my_jobs",
                Message = response.ToString(),
                ActionText = "View My Jobs",
                ActionUrl = "~/Secure/Customer/MyJobs.aspx"
            };
        }

        private ChatResponse CreateJobStatusResponse(string message, int customerID)
        {
            int jobID = ExtractJobID(message);
            if (jobID <= 0)
            {
                return new ChatResponse
                {
                    Intent = "job_status",
                    Message = "Please provide the Job ID you want me to check. For example: <strong>What is the status of Job 1042?</strong>",
                    ActionText = "View My Jobs",
                    ActionUrl = "~/Secure/Customer/MyJobs.aspx"
                };
            }

            DataTable job = _jobService.GetJobByCustomerID(customerID, jobID);
            if (job == null || job.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "job_status",
                    Message = "I could not find that job on your account. Please check the Job ID and try again.",
                    ActionText = "View My Jobs",
                    ActionUrl = "~/Secure/Customer/MyJobs.aspx"
                };
            }

            DataRow row = job.Rows[0];
            string status = GetColumnValue(row, "JobStatus");
            string promiseDate = GetColumnValue(row, "PromiseDate");
            string services = GetColumnValue(row, "Services");
            StringBuilder response = new StringBuilder();
            response.Append("<strong>Job #");
            response.Append(jobID);
            response.Append("</strong><br />");
            if (!string.IsNullOrWhiteSpace(status))
            {
                response.Append("Status: ");
                response.Append(EncodeHtml(status));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(services))
            {
                response.Append("Service: ");
                response.Append(EncodeHtml(services));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(promiseDate))
            {
                response.Append("Scheduled date: ");
                response.Append(FormatDate(promiseDate));
            }

            return new ChatResponse
            {
                Intent = "job_status",
                Message = response.ToString(),
                ActionText = "View My Jobs",
                ActionUrl = "~/Secure/Customer/MyJobs.aspx"
            };
        }

        private ChatResponse CreateMyInvoicesResponse(int customerID)
        {
            DataTable invoices = _invoiceService.GetInvoicesByCustomerID(customerID);
            if (invoices == null || invoices.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "my_invoices",
                    Message = "I could not find any available invoices on your account.",
                    ActionText = "Open Invoices",
                    ActionUrl = "~/Secure/Customer/Invoices.aspx"
                };
            }

            return new ChatResponse
            {
                Intent = "my_invoices",
                Message = "Your available invoices can be viewed from the customer invoices page.",
                ActionText = "Open Invoices",
                ActionUrl = "~/Secure/Customer/Invoices.aspx"
            };
        }

        private ChatResponse CreateInvoiceDetailsResponse(string message, int customerID)
        {
            int jobID = ExtractJobID(message);
            if (jobID <= 0)
            {
                return new ChatResponse
                {
                    Intent = "invoice_details",
                    Message = "Please provide the Job ID for the invoice you want to view.",
                    ActionText = "Open Invoices",
                    ActionUrl = "~/Secure/Customer/Invoices.aspx"
                };
            }

            DataTable invoice = _invoiceService.GetCustomerInvoice(customerID, jobID);
            if (invoice == null || invoice.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "invoice_details",
                    Message = "I could not find an invoice for that job on your account.",
                    ActionText = "Open Invoices",
                    ActionUrl = "~/Secure/Customer/Invoices.aspx"
                };
            }

            return new ChatResponse
            {
                Intent = "invoice_details",
                Message = "The invoice for Job #" + jobID + " is available on your invoices page.",
                ActionText = "Open Invoices",
                ActionUrl = "~/Secure/Customer/Invoices.aspx"
            };
        }

        private ChatResponse CreateProfileResponse()
        {
            return new ChatResponse
            {
                Intent = "profile",
                Message = "You can view and manage your customer profile from your profile page.",
                ActionText = "Open Profile",
                ActionUrl = "~/Secure/Customer/Profile.aspx"
            };
        }

        private ChatResponse CreateCustomerFallbackResponse()
        {
            return new ChatResponse
            {
                Intent = "customer_fallback",
                Message = "I can help you with your jobs, job status, invoices, profile, bookings and general THY WILL service information."
            };
        }

        // ============================================================
        // TECHNICIAN RESPONSES
        // ============================================================
        private ChatResponse CreateTechnicianJobsResponse(int staffID)
        {
            DataTable jobs = _jobService.GetTechnicianJobs(staffID);
            if (jobs == null || jobs.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "technician_jobs",
                    Message = "You currently have no jobs assigned to you.",
                    ActionText = "Open My Jobs",
                    ActionUrl = "~/Secure/Technician/Jobs.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("Here are your assigned jobs:<br /><br />");
            int count = 0;
            foreach (DataRow row in jobs.Rows)
            {
                if (count >= 5)
                {
                    break;
                }

                string jobID = GetColumnValue(row, "JobID");
                string status = GetColumnValue(row, "JobStatus");
                string promiseDate = GetColumnValue(row, "PromiseDate");
                response.Append("<strong>Job #");
                response.Append(EncodeHtml(jobID));
                response.Append("</strong>");
                if (!string.IsNullOrWhiteSpace(status))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(status));
                }

                if (!string.IsNullOrWhiteSpace(promiseDate))
                {
                    response.Append(" — ");
                    response.Append(FormatDate(promiseDate));
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "technician_jobs",
                Message = response.ToString(),
                ActionText = "Open My Jobs",
                ActionUrl = "~/Secure/Technician/Jobs.aspx"
            };
        }

        private ChatResponse CreateTechnicianJobStatusResponse(string message, int staffID)
        {
            int jobID = ExtractJobID(message);
            if (jobID <= 0)
            {
                return new ChatResponse
                {
                    Intent = "technician_job_status",
                    Message = "Please provide the Job ID you want me to check. For example: <strong>What is the status of Job 1042?</strong>",
                    ActionText = "Open My Jobs",
                    ActionUrl = "~/Secure/Technician/Jobs.aspx"
                };
            }

            DataTable job = _jobService.GetTechnicianJobByID(jobID, staffID);
            if (job == null || job.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "technician_job_status",
                    Message = "I could not find that job among your assigned jobs. Please check the Job ID and try again.",
                    ActionText = "Open My Jobs",
                    ActionUrl = "~/Secure/Technician/Jobs.aspx"
                };
            }

            DataRow row = job.Rows[0];
            StringBuilder response = new StringBuilder();
            response.Append("<strong>Job #");
            response.Append(jobID);
            response.Append("</strong><br />");
            string status = GetColumnValue(row, "JobStatus");
            string promiseDate = GetColumnValue(row, "PromiseDate");
            string services = GetColumnValue(row, "Services");
            if (!string.IsNullOrWhiteSpace(status))
            {
                response.Append("Status: ");
                response.Append(EncodeHtml(status));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(services))
            {
                response.Append("Service: ");
                response.Append(EncodeHtml(services));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(promiseDate))
            {
                response.Append("Scheduled date: ");
                response.Append(FormatDate(promiseDate));
            }

            return new ChatResponse
            {
                Intent = "technician_job_status",
                Message = response.ToString(),
                ActionText = "Open My Jobs",
                ActionUrl = "~/Secure/Technician/Jobs.aspx"
            };
        }

        private ChatResponse CreateTechnicianLowStockResponse()
        {
            DataTable inventory = _inventoryService.GetAllInventoryItems();
            if (inventory == null || inventory.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "low_stock",
                    Message = "I could not find any inventory items.",
                    ActionText = "Open Inventory",
                    ActionUrl = "~/Secure/Technician/Inventory.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("Inventory items currently at or below their reorder level:<br /><br />");
            int lowStockCount = 0;
            foreach (DataRow row in inventory.Rows)
            {
                string quantityText = GetColumnValue(row, "QuantityInStock");
                if (string.IsNullOrWhiteSpace(quantityText))
                {
                    quantityText = GetColumnValue(row, "Quantity");
                }

                string minimumText = GetColumnValue(row, "ReorderLevel");
                if (string.IsNullOrWhiteSpace(minimumText))
                {
                    minimumText = GetColumnValue(row, "MinimumStockLevel");
                }

                string itemName = GetColumnValue(row, "ItemName");
                if (string.IsNullOrWhiteSpace(itemName))
                {
                    itemName = GetColumnValue(row, "InventoryItemName");
                }

                decimal quantity;
                decimal minimumStock;
                if (!decimal.TryParse(quantityText, out quantity))
                {
                    continue;
                }

                if (!decimal.TryParse(minimumText, out minimumStock))
                {
                    continue;
                }

                if (quantity <= minimumStock)
                {
                    response.Append("<strong>");
                    response.Append(EncodeHtml(itemName));
                    response.Append("</strong> — ");
                    response.Append(quantity);
                    response.Append(" available");
                    response.Append("<br />");
                    lowStockCount++;
                }
            }

            if (lowStockCount == 0)
            {
                return new ChatResponse
                {
                    Intent = "low_stock",
                    Message = "There are currently no inventory items at or below their reorder level.",
                    ActionText = "Open Inventory",
                    ActionUrl = "~/Secure/Technician/Inventory.aspx"
                };
            }

            return new ChatResponse
            {
                Intent = "low_stock",
                Message = response.ToString(),
                ActionText = "Open Inventory",
                ActionUrl = "~/Secure/Technician/Inventory.aspx"
            };
        }

        private ChatResponse CreateTechnicianProfileResponse()
        {
            return new ChatResponse
            {
                Intent = "technician_profile",
                Message = "You can view and manage your technician profile from your profile page.",
                ActionText = "Open Profile",
                ActionUrl = "~/Secure/Technician/Profile.aspx"
            };
        }

        private ChatResponse CreateTechnicianFallbackResponse()
        {
            return new ChatResponse
            {
                Intent = "technician_fallback",
                Message = "I can help with your assigned jobs, job status, low-stock inventory, your profile and general THY WILL electrical information."
            };
        }

        // ============================================================
        // ADMINISTRATOR RESPONSES
        // ============================================================
        private ChatResponse CreateAdministratorCreateJobResponse(string message)
        {
            return new ChatResponse
            {
                Intent = "administrator_create_job",
                Message = "To create a customer job or work order, use the Administrator Work Orders page. I can guide you to the page, but I do not create or modify jobs directly through the chatbot.",
                ActionText = "Open Work Orders",
                ActionUrl = "~/Secure/Admin/WorkOrders.aspx"
            };
        }

        private ChatResponse CreateAdministratorRescheduleResponse(string message)
        {
            return new ChatResponse
            {
                Intent = "administrator_reschedule",
                Message = "Jobs can be rescheduled from the Administrator Jobs page. Select the job, choose a valid future date and technician, then use the reschedule controls.",
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Admin/Jobs.aspx"
            };
        }

        private ChatResponse CreateAdministratorCancelJobResponse(string message)
        {
            return new ChatResponse
            {
                Intent = "administrator_cancel_job",
                Message = "Job cancellations are handled from the Administrator Jobs page. Open the job and use the available cancellation controls there.",
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Admin/Jobs.aspx"
            };
        }

        private ChatResponse CreateAdministratorJobsResponse()
        {
            return new ChatResponse
            {
                Intent = "administrator_jobs",
                Message = "You can search and manage operational job information from the Administrator Jobs page.",
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Admin/Jobs.aspx"
            };
        }

        private ChatResponse CreateAdministratorJobStatusResponse(string message)
        {
            int jobID = ExtractJobID(message);
            if (jobID <= 0)
            {
                return new ChatResponse
                {
                    Intent = "administrator_job_status",
                    Message = "Please provide the Job ID you want me to check. For example: <strong>What is the status of Job 1042?</strong>",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Admin/Jobs.aspx"
                };
            }

            DataTable job = _jobService.GetJobManagementDetails(jobID);
            if (job == null || job.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "administrator_job_status",
                    Message = "I could not find that job. Please check the Job ID and try again.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Admin/Jobs.aspx"
                };
            }

            DataRow row = job.Rows[0];
            string customerName = GetColumnValue(row, "CustomerName");
            string technicianName = GetColumnValue(row, "TechnicianName");
            string services = GetColumnValue(row, "Services");
            string promiseDate = GetColumnValue(row, "PromiseDate");
            string status = GetColumnValue(row, "JobStatus");
            StringBuilder response = new StringBuilder();
            response.Append("<strong>Job #");
            response.Append(jobID);
            response.Append("</strong><br />");
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                response.Append("Customer: ");
                response.Append(EncodeHtml(customerName));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(technicianName))
            {
                response.Append("Technician: ");
                response.Append(EncodeHtml(technicianName));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(services))
            {
                response.Append("Service: ");
                response.Append(EncodeHtml(services));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(promiseDate))
            {
                response.Append("Promise date: ");
                response.Append(FormatDate(promiseDate));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                response.Append("Status: ");
                response.Append(EncodeHtml(status));
            }

            return new ChatResponse
            {
                Intent = "administrator_job_status",
                Message = response.ToString(),
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Admin/Jobs.aspx"
            };
        }

        private ChatResponse CreateAdministratorCustomerLookupResponse(string message)
        {
            string searchTerm = ExtractCustomerSearchTerm(message);
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ChatResponse
                {
                    Intent = "administrator_customer_lookup",
                    Message = "Please provide the customer's name. For example: <strong>Find customer John Smith.</strong>",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            DataTable customers = _customerService.SearchCustomersByName(searchTerm);
            if (customers == null || customers.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "administrator_customer_lookup",
                    Message = "I could not find a customer matching <strong>" + EncodeHtml(searchTerm) + "</strong>.",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("I found the following customer records:<br /><br />");
            int count = 0;
            foreach (DataRow row in customers.Rows)
            {
                if (count >= 5)
                {
                    break;
                }

                string fullName = GetColumnValue(row, "FullName");
                string customerType = GetColumnValue(row, "CustomerType");
                string area = GetColumnValue(row, "Area");
                string phone = GetColumnValue(row, "PhoneNumber");
                string email = GetColumnValue(row, "EmailAddress");
                response.Append("<strong>");
                response.Append(EncodeHtml(fullName));
                response.Append("</strong>");
                if (!string.IsNullOrWhiteSpace(customerType))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(customerType));
                }

                response.Append("<br />");
                if (!string.IsNullOrWhiteSpace(area))
                {
                    response.Append("Area: ");
                    response.Append(EncodeHtml(area));
                    response.Append("<br />");
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    response.Append("Phone: ");
                    response.Append(EncodeHtml(phone));
                    response.Append("<br />");
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    response.Append("Email: ");
                    response.Append(EncodeHtml(email));
                    response.Append("<br />");
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "administrator_customer_lookup",
                Message = response.ToString(),
                ActionText = "Open Customers",
                ActionUrl = "~/Secure/Admin/Customers.aspx"
            };
        }

        private ChatResponse CreateAdministratorProfileResponse()
        {
            return new ChatResponse
            {
                Intent = "administrator_profile",
                Message = "You can view your administrator profile, update your contact information and change your password from your profile page.",
                ActionText = "Open Profile",
                ActionUrl = "~/Secure/Admin/Profile.aspx"
            };
        }

        private ChatResponse CreateAdministratorRestrictedResponse()
        {
            return new ChatResponse
            {
                Intent = "administrator_restricted",
                Message = "I can help with customer records, jobs, work orders and your administrator profile. Finance, invoice information, reports and other restricted management information are not available through the Administrator assistant."
            };
        }

        private ChatResponse CreateAdministratorFallbackResponse()
        {
            return new ChatResponse
            {
                Intent = "administrator_fallback",
                Message = "I can help with customer lookups, jobs, work orders, job status, rescheduling guidance, cancellations, your administrator profile and general THY WILL electrical information. Finance and reports are not available through the Administrator assistant."
            };
        }

        // ============================================================
        // MANAGER RESPONSES
        // ============================================================
        private ChatResponse CreateManagerCreateJobResponse()
        {
            return new ChatResponse
            {
                Intent = "manager_create_job",
                Message = "You can create a customer job or work order from the Manager Work Orders page. The chatbot does not create jobs directly so that the normal validation, technician availability, inventory and financial transaction process is used.",
                ActionText = "Open Work Orders",
                ActionUrl = "~/Secure/Manager/WorkOrders.aspx"
            };
        }

        private ChatResponse CreateManagerRescheduleResponse()
        {
            return new ChatResponse
            {
                Intent = "manager_reschedule",
                Message = "You can reschedule an active job from the Manager Jobs page. The system checks the new date and technician availability before the change is saved.",
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerCancelJobResponse()
        {
            return new ChatResponse
            {
                Intent = "manager_cancel_job",
                Message = "You can cancel an eligible job from the Manager Jobs page. The normal cancellation process handles the job status and any inventory restoration required by the system.",
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerJobStatusResponse(string message)
        {
            int jobID = ExtractJobID(message);
            if (jobID <= 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_job_status",
                    Message = "Please provide the Job ID you want me to check. For example: What is the status of Job 1042?",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            DataTable job = _jobService.GetJobManagementDetails(jobID);
            if (job == null || job.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_job_status",
                    Message = "I could not find that job. Please check the Job ID and try again.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            DataRow row = job.Rows[0];
            string customerName = GetColumnValue(row, "CustomerName");
            string technicianName = GetColumnValue(row, "TechnicianName");
            string services = GetColumnValue(row, "Services");
            string promiseDate = GetColumnValue(row, "PromiseDate");
            string status = GetColumnValue(row, "JobStatus");
            StringBuilder response = new StringBuilder();
            response.Append("<strong>Job #");
            response.Append(jobID);
            response.Append("</strong><br />");
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                response.Append("Customer: ");
                response.Append(EncodeHtml(customerName));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(technicianName))
            {
                response.Append("Technician: ");
                response.Append(EncodeHtml(technicianName));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(services))
            {
                response.Append("Service: ");
                response.Append(EncodeHtml(services));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(promiseDate))
            {
                response.Append("Promise date: ");
                response.Append(FormatDate(promiseDate));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                response.Append("Status: ");
                response.Append(EncodeHtml(status));
            }

            return new ChatResponse
            {
                Intent = "manager_job_status",
                Message = response.ToString(),
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerJobsResponse(string message)
        {
            DataTable jobs;
            if (message.Contains("pending"))
            {
                jobs = _jobService.GetJobsByStatus("Pending");
            }
            else if (message.Contains("in progress"))
            {
                jobs = _jobService.GetJobsByStatus("In Progress");
            }
            else if (message.Contains("completed"))
            {
                jobs = _jobService.GetJobsByStatus("Completed");
            }
            else if (message.Contains("cancelled") || message.Contains("canceled"))
            {
                jobs = _jobService.GetJobsByStatus("Cancelled");
            }
            else
            {
                jobs = _jobService.GetAllJobs();
            }

            if (jobs == null || jobs.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_jobs",
                    Message = "I could not find any jobs matching that request.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("I found ");
            response.Append(jobs.Rows.Count);
            response.Append(" job(s)");
            if (message.Contains("pending"))
            {
                response.Append(" with Pending status");
            }
            else if (message.Contains("in progress"))
            {
                response.Append(" with In Progress status");
            }
            else if (message.Contains("completed"))
            {
                response.Append(" with Completed status");
            }
            else if (message.Contains("cancelled") || message.Contains("canceled"))
            {
                response.Append(" with Cancelled status");
            }

            response.Append(".<br /><br />");
            int count = 0;
            foreach (DataRow row in jobs.Rows)
            {
                if (count >= 8)
                {
                    break;
                }

                string jobID = GetColumnValue(row, "JobID");
                string status = GetColumnValue(row, "JobStatus");
                string promiseDate = GetColumnValue(row, "PromiseDate");
                response.Append("Job #");
                response.Append(EncodeHtml(jobID));
                if (!string.IsNullOrWhiteSpace(status))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(status));
                }

                if (!string.IsNullOrWhiteSpace(promiseDate))
                {
                    response.Append(" — ");
                    response.Append(FormatDate(promiseDate));
                }

                response.Append("<br />");
                count++;
            }

            if (jobs.Rows.Count > 8)
            {
                response.Append("<br />Showing the first 8 jobs. ");
                response.Append("Open the Jobs page to search and filter the complete list.");
            }

            return new ChatResponse
            {
                Intent = "manager_jobs",
                Message = response.ToString(),
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerCustomerLookupResponse(string message)
        {
            string searchTerm = ExtractCustomerSearchTerm(message);
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ChatResponse
                {
                    Intent = "manager_customer_lookup",
                    Message = "Please provide the customer's name. For example: Find customer John Smith.",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            DataTable customers = _customerService.SearchCustomersByName(searchTerm);
            if (customers == null || customers.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_customer_lookup",
                    Message = "I could not find a customer matching " + "<strong>" + EncodeHtml(searchTerm) + "</strong>.",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("I found ");
            response.Append(customers.Rows.Count);
            response.Append(" matching customer record(s).<br /><br />");
            int count = 0;
            foreach (DataRow row in customers.Rows)
            {
                if (count >= 5)
                {
                    break;
                }

                string fullName = GetColumnValue(row, "FullName");
                string customerType = GetColumnValue(row, "CustomerType");
                string area = GetColumnValue(row, "Area");
                string phone = GetColumnValue(row, "PhoneNumber");
                string email = GetColumnValue(row, "EmailAddress");
                response.Append("<strong>");
                response.Append(EncodeHtml(fullName));
                response.Append("</strong>");
                if (!string.IsNullOrWhiteSpace(customerType))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(customerType));
                }

                response.Append("<br />");
                if (!string.IsNullOrWhiteSpace(area))
                {
                    response.Append("Area: ");
                    response.Append(EncodeHtml(area));
                    response.Append("<br />");
                }

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    response.Append("Phone: ");
                    response.Append(EncodeHtml(phone));
                    response.Append("<br />");
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    response.Append("Email: ");
                    response.Append(EncodeHtml(email));
                    response.Append("<br />");
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "manager_customer_lookup",
                Message = response.ToString(),
                ActionText = "Open Customers",
                ActionUrl = "~/Secure/Admin/Customers.aspx"
            };
        }

        private ChatResponse CreateManagerCustomerJobsResponse(string message)
        {
            string searchTerm = ExtractCustomerSearchTerm(message);
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ChatResponse
                {
                    Intent = "manager_customer_jobs",
                    Message = "Please provide the customer's name. For example: Show jobs for customer John Smith.",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            DataTable customers = _customerService.SearchCustomersByName(searchTerm);
            if (customers == null || customers.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_customer_jobs",
                    Message = "I could not find that customer.",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            DataRow customerRow = customers.Rows[0];
            string customerIDText = GetColumnValue(customerRow, "CustomerID");
            int customerID;
            if (!int.TryParse(customerIDText, out customerID))
            {
                return new ChatResponse
                {
                    Intent = "manager_customer_jobs",
                    Message = "I found the customer, but I could not retrieve the customer account ID.",
                    ActionText = "Open Customers",
                    ActionUrl = "~/Secure/Admin/Customers.aspx"
                };
            }

            DataTable jobs = _jobService.GetJobsByCustomerID(customerID);
            if (jobs == null || jobs.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_customer_jobs",
                    Message = "That customer does not currently have any jobs.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("Jobs for ");
            response.Append("<strong>" + EncodeHtml(GetColumnValue(customerRow, "FullName")) + "</strong>");
            response.Append(":<br /><br />");
            int count = 0;
            foreach (DataRow row in jobs.Rows)
            {
                if (count >= 8)
                {
                    break;
                }

                response.Append("Job #");
                response.Append(EncodeHtml(GetColumnValue(row, "JobID")));
                string status = GetColumnValue(row, "JobStatus");
                string promiseDate = GetColumnValue(row, "PromiseDate");
                if (!string.IsNullOrWhiteSpace(status))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(status));
                }

                if (!string.IsNullOrWhiteSpace(promiseDate))
                {
                    response.Append(" — ");
                    response.Append(FormatDate(promiseDate));
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "manager_customer_jobs",
                Message = response.ToString(),
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerTechnicianResponse(string message)
        {
            if (message.Contains("available technician") || message.Contains("available technicians") || message.Contains("technician availability") || message.Contains("staff availability"))
            {
                return new ChatResponse
                {
                    Intent = "manager_technician_availability",
                    Message = "Technician availability is checked from the Manager Jobs page when scheduling or rescheduling a job. The system checks which technicians are available for the selected date.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            DataTable staff = _staffService.GetAllStaff();
            if (staff == null || staff.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_technicians",
                    Message = "I could not find any staff records.",
                    ActionText = "Open Staff Management",
                    ActionUrl = "~/Secure/Manager/StaffManagement.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("Here is the available operational staff information:<br /><br />");
            int count = 0;
            foreach (DataRow row in staff.Rows)
            {
                if (count >= 8)
                {
                    break;
                }

                string fullName = GetColumnValue(row, "FullName");
                string roleName = GetColumnValue(row, "RoleName");
                string employmentStatus = GetColumnValue(row, "EmploymentStatus");
                string isAvailable = GetColumnValue(row, "IsAvailable");
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    continue;
                }

                response.Append(EncodeHtml(fullName));
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(roleName));
                }

                if (!string.IsNullOrWhiteSpace(employmentStatus))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(employmentStatus));
                }

                if (!string.IsNullOrWhiteSpace(isAvailable))
                {
                    response.Append(" — Available: ");
                    response.Append(EncodeHtml(isAvailable));
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "manager_technicians",
                Message = response.ToString(),
                ActionText = "Open Staff Management",
                ActionUrl = "~/Secure/Manager/StaffManagement.aspx"
            };
        }

        private ChatResponse CreateManagerInventoryResponse(string message)
        {
            DataTable inventory = _inventoryService.GetAllInventoryItems();
            if (inventory == null || inventory.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_inventory",
                    Message = "I could not find any inventory items.",
                    ActionText = "Open Inventory",
                    ActionUrl = "~/Secure/Manager/Inventory.aspx"
                };
            }

            bool lowStockOnly = message.Contains("low stock") || message.Contains("low inventory") || message.Contains("stock running low");
            StringBuilder response = new StringBuilder();
            if (lowStockOnly)
            {
                response.Append("Inventory items at or below their reorder level:<br /><br />");
            }
            else
            {
                response.Append("Current inventory information:<br /><br />");
            }

            int count = 0;
            foreach (DataRow row in inventory.Rows)
            {
                string itemName = GetColumnValue(row, "ItemName");
                string quantity = GetColumnValue(row, "QuantityInStock");
                if (string.IsNullOrWhiteSpace(quantity))
                {
                    quantity = GetColumnValue(row, "Quantity");
                }

                string reorderLevel = GetColumnValue(row, "ReorderLevel");
                if (string.IsNullOrWhiteSpace(reorderLevel))
                {
                    reorderLevel = GetColumnValue(row, "MinimumStockLevel");
                }

                decimal quantityValue;
                decimal reorderValue;
                bool quantityValid = decimal.TryParse(quantity, out quantityValue);
                bool reorderValid = decimal.TryParse(reorderLevel, out reorderValue);
                if (lowStockOnly)
                {
                    if (!quantityValid || !reorderValid || quantityValue > reorderValue)
                    {
                        continue;
                    }
                }

                response.Append(EncodeHtml(itemName));
                response.Append(" — ");
                response.Append(EncodeHtml(quantity));
                response.Append(" in stock");
                if (!string.IsNullOrWhiteSpace(reorderLevel))
                {
                    response.Append(" — reorder level ");
                    response.Append(EncodeHtml(reorderLevel));
                }

                response.Append("<br />");
                count++;
                if (count >= 10)
                {
                    break;
                }
            }

            if (count == 0 && lowStockOnly)
            {
                return new ChatResponse
                {
                    Intent = "manager_inventory",
                    Message = "There are currently no inventory items at or below their reorder level.",
                    ActionText = "Open Inventory",
                    ActionUrl = "~/Secure/Manager/Inventory.aspx"
                };
            }

            return new ChatResponse
            {
                Intent = "manager_inventory",
                Message = response.ToString(),
                ActionText = "Open Inventory",
                ActionUrl = "~/Secure/Manager/Inventory.aspx"
            };
        }

        private ChatResponse CreateManagerServiceResponse(string message)
        {
            DataTable services = _jobTypeService.GetAllJobTypes();
            if (services == null || services.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_services",
                    Message = "I could not find any service types.",
                    ActionText = "Open Services",
                    ActionUrl = "~/Secure/Manager/Services.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("Available service types:<br /><br />");
            int count = 0;
            foreach (DataRow row in services.Rows)
            {
                if (count >= 10)
                {
                    break;
                }

                string jobTypeName = GetColumnValue(row, "JobTypeName");
                if (string.IsNullOrWhiteSpace(jobTypeName))
                {
                    jobTypeName = GetColumnValue(row, "ServiceName");
                }

                string labourCost = GetColumnValue(row, "BaseLabourCost");
                string transportCost = GetColumnValue(row, "BaseTransportCost");
                string taxPercentage = GetColumnValue(row, "TaxPercentage");
                response.Append(EncodeHtml(jobTypeName));
                if (!string.IsNullOrWhiteSpace(labourCost))
                {
                    response.Append(" — Labour: ");
                    response.Append(FormatMoney(labourCost));
                }

                if (!string.IsNullOrWhiteSpace(transportCost))
                {
                    response.Append(" — Transport: ");
                    response.Append(FormatMoney(transportCost));
                }

                if (!string.IsNullOrWhiteSpace(taxPercentage))
                {
                    response.Append(" — Tax: ");
                    response.Append(EncodeHtml(taxPercentage));
                    response.Append("%");
                }

                response.Append("<br />");
                count++;
            }

            return new ChatResponse
            {
                Intent = "manager_services",
                Message = response.ToString(),
                ActionText = "Open Services",
                ActionUrl = "~/Secure/Manager/Services.aspx"
            };
        }

        private ChatResponse CreateManagerInvoicesResponse()
        {
            DataTable invoices = _invoiceService.GetAllInvoices();
            if (invoices == null || invoices.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_invoices",
                    Message = "I could not find any invoice records.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            StringBuilder response = new StringBuilder();
            response.Append("I found ");
            response.Append(invoices.Rows.Count);
            response.Append(" invoice record(s).<br /><br />");
            int count = 0;
            foreach (DataRow row in invoices.Rows)
            {
                if (count >= 8)
                {
                    break;
                }

                string jobID = GetColumnValue(row, "JobID");
                string status = GetColumnValue(row, "JobStatus");
                string grandTotal = GetColumnValue(row, "GrandTotal");
                if (string.IsNullOrWhiteSpace(grandTotal))
                {
                    grandTotal = GetColumnValue(row, "TotalAmount");
                }

                response.Append("Job #");
                response.Append(EncodeHtml(jobID));
                if (!string.IsNullOrWhiteSpace(status))
                {
                    response.Append(" — ");
                    response.Append(EncodeHtml(status));
                }

                if (!string.IsNullOrWhiteSpace(grandTotal))
                {
                    response.Append(" — Total: ");
                    response.Append(FormatMoney(grandTotal));
                }

                response.Append("<br />");
                count++;
            }

            if (invoices.Rows.Count > 8)
            {
                response.Append("<br />Showing the first 8 invoice records.");
            }

            return new ChatResponse
            {
                Intent = "manager_invoices",
                Message = response.ToString(),
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerInvoiceDetailsResponse(string message)
        {
            int jobID = ExtractJobID(message);
            if (jobID <= 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_invoice_details",
                    Message = "Please provide the Job ID for the invoice. For example: Show invoice details for Job 1042.",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            DataTable invoice = _invoiceService.GetInvoiceByJobID(jobID);
            if (invoice == null || invoice.Rows.Count == 0)
            {
                return new ChatResponse
                {
                    Intent = "manager_invoice_details",
                    Message = "I could not find invoice information for Job #" + jobID + ".",
                    ActionText = "Open Jobs",
                    ActionUrl = "~/Secure/Manager/Jobs.aspx"
                };
            }

            DataRow row = invoice.Rows[0];
            StringBuilder response = new StringBuilder();
            response.Append("<strong>Invoice details for Job #");
            response.Append(jobID);
            response.Append("</strong><br /><br />");
            AppendMoneyField(response, "Labour cost", row, "LabourCost");
            AppendMoneyField(response, "Material cost", row, "TotalMaterialCost");
            AppendMoneyField(response, "Transport cost", row, "TransportCost");
            AppendMoneyField(response, "Tax", row, "TaxAmount");
            AppendMoneyField(response, "Grand total", row, "GrandTotal");
            string invoiceSentDate = GetColumnValue(row, "InvoiceSentDate");
            string sentToEmail = GetColumnValue(row, "InvoiceSentToEmail");
            if (!string.IsNullOrWhiteSpace(invoiceSentDate))
            {
                response.Append("Invoice sent: ");
                response.Append(FormatDate(invoiceSentDate));
                response.Append("<br />");
            }

            if (!string.IsNullOrWhiteSpace(sentToEmail))
            {
                response.Append("Sent to: ");
                response.Append(EncodeHtml(sentToEmail));
                response.Append("<br />");
            }

            return new ChatResponse
            {
                Intent = "manager_invoice_details",
                Message = response.ToString(),
                ActionText = "Open Jobs",
                ActionUrl = "~/Secure/Manager/Jobs.aspx"
            };
        }

        private ChatResponse CreateManagerReportsResponse()
        {
            return new ChatResponse
            {
                Intent = "manager_reports",
                Message = "Manager reports are available through the Power BI reports page. I can direct you there, but I do not invent or reproduce report figures that are not available through the chatbot's approved data services.",
                ActionText = "Open Reports",
                ActionUrl = "~/Secure/Manager/Reports.aspx"
            };
        }

        private ChatResponse CreateManagerProfileResponse()
        {
            return new ChatResponse
            {
                Intent = "manager_profile",
                Message = "You can view your manager profile, update your contact details and change your password from your profile page.",
                ActionText = "Open Profile",
                ActionUrl = "~/Secure/Manager/Profile.aspx"
            };
        }

        private ChatResponse CreateManagerFallbackResponse()
        {
            return new ChatResponse
            {
                Intent = "manager_fallback",
                Message = "I can help with jobs, customers, technicians, inventory, services, invoices, finance, reports and your manager profile. I can also guide you to the correct management page for creating, rescheduling or cancelling jobs."
            };
        }

        // ============================================================
        // PUBLIC INTENT DETECTION
        // ============================================================
        private bool IsEmergencyMessage(string message)
        {
            return message.Contains("emergency") || message.Contains("electrical emergency") || message.Contains("electric shock") || message.Contains("sparking") || message.Contains("sparks") || message.Contains("burning smell") || message.Contains("electrical fire") || message.Contains("fire");
        }

        private bool IsServiceDetailMessage(string message)
        {
            return message.Contains("geyser") || message.Contains("ev charger") || message.Contains("electric vehicle charger") || message.Contains("db board") || message.Contains("distribution board") || message.Contains("high load circuit") || message.Contains("general electrical");
        }

        private bool IsServicesMessage(string message)
        {
            return message == "services" || message.Contains("what services") || message.Contains("what do you offer") || message.Contains("services do you offer") || message.Contains("what electrical services");
        }

        private bool IsServiceAreaMessage(string message)
        {
            return message.Contains("service area") || message.Contains("where do you operate") || message.Contains("where do you work") || message.Contains("areas you cover") || message.Contains("where are you located");
        }

        private bool IsBookingMessage(string message)
        {
            return message.Contains("book") || message.Contains("booking") || message.Contains("appointment") || message.Contains("schedule a service") || message.Contains("request a service");
        }

        private bool IsContactMessage(string message)
        {
            return message.Contains("contact") || message.Contains("phone number") || message.Contains("email address") || message.Contains("how can i contact") || message.Contains("contact you");
        }

        private bool IsAboutMessage(string message)
        {
            return message.Contains("about thy will") || message.Contains("who are you") || message.Contains("about the company") || message.Contains("what is thy will");
        }

        // ============================================================
        // PUBLIC RESPONSES
        // ============================================================
        private ChatResponse CreateEmergencyResponse()
        {
            return new ChatResponse
            {
                Intent = "emergency",
                Message = "If there is an immediate electrical danger, switch off the power at the main supply if it is safe to do so, keep away from exposed electrical equipment and contact emergency services or a qualified electrician. Do not touch someone who is receiving an electric shock until the electrical supply has been safely isolated."
            };
        }

        private ChatResponse CreateServiceDetailResponse(string message)
        {
            if (message.Contains("geyser"))
            {
                return new ChatResponse
                {
                    Intent = "service_detail",
                    Message = "THY WILL provides geyser-related electrical services, including electrical connections, fault diagnosis and related installation work. A qualified technician should assess the specific installation.",
                    ActionText = "Book a Service",
                    ActionUrl = "~/Secure/Customer/BookJob.aspx"
                };
            }

            if (message.Contains("ev charger") || message.Contains("electric vehicle charger"))
            {
                return new ChatResponse
                {
                    Intent = "service_detail",
                    Message = "THY WILL provides EV charger electrical installation and related electrical work. The installation requirements depend on the property, charger and available electrical capacity.",
                    ActionText = "Book a Service",
                    ActionUrl = "~/Secure/Customer/BookJob.aspx"
                };
            }

            if (message.Contains("db board") || message.Contains("distribution board"))
            {
                return new ChatResponse
                {
                    Intent = "service_detail",
                    Message = "THY WILL provides distribution board-related electrical services, including installation, upgrades, fault diagnosis and associated electrical work.",
                    ActionText = "Book a Service",
                    ActionUrl = "~/Secure/Customer/BookJob.aspx"
                };
            }

            if (message.Contains("high load circuit"))
            {
                return new ChatResponse
                {
                    Intent = "service_detail",
                    Message = "THY WILL provides high-load circuit electrical work for suitable installations. The required protection, cabling and capacity depend on the specific electrical load and property.",
                    ActionText = "Book a Service",
                    ActionUrl = "~/Secure/Customer/BookJob.aspx"
                };
            }

            return new ChatResponse
            {
                Intent = "service_detail",
                Message = "THY WILL provides general electrical services for homes, businesses and other suitable properties.",
                ActionText = "View Services",
                ActionUrl = "~/Public/Services.aspx"
            };
        }

        private ChatResponse CreateServicesResponse()
        {
            return new ChatResponse
            {
                Intent = "services",
                Message = "THY WILL provides electrical services including geyser work, EV charger installations, distribution board work, general electrical services and high-load circuit work.",
                ActionText = "View Services",
                ActionUrl = "~/Public/Services.aspx"
            };
        }

        private ChatResponse CreateServiceAreaResponse()
        {
            return new ChatResponse
            {
                Intent = "service_area",
                Message = "THY WILL Electrical & Construction operates in Durban and the surrounding KwaZulu-Natal service area. Availability depends on the requested service and location.",
                ActionText = "Contact THY WILL",
                ActionUrl = "~/Public/Contact.aspx"
            };
        }

        private ChatResponse CreateBookingResponse()
        {
            return new ChatResponse
            {
                Intent = "booking",
                Message = "You can request an electrical service through the THY WILL booking process. The booking page captures the information needed to arrange the job.",
                ActionText = "Book a Service",
                ActionUrl = "~/Secure/Customer/BookJob.aspx"
            };
        }

        private ChatResponse CreateContactResponse()
        {
            return new ChatResponse
            {
                Intent = "contact",
                Message = "You can contact THY WILL Electrical & Construction through the contact page. The business email address is info@thywill.co.za.",
                ActionText = "Contact THY WILL",
                ActionUrl = "~/Public/Contact.aspx"
            };
        }

        private ChatResponse CreateAboutResponse()
        {
            return new ChatResponse
            {
                Intent = "about",
                Message = "THY WILL Electrical & Construction (Pty) Ltd provides electrical and construction-related services for customers in Durban and the surrounding KwaZulu-Natal service area.",
                ActionText = "About THY WILL",
                ActionUrl = "~/Public/About.aspx"
            };
        }

        private ChatResponse CreateFallbackResponse()
        {
            return new ChatResponse
            {
                Intent = "fallback",
                Message = "I can help with THY WILL services, service areas, bookings, contact information and general electrical information."
            };
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private int ExtractJobID(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return 0;
            }

            Match match = Regex.Match(message, @"(?:job\s*(?:#|number)?\s*|#\s*)(\d+)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return 0;
            }

            int jobID;
            if (int.TryParse(match.Groups[1].Value, out jobID))
            {
                return jobID;
            }

            return 0;
        }

        private string ExtractCustomerSearchTerm(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return string.Empty;
            }

            Match match = Regex.Match(message, @"(?:customer|client)\s+(?:named\s+|called\s+)?(.+?)(?:\s+jobs?$|\s+job\s+history$|\s+history$|$)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string result = match.Groups[1].Value.Trim();
                result = result.Trim(' ', '.', '?', '!', ',', ':');
                return result;
            }

            return string.Empty;
        }

        private DateTime? ExtractRequestedDate(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            DateTime today = DateTime.Today;
            if (message.Contains("today"))
            {
                return today;
            }

            if (message.Contains("tomorrow"))
            {
                return today.AddDays(1);
            }

            Match dateMatch = Regex.Match(message, @"\b(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{2,4})\b");
            if (!dateMatch.Success)
            {
                dateMatch = Regex.Match(message, @"\b(\d{4})[\/\-](\d{1,2})[\/\-](\d{1,2})\b");
            }

            if (!dateMatch.Success)
            {
                return null;
            }

            int first;
            int second;
            int third;
            if (!int.TryParse(dateMatch.Groups[1].Value, out first))
            {
                return null;
            }

            if (!int.TryParse(dateMatch.Groups[2].Value, out second))
            {
                return null;
            }

            if (!int.TryParse(dateMatch.Groups[3].Value, out third))
            {
                return null;
            }

            DateTime parsedDate;
            if (dateMatch.Groups[1].Value.Length == 4)
            {
                try
                {
                    parsedDate = new DateTime(first, second, third);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                if (third < 100)
                {
                    third += 2000;
                }

                try
                {
                    parsedDate = new DateTime(third, second, first);
                }
                catch
                {
                    return null;
                }
            }

            return parsedDate;
        }

        private string GetColumnValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || string.IsNullOrWhiteSpace(columnName))
            {
                return string.Empty;
            }

            if (!row.Table.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            object value = row[columnName];
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }

        private string FormatDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            DateTime date;
            if (DateTime.TryParse(value, out date))
            {
                return date.ToString("dd MMM yyyy");
            }

            return EncodeHtml(value);
        }

        private string FormatMoney(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            decimal amount;
            if (decimal.TryParse(value, out amount))
            {
                return "R" + amount.ToString("N2");
            }

            return EncodeHtml(value);
        }

        private void AppendMoneyField(StringBuilder response, string label, DataRow row, string columnName)
        {
            string value = GetColumnValue(row, columnName);
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            response.Append(EncodeHtml(label));
            response.Append(": ");
            response.Append(FormatMoney(value));
            response.Append("<br />");
        }
    }
}
