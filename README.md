# MiniAccounts
🧾 Mini Account Management System
A lightweight, role-based accounting management system built using ASP.NET Core Razor Pages, SQL Server (Stored Procedures), and ASP.NET Identity.  

**📌 Overview**
The Mini Account Management System is designed for small businesses or educational projects to manage essential accounting operations. It offers core features like user roles & permissions, a dynamic chart of accounts, and voucher entry (journal, payment, and receipt). It strictly uses ADO.NET and stored procedures — no Entity Framework or LINQ — for high performance and fine-grained control over data operations.  

<br>
![image](https://github.com/user-attachments/assets/5efbbabc-e71b-405d-80f0-96c030cdcf4c)  

________________________________________

**🛠️ Technologies Used**  
Technology	Description  
• ASP.NET Core 8	Web framework for building Razor Pages UI   
• Razor Pages	Page-based coding model for the frontend    
• ADO.NET	Data access layer using raw SQL    
• SQL Server	Backend database with stored procedures only    
• ASP.NET Identity	Built-in user management and authentication    
• Bootstrap 5	UI styling and layout      

**✅ Prerequisites**  
•	Visual Studio 2022+  
•	.NET 8 SDK  
•	SQL Server (LocalDB or Express)  
•	Set up DB and run StoredProcedures.sql before starting  
________________________________________

**👥 User Roles**  
•	Admin: Full access to user and role management, vouchers, reports.  
•	Accountant: Can enter and view vouchers.  
•	Viewer: Can only view reports and data; read-only access.  


![image](https://github.com/user-attachments/assets/a62df3f9-0dd7-4d4a-aa42-31b08374a9d2)   


________________________________________
**📚 Features**  
**🔐 User Management (Role-Based Access)**  
•	Register and login functionality  
•	Admin can assign/revoke roles to users  
•	Only authorized users can access certain pages  


![image](https://github.com/user-attachments/assets/3d435bf3-cd38-4029-81ad-fafdefb003a6)  



**🧾 Chart of Accounts (COA)**  
•	Tree-structured chart of accounts  
•	Managed via stored procedure sp_ManageChartOfAccounts  
•	Supports hierarchical levels (e.g., Assets → Current Assets → Cash)  
![image](https://github.com/user-attachments/assets/bcb745aa-a93a-43c0-9f3a-00b3993c7082)  
![image](https://github.com/user-attachments/assets/78fe173e-d31d-4a25-995f-9a163506369f)  

**💰 Voucher Entry Module**  
•	Add vouchers (Journal, Payment, Receipt)  
•	Select accounts from chart of accounts dropdowns  
•	Save voucher entries using stored procedure sp_SaveVoucher  
•	Table-valued parameter for voucher details  
•	List and view all vouchers with filtering  
![image](https://github.com/user-attachments/assets/77957ddb-108b-41ab-bd52-c30add395048)  
![image](https://github.com/user-attachments/assets/ce4cd5cc-2ff2-42ba-85b8-5cf5b339144c)  


**📤 Export to Excel (Optional Feature)**  
•	Export filtered voucher lists to Excel  
•	Generates .xlsx file using EPPlus or ClosedXML  
![image](https://github.com/user-attachments/assets/bbfc2d56-747d-4bad-9f48-093e105436c3)  

________________________________________  
**💡 How It Works (Example)**  
Scenario: Accountant wants to record a cash payment to a vendor.  
1.	Login as an Accountant  
2.	Navigate to Voucher Entry  
3.	Choose Payment Voucher  
4.	Select Cash (Dr) and Accounts Payable (Cr)  
5.	Enter description and amount  
6.	Save → Data is stored via sp_SaveVoucher  
________________________________________  
**🧩 Project Structure**  

/Pages  
 └── /Admin/Users       → User + Role management (Admin only)  
 └── /Accounts          → Chart of Accounts CRUD  
 └── /Vouchers          → Voucher Entry & Listing  
/Areas/Identity         → Login, Register, Identity UI  
/StoredProcedures.sql   → All DB procedures  
________________________________________  

