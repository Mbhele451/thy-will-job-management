/* THY WILL shared UI: theme, navigation, record tabs and Web Forms lifecycle. */
"use strict";
(function (window, document) {
    var key = "thywill-theme", recordStates = new WeakMap(), uiStates = new WeakMap();
    var revealObserver, feedbackQueued = false, requestManagerBound = false;
    var sun = '<svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="1.7" aria-hidden="true"><circle cx="12" cy="12" r="4"/><path d="M12 2v2m0 16v2M2 12h2m16 0h2M5 5l1.5 1.5m11 11L19 19M5 19l1.5-1.5m11-11L19 5"/></svg>';
    var moon = '<svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="1.7" aria-hidden="true"><path d="M20.5 14.2A8.5 8.5 0 0 1 9.8 3.5a8.5 8.5 0 1 0 10.7 10.7Z"/></svg>';
    function readStorage(storage, name) { try { return window[storage].getItem(name); } catch (_) { return null; } }
    function writeStorage(storage, name, value) { try { if (value === null) window[storage].removeItem(name); else window[storage].setItem(name, value); } catch (_) {} }
    function validTheme(value) { return value === "light" || value === "dark"; }
    function cookieTheme() { try { var match = document.cookie.match(/(?:^|;\s*)thywill-theme=(light|dark)(?:;|$)/); return match ? match[1] : null; } catch (_) { return null; } }
    function applyTheme(value, persist) {
        var theme = validTheme(value) ? value : "dark";
        document.documentElement.setAttribute("data-theme", theme);
        document.documentElement.setAttribute("data-bs-theme", theme);
        document.documentElement.style.colorScheme = theme;
        document.querySelectorAll("[data-theme-toggle]").forEach(function (button) {
            button.type = "button";
            var label = theme === "dark" ? "Light mode" : "Dark mode";
            button.innerHTML = (theme === "dark" ? sun : moon) + '<span class="tw-theme-label">' + label + '</span>';
            button.setAttribute("aria-label", "Switch to " + label.toLowerCase());
            button.setAttribute("aria-pressed", theme === "dark" ? "true" : "false");
            button.title = "Switch to " + label.toLowerCase();
        });
        var meta = document.querySelector('meta[name="theme-color"]');
        if (meta && document.body) meta.content = window.getComputedStyle(document.documentElement).getPropertyValue("--color-background").trim();
        if (persist) {
            writeStorage("localStorage", key, theme);
            try { document.cookie = key + "=" + theme + "; Max-Age=31536000; Path=/; SameSite=Lax" + (window.location.protocol === "https:" ? "; Secure" : ""); } catch (_) {}
        }
        return theme;
    }
    var storedTheme = readStorage("localStorage", key);
    applyTheme(validTheme(storedTheme) ? storedTheme : cookieTheme() || "dark", false);
    window.applyThyWillTheme = function (value) { return applyTheme(value, false); };

    function byServerID(root, id) {
        if (!id) return null;
        var exact = document.getElementById(id);
        if (exact && (root === document || root.contains(exact))) return exact;
        return Array.prototype.find.call(root.querySelectorAll("[id]"), function (node) { return node.id.slice(-(id.length + 1)) === "_" + id; }) || null;
    }
    function textValue(node) { return node ? String("value" in node ? node.value : node.textContent).trim() : ""; }
    function focusNode(node) { if (!node) return; if (!node.matches("a[href],button,input,select,textarea,[tabindex]")) node.tabIndex = -1; try { node.focus({ preventScroll: true }); } catch (_) { node.focus(); } }
    function storageName(root, suffix) { return "thywill:tabs:" + window.location.pathname.toLowerCase() + ":" + (root.getAttribute("data-record-tabs") || root.getAttribute("data-ui-tabs")) + ":" + suffix; }
    function paneReady(root, pane) {
        var id = pane.getAttribute("data-ready-id"), kind = pane.getAttribute("data-ready-kind");
        if (!id) return true;
        var probe = byServerID(root, id), value = textValue(probe);
        return kind === "exists" ? !!probe : kind === "positive" ? /^\d+$/.test(value) && Number(value) > 0 : value.length > 0;
    }
    function owned(root, selector, boundary) { return Array.prototype.filter.call(root.querySelectorAll(selector), function (node) { return node.closest(boundary) === root; }); }
    function showState(state, wanted, focus) {
        var enabled = state.keys.filter(function (name) { return !state.buttons[name].disabled; });
        var active = enabled.indexOf(wanted) >= 0 ? wanted : enabled[0];
        if (!active) return;
        state.active = active;
        state.keys.forEach(function (name) {
            var selected = name === active, button = state.buttons[name];
            button.classList.toggle("active", selected);
            button.setAttribute("aria-selected", selected ? "true" : "false");
            button.tabIndex = selected ? 0 : -1;
            (state.panes[name] || []).forEach(function (pane) { pane.hidden = !selected; pane.classList.toggle("active", selected); });
        });
        // Old split layouts become a single content area, while nested editors remain intact.
        state.root.querySelectorAll(".manager-inventory-workspace").forEach(function (layout) {
            var panes = Array.prototype.filter.call(layout.children, function (child) { return child.hasAttribute("data-tab-pane"); });
            if (panes.length) layout.hidden = panes.every(function (pane) { return pane.hidden; });
        });
        writeStorage("sessionStorage", storageName(state.root, "active"), active);
        if (focus) { focusNode(state.buttons[active]); if (state.bar.scrollIntoView) state.bar.scrollIntoView({ block: "nearest", behavior: "auto" }); }
    }
    function initializeRecordTabs(root) {
        var labels = (root.getAttribute("data-tab-labels") || "").split(";").map(function (item) { var parts = item.split("="); return { key: parts.shift(), label: parts.join("=") }; }).filter(function (item) { return item.key && item.label; });
        if (!labels.length) return;
        var panes = owned(root, "[data-tab-pane]", "[data-record-tabs]"), state = recordStates.get(root);
        if (!state) {
            var bar = document.createElement("div"); bar.className = "tw-record-tabs"; bar.setAttribute("role", "tablist"); bar.setAttribute("aria-label", "Record views");
            var first = panes[0]; while (first && first.parentElement !== root) first = first.parentElement;
            root.insertBefore(bar, first || root.firstChild);
            state = { root: root, bar: bar, keys: [], buttons: {}, panes: {} };
            labels.forEach(function (item) {
                var button = document.createElement("button"); button.type = "button"; button.className = "tw-record-tab"; button.textContent = item.label;
                button.setAttribute("role", "tab"); button.setAttribute("data-record-tab", item.key); button.id = "tw-tab-" + root.getAttribute("data-record-tabs") + "-" + item.key;
                bar.appendChild(button); state.keys.push(item.key); state.buttons[item.key] = button;
            });
            recordStates.set(root, state);
        }
        state.panes = {};
        panes.forEach(function (pane, index) {
            var name = pane.getAttribute("data-tab-pane"); if (!state.panes[name]) state.panes[name] = [];
            state.panes[name].push(pane);
            if (!pane.id) pane.id = "tw-pane-" + root.getAttribute("data-record-tabs") + "-" + index;
            pane.setAttribute("role", "tabpanel");
            if (state.buttons[name]) pane.setAttribute("aria-labelledby", state.buttons[name].id);
        });
        state.keys.forEach(function (name) {
            var matches = state.panes[name] || [], ready = matches.length > 0 && matches.every(function (pane) { return paneReady(root, pane); });
            state.buttons[name].disabled = !ready;
            state.buttons[name].setAttribute("aria-disabled", ready ? "false" : "true");
            state.buttons[name].title = ready ? "" : "Choose View in the table to open this record.";
            state.buttons[name].setAttribute("aria-controls", matches.map(function (pane) { return pane.id; }).join(" "));
        });
        var pending = readStorage("sessionStorage", storageName(root, "pending"));
        var saved = readStorage("sessionStorage", storageName(root, "active"));
        var selected = byServerID(root, root.getAttribute("data-selected-id"));
        var selectedValue = textValue(selected), hasSelection = selected && selectedValue !== "" && selectedValue !== "—" && selectedValue !== "..." && selectedValue !== "0";
        var initial = pending || state.active || saved || (hasSelection && state.buttons.details && !state.buttons.details.disabled ? "details" : "list");
        root.classList.add("is-record-enhanced");
        if (!state.buttons[initial]) initial = "list";
        showState(state, initial, !!pending && !state.buttons[initial].disabled);
        writeStorage("sessionStorage", storageName(root, "pending"), null);
    }
    function initializeUITabs(root) {
        var buttons = owned(root, "[data-ui-tab]", "[data-ui-tabs]"), panes = owned(root, "[data-ui-pane]", "[data-ui-tabs]");
        var state = { root: root, bar: (buttons[0] && buttons[0].parentElement) || root, keys: [], buttons: {}, panes: {} };
        buttons.forEach(function (button) {
            var name = button.getAttribute("data-ui-tab"); state.keys.push(name); state.buttons[name] = button;
            button.setAttribute("role", "tab");
            if (!button.id) button.id = "tw-ui-" + root.getAttribute("data-ui-tabs") + "-" + name;
            button.style.removeProperty("display");
        });
        panes.forEach(function (pane, index) {
            var name = pane.getAttribute("data-ui-pane"); if (!state.panes[name]) state.panes[name] = [];state.panes[name].push(pane);
            if (!pane.id) pane.id = "tw-ui-pane-" + root.getAttribute("data-ui-tabs") + "-" + index;
            pane.setAttribute("role", "tabpanel"); if (state.buttons[name]) pane.setAttribute("aria-labelledby", state.buttons[name].id);
        });
        state.keys.forEach(function (name) { var matches = state.panes[name] || []; state.buttons[name].disabled = !matches.length; state.buttons[name].setAttribute("aria-controls", matches.map(function (pane) { return pane.id; }).join(" ")); });
        uiStates.set(root, state);
        var preferred = root.getAttribute("data-prefer-tab"), saved = readStorage("sessionStorage", storageName(root, "active"));
        showState(state, preferred && state.buttons[preferred] && !state.buttons[preferred].disabled ? preferred : saved || state.keys[0], false);
    }
    function stateFor(node) { var record = node.closest("[data-record-tabs]"); if (record) return recordStates.get(record); var simple = node.closest("[data-ui-tabs]"); return simple && uiStates.get(simple); }
    function activateTab(name, target) {
        var root = document.querySelector('[data-ui-tabs="' + name + '"]') || document.querySelector('[data-record-tabs="' + name + '"]');
        var state = root && (uiStates.get(root) || recordStates.get(root)); if (state) { showState(state, target, false); var pane=(state.panes[target]||[])[0];if(pane){focusNode(pane);if(pane.scrollIntoView)pane.scrollIntoView({block:"start",behavior:"auto"});} }
    }

    function setPublicMenu(open) {
        var menu = document.getElementById("twMobileMenu"), button = document.getElementById("twMobileMenuToggle"); if (!menu || !button) return;
        menu.classList.toggle("is-open", open); button.classList.toggle("is-open", open); button.setAttribute("aria-expanded", String(open)); menu.setAttribute("aria-hidden", String(!open));
        button.setAttribute("aria-label", open ? "Close navigation menu" : "Open navigation menu");
    }
    function setStaffMenu(open) {
        var menu = document.querySelector(".staff-navigation"), button = document.querySelector(".staff-nav-toggle"); if (!menu || !button) return;
        menu.classList.toggle("is-open", open); button.setAttribute("aria-expanded", String(open)); button.setAttribute("aria-controls", menu.id);
    }
    function setDrawer(open, restoreFocus) {
        var sidebar = document.querySelector(".customer-sidebar"), overlay = document.querySelector(".customer-sidebar-overlay"), button = document.querySelector(".customer-sidebar-toggle");
        if (!sidebar) return;
        var mobile = window.innerWidth <= 768; open = mobile && open;
        sidebar.classList.toggle("is-open", open); sidebar.inert = mobile && !open; sidebar.setAttribute("aria-hidden", String(mobile && !open));
        sidebar.setAttribute("aria-label", "Customer navigation");
        if (open) { sidebar.setAttribute("role", "dialog"); sidebar.setAttribute("aria-modal", "true"); } else { sidebar.removeAttribute("role"); sidebar.removeAttribute("aria-modal"); }
        if (overlay) overlay.classList.toggle("is-visible", open);
        if (button) { button.setAttribute("aria-expanded", String(open)); button.setAttribute("aria-controls", sidebar.id); }
        document.body.classList.toggle("sidebar-open", open);
        document.querySelectorAll(".dashboard-main, .staff-topbar, .customer-sidebar-toggle").forEach(function (element) { element.inert = open; });
        if (open) focusNode(sidebar.querySelector(".customer-sidebar-close")); else if (restoreFocus) focusNode(button);
    }
    function initializeNavigation() {
        var sidebar = document.querySelector(".customer-sidebar"), button = document.querySelector(".customer-sidebar-toggle");
        if (sidebar && !button) { button = document.createElement("button"); button.type = "button"; button.className = "customer-sidebar-toggle";button.setAttribute("aria-label", "Open customer navigation");button.innerHTML = '<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.7" aria-hidden="true"><path d="M4 6h16M4 12h16M4 18h16"/></svg>';document.body.appendChild(button); }
        if (sidebar) setDrawer(sidebar.classList.contains("is-open"), false);
        document.querySelectorAll(".staff-nav-link,.customer-nav-link,.tw-nav-link").forEach(function (link) {
            var href = link.getAttribute("href"); if (!href || href.charAt(0) === "#") return;
            try {
                var url = new URL(href, window.location.href), current = window.location.pathname.toLowerCase().replace(/\/(?:default)?(?:\.aspx)?$/, "/").replace(/\.aspx$/, "");
                var path = url.pathname.toLowerCase().replace(/\/(?:default)?(?:\.aspx)?$/, "/").replace(/\.aspx$/, "");
                if (/\/customer\/payinvoice$/.test(current)) current = current.replace(/payinvoice$/, "invoices");
                var active = !url.hash && current === path;link.classList.toggle("active", active);if (active) link.setAttribute("aria-current", "page");else link.removeAttribute("aria-current");
            } catch (_) {}
        });
    }
    function initializeIdentity() {
        document.querySelectorAll('[data-identity-choice]').forEach(function(root){
            var choice=root.querySelector('[data-nationality]'),type=byServerID(document,'ddlCustomerType');if(!choice||!type)return;
            var business=type.value==='Business',passport=choice.value==='Passport';root.hidden=business;
            var sa=byServerID(document,'txtSouthAfricanID'),number=byServerID(document,'txtPassportNumber'),country=byServerID(document,'txtPassportCountry');
            var group=byServerID(document,'residentialIdentityGroup')||document.getElementById('residentialIdentity');
            if(group){group.hidden=business||passport;group.style.display=business||passport?'none':'';}
            var fields=root.querySelector('[data-passport-fields]');if(fields)fields.hidden=business||!passport;
            [number,country].forEach(function(input){if(input)input.disabled=choice.disabled||business||!passport;});
            if(sa)sa.disabled=choice.disabled||business||passport;
            var company=byServerID(document,'txtCompanyRegistration'),businessGroup=byServerID(document,'businessIdentityGroup')||document.getElementById('businessIdentity');
            if(businessGroup){businessGroup.hidden=!business;businessGroup.style.display=business?'':'none';}
            if(company)company.disabled=choice.disabled||!business;
        });
    }
    function initializeTables() {
        document.querySelectorAll("main table,.dashboard-content table").forEach(function (table) {
            if (table.closest("td,th") || table.classList.contains("booking-cart-list")) return;
            var first = table.rows[0];
            if (first && first.querySelector("th") && first.parentElement.tagName !== "THEAD") {
                var head = table.tHead || table.createTHead(); head.appendChild(first);
            }
            var parent = table.parentElement;
            var outer = parent.closest('[class*="table-wrapper"],.table-responsive,.tw-table-scroll,.tw-data-region');
            // Use the original scrolling region. A second overflow container breaks sticky headers.
            if (outer && !outer.matches("main,.dashboard-content")) parent = outer;
            else if (!parent.matches(".tw-table-scroll,.table-responsive,.dashboard-table-wrapper,.invoice-table-wrapper")) {
                var wrapper = document.createElement("div"); wrapper.className = "tw-table-scroll";
                parent.insertBefore(wrapper,table); wrapper.appendChild(table); parent = wrapper;
            }
            parent.classList.add("tw-data-region");
            parent.tabIndex=0;parent.setAttribute("role","region");parent.setAttribute("aria-label","Data table. Scroll for more rows and columns. Headers stay visible.");
            table.querySelectorAll("thead th").forEach(function(th){th.setAttribute("scope","col");if(/\b(amount|total|price|quantity|balance|cost|hours|profit|stock)\b/i.test(th.textContent)){var index=th.cellIndex;Array.prototype.forEach.call(table.rows,function(row){if(row.cells[index]&&row.cells[index].colSpan===1)row.cells[index].classList.add("tw-numeric");});}});
        });
    }
    function initializePasswords() {
        document.querySelectorAll('input[type="password"]').forEach(function(input){
            if(input.dataset.visibilityReady || input.parentElement.querySelector('[onclick*="togglePassword"]'))return;
            input.dataset.visibilityReady="true";if(!input.id)return;
            var wrapper=document.createElement("div");wrapper.className="tw-password-control";input.parentNode.insertBefore(wrapper,input);wrapper.appendChild(input);
            var button=document.createElement("button");button.type="button";button.className="tw-password-toggle";button.setAttribute("data-password-target",input.id);button.setAttribute("aria-controls",input.id);button.setAttribute("aria-pressed","false");button.setAttribute("aria-label","Show password");button.textContent="Show";wrapper.appendChild(button);
        });
    }
    function initializeLabels() {
        document.querySelectorAll("label").forEach(function(label){
            var target=label.getAttribute("for");if(target&&document.getElementById(target))return;
            var input=target?byServerID(document,target):label.querySelector("input,select,textarea");
            if(!input){var field=label.closest(".dashboard-form-group,.booking-field,.col-md-6,.col-12,.form-group");if(field)input=field.querySelector("input:not([type=hidden]),select,textarea");}
            if(input&&input.id)label.htmlFor=input.id;
        });
    }
    function openContainingPane(node) {
        var pane=node.closest("[data-tab-pane],[data-ui-pane]");if(!pane)return;var state=stateFor(pane);if(state)showState(state,pane.getAttribute("data-tab-pane")||pane.getAttribute("data-ui-pane"),false);
    }
    function refreshFeedback(focusError) {
        document.querySelectorAll(".dashboard-alert,.booking-message,.staff-message,.dashboard-info-banner,.tw-auth-alert,.tw-booking-message,[id$='_lblMessage'],[id$='_lblError']").forEach(function(message){
            var empty=!message.textContent.trim();if(empty){message.hidden=true;message.dataset.emptyMessage="true";return;}
            if(message.dataset.emptyMessage){message.hidden=false;delete message.dataset.emptyMessage;}
            var error=/danger|error/.test(message.className);message.setAttribute("role",error?"alert":"status");message.setAttribute("aria-live",error?"assertive":"polite");message.classList.add("tw-feedback");if(error)message.setAttribute("data-ui-error","true");else message.removeAttribute("data-ui-error");
        });
        var invalid=[], fields=new Map();
        Array.prototype.forEach.call(window.Page_Validators||[],function(validator){
            var field=document.getElementById(validator.controltovalidate||"");validator.classList.add("tw-field-validation");
            var failed=validator.isvalid===false;if(failed)invalid.push(validator);
            if(field){var entry=fields.get(field)||{failed:false,ids:[],all:[]};entry.failed=entry.failed||failed;if(validator.id)entry.all.push(validator.id);if(failed&&validator.id)entry.ids.push(validator.id);fields.set(field,entry);}
        });
        fields.forEach(function(entry,field){field.setAttribute("aria-invalid",String(entry.failed));var described=(field.getAttribute("aria-describedby")||"").split(/\s+/).filter(function(id){return id&&entry.all.indexOf(id)<0;});described=described.concat(entry.ids);if(described.length)field.setAttribute("aria-describedby",described.join(" "));else field.removeAttribute("aria-describedby");});
        document.querySelectorAll(".booking-field-error,.profile-field-error").forEach(function(error){var style=window.getComputedStyle(error);if(error.textContent.trim()&&style.display!=="none"&&style.visibility!=="hidden"){error.setAttribute("role","alert");error.setAttribute("data-ui-error","true");}else error.removeAttribute("data-ui-error");});
        document.querySelectorAll("[id$='_vsBooking'],[id$='_vsContact'],.booking-validation-summary,.validation-summary-errors").forEach(function(summary){summary.classList.add("tw-validation-summary");summary.setAttribute("role","alert");});
        if(focusError){var validator=invalid[0],field=validator&&document.getElementById(validator.controltovalidate||"");var target=field||document.querySelector("[data-ui-error]");if(target){openContainingPane(target);focusNode(target);if(target.scrollIntoView)target.scrollIntoView({block:"center",behavior:"auto"});}resetBusy();}
    }
    function queueFeedback(focus) {if(feedbackQueued)return;feedbackQueued=true;window.setTimeout(function(){feedbackQueued=false;refreshFeedback(focus);},0);}
    function hookValidation() {
        ["Page_ClientValidate","ValidatorOnSubmit"].forEach(function(name){var original=window[name];if(typeof original!=="function"||original.thyWillHook)return;var wrapped=function(){var result=original.apply(this,arguments);queueFeedback(result===false);return result;};wrapped.thyWillHook=true;window[name]=wrapped;});
    }
    function sessionGetAction() {
        try {
            var key = "tw-action:" + location.pathname;
            var action = sessionStorage.getItem(key);
            sessionStorage.removeItem(key);
            return action ? JSON.parse(action) : null;
        } catch (_) { return null; }
    }
    document.addEventListener("submit", function(event) {
        if (event.defaultPrevented || window.Page_IsValid === false) return;
        var control = event.submitter || document.activeElement;
        if (!control || !control.matches("input[type=submit],button[type=submit]")) return;
        var scope = control.closest("section,[data-ui-pane],[data-record-pane]");
        var nearby = scope && scope.querySelector("[id$=lblMessage],[id$=lblError]");
        var before = {};
        document.querySelectorAll(".tw-feedback[id],[role=status][id]").forEach(function(node) { before[node.id] = node.textContent.trim(); });
        try { sessionStorage.setItem("tw-action:" + location.pathname, JSON.stringify({message:nearby && nearby.id,before:before})); } catch (_) {}
        control.setAttribute("aria-busy","true");
        control.setAttribute("data-action-busy", "");
        var hint = document.createElement("span");
        hint.className = "tw-action-progress";
        hint.setAttribute("role","status");
        hint.textContent = (control.value || control.textContent || "Request").trim() + "…";
        control.insertAdjacentElement("afterend", hint);
    });
    function resetBusy() {
        document.querySelectorAll("[data-book-busy],[data-action-busy]").forEach(function(button) {
            button.removeAttribute("data-book-busy");
            button.removeAttribute("data-action-busy");
            button.removeAttribute("aria-disabled");
            button.removeAttribute("aria-busy");
        });
        document.querySelectorAll(".tw-action-progress").forEach(function(hint) { hint.remove(); });
    }
    function initializeMotion() {
        var reduced=window.matchMedia&&window.matchMedia("(prefers-reduced-motion: reduce)").matches;
        var targets=document.querySelectorAll("[data-reveal],.reveal,.tw-reveal");
        if(reduced||!("IntersectionObserver" in window)){document.body.classList.remove("motion-ready");targets.forEach(function(node){node.classList.add("is-revealed","is-visible");});return;}
        document.body.classList.add("motion-ready");
        if(!revealObserver)revealObserver=new IntersectionObserver(function(entries){entries.forEach(function(entry){if(entry.isIntersecting){entry.target.classList.add("is-revealed","is-visible");revealObserver.unobserve(entry.target);}});},{threshold:.08,rootMargin:"0px 0px 40px 0px"});
        targets.forEach(function(node,index){if(node.dataset.motionReady)return;node.dataset.motionReady="true";node.style.setProperty("--reveal-delay",(index%3)*60+"ms");revealObserver.observe(node);});
    }
    function initializeDatePickers() {
        document.querySelectorAll('input[type="date"]').forEach(function(field) {
            if (typeof field.showPicker !== "function" || field.readOnly || field.disabled || field.closest(".tw-date-picker")) return;
            var wrapper = document.createElement("span");
            wrapper.className = "tw-date-picker";
            field.parentNode.insertBefore(wrapper, field);
            wrapper.appendChild(field);
            var button = document.createElement("button");
            button.type = "button";
            button.className = "tw-date-trigger";
            button.setAttribute("aria-label", "Open calendar" + (field.labels && field.labels[0] ? " for " + field.labels[0].textContent.trim() : ""));
            var icon = document.createElement("i");
            icon.className = "bi bi-calendar3";
            icon.setAttribute("aria-hidden", "true");
            button.appendChild(icon);
            button.addEventListener("click", function() {
                field.focus();
                try { field.showPicker(); } catch (_) { /* Keyboard date entry remains available. */ }
            });
            wrapper.appendChild(button);
        });
    }
    function initialize() {
        initializeIdentity();
        applyTheme(document.documentElement.getAttribute("data-theme"),false);
        initializeNavigation();initializeTables();initializePasswords();initializeLabels();initializeDatePickers();
        document.querySelectorAll("[data-record-tabs]").forEach(initializeRecordTabs);
        document.querySelectorAll("[data-ui-tabs]").forEach(initializeUITabs);
        hookValidation();refreshFeedback(false);initializeMotion();resetBusy();
        var serverError=Array.prototype.find.call(document.querySelectorAll("[data-ui-error]"),function(error){var style=window.getComputedStyle(error);return error.textContent.trim() && !error.hidden&&style.display!=="none"&&style.visibility!=="hidden";});
        if(serverError&&!serverError.dataset.errorFocused){serverError.dataset.errorFocused="true";openContainingPane(serverError);focusNode(serverError);}
        var postback = sessionGetAction();
        if (postback && !serverError) {
            var messages = Array.prototype.slice.call(document.querySelectorAll(".tw-feedback, [role='status']"));
            var message = messages.find(function(node) {
                return node.id && node.textContent.trim() && node.getClientRects().length &&
                    (!postback.before || postback.before[node.id] !== node.textContent.trim() || node.id === postback.message);
            });
            if (message) { openContainingPane(message); focusNode(message); message.scrollIntoView({block:"nearest"}); }
        }
        if(!requestManagerBound&&window.Sys&&Sys.WebForms&&Sys.WebForms.PageRequestManager){requestManagerBound=true;Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function(){initialize();if(window.Page_IsValid===false)refreshFeedback(true);});}
    }
    function viewControl(row) {return row&&row.querySelector('[data-record-select],.invoice-row-view,a[href*="Select$"],a[href*="Select%24"]');}
    document.addEventListener("click",function(event){
        var target=event.target.closest?event.target:null;if(!target)return;
        var view=target.closest("[data-record-select],.invoice-row-view");
        if(!view){var anchor=target.closest("a");if(anchor&&/select(?:\$|%24)\d+/i.test(anchor.getAttribute("href")||""))view=anchor;}
        if(view){var root=view.closest("[data-record-tabs]");if(root)writeStorage("sessionStorage",storageName(root,"pending"),"details");}
        if (target.closest("[data-profile-save]")) {
            var rules = [
                ["txtFullName", function(s) { return !!s; }, "Enter your full name."],
                ["txtPhone,txtPhoneNumber", function(s) { return /^\d{10}$/.test(s); }, "Enter a 10-digit phone number."],
                ["txtEmail,txtEmailAddress", function(s) { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(s); }, "Enter a valid email address."],
                ["txtAddress", function(s) { return !!s; }, "Enter your address."],
                ["ddlArea", function(s) { return !!s; }, "Select your service area."]
            ];
            for (var rule of rules) {
                var field = rule[0].split(",").map(function(id) { return byServerID(document,id); }).find(Boolean);
                if (field && !field.readOnly && !rule[1](field.value.trim())) {
                    event.preventDefault();
                    event.stopImmediatePropagation();
                    field.setCustomValidity(rule[2]);
                    field.reportValidity();
                    field.setCustomValidity("");
                    field.focus();
                    return;
                }
            }
        }
        var submit=target.closest("[data-booking-submit]");
        if(submit){if(submit.hasAttribute("data-book-busy")){event.preventDefault();event.stopImmediatePropagation();return;}if(typeof window.Page_ClientValidate==="function"&&!window.Page_ClientValidate("Booking")){event.preventDefault();event.stopImmediatePropagation();refreshFeedback(true);return;}submit.setAttribute("data-book-busy","true");submit.setAttribute("aria-busy","true");submit.setAttribute("aria-disabled","true");}
    },true);
    document.addEventListener("click",function(event){
        var target=event.target.closest?event.target:null;if(!target)return;
        var button=target.closest("[data-theme-toggle]");if(button){event.preventDefault();applyTheme(document.documentElement.getAttribute("data-theme")==="dark"?"light":"dark",true);return;}
        var tab=target.closest("[data-record-tab],[data-ui-tab],[data-tab-open]");if(tab){var state=stateFor(tab);if(state){event.preventDefault();showState(state,tab.getAttribute("data-record-tab")||tab.getAttribute("data-ui-tab")||tab.getAttribute("data-tab-open"),false);}return;}
        var password=target.closest("[data-password-target]");if(password){var field=document.getElementById(password.getAttribute("data-password-target"));if(field){var visible=field.type==="password";field.type=visible?"text":"password";password.textContent=visible?"Hide":"Show";password.setAttribute("aria-label",visible?"Hide password":"Show password");password.setAttribute("aria-pressed",String(visible));}return;}
        if(target.closest("#twMobileMenuToggle")){var menu=document.getElementById("twMobileMenu");setPublicMenu(!menu.classList.contains("is-open"));return;}
        if(target.closest(".staff-nav-toggle")){var nav=document.querySelector(".staff-navigation");setStaffMenu(!nav.classList.contains("is-open"));return;}
        if(target.closest(".customer-sidebar-toggle")){var sidebar=document.querySelector(".customer-sidebar");setDrawer(!sidebar.classList.contains("is-open"),false);return;}
        if(target.closest(".customer-sidebar-close,.customer-sidebar-overlay")){setDrawer(false,true);return;}
        if(target.closest(".customer-navigation a"))setDrawer(false,false);
        if(target.closest(".tw-mobile-menu a"))setPublicMenu(false);
        if(target.closest(".staff-navigation a"))setStaffMenu(false);
        if(!target.closest(".tw-navbar-glass"))setPublicMenu(false);
        if(!target.closest(".staff-topbar"))setStaffMenu(false);
        if(!target.closest("a,button,input,select,textarea,label")){var control=viewControl(target.closest("tr"));if(control)control.click();}
        var link=target.closest('a[href*="#"]');
        if(link&&link.getAttribute("href").charAt(0)!=="j"){
            try{var url=new URL(link.href,window.location.href);if(url.origin===window.location.origin&&url.pathname===window.location.pathname&&url.hash.length>1){var anchor=document.getElementById(decodeURIComponent(url.hash.slice(1)));if(anchor){event.preventDefault();if(anchor.scrollIntoView)anchor.scrollIntoView({behavior:window.matchMedia&&window.matchMedia("(prefers-reduced-motion: reduce)").matches?"auto":"smooth",block:"start"});try{window.history.replaceState(null,"",url.hash);}catch(_){}}}}catch(_){}
        }
    });
    document.addEventListener("change",function(event){var input=event.target;if(input.matches("[data-nationality],select[id$=ddlCustomerType]"))initializeIdentity();if(input.matches('[data-service-catalog] input[type="checkbox"]')){var card=input.closest(".service-type-card");if(card)card.classList.toggle("selected",input.checked);}});
    document.addEventListener("keydown",function(event){
        if(event.key==="Escape"){
            var publicOpen=document.querySelector("#twMobileMenu.is-open"),staffOpen=document.querySelector(".staff-navigation.is-open"),open=document.querySelector(".customer-sidebar.is-open");
            setPublicMenu(false);setStaffMenu(false);if(open)setDrawer(false,true);else if(publicOpen)focusNode(document.getElementById("twMobileMenuToggle"));else if(staffOpen)focusNode(document.querySelector(".staff-nav-toggle"));return;
        }
        var tab=event.target.closest&&event.target.closest('[role="tab"]');
        if(tab&&["ArrowLeft","ArrowRight","Home","End"].indexOf(event.key)>=0){var state=stateFor(tab);if(state){var enabled=state.keys.filter(function(key){return!state.buttons[key].disabled;}),current=enabled.indexOf(state.active),index=event.key==="Home"?0:event.key==="End"?enabled.length-1:(current+(event.key==="ArrowRight"?1:-1)+enabled.length)%enabled.length;event.preventDefault();showState(state,enabled[index],true);}return;}
        var drawer=document.querySelector(".customer-sidebar.is-open");if(event.key!=="Tab"||!drawer||window.innerWidth>768)return;
        var items=Array.prototype.filter.call(drawer.querySelectorAll('a[href],button:not([disabled])'),function(node){return node.getClientRects().length>0;}),first=items[0],last=items[items.length-1];
        if(!first)return;if(event.shiftKey&&document.activeElement===first){event.preventDefault();last.focus();}else if(!event.shiftKey&&document.activeElement===last){event.preventDefault();first.focus();}
    });
    window.addEventListener("resize",function(){if(window.innerWidth>1100)setPublicMenu(false);if(window.innerWidth>768){setStaffMenu(false);setDrawer(false,false);}else{var sidebar=document.querySelector(".customer-sidebar");if(sidebar)setDrawer(sidebar.classList.contains("is-open"),false);}});
    window.addEventListener("storage",function(event){if(event.key===key)applyTheme(validTheme(event.newValue)?event.newValue:cookieTheme()||"dark",false);});
    window.addEventListener("pageshow",resetBusy);
    window.ThyWillUI={initializeIdentity:initializeIdentity,initialize:initialize,activateTab:activateTab,refreshValidation:function(){refreshFeedback(true);},applyTheme:function(theme){applyTheme(theme,true);}};
    window.ThyWillDashboard={openCustomerSidebar:function(){setDrawer(true,false);},closeCustomerSidebar:function(){setDrawer(false,true);},activateTab:function(key){document.querySelectorAll("[data-record-tabs],[data-ui-tabs]").forEach(function(root){var state=recordStates.get(root)||uiStates.get(root);if(state&&state.buttons[key])showState(state,key,false);});}};
    if(document.readyState==="loading")document.addEventListener("DOMContentLoaded",initialize);else initialize();
})(window,document);

// Optional demo analytics stay in this tab and are never transmitted.
(function () {
    'use strict';
    var consentStorageName='thywill-privacy-v1', choice='unset', metrics={views:0,actions:0};
    try { choice=localStorage.getItem(consentStorageName)||'unset'; } catch (_) {}
    function init() {
        if(document.body.getAttribute('data-staff-session')==='true') {
            var notice=document.createElement('p');notice.textContent='Staff notice: access, allocations, job requests and security decisions are recorded automatically for operations and security. No acceptance is required. Form contents and screen recordings are not collected by these analytics.';
            notice.style.cssText='margin:16px;padding:12px 18px;font:13px/1.5 system-ui;border-top:1px solid #7890ab66';
            document.body.appendChild(notice);
            window.ThyWillPrivacy={summary:function(){return {mode:'required-operational-records',optionalAnalytics:false};}};
            return;
        }
        var choices = document.querySelector("[data-privacy-preferences]");
        function updateChoices() {
            if (!choices) return;
            choices.querySelectorAll("[data-choice]").forEach(function(button) {
                var selected = button.getAttribute("data-choice") === (choice === "analytics" ? "analytics" : "essential");
                button.setAttribute("aria-pressed", String(selected));
            });
            choices.querySelector("[role=status]").textContent = choice === "analytics" ?
                "Optional demo analytics are enabled." : "Only essential features are enabled.";
        }
        function count(name) { if (choice === "analytics") metrics[name]++; }
        if (choices) {
            choices.addEventListener("click", function(event) {
                var button = event.target.closest("[data-choice]");
                if (!button) return;
                choice = button.getAttribute("data-choice");
                metrics = {views:0,actions:0};
                try { localStorage.setItem(consentStorageName, choice); } catch (_) {}
                updateChoices();
            });
            updateChoices();
        }
        count("views");
        document.addEventListener("click", function(event) {
            if (!(choices && choices.contains(event.target)) && event.target.closest("a[href], [data-tab-pane] button")) count("actions");
        });
        window.addEventListener("storage",function(event) {
            if (event.consentStorageName === consentStorageName) { choice = event.newValue || "unset"; metrics = {views:0,actions:0}; updateChoices(); }
        });
        window.ThyWillPrivacy = {summary:function() { return {consent:choice,pageViews:metrics.views,navigationActions:metrics.actions}; }};
    }
    if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
})();
