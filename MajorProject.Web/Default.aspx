<%@ Page Title="THY WILL Electrical & Construction" Language="C#" MasterPageFile="~/MasterPages/Public.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MajorProject.Web._Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="hp-hero" id="home" aria-labelledby="hp-title">
        <div class="hp-wrap hp-hero-grid">
            <div class="hp-hero-copy">
                <p class="hp-kicker"><span class="hp-rule" aria-hidden="true"></span> Durban, KwaZulu-Natal</p>
                <h1 id="hp-title">Electrical work.<br /><span>Done properly.</span></h1>
                <p class="hp-intro">From a fault at home to a complete installation. Electrical, maintenance and construction services, with care in every detail.</p>
                <% if (MajorProject.Web.Helpers.WebSession.IsAuthenticated) { %>
                    <p class="hp-intro">Welcome back, <%: MajorProject.Web.Helpers.WebSession.FullName %>. You are signed in to your <%: MajorProject.Web.Helpers.WebSession.Role %> workspace.</p>
                <% } %>
                <div class="hp-actions">
                    <a class="hp-button" href="~/Public/Contact.aspx" runat="server">Discuss your project <span aria-hidden="true">&#8599;</span></a>
                    <a class="hp-text-link" href="#services">Explore our services <span aria-hidden="true">&#8595;</span></a>
                </div>
                <div class="hp-hero-note"><span>Residential &amp; commercial</span><span>Durban &amp; surrounding areas</span></div>
            </div>
            <figure class="hp-hero-image">
                <img src="Content/images/editorial/installation.jpg" alt="Electrical installation work on a construction site" width="1376" height="768" fetchpriority="high" />
                <figcaption><span>THY WILL</span><span>Electrical &amp; Construction</span></figcaption>
            </figure>
        </div>
        <div class="hp-wrap hp-hero-bottom"><span>Considered from the first conversation.</span><a href="#services">View our expertise <span aria-hidden="true">&#8595;</span></a></div>
    </section>

    <section class="hp-services hp-section" id="services" aria-labelledby="hp-services-title">
        <div class="hp-wrap">
            <div class="hp-section-head" data-reveal><p class="hp-kicker">01 / Our expertise</p><div><h2 id="hp-services-title">The right expertise.<br /><span>For the work ahead.</span></h2><p>Installations, upgrades and repairs for your property. Start with the service you need.</p></div><a class="hp-text-link" href="~/Public/Services.aspx" runat="server">All services <span aria-hidden="true">&#8599;</span></a></div>
            <div class="hp-service-grid">
                <a class="hp-service" data-reveal href="~/Public/Services.aspx" runat="server"><span class="hp-service-no">01</span><img class="pr-service-thumb" src="Content/images/editorial/geyser.jpg" alt="" loading="lazy" width="120" height="120" /><div><h3>Geyser installations</h3><p>Electrical connections for new and replacement geysers.</p></div><span class="hp-service-arrow" aria-hidden="true">&#8599;</span></a>
                <a class="hp-service" data-reveal href="~/Public/Services.aspx" runat="server"><span class="hp-service-no">02</span><img class="pr-service-thumb" src="Content/images/editorial/charging.jpg" alt="" loading="lazy" width="120" height="120" /><div><h3>EV charger installations</h3><p>Charging connections for your home or business.</p></div><span class="hp-service-arrow" aria-hidden="true">&#8599;</span></a>
                <a class="hp-service" data-reveal href="~/Public/Services.aspx" runat="server"><span class="hp-service-no">03</span><img class="pr-service-thumb" src="Content/images/editorial/distribution.jpg" alt="" loading="lazy" width="120" height="120" /><div><h3>Distribution boards</h3><p>DB installations and upgrades for your property.</p></div><span class="hp-service-arrow" aria-hidden="true">&#8599;</span></a>
                <a class="hp-service" data-reveal href="~/Public/Services.aspx" runat="server"><span class="hp-service-no">04</span><img class="pr-service-thumb" src="Content/images/editorial/diagnostics.jpg" alt="" loading="lazy" width="120" height="120" /><div><h3>Fault finding &amp; repairs</h3><p>Identify electrical faults and arrange the repair work.</p></div><span class="hp-service-arrow" aria-hidden="true">&#8599;</span></a>
                <a class="hp-service" data-reveal href="~/Public/Services.aspx" runat="server"><span class="hp-service-no">05</span><img class="pr-service-thumb" src="Content/images/editorial/energy.jpg" alt="" loading="lazy" width="120" height="120" /><div><h3>General electrical work</h3><p>Maintenance and electrical work for everyday needs.</p></div><span class="hp-service-arrow" aria-hidden="true">&#8599;</span></a>
                <a class="hp-service" data-reveal href="~/Public/Services.aspx" runat="server"><span class="hp-service-no">06</span><img class="pr-service-thumb" src="Content/images/editorial/wiring.jpg" alt="" loading="lazy" width="120" height="120" /><div><h3>House &amp; property wiring</h3><p>Wiring for new spaces, renovations and extensions.</p></div><span class="hp-service-arrow" aria-hidden="true">&#8599;</span></a>
            </div>
        </div>
    </section>

    <section class="hp-approach hp-section" id="about" aria-labelledby="hp-approach-title">
        <div class="hp-wrap hp-approach-grid">
            <figure class="hp-detail-image"><img src="Content/images/editorial/wiring.jpg" alt="Wiring and tools during electrical installation" width="1840" height="1840" loading="lazy" /><figcaption><span>Precision in the details.</span><span>THY WILL / Electrical</span></figcaption></figure>
            <div class="hp-approach-copy" data-reveal><p class="hp-kicker">02 / Our approach</p><h2 id="hp-approach-title">Good work starts<br />with <span>listening.</span></h2><p>Tell us about your property, the problem or the project you have in mind. We’ll discuss the work required and the next steps.</p><p>Our approach centres on safety, clear communication and dependable workmanship, from the first enquiry through to completion.</p><a class="hp-text-link" href="~/Public/About.aspx" runat="server">More about THY WILL <span aria-hidden="true">&#8599;</span></a><div class="hp-principles"><span>Safety focused</span><span>Clear communication</span><span>Careful workmanship</span></div></div>
        </div>
    </section>

    <section class="hp-process hp-section" id="how-it-works" aria-labelledby="hp-process-title">
        <div class="hp-wrap"><div class="hp-section-head" data-reveal><p class="hp-kicker">03 / Getting started</p><div><h2 id="hp-process-title">A clear next step.<br /><span>At every stage.</span></h2></div></div>
            <ol class="hp-process-list"><li data-reveal><span class="hp-step">01</span><h3>Tell us what you need.</h3><p>Contact our team to discuss your electrical work and property.</p><a class="hp-text-link" href="~/Public/Contact.aspx" runat="server">Get in touch <span aria-hidden="true">&#8599;</span></a></li><li data-reveal><span class="hp-step">02</span><h3>Request your service.</h3><p>Create a customer profile, sign in and submit your service request.</p><% if (MajorProject.Web.Helpers.WebSession.IsAuthenticated) { %><a class="hp-text-link" href="<%= ResolveUrl(MajorProject.Web.Helpers.WebSession.DashboardPath) %>">Open my dashboard</a><% } else { %><a class="hp-text-link" href="~/Account/Register.aspx" runat="server">Create a profile <span aria-hidden="true">&#8599;</span></a><% } %></li><li data-reveal><span class="hp-step">03</span><h3>Keep everything in view.</h3><p>Follow job progress and access your invoices through your customer portal.</p><% if (MajorProject.Web.Helpers.WebSession.IsAuthenticated) { %><a class="hp-text-link" href="<%= ResolveUrl(MajorProject.Web.Helpers.WebSession.DashboardPath) %>">Open my dashboard</a><% } else { %><a class="hp-text-link" href="~/Account/Login.aspx" runat="server">Customer login <span aria-hidden="true">&#8599;</span></a><% } %></li></ol>
        </div>
    </section>

    <section class="hp-contact hp-section" id="contact" aria-labelledby="hp-contact-title"><div class="hp-wrap"><div class="hp-contact-top"><p class="hp-kicker">04 / Let's talk</p><h2 id="hp-contact-title">Your next project.<br /><span>Let’s start here.</span></h2><a class="hp-contact-arrow" href="~/Public/Contact.aspx" runat="server" aria-label="Contact THY WILL about your project">&#8599;</a></div><div class="hp-contact-details"><div><span class="hp-label">Call our team</span><a class="hp-phone" href="tel:+27837585301">083 758 5301</a></div><div><span class="hp-label">Send an enquiry</span><a href="mailto:contact@example.invalid">contact@example.invalid</a></div><div><span class="hp-label">Find us</span><address>267 Kenyon Howden Road<br />Woodlands, Durban, 4004</address></div></div></div></section>
    
</asp:Content>
