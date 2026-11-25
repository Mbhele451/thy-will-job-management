# THY WILL design specification

Implemented 15 November 2026. ASP.NET Web Forms, .NET Framework 4.7.2.

## Visual identity

A service workspace with deep teal actions, slate typography, quiet surfaces and amber supporting accents. The public website and signed-in application share typography, controls and semantic colours. Existing company content, contact details and service destinations are retained. The homepage reuses the supplied electrical-work photograph rather than adding an external image dependency.

## Palette

All interface colours resolve through `Content/css/theme.css`. Existing `--tw-*`, `--dash-*` and short variable names are compatibility aliases to the same semantic tokens.

| Token | Light | Dark |
| --- | --- | --- |
| Primary | #12685F | #76D8C6 |
| Primary hover | #0C524B | #99E7D9 |
| Secondary | #52647B | #ADBED6 |
| Accent | #B66B12 | #EDB766 |
| Background | #F3F6F8 | #111A22 |
| Surface | #FFFFFF | #19252F |
| Secondary surface | #EDF2F5 | #22313D |
| Primary text | #172B37 | #EAF1F5 |
| Secondary text | #465B69 | #BCCBD5 |
| Muted text | #596E7C | #A0B3C1 |
| Border | #D4DFE5 | #354754 |
| Text on primary | #FFFFFF | #102B29 |
| Success | #187145 | #87D8AA |
| Warning | #925509 | #EDC27F |
| Danger | #B93242 | #FFA1AB |
| Information | #275E9A | #9BC4FF |

Soft semantic backgrounds, overlays and shadow colours are also defined in the token file. Twenty-four palette foreground/background pairs were calculated at 4.5:1 or better, with a minimum of 4.9:1. This verifies these token combinations, not every rendered element or WCAG conformance.

## Theme lifecycle

Every master page loads `Content/js/Site.js` synchronously in its head. The script applies `data-theme` before styles render. A valid `thywill-theme` localStorage preference wins; otherwise the operating system preference supplies the initial value. Storage failures leave theme switching available for the current page. Sun and moon controls expose accessible labels and pressed state. Theme updates synchronise across open tabs through the storage event. Full postbacks restore the saved preference. Partial postbacks reinitialise shared controls through PageRequestManager without duplicating handlers.

## Typography and spacing

Inter is retained through the existing Google Fonts reference, with Segoe UI and system fonts as fallbacks. Body text uses 15px and line-height 1.6. Public hero headings scale between 2.4rem and 4.7rem. Shared dashboard titles scale between 1.65rem and 2.15rem. Form labels use 13px; table cells use 13px and table headings use 11px.

Spacing tokens use 4, 8, 12, 16, 24, 32 and 48px. Control corners use 6px, panels 10px and larger public surfaces 12px. Public content has a maximum width of 1200px; dashboard content has a maximum width of 1440px. Existing page-specific layout arrangements remain available through scoped rules.

## Components

- Primary actions use the primary colour and contrasting on-primary text. Secondary actions use a bordered surface. Shared controls have a minimum height of 44px; compact table actions retain their original semantics.
- Cards use a solid surface and a subtle border. Decorative glass effects, floating background orbs and navigation sheen are retired.
- Forms use semantic labels, surfaces, borders, placeholders and focus rings. Existing TextBox, DropDownList, validators, validation groups and event bindings remain in place. Shared password controls are added only where an existing visibility button is absent.
- Selected service cards use the primary soft background and primary border. Existing selection inputs and scripts remain authoritative.
- Status text remains visible alongside semantic colour. Existing statuses and data bindings remain unchanged.
- Bootstrap modal, dropdown and pagination surfaces use the shared tokens. The existing payment-modal visibility logic remains unchanged.

## Navigation

Customer navigation uses a 248px desktop sidebar with Overview, Services, Financial and Account groups. Destinations are Dashboard, Book Job, My Jobs, Invoices and Profile, plus the existing logout page. PayInvoice highlights Invoices. Active-page logic also accepts friendly URLs without the `.aspx` extension.

At 768px and below the sidebar becomes a fixed drawer with an overlay, explicit close control, Escape support, focus containment and inert background content. Closed mobile navigation is inert. Resizing restores desktop access and page scrolling. Staff retain their role-specific top navigation, with a mobile dropdown and an explicit mobile sign-out destination.

Public navigation collapses at 1100px. Its menu is positioned below the header, has bounded vertical scrolling and closes after destination selection, outside clicks or Escape. The theme action remains outside collapsed desktop actions. The legacy Site master gains the same mobile destinations. The mobile master retains its original content placeholders and ViewSwitcher.

## Tables

Shared JavaScript wraps actual rendered top-level tables in a focusable horizontal-scroll region when a suitable wrapper does not already exist. The original table, GridView commands, empty rows, pager links and server IDs remain intact. Nested pager tables are excluded. Header cells receive column scope. Numeric columns identified from their header text align right and use tabular numerals.

Standard tables have a minimum width of 600px. Tables with seven or more headers have a minimum width of 900px; page-specific widths remain where appropriate. Tables containing only an empty spanning row do not force a minimum width. Scroll containers and their grid ancestors use `min-width: 0` to contain overflow within the table region.

## Responsive rules

The shared component layer provides navigation and layout changes at 1100, 900, 768 and 430px. Existing page-specific breakpoints are retained in scoped layout rules. Mobile forms, invoice summaries, payment options, cards and action groups stack or wrap. Reduced-motion preferences disable nonessential animation and transitions.

Requested verification widths are 320, 375, 430, 768, 1024, 1366 and 1920px. DOM tests ran with these simulated window widths. Browser geometry and screenshot inspection at these widths remain pending because browser preview access was unavailable.

## CSS architecture

1. `theme.css`: semantic colours, dimensions and compatibility aliases.
2. Existing local Bootstrap: grid, utility and component foundations.
3. `Site.css`: public layout and existing shared structural classes.
4. `navigation.css`: signed-in shell structure.
5. `dashboard.css`: shared dashboard layouts, followed by scoped page layouts extracted from 15 secure pages.
6. `auth.css`: authentication layouts.
7. `components.css`: final shared visual and responsive component layer, loaded after master head placeholders.
8. `Chatbot/Chatbot.css`: existing chat component, converted to semantic colours.

Master pages load local Bootstrap and their relevant layout files, followed by theme.css and components.css. The theme tokens therefore override Bootstrap defaults before the final component layer. `global.css` and `workorders.css` remain compatible where already referenced. No new frontend framework or NuGet package is introduced. Exact duplicate CSS rules were removed during consolidation. Existing class names remain to preserve compatibility with markup and code-behind.

## Accessibility and verification limits

Implemented features include visible focus, labelled navigation, theme state, mobile drawer focus handling, table regions and header scope, password visibility labels, reduced motion and label-to-ClientID repair. The palette calculation is not a full accessibility audit. Keyboard behaviour was checked in a DOM simulation. Browser rendering, screen-reader behaviour, live GridView output, authentication, form submission and database-driven operations require validation in the original Windows solution.

## Default page refinement

The later homepage refinement uses a scoped visual system in `landing.css`. On Default.aspx, warm ivory, charcoal and bronze replace the shared teal treatment. The new hero and contact section retain dark surfaces in both themes; the intervening content adapts to light or dark mode. Large typography, numbered service rows, restrained dividers and two supplied images replace the former card-led homepage. The master conditionally applies `tw-home-premium`; other public and secure pages retain the shared design system described above. See `HOMEPAGE-REFINEMENT.md` for exact colours, integration and test limits.

## Black and blue revision, all interface pages

The current implementation supersedes the earlier ivory/bronze homepage palette. Dark mode uses near-black backgrounds with pale blue accents; light mode uses blue-tinted surfaces and navy text. All six public pages now share the approved editorial layout, header and footer. Nine curated images live under `Content/images/editorial`. Public images and sections have progressive reveal animations, with reduced-motion and no-JavaScript fallbacks. All account and signed-in interface pages use the same refined controls and theme system. See `PREMIUM-REVIEW.md` for current colours, motion, page coverage, image inventory, HTML review behaviour and verification limits.
