using MajorProject.Business.Models;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace MajorProject.Web.Chatbot
{
    public partial class Chatbot : UserControl
    {
        private const string ChatHistoryViewStateKey = "ChatHistory";
        private const int MaximumMessageLength = 500;
        private const int MaximumHistoryLength = 12000;
        private ChatbotService ChatbotService
        {
            get
            {
                bool requiresDatabase = WebSession.HasRole("Customer") || WebSession.HasRole("Technician") || WebSession.HasRole("Administrator") || WebSession.HasRole("Manager");
                if (requiresDatabase)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
                    return new ChatbotService(connectionString);
                }

                return new ChatbotService();
            }
        }

        private bool IsCustomer
        {
            get
            {
                return WebSession.HasRole("Customer") && WebSession.CustomerID > 0;
            }
        }

        private bool IsTechnician
        {
            get
            {
                return WebSession.HasRole("Technician") && WebSession.StaffID > 0;
            }
        }

        private bool IsAdministrator
        {
            get
            {
                return WebSession.HasRole("Administrator") && WebSession.StaffID > 0;
            }
        }

        private bool IsManager
        {
            get
            {
                return WebSession.HasRole("Manager") && WebSession.StaffID > 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hfChatbotOpen.Value = "false";
                LoadChatHistory();
            }
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            string userMessage = txtChatMessage.Text.Trim();
            /*
             * Keep the chatbot open after the postback.
             */
            hfChatbotOpen.Value = "true";
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }

            /*
             * Validate the chatbot input before it reaches
             * the chatbot service.
             */
            string validationMessage = ValidateChatbotMessage(userMessage);
            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                AddUserMessage(userMessage);
                AddAssistantMessage(new ChatResponse { Message = validationMessage, Intent = "INPUT_VALIDATION", Success = false });
                txtChatMessage.Text = string.Empty;
                return;
            }

            AddUserMessage(userMessage);
            ChatResponse response;
            /*
             * Customer chatbot.
             */
            if (IsCustomer)
            {
                int customerID = WebSession.CustomerID;
                response = ChatbotService.ProcessCustomerMessage(userMessage, customerID);
            }
            /*
             * Technician chatbot.
             */
            else if (IsTechnician)
            {
                int staffID = WebSession.StaffID;
                response = ChatbotService.ProcessTechnicianMessage(userMessage, staffID);
            }
            /*
             * Administrator chatbot.
             */
            else if (IsAdministrator)
            {
                int staffID = WebSession.StaffID;
                response = ChatbotService.ProcessAdministratorMessage(userMessage, staffID);
            }
            /*
             * Manager chatbot.
             */
            else if (IsManager)
            {
                int staffID = WebSession.StaffID;
                response = ChatbotService.ProcessManagerMessage(userMessage, staffID);
            }
            /*
             * Public chatbot.
             */
            else
            {
                response = ChatbotService.ProcessMessage(userMessage);
            }

            if (response != null)
            {
                AddAssistantMessage(response);
            }

            txtChatMessage.Text = string.Empty;
        }

        private string ValidateChatbotMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Please enter a message.";
            }

            if (message.Length > MaximumMessageLength)
            {
                return "Your message is too long. Please keep it under " + MaximumMessageLength + " characters.";
            }

            if (ContainsDangerousMarkup(message))
            {
                return "I can't process that message because it contains " + "unsupported markup or script content. " + "Please enter your question as normal text.";
            }

            if (ContainsControlCharacters(message))
            {
                return "Your message contains unsupported characters. " + "Please try entering the question again.";
            }

            return string.Empty;
        }

        private bool ContainsDangerousMarkup(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            string lowerMessage = message.ToLowerInvariant();
            /*
             * Script tags.
             */
            if (Regex.IsMatch(lowerMessage, @"<\s*script\b", RegexOptions.IgnoreCase))
            {
                return true;
            }

            if (Regex.IsMatch(lowerMessage, @"<\s*/\s*script\s*>", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*
             * HTML event handlers.
             */
            if (Regex.IsMatch(lowerMessage, @"<[^>]*\bon[a-z]+\s*=", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*
             * JavaScript and VBScript URI schemes.
             */
            if (Regex.IsMatch(lowerMessage, @"\bjavascript\s*:", RegexOptions.IgnoreCase))
            {
                return true;
            }

            if (Regex.IsMatch(lowerMessage, @"\bvbscript\s*:", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*
             * Dangerous data URLs.
             */
            if (Regex.IsMatch(lowerMessage, @"\bdata\s*:\s*(text\/html|application\/javascript|application\/x-javascript)", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*
             * Dangerous executable HTML elements.
             */
            if (Regex.IsMatch(lowerMessage, @"<\s*(iframe|object|embed|applet|base|form|meta|link)\b", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*
             * CSS expression().
             */
            if (Regex.IsMatch(lowerMessage, @"expression\s*\(", RegexOptions.IgnoreCase))
            {
                return true;
            }

            /*
             * Common encoded script patterns.
             */
            if (lowerMessage.Contains("&lt;script") || lowerMessage.Contains("&#60;script") || lowerMessage.Contains("&#x3c;script"))
            {
                return true;
            }

            return false;
        }

        private bool ContainsControlCharacters(string message)
        {
            foreach (char character in message)
            {
                /*
                 * Allow normal characters and tabs.
                 * Reject other ASCII control characters.
                 */
                if (character < 32 && character != '\t')
                {
                    return true;
                }
            }

            return false;
        }

        private void AddUserMessage(string message)
        {
            AddMessageToHistory("User", HttpUtility.HtmlEncode(message), string.Empty, string.Empty, false);
        }

        private void AddAssistantMessage(ChatResponse response)
        {
            if (response == null)
            {
                return;
            }

            AddMessageToHistory("Assistant", response.Message, response.ActionText, response.ActionUrl, true);
        }

        private void AddMessageToHistory(string sender, string message, string actionText, string actionUrl, bool allowTrustedHtml)
        {
            string history = GetChatHistory();
            string safeMessage;
            if (allowTrustedHtml)
            {
                /*
                 * ChatbotService creates the assistant HTML.
                 *
                 * Database values used inside the service responses
                 * must be HTML-encoded by ChatbotService before being
                 * inserted into the response.
                 */
                safeMessage = message ?? string.Empty;
            }
            else
            {
                /*
                 * User messages must always be encoded.
                 */
                safeMessage = HttpUtility.HtmlEncode(message ?? string.Empty);
                safeMessage = safeMessage.Replace(Environment.NewLine, "<br />");
                safeMessage = safeMessage.Replace("\n", "<br />");
            }

            string messageClass = "tw-chatbot-message-assistant";
            if (string.Equals(sender, "User", StringComparison.OrdinalIgnoreCase))
            {
                messageClass = "tw-chatbot-message-user";
            }

            string actionHtml = string.Empty;
            if (!string.IsNullOrWhiteSpace(actionText) && !string.IsNullOrWhiteSpace(actionUrl))
            {
                string safeActionText = HttpUtility.HtmlEncode(actionText);
                string resolvedActionUrl = ResolveUrl(actionUrl);
                string safeActionUrl = HttpUtility.HtmlAttributeEncode(resolvedActionUrl);
                actionHtml = "<a href=\"" + safeActionUrl + "\" class=\"tw-chatbot-action\">" + safeActionText + "</a>";
            }

            string html = "<div class=\"tw-chatbot-message " + messageClass + "\">" + "<div class=\"tw-chatbot-message-content\">" + safeMessage + actionHtml + "</div>" + "</div>";
            history += html;
            /*
             * Prevent ViewState from growing indefinitely.
             */
            if (history.Length > MaximumHistoryLength)
            {
                history = TrimChatHistory(history);
            }

            ViewState[ChatHistoryViewStateKey] = history;
            LoadChatHistory();
        }

        private string TrimChatHistory(string history)
        {
            if (string.IsNullOrWhiteSpace(history))
            {
                return string.Empty;
            }

            if (history.Length <= MaximumHistoryLength)
            {
                return history;
            }

            string trimmedHistory = history.Substring(history.Length - MaximumHistoryLength);
            int firstMessageStart = trimmedHistory.IndexOf("<div class=\"tw-chatbot-message");
            if (firstMessageStart > 0)
            {
                trimmedHistory = trimmedHistory.Substring(firstMessageStart);
            }

            return trimmedHistory;
        }

        private string GetChatHistory()
        {
            object history = ViewState[ChatHistoryViewStateKey];
            if (history == null)
            {
                return string.Empty;
            }

            return Convert.ToString(history);
        }

        private void LoadChatHistory()
        {
            string history = GetChatHistory();
            if (string.IsNullOrWhiteSpace(history))
            {
                chatbotMessages.InnerHtml = string.Empty;
                return;
            }

            chatbotMessages.InnerHtml = history;
        }
    }
}
