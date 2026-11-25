var paymentReturnFocus = null;
var paymentBodyOverflow = "";
function mountPaymentModals() {
    var host = document.getElementById("paymentDialogHost");
    if (!host) {
        host = document.createElement("div");
        host.id = "paymentDialogHost";
        host.className = "tw-page-secure-customer-payinvoice tw-payment-host";
        // Keep controls within the ASP.NET server form for postback, outside transformed page stages.
        var firstModal = document.querySelector(".payment-modal-overlay");
        if (!firstModal) return;
        (firstModal.closest("form") || document.body).appendChild(host);
    }
    document.querySelectorAll(".payment-modal-overlay").forEach(function(modal) { host.appendChild(modal); });
}
function openPaymentModal(modalId, cardTab) {
    mountPaymentModals();
    var modal = document.getElementById(modalId);
    if (!modal) return;
    var previous = document.querySelector('.payment-modal-overlay[aria-hidden="false"]');
    if (!previous) {
        paymentReturnFocus = document.activeElement;
        paymentBodyOverflow = document.body.style.overflow;
    } else if (previous !== modal) closePaymentModal(previous.id, false);
    modal.setAttribute("aria-hidden", "false");
    document.body.style.overflow = "hidden";
    if (modalId === "cardModal") {
        initializeCardCheckout();
        if (cardTab) selectCardTab(cardTab);
    }
    modal.querySelector('[role="dialog"]').focus();
}
function clearPaymentErrors(modal) {
    if (!modal) return;
    modal.querySelectorAll("[data-payment-error]").forEach(function(error) { error.hidden = true; error.textContent = ""; });
    modal.querySelectorAll(".checkout-field-error").forEach(function(error) {
        var field = document.getElementById(error.getAttribute("data-field-id"));
        if (field) {
            field.removeAttribute("aria-invalid");
            var ids = (field.getAttribute("aria-describedby") || "").split(" ").filter(function(id) { return id && id !== error.id; });
            if (ids.length) field.setAttribute("aria-describedby", ids.join(" ")); else field.removeAttribute("aria-describedby");
        }
        error.remove();
    });
}
function closePaymentModal(modalId, restoreFocus) {
    var modal = document.getElementById(modalId);
    if (!modal) return;
    modal.setAttribute("aria-hidden", "true");
    modal.querySelectorAll('[id$="CardCvv"],#txtCardNumber').forEach(function(field) { field.value = ""; });
    modal.querySelectorAll(".checkout-cvv-control input").forEach(function(field) { field.type = "password"; });
    modal.querySelectorAll(".checkout-cvv-toggle").forEach(function(button) { button.textContent = "Show"; button.setAttribute("aria-pressed", "false"); button.setAttribute("aria-label", "Show security code"); });
    clearPaymentErrors(modal);
    updateNewCardPreview();
    if (restoreFocus !== false) {
        document.body.style.overflow = paymentBodyOverflow;
        if (paymentReturnFocus && paymentReturnFocus.isConnected) paymentReturnFocus.focus();
    }
}
function togglePaymentCvv(id, button) {
    var field = document.getElementById(id), show = field.type === "password";
    field.type = show ? "text" : "password";
    button.textContent = show ? "Hide" : "Show";
    button.setAttribute("aria-pressed", String(show));
    button.setAttribute("aria-label", show ? "Hide security code" : "Show security code");
    field.focus();
}
function paymentLuhn(value) {
    if (!/^[0-9]{13,19}$/.test(value) || /^([0-9])\1+$/.test(value)) return false;
    var sum = 0, doubleDigit = false;
    for (var i = value.length - 1; i >= 0; i--) {
        var n = Number(value.charAt(i));
        if (doubleDigit) { n *= 2; if (n > 9) n -= 9; }
        sum += n; doubleDigit = !doubleDigit;
    }
    return sum % 10 === 0;
}
function paymentNameValid(value) { return /^[\p{L}\p{M} '.-]{2,100}$/u.test(value) && /\p{L}/u.test(value); }
function validatePaymentCheckout(mode) {
    var modal = document.getElementById(mode === "eft" ? "eftModal" : mode === "wallet" ? "walletModal" : "cardModal");
    clearPaymentErrors(modal);
    var errors = [];
    function value(id) { return document.getElementById(id).value.trim(); }
    function requireField(id, condition, message) {
        if (condition) return;
        var field = document.getElementById(id), error = document.createElement("span");
        error.id = id + "Error"; error.className = "checkout-field-error"; error.textContent = message;
        error.setAttribute("data-field-id", id);
        field.setAttribute("aria-invalid", "true");
        field.setAttribute("aria-describedby", ((field.getAttribute("aria-describedby") || "") + " " + error.id).trim());
        (field.closest(".payment-form-group") || field.parentElement).appendChild(error);
        errors.push({field:field,message:message});
    }
    if (mode === "new") {
        requireField("txtCardholderName", paymentNameValid(value("txtCardholderName")), "Enter the name on your card (2 to 100 characters).");
        var raw = value("txtCardNumber");
        requireField("txtCardNumber", /^[0-9 -]{13,25}$/.test(raw) && paymentLuhn(raw.replace(/[ -]/g,"")), "Enter a valid card number.");
        var month = Number(value("ddlExpiryMonth")), year = value("txtExpiryYear"), now = new Date();
        requireField("ddlExpiryMonth", month >= 1 && month <= 12, "Select an expiry month.");
        requireField("txtExpiryYear", /^[0-9]{4}$/.test(year) && Number(year) >= 2000 && (Number(year) > now.getFullYear() || (Number(year) === now.getFullYear() && month >= now.getMonth()+1)), "Enter a valid, unexpired four-digit year.");
        requireField("txtNewCardCvv", /^[0-9]{3,4}$/.test(value("txtNewCardCvv")), "Enter a 3 or 4 digit security code.");
    } else if (mode === "saved") {
        requireField("ddlSavedCards", !!value("ddlSavedCards"), "Select a saved card.");
        requireField("txtSavedCardCvv", /^[0-9]{3,4}$/.test(value("txtSavedCardCvv")), "Enter a 3 or 4 digit security code.");
    } else if (mode === "eft") {
        requireField("ddlEFTBank", ["ABSA","FNB","NEDBANK","STANDARD","CAPITEC","OTHER"].indexOf(value("ddlEFTBank")) >= 0, "Select your bank.");
        requireField("txtEFTPayerName", paymentNameValid(value("txtEFTPayerName")), "Enter a valid payer name.");
    } else if (mode === "wallet") {
        requireField("ddlWalletProvider", ["MobileWallet","DemoWallet"].indexOf(value("ddlWalletProvider")) >= 0, "Select a wallet provider.");
        requireField("txtWalletMobile", /^0[6-8][0-9]{8}$/.test(value("txtWalletMobile")), "Enter a ten-digit South African mobile number.");
    } else return false;
    if (errors.length) {
        var summary = modal.querySelector("[data-payment-error]");
        summary.textContent = "Please check " + errors.length + (errors.length === 1 ? " field below." : " fields below.");
        summary.hidden = false;
        // The native saved-card selector is visually hidden. Focus its visible card choice instead.
        var field = errors[0].field;
        if (field.id === "ddlSavedCards") field = modal.querySelector(".saved-card-option") || summary;
        field.focus();
        return false;
    }
    // Suppress repeated submits without disabling the WebForms event-source input.
    if (modal.dataset.submitting === "true") return false;
    modal.dataset.submitting = "true";
    window.setTimeout(function() { delete modal.dataset.submitting; }, 10000);
    return true;
}
function restorePaymentError(modalId, tabName, message) {
    openPaymentModal(modalId, tabName);
    var error = document.getElementById(modalId).querySelector("[data-payment-error]");
    error.textContent = message; error.hidden = false; error.focus();
}
window.addEventListener("click", function(event) {
    if (event.target.classList.contains("payment-modal-overlay")) closePaymentModal(event.target.id);
});
window.addEventListener("keydown", function(event) {
    var modal = document.querySelector('.payment-modal-overlay[aria-hidden="false"]');
    if (!modal) return;
    if (event.key === "Escape") { event.preventDefault(); closePaymentModal(modal.id); return; }
    if (event.target.matches && event.target.matches('[role="tab"]') && ["ArrowLeft","ArrowRight","Home","End"].indexOf(event.key) >= 0) {
        event.preventDefault();
        var tabs = Array.prototype.filter.call(modal.querySelectorAll('[role="tab"]'), function(tab) { return tab.style.display !== "none"; });
        var current = tabs.indexOf(event.target), index = event.key === "Home" ? 0 : event.key === "End" ? tabs.length-1 : (current + (event.key === "ArrowLeft" ? -1 : 1) + tabs.length) % tabs.length;
        tabs[index].click(); tabs[index].focus(); return;
    }
    if (event.key !== "Tab") return;
    var nodes = Array.prototype.filter.call(modal.querySelectorAll('button,input,select,a[href],[tabindex="0"]'), function(node) { return !node.disabled && node.type !== "hidden" && node.getClientRects().length > 0; });
    var first = nodes[0], last = nodes[nodes.length-1];
    if (!first) return;
    if (event.shiftKey && (document.activeElement === first || !nodes.includes(document.activeElement))) { event.preventDefault(); last.focus(); }
    else if (!event.shiftKey && (document.activeElement === last || !nodes.includes(document.activeElement))) { event.preventDefault(); first.focus(); }
});

    function selectCardTab(tabName) {
        var savedTab = document.getElementById("savedCardTab");
        var newTab = document.getElementById("newCardTab");

        var savedPanel = document.getElementById("savedCardPanel");
        var newPanel = document.getElementById("newCardPanel");

        if (!savedTab || !newTab || !savedPanel || !newPanel) {
            return;
        }

        var showSaved = tabName === "saved";
        clearPaymentErrors(document.getElementById("cardModal"));

        savedTab.classList.toggle("active", showSaved);
        newTab.classList.toggle("active", !showSaved);

        savedTab.setAttribute("aria-selected", showSaved ? "true" : "false");
        newTab.setAttribute("aria-selected", showSaved ? "false" : "true");

        savedTab.tabIndex = showSaved ? 0 : -1;
        newTab.tabIndex = showSaved ? -1 : 0;
        savedPanel.hidden = !showSaved;
        newPanel.hidden = showSaved;
        savedPanel.classList.toggle("active", showSaved);
        newPanel.classList.toggle("active", !showSaved);
    }

    function getSavedCardSelect() {
        return document.getElementById(
            "ddlSavedCards"
        );
    }

    function getSavedCardOptionDetails(option) {
        return {
            text: option ? option.text : "",
            lastFour: option ? option.getAttribute("data-last-four") || "----" : "----",
            brand: option ? option.getAttribute("data-brand") || "CARD" : "CARD",
            expiry: option ? option.getAttribute("data-expiry") || "--/--" : "--/--"
        };
    }

    function renderSavedCards() {
        var select = getSavedCardSelect();
        var list = document.getElementById("savedCardList");

        if (!select || !list) return;

        list.innerHTML = "";

        var availableOptions = [];

        for (var i = 0; i < select.options.length; i++) {
            var option = select.options[i];

            if (option.value !== "") {
                availableOptions.push(option);
            }
        }

        if (availableOptions.length === 0) {
            list.innerHTML =
                '<div class="saved-card-empty">' +
                '<span>No saved cards are currently available. ' +
                'Choose Add New Card to continue.</span></div>';
            return;
        }

        availableOptions.forEach(function (option) {
            var details = getSavedCardOptionDetails(option);

            var button = document.createElement("button");
            button.type = "button";
            button.className = "saved-card-option";

            button.setAttribute(
                "aria-pressed",
                option.value === select.value ? "true" : "false"
            );

            var brand = document.createElement("span");
            brand.className = "saved-card-brand";
            brand.textContent = details.brand;

            var info = document.createElement("span");
            info.className = "saved-card-option-info";

            var number = document.createElement("span");
            number.className = "saved-card-option-number";
            number.textContent = "•••• " + details.lastFour;

            var meta = document.createElement("span");
            meta.className = "saved-card-option-meta";
            meta.textContent = details.expiry
                ? "Expires " + details.expiry
                : "Saved payment card";

            var radio = document.createElement("span");
            radio.className = "saved-card-radio";

            info.appendChild(number);
            info.appendChild(meta);

            button.appendChild(brand);
            button.appendChild(info);
            button.appendChild(radio);

            if (option.value === select.value) {
                button.classList.add("selected");
            }

            button.addEventListener("click", function () {
                select.value = option.value;
                updateSavedCardDisplay();
                renderSavedCards();
            });

            list.appendChild(button);
        });
    }

    function updateSavedCardDisplay() {
        var select = getSavedCardSelect();

        if (!select) return;

        var option = select.options[select.selectedIndex];

        if (!option || option.value === "") return;

        var details = getSavedCardOptionDetails(option);

        var brandElement = document.getElementById("savedCardBrand");
        var numberElement = document.getElementById("savedCardNumber");
        var expiryElement = document.getElementById("savedCardExpiry");

        if (brandElement) {
            brandElement.textContent = details.brand;
        }

        if (numberElement) {
            numberElement.textContent =
                "••••  ••••  ••••  " + details.lastFour;
        }

        if (expiryElement) {
            expiryElement.textContent = details.expiry || "--/--";
        }
    }

    function updateNewCardPreview() {
        var nameInput = document.getElementById(
            "txtCardholderName"
        );

        var numberInput = document.getElementById(
            "txtCardNumber"
        );

        var monthInput = document.getElementById(
            "ddlExpiryMonth"
        );

        var yearInput = document.getElementById(
            "txtExpiryYear"
        );

        var namePreview = document.getElementById("newCardHolderPreview");
        var numberPreview = document.getElementById("newCardNumberPreview");
        var expiryPreview = document.getElementById("newCardExpiryPreview");
        var brandPreview = document.getElementById("newCardBrandPreview");

        if (!nameInput || !numberInput) return;

        var holder = nameInput.value.trim();

        if (namePreview) {
            namePreview.textContent = holder || "CARDHOLDER NAME";
        }

        var digits = numberInput.value.replace(/\D/g, "");

        var lastFour = digits.length >= 4
            ? digits.slice(-4)
            : "••••";

        if (numberPreview) {
            numberPreview.textContent =
                "••••  ••••  ••••  " + lastFour;
        }

        if (brandPreview) {
            if (digits.charAt(0) === "4") {
                brandPreview.textContent = "VISA";
            }
            else if (/^5[1-5]/.test(digits) ||
                     /^2[2-7]/.test(digits)) {
                brandPreview.textContent = "MASTERCARD";
            }
            else {
                brandPreview.textContent = "CARD";
            }
        }

        var month = monthInput ? monthInput.value : "";
        var year = yearInput ? yearInput.value : "";

        if (expiryPreview) {
            expiryPreview.textContent =
                month && year
                    ? String(month).padStart(2, "0") +
                      "/" + year.slice(-2)
                    : "--/--";
        }
    }

    function initializeCardCheckout() {
        var select = getSavedCardSelect();
        var savedTabButton = document.getElementById("savedCardTab");
        var savedTabPanel = document.getElementById("savedCardPanel");

        var hasSavedCards = false;

        if (select) {
            for (var i = 0; i < select.options.length; i++) {
                if (select.options[i].value !== "") {
                    hasSavedCards = true;
                    break;
                }
            }
        }

        if (!hasSavedCards) {
            if (savedTabButton) {
                savedTabButton.style.display = "none";
            }

            if (savedTabPanel) {
                savedTabPanel.style.display = "none";
            }

            selectCardTab("new");
        }
        else {
            if (savedTabButton) {
                savedTabButton.style.display = "";
            }

            if (savedTabPanel) {
                savedTabPanel.style.display = "";
            }

            renderSavedCards();
            updateSavedCardDisplay();
            selectCardTab("saved");
        }

        updateNewCardPreview();
    }

    document.addEventListener("DOMContentLoaded", function () {
        mountPaymentModals();
        initializeCardCheckout();

        var nameInput = document.getElementById(
            "txtCardholderName"
        );

        var numberInput = document.getElementById(
            "txtCardNumber"
        );

        var monthInput = document.getElementById(
            "ddlExpiryMonth"
        );

        var yearInput = document.getElementById(
            "txtExpiryYear"
        );

        if (nameInput) {
            nameInput.addEventListener("input", updateNewCardPreview);
        }

        if (numberInput) {
            numberInput.addEventListener("input", updateNewCardPreview);
        }

        if (monthInput) {
            monthInput.addEventListener("change", updateNewCardPreview);
        }

        if (yearInput) {
            yearInput.addEventListener("input", updateNewCardPreview);
        }
    });

    function copyPaymentReference() {
        var referenceElement = document.getElementById(
            "lblPaymentReference"
        );

        if (!referenceElement) return;

        var reference = referenceElement.textContent.trim();

        if (!reference) return;

        if (navigator.clipboard && window.isSecureContext) {
            navigator.clipboard.writeText(reference)
                .then(function () {
                    showCopyFeedback();
                })
                .catch(function () {
                    fallbackCopyReference(reference);
                });
        }
        else {
            fallbackCopyReference(reference);
        }
    }

    function fallbackCopyReference(reference) {
        var temporaryInput = document.createElement("textarea");

        temporaryInput.value = reference;
        temporaryInput.style.position = "fixed";
        temporaryInput.style.opacity = "0";

        document.body.appendChild(temporaryInput);
        temporaryInput.select();

        try {
            document.execCommand("copy");
            showCopyFeedback();
        }
        catch (error) {
            alert("Unable to copy the payment reference. Please select it manually.");
        }

        document.body.removeChild(temporaryInput);
    }

    function showCopyFeedback() {
        var button = document.getElementById("copyPaymentReferenceButton");

        if (!button) return;

        var originalContent = button.innerHTML;

        button.innerHTML = '<i class="bi bi-check-lg"></i>';
        button.setAttribute("aria-label", "Payment reference copied");

        setTimeout(function () {
            button.innerHTML = originalContent;
            button.setAttribute("aria-label", "Copy payment reference");
        }, 1800);
    }
