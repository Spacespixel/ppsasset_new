Back Office Implementation Plan
Goal Description
Create a secure Back Office system accessible at admin.ppsasset.com to manage lead registrations (tr_transaction), view dashboard statistics, and export data for marketing activities.

User Review Required
IMPORTANT

Domain Configuration: The application must be configured to respond to admin.ppsasset.com. In a local development environment, you may need to modify your hosts file to map admin.ppsasset.com to 127.0.0.1. Database Changes: A new table sy_admin_user will be created to store back-office administrator credentials. Authentication: We will implement a custom username/password authentication for admins, distinct from the existing social login for users.

Proposed Changes
Database
[NEW] sy_user Table
Unified user table with role-based access.

Id (INT, PK)
Username (VARCHAR)
PasswordHash (VARCHAR) - BCrypt
DisplayName (VARCHAR)
Role (VARCHAR) - e.g., 'Admin', 'User'
LastLogin (DATETIME)
IsActive (BOOL)
Application Structure
We will use ASP.NET Core Areas to separate the Back Office logic from the main site. Structure: Areas/Admin/Controllers, Areas/Admin/Views.

Features
Admin Authentication
Login Page (Username/Password).
Secure Cookie Authentication with Admin Role.
Dashboard
Summary of total leads.
Leads today/this week.
Chart/Graph (optional, or just stats cards) by Project.
Lead Management
DataTable listing all registrations from tr_transaction.
Filters: Date Range, Project, UTM Source.
Export: Button to export filtered list to Excel/CSV.
User Management (New)
List all users (sy_user).
Create/Edit/Delete users.
Manage Roles (Admin/User).
File Definitions
[NEW] 

AdminAreaRegistration
Setup the Admin area folder structure.

[NEW] 

AdminBaseController
Base controller for all admin controllers to enforce [Authorize(Roles = "Admin")].

[NEW] 

AuthController
Handles Login/Logout logic for admins.

[NEW] 

DashboardController
Displays the main dashboard stats.

[NEW] 

LeadController
Handles listing leads and export functionality.

[NEW] 

UserController
Handles CRUD operations for sy_user.

[NEW] 

AdminService
Service layer for Admin authentication, user management, and fetching dashboard stats.

[MODIFY] 

Program.cs
Add Routing for Admin Area with Host Matching (admin.ppsasset.com).
usage: app.MapControllerRoute(name: "admin", pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}").RequireHost("admin.ppsasset.com");
Verification Plan
Automated Tests
None planned for this phase (manual verification).
Manual Verification
Database: Verify sy_user is created and seeded with an initial admin.
Access Control:
Try to access /Admin/Dashboard without login -> Redirect to Login.
Try to login with invalid credentials -> Show error.
Login with valid credentials -> Access Dashboard.
Functionality:
Dashboard: Check if numbers match database counts.
Leads: Verify list shows recent data from tr_transaction.
Export: Click export and verify the downloaded Excel/CSV file contains correct data.
User Management: Create a new user, log out, and log in with the new user.
Routing: Test accessing via admin.ppsasset.com (simulated locally).