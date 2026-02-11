# 📘 Attendance Management System (LMS)

## 📌 Project Description
The **Attendance Management System** is a role-based web application developed using **ASP.NET Core**.  
It manages student attendance across multiple programs, sessions (badges), sections, and courses with controlled access for **Admin**, **Teacher**, and **Student** roles.

---

## 🛠️ Technology Stack
- ASP.NET Core (MVC)
- Razor Pages
- HTML, CSS, Bootstrap
- SQL Server
- Role-Based Authentication

---

## 👥 User Roles

### Student
- View attendance
- Generate attendance report
- View timetable
- Change password

### Teacher
- Mark attendance (within limited time)
- View registered students
- View timetable
- Change password

### Admin
- Add / delete students and teachers
- Assign courses to students and teachers
- Manage programs, sessions, and sections
- View timetable
- Change password

---

## 🌟 System Features
- Multiple programs (e.g., Computer Science, Engineering)
- One program has multiple sessions (badges)
- One session has multiple sections
- One section has multiple students
- A student belongs to only one program-session-section
- A student can enroll in multiple courses
- A course can be assigned to multiple teachers
- One course per section is taught by only one teacher
- Time-restricted attendance marking

---

## 📄 Common Pages
- Login
- Dashboard
- Change Password
- Timetable View

---

## 🎓 Student Pages
- Student Dashboard
- View Attendance
- Generate Attendance Report
- View Timetable
- Change Password

---

## 🧑‍🏫 Teacher Pages
- Teacher Dashboard
- Mark Attendance
- View Registered Students
- View Timetable
- Change Password

---

## 🛠️ Admin Pages
- Admin Dashboard
- Manage Students
- Manage Programs
- Manage Sessions
- Manage Sections
- Set Attendance Time Period
- View Timetable
- Change Password

---

## 🎮 Controllers
- LoginController
- DashboardController
- ChangePasswordController
- TimetableController
- AttendanceController
- ReportController
- SessionController
- SectionController
- ProgramController
- UserController
- CourseController


## 🚀 How to Run
1. Clone the repository
2. Open the project in Visual Studio
3. Configure the SQL Server connection string
4. Apply database migrations
5. Run the project

---

## 🔐 Security
- Role-based authorization
- Secure login system
- Password management
- Restricted attendance timing

---


