# THY WILL: passport registration and SysAdmin

The update changes MajorProject.Web, MajorProject.Business and MajorProject.Data. It adds the SysAdmin console and passport registration to the public, Administrator and Manager customer forms. The approved theme and booking cart remain in place.

## Shared booking and handover workflow

After the identity migration, run `04_JobChangeWorkflow.sql` followed by `05_CustomerCancellation.sql` on the SQL database shared by the website and desktop. Apply both migrations before deploying the matching application builds. The second migration adds customer cancellation requests, technician material reports and the fee invoice email queue. A runtime database principal with object-specific grants needs SELECT, INSERT and UPDATE on `dbo.JobFeeInvoiceOutbox` in addition to its existing job workflow permissions. Both applications use the configured Mailjet sender to deliver queued messages. The approval transaction records the charge even if delivery needs a retry.

## Install in this order

1. Extract SysAdmin_SQL.zip. Review and run `00_Preflight.sql` against GroupWst34. Review flagged customer IDs, duplicate passport groups, login collisions and unexpected object/index dependencies. No personal document numbers or password hashes are selected. Do not disable constraints to bypass a failed check.
2. Validate `01_Identity_and_SysAdmin.sql` on a copy of the database, then apply the migration to GroupWst34. The script uses a transaction. Invalid legacy identity combinations or duplicate passport/country pairs stop the migration without rewriting customer identity data.
3. Edit `@StaffUsername` in `02_Bootstrap_SysAdmin.sql` to the existing Administrator login you choose. Run the script. No username or password was guessed or seeded. The script rejects blocked/inactive accounts and accounts with open assigned jobs. Manager and Technician accounts are not promoted through bootstrap.
4. Run `03_Verify.sql`. Check the trusted/enabled identity constraint, four new tables and at least one usable SysAdmin. Check permissions under the application's database principal, rather than relying only on your SSMS owner's permissions.
5. Use `04_Optional_App_Permissions.sql` only if the runtime database principal lacks the listed object permissions. Set its placeholder explicitly. This grants no SQL Server `sysadmin` role and no `db_owner` membership.
6. Replace all three project folders in the existing solution. Restore packages, then Clean and Rebuild Solution in Visual Studio. Do not mix the older Business/Data DLLs with the updated Web project. Compiled assemblies are included and synchronized across the existing bin folders.
7. Sign out, restart IIS Express and sign in with the chosen account's existing credential. Open `/Secure/SysAdmin/Dashboard.aspx`.

The bootstrap changes the chosen account's application role. It then uses the security console rather than the ordinary Administrator workspace. Other Administrator accounts retain their existing permissions. Further SysAdmin account creation/role changes use the console. The normal staff-management screen cannot grant or modify SysAdmin accounts.

## Database changes

| Object | Purpose |
| --- | --- |
| `dbo.Roles`, one SysAdmin row | The actual role table referenced by `Staff.RoleID`. No role ID is hardcoded. `StaffRoles` is untouched. |
| `dbo.AppAccountSecurity` | Separate security blocks, reasons and credential/session version counters for staff or customers. |
| `dbo.AppUserSessions` | Session owner, hashed random token, IP, client description, application, UTC activity, expiry and revocation state. |
| `dbo.AppSecurityAudit` | Privileged actor, target account, action, UTC timestamp and reason. Passwords and token secrets are excluded. |
| `dbo.AppSystemSettings` | A limited set of application settings, with the last editor and timestamp. |
| Customer identity checks | Replaces the two conflicting identity constraints with one mutually exclusive rule for SA IDs, passports and businesses. Existing SA-ID and company-number checks remain. |
| Passport/country unique index | Prevents repeating the same document/issuing-country pair. Different countries retain separate document namespaces. |
| Unicode text columns | Converts Customers.FullName and Staff.FullName to nvarchar(100), and PassportCountry to nvarchar(50), preserving international characters. Existing nullability is retained. |

Your PassportNumber varchar(20) field remains the document-number field. A missing passport column is added conditionally. Existing document values are not cleared, guessed or converted into other identity types.

## Registration

Residential users choose SA ID or non-SA passport. SA ID requires exactly 13 ASCII digits. Passport requires a number up to 20 characters and an issuing country up to 50 characters. Passport numbers use the supported ASCII document-number field; country/name fields support Unicode. Passport number and issuing country are trimmed and normalized to uppercase before storage. Use consistent country names.

Business registration retains the supplied company-registration rules and clears unrelated identity fields. Saved identity fields are displayed read-only during contact-profile updates, including existing passport customers. Full name, email and address limits now match the schema: 100, 100 and 255 characters.

This update follows your current local service model: a supported Durban service area and a 10-digit local contact number. Nationality does not automatically expand phone length, service territory or company-number formats. Shared household phone numbers are allowed, matching the database and the existing form's description.

Customer email is the customer login. No separate Customers.Username column was invented. Staff usernames accept ASCII letters, digits, dots, underscores, @ and hyphens, up to 50 characters. New/changed logins are checked for staff/customer email/username collisions. The preflight also reports legacy collisions and duplicate staff-email groups for review.

## SysAdmin functions

- Search staff and customer accounts. Accounts are separated by type and ID.
- Edit staff usernames, emails and roles; edit customer login emails. Reset passwords with confirmation and existing compatible PBKDF2 hashing. Blank password fields preserve the old hash.
- Block or unblock an account with a reason. Security blocks do not rewrite employment or customer status. Unblocking does not reactivate an inactive/deactivated business account.
- Register staff accounts, including another SysAdmin. Register customers through the linked registration form. SysAdmin customer creation is authorized and audited in the same database transaction, and is permitted while public registration is closed.
- Revoke tracked sessions. Credential changes and blocks revoke the target's sessions. Existing reset-code fields are cleared on privileged credential changes.
- Review session IP/client/activity and privileged-change reports, with UTC date filters and CSV export. CSV quoting and formula-prefix protection are included.
- Edit CompanyDisplayName, SupportEmail, PublicRegistrationEnabled and SessionIdleMinutes only. Credentials, connection strings and API keys are not exposed through configuration controls.
- Protect the last usable SysAdmin from blocking or demotion. Role changes also reject targets with open assigned jobs until those jobs are reassigned. Own-account blocking is rejected.

CompanyDisplayName updates header branding. SupportEmail is a display contact in the console footer; mail-provider sender configuration remains in the original configuration. PublicRegistrationEnabled controls public customer registration. SessionIdleMinutes accepts 5 to 60; reducing the timeout shortens existing tracked expiry, while increases apply on subsequent activity. Settings changes are audited.

Account searches/exports show up to 1,000 matching accounts. Historic session/audit reports return the latest 2,000 rows in a range of at most 366 days. Narrow the name/date filters for larger datasets. Tables support paging within those results.

## Session/device tracking scope

Web tracking is wired into authenticated requests. Checks bind the signed Forms Authentication identity to the session account, then verify the current database role, account status, security version, block flag, token hash and expiry. Failed checks terminate the authenticated session. Authenticated form ViewState is bound to both session and account identity. Logout ends the tracked session.

The Web IP is the request's direct peer address. Arbitrary X-Forwarded-For headers are not trusted. A reverse proxy needs a deliberate trusted-proxy configuration to expose its original client IP. Browser descriptions and an IP do not prove a unique physical device. “Active” means the tracked token has not been revoked or expired; LastSeenAtUtc shows observed activity. Browser closure or an application restart does not instantly prove a remote device disconnected.

Only sessions observed after deployment are available. No historical IP/device information is invented.

The desktop application uses the shared DeviceSessionService for session reporting and revocation. Its session lifecycle is implemented in `Core/UserSession.cs`. Desktop IPs are client-reported and should be treated accordingly.

## Clashes found and addressed

1. `CK_Customers_IdentityByCustomerType` still required an SA ID for every residential customer. Adding PassportNumber alone did not enable foreign-client registration. The migration replaces this conflict atomically.
2. The added ID check permitted some mixed company/individual identity combinations. The replacement requires exactly one valid branch, including null passport country for businesses.
3. Staff uses `Roles`, not the duplicate `StaffRoles` table. SysAdmin is provisioned in the referenced table.
4. Adding a role without guarding the generic staff role picker would expose SysAdmin to Manager workflows. Both the picker and legacy database writes now protect this role and its accounts.
5. Old name/email/address limits exceeded the supplied database capacities. Input and service limits now match the columns.
6. The service rejected shared phone numbers even though the database and public form allowed them. The inconsistent duplicate-phone checks were removed.
7. Username/email edits could create ambiguous staff/customer sign-ins. New/changed credentials are checked across both account stores. Legacy collisions are reported rather than silently renamed.
8. Changing an operational staff role could strand assigned jobs. Bootstrap and SysAdmin role changes reject open-job targets. Blocking remains available for immediate security needs.
9. Character storage used varchar for names. Unicode column conversions avoid silently replacing international characters.
10. Security blocks and employment/customer lifecycle states are different concerns. Separate security rows preserve those existing business rules and relationships.

Existing CreateCustomer overloads were retained for existing call sites. New passport and SysAdmin entry points are additive. Password hash format, existing account IDs, payment/job/inventory keys and original configuration values were preserved. All supplied Web call sites compile against the updated shared projects. The separate desktop UI still needs its own rebuild and integration verification.

## Verification and remaining checks

All three projects compile against official .NET Framework 4.7.2 references: Data 13, Business 35 and Web 120 C# source files. Two existing async-without-await warnings remain in AuthenticationService. The policy tests also exercise the original hash implementation under .NET 8; its framework-obsolescence warnings do not represent a changed hash format.

58 C# assertions passed for identity combinations and boundaries, Unicode names, password validation/hash roundtrip, role/settings policy, date limits and CSV escaping. 1,834 DOM assertions passed across shared page structures and the new passport/console interactions. 162 T-SQL batches, complete literals and constructed variants parsed with the SQL Server 2019 grammar without syntax errors. Existing server controls and handlers were retained, the original theme tokens match, and local assets/project XML passed checks.

SQL syntax parsing does not execute a migration, verify database locks/permissions or run stored procedures. No connection to your live SQL Server was made and no migration was executed. IIS/Web Forms execution, real ViewState/UpdatePanel postbacks, authentication, permission enforcement, HTTP CSV downloads, PDF/email and payment regression checks still need a run in the original Windows solution. A usable local browser renderer was unavailable; DOM tests do not establish visual geometry or browser autofill rendering.

The existing invoice renderer uses Helvetica. Test non-Latin invoice names with a suitable PDF font before relying on native-script PDF output. Registration/database Unicode support does not by itself add glyphs to that existing PDF font.

Use a test account for each role. Verify anonymous/Administrator/Manager/Technician/Customer accounts cannot open SysAdmin, normal staff management cannot grant or modify SysAdmin, blocking rejects the next request/sign-in, revocation ends the chosen session, passport registration leaves SA/company fields null, contact updates preserve passport fields, and original booking/payment workflows still succeed.

The HTML reviews use fictional records and example IPs. Their actions do not modify accounts, passwords, sessions or database settings.
