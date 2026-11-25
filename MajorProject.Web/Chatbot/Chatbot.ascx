<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Chatbot.ascx.cs" Inherits="MajorProject.Web.Chatbot.Chatbot" %>

<div id="thyWillChatbot"
     class="tw-chatbot">

    <asp:HiddenField
        ID="hfChatbotOpen"
        runat="server"
        Value="false" />

    <!-- ===================================================== -->
    <!-- CHATBOT TOGGLE                                        -->
    <!-- ===================================================== -->

    <button type="button"
            id="btnOpenChatbot"
            runat="server"
            class="tw-chatbot-toggle"
            aria-expanded="false"
            aria-controls="chatbotPanel">

        <span class="tw-chatbot-toggle-icon">
            <i class="bi bi-chat-dots"></i>
        </span>

        <span class="tw-chatbot-toggle-text">
            Ask THY WILL
        </span>

    </button>


    <!-- ===================================================== -->
    <!-- CHATBOT PANEL                                         -->
    <!-- ===================================================== -->

    <section id="chatbotPanel"
             runat="server"
             class="tw-chatbot-panel"
             aria-label="THY WILL Assistant"
             aria-hidden="true">

        <header class="tw-chatbot-header">

            <div class="tw-chatbot-header-content">

                <div class="tw-chatbot-brand-icon">
                    <i class="bi bi-stars"></i>
                </div>

                <div>

                    <h2 class="tw-chatbot-title">
                        THY WILL Assistant
                    </h2>

                    <p class="tw-chatbot-status">
                        Electrical support
                    </p>

                </div>

            </div>

            <button type="button"
                    id="btnCloseChatbot"
                    runat="server"
                    class="tw-chatbot-close"
                    aria-label="Close chatbot">

                <i class="bi bi-x-lg"></i>

            </button>

        </header>


        <!-- ================================================= -->
        <!-- MESSAGES                                           -->
        <!-- ================================================= -->

        <div id="chatbotMessages"
             runat="server"
             class="tw-chatbot-messages">

            <div class="tw-chatbot-message tw-chatbot-message-assistant">

                <div class="tw-chatbot-message-content">

                    Hello 👋
                    <br />
                    How can I help you today?

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- QUICK ACTIONS                                      -->
        <!-- ================================================= -->

        <div class="tw-chatbot-quick-actions">

            <!-- Emergency -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="I have an electrical emergency">

                <i class="bi bi-exclamation-triangle"></i>
                Emergency Help

            </button>


            <% if (MajorProject.Web.Helpers.WebSession.HasRole("Customer")) { %>
            <button type="button" class="tw-chatbot-quick-action" data-chat-message="How do I change my booking?">Booking changes &amp; fees</button>
            <!-- Customer jobs -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="Show me my jobs">

                <i class="bi bi-briefcase"></i>
                My Jobs

            </button>


            <!-- Customer invoices -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="Show me my invoices">

                <i class="bi bi-receipt"></i>
                My Invoices

            </button>


            <% } %>
            <!-- Services -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="What services do you offer?">

                <i class="bi bi-lightning-charge"></i>
                Our Services

            </button>


            <!-- Service areas -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="What areas do you service?">

                <i class="bi bi-geo-alt"></i>
                Service Areas

            </button>


            <!-- Booking -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="How do I book a service?">

                <i class="bi bi-calendar-check"></i>
                How to Book

            </button>


            <!-- Contact -->

            <button type="button"
                    class="tw-chatbot-quick-action"
                    data-chat-message="How can I contact THY WILL?">

                <i class="bi bi-telephone"></i>
                Contact Us

            </button>

        </div>


        <!-- ================================================= -->
        <!-- INPUT                                              -->
        <!-- ================================================= -->

        <div class="tw-chatbot-input-area">

            <asp:TextBox
                ID="txtChatMessage"
                runat="server"
                CssClass="tw-chatbot-input"
                placeholder="Ask THY WILL..."
                MaxLength="500"
                TextMode="SingleLine"
                ValidateRequestMode="Disabled" />

            <asp:Button
                ID="btnSendMessage"
                runat="server"
                Text="Send"
                CssClass="tw-chatbot-send"
                OnClick="btnSendMessage_Click"
                CausesValidation="false" />

        </div>

    </section>

</div>


<script type="text/javascript">

    (function () {

        var chatbot =
            document.getElementById("thyWillChatbot");

        if (!chatbot) {
            return;
        }

        var openButton =
            chatbot.querySelector(".tw-chatbot-toggle");

        var closeButton =
            chatbot.querySelector(".tw-chatbot-close");

        var panel =
            chatbot.querySelector(".tw-chatbot-panel");

        var input =
            chatbot.querySelector(".tw-chatbot-input");

        var quickActions =
            chatbot.querySelectorAll(".tw-chatbot-quick-action");

        var hiddenField =
            chatbot.querySelector(
                "input[type='hidden']");


        function openChatbot() {

            chatbot.classList.add("tw-chatbot-open");

            if (panel) {
                panel.setAttribute(
                    "aria-hidden",
                    "false");
            }

            if (openButton) {
                openButton.setAttribute(
                    "aria-expanded",
                    "true");
            }

            if (hiddenField) {
                hiddenField.value = "true";
            }

            if (input) {

                setTimeout(function () {

                    input.focus();

                }, 100);

            }
        }


        function closeChatbot() {

            chatbot.classList.remove("tw-chatbot-open");

            if (panel) {
                panel.setAttribute(
                    "aria-hidden",
                    "true");
            }

            if (openButton) {
                openButton.setAttribute(
                    "aria-expanded",
                    "false");
            }

            if (hiddenField) {
                hiddenField.value = "false";
            }

            if (openButton) {
                openButton.focus();
            }
        }


        /*
         * Restore the chatbot state after an ASP.NET
         * Web Forms postback.
         */
        if (hiddenField &&
            hiddenField.value === "true") {

            openChatbot();

        }


        if (openButton) {

            openButton.addEventListener(
                "click",
                function () {

                    openChatbot();

                });

        }


        if (closeButton) {

            closeButton.addEventListener(
                "click",
                function () {

                    closeChatbot();

                });

        }


        for (var i = 0;
            i < quickActions.length;
            i++) {

            quickActions[i].addEventListener(
                "click",
                function () {

                    var message =
                        this.getAttribute(
                            "data-chat-message");

                    if (!message || !input) {
                        return;
                    }

                    input.value = message;
                    var send = chatbot.querySelector(".tw-chatbot-send");
                    if (send) send.click();

                });

        }


        document.addEventListener(
            "keydown",
            function (event) {

                if (event.key === "Escape" &&
                    chatbot.classList.contains(
                        "tw-chatbot-open")) {

                    closeChatbot();

                }

            });

    })();

</script>