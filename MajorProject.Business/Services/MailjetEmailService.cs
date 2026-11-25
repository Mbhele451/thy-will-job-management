using MajorProject.Business.Helpers; // Logger is now here
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MajorProject.Business.Services
{
    public class MailjetEmailService
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly HttpClient _httpClient;
        /// <summary>
        /// Constructor that receives all Mailjet configuration from the calling application.
        /// </summary>
        public MailjetEmailService(string apiKey, string apiSecret, string fromEmail, string fromName)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
            _fromEmail = fromEmail;
            _fromName = fromName;
            _httpClient = new HttpClient();
            var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_apiKey}:{_apiSecret}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");
            Logger.Info($"Mailjet REST API v3.1 configured: From={_fromEmail}");
        }

        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
                {
                    Logger.Error("Mailjet API keys are missing.");
                    return false;
                }

                if (string.IsNullOrEmpty(_fromEmail))
                {
                    Logger.Error("Mailjet FromEmail is missing.");
                    return false;
                }

                var payload = new JObject
                {
                    ["Messages"] = new JArray
                    {
                        new JObject
                        {
                            ["From"] = new JObject
                            {
                                ["Email"] = _fromEmail,
                                ["Name"] = _fromName
                            },
                            ["To"] = new JArray
                            {
                                new JObject
                                {
                                    ["Email"] = toEmail,
                                    ["Name"] = toName ?? toEmail
                                }
                            },
                            ["Subject"] = subject,
                            ["HTMLPart"] = htmlBody
                        }
                    }
                };
                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                string url = "https://api.mailjet.com/v3.1/send";
                Logger.Info($"Mailjet API v3.1: Sending to {toEmail}");
                var response = await _httpClient.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Logger.Info($"Mailjet API v3.1: Email sent to {toEmail}. Status: {response.StatusCode}");
                    return true;
                }
                else
                {
                    Logger.Error($"Mailjet API v3.1 error: Status={response.StatusCode}, Response={responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Mailjet API v3.1 exception: {ex.Message}", ex);
                return false;
            }
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string fullName, string otp)
        {
            string subject = "Password Reset OTP";
            string htmlBody = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hi {fullName},</p>
                    <p>Your OTP is: <strong>{otp}</strong></p>
                    <p>This code expires in 5 minutes.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>Regards,<br/>Support Team</p>
                </body>
                </html>
            ";
            return await SendEmailAsync(toEmail, fullName, subject, htmlBody);
        }

        /// <summary>
        /// Sends an email with a PDF attachment (byte array).
        /// </summary>
        public async Task<bool> SendEmailWithAttachmentAsync(string toEmail, string toName, string subject, string htmlBody, byte[] attachmentData, string attachmentFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_apiSecret))
                {
                    Logger.Error("Mailjet API keys are missing.");
                    return false;
                }

                // Mailjet v3.1 supports attachments via base64 encoding
                string base64Attachment = Convert.ToBase64String(attachmentData);
                var payload = new JObject
                {
                    ["Messages"] = new JArray
                    {
                        new JObject
                        {
                            ["From"] = new JObject
                            {
                                ["Email"] = _fromEmail,
                                ["Name"] = _fromName
                            },
                            ["To"] = new JArray
                            {
                                new JObject
                                {
                                    ["Email"] = toEmail,
                                    ["Name"] = toName ?? toEmail
                                }
                            },
                            ["Subject"] = subject,
                            ["HTMLPart"] = htmlBody,
                            ["Attachments"] = new JArray
                            {
                                new JObject
                                {
                                    ["ContentType"] = "application/pdf",
                                    ["Filename"] = attachmentFileName,
                                    ["Base64Content"] = base64Attachment
                                }
                            }
                        }
                    }
                };
                string jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                string url = "https://api.mailjet.com/v3.1/send";
                Logger.Info($"Mailjet API v3.1: Sending invoice to {toEmail}");
                var response = await _httpClient.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Logger.Info($"Mailjet API v3.1: Invoice sent to {toEmail}. Status: {response.StatusCode}");
                    return true;
                }
                else
                {
                    Logger.Error($"Mailjet API v3.1 error: Status={response.StatusCode}, Response={responseBody}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Mailjet API v3.1 exception: {ex.Message}", ex);
                return false;
            }
        }
    }
}
