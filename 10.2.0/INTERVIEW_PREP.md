# Interview Preparation: Course Feedback Management System

This document contains a curated list of potential interview questions and answers based on your specific project. These are designed to help you demonstrate your knowledge of ASP.NET Core, ABP Framework, Angular, and software design principles.

## 1. Project Overview

**Q: Can you briefly describe the project you built?**

**A:**
"I developed a full-stack **Course Feedback Management System** using **ASP.NET Core** with the **ABP Framework** for the backend and **Angular** for the frontend.

The system allows students to submit feedback for courses, including ratings, comments, and file attachments (like proof of completion). It features a **multi-tenant architecture**, meaning different organizations can use the same instance with their own isolated data.

Key features include:
-   **CRUD operations** for Courses and Feedbacks.
-   **Role-based permissions** to restrict access (e.g., only Admin can create courses).
-   **Tenant-specific settings**, such as limiting the maximum number of feedbacks per course.
-   **Background jobs** using **Hangfire** to send daily alerts for courses with no recent activity.
-   **File uploads** for supporting documents."

---

## 2. Architecture & Design Patterns (ABP Framework)

**Q: Why did you use the ABP Framework? What benefits does it provide?**

**A:**
"I used ABP because it provides a robust, production-ready infrastructure out of the box based on **Domain Driven Design (DDD)** principles. It saved me time by handling cross-cutting concerns like:
-   **Exception Handling**: Automatically wraps errors in user-friendly messages.
-   **Dependency Injection**: Built-in support without manual configuration.
-   **Unit of Work**: Manages database transactions automatically.
-   **Multi-tenancy**: Built-in `IMustHaveTenant` interface makes data isolation easy.
-   **Audit Logging**: Automatically tracks who created or modified entities."

**Q: Explain the architecture layers in your project.**

**A:**
"The project follows a layered architecture:
1.  **Core Layer (Domain)**: Contains my entities (`Course`, `Feedback`) and domain logic. It has no dependencies on other layers.
2.  **Application Layer**: Contains `AppServices` (like `FeedbackAppService`) and DTOs. This layer orchestrates the logic, maps entities to DTOs, and handles validation.
3.  **EntityFrameworkCore Layer**: Handles database context, migrations, and repositories.
4.  **Web.Host Layer**: The entry point that exposes the REST APIs and hosts the application."

**Q: What is Dependency Injection (DI) and how is it used in your project?**

**A:**
"Dependency Injection is a design pattern where objects receive their dependencies rather than creating them. In my project, I inject repositories and services via the **constructor**.
For example, in `FeedbackAppService`, I inject `IRepository<Course>` and `ISettingManager`. This makes my code **testable** (I can mock these dependencies) and **loosely coupled**."

---

## 3. Backend (ASP.NET Core & C#)

**Q: What is a DTO and why do we use it instead of returning Entities directly?**

**A:**
"A **Data Transfer Object (DTO)** is a simple object used to transfer data between layers. I use them to:
1.  **Hide Domain Logic**: Entities might have sensitive data or internal logic I don't want to expose to the API.
2.  **Prevent Over-posting**: I can control exactly which properties a user can update (e.g., a user shouldn't be able to change a `CreationTime` manually).
3.  **Serialization**: DTOs avoid circular reference issues that can happen with Entity Framework navigation properties."

**Q: How did you handle Validation?**

**A:**
"I used a mix of **Data Annotations** on DTOs (like `[Required]`, `[Range(1,5)]`) for basic validation, and **Custom Business Logic** in my AppService.
For example, in `FeedbackAppService.CreateAsync`, I manually check if the related `Course` is active before allowing feedback. If it's inactive, I throw a `UserFriendlyException`, which ABP automatically converts to a 403/500 error with a readable message for the client."

**Q: Explain the `AsyncCrudAppService` you used.**

**A:**
"It's a base class provided by ABP that implements standard CRUD operations (Create, Read, Update, Delete) automatically.
I inherited from it in `FeedbackAppService` to get the basic functionality for free, but I **overrode** methods like `CreateAsync` and `CreateFilteredQuery` to add my specific business logic, such as checking the 'Max Feedback' tenant setting or filtering by specific keywords."

---

## 4. Database (EF Core & SQL Server)

**Q: How did you handle the relationship between Course and Feedback?**

**A:**
"It's a **One-to-Many** relationship. One Course can have many Feedbacks.
In the `Feedback` entity, I added a `CourseId` foreign key and a `virtual Course` navigation property.
In the `Course` entity, I added a `ICollection<Feedback> Feedbacks` property.
When querying feedbacks, I used `.Include(x => x.Course)` (or `Repository.GetAllIncluding`) to eagerly load the course details so I could display the Course Name in the list."

**Q: What is Code-First Migration?**

**A:**
"Code-First means I define my C# classes (Entities) first, and EF Core generates the database schema from them.
When I added the `Feedback` entity, I ran `Add-Migration Added_Feedback_Entity` and then `Update-Database`. This ensures my database schema is always in sync with my code and can be version controlled."

---

## 5. Frontend (Angular)

**Q: How does the Angular frontend communicate with the Backend?**

**A:**
"I used **Swagger-generated proxies** (via `nswag`). This automatically generates TypeScript services (like `FeedbackServiceProxy`) that match my backend API methods.
In my components, I inject these services and call methods like `.create()` or `.getAll()`. These methods return **Observables**, so I use `.subscribe()` to handle the response or errors."

**Q: How did you implement the File Upload?**

**A:**
"For the file upload:
1.  **Frontend**: I used a standard `<input type='file'>` (or PrimeNG FileUpload). When a user selects a file, I read it as `FormData`.
2.  **Backend**: I created a specific API endpoint that accepts `IFormFile`. I validate the extension (.pdf, .jpg) and save the file to a static folder (like `wwwroot/uploads`).
3.  **Database**: I only save the **file path** (URL) in the `Feedback` entity, not the file itself, to keep the database light."

---

## 6. Advanced Features (Hangfire, Settings, Permissions)

**Q: How does the Background Job (Hangfire) work?**

**A:**
"I implemented a class `FeedbackMonitoringJob` which inherits from `AsyncBackgroundJob`.
In the `ExecuteAsync` method, I query the database for all **Active** courses.
Then, for each course, I check if there are any feedbacks created in the last **10 days**.
If no feedback is found, I log a warning message using `Logger.Warn`.
This job is registered to run daily, ensuring we can monitor course engagement without manual checks."

**Q: How did you implement the 'Max Feedback' restriction?**

**A:**
"I defined a setting `App.Feedback.MaxFeedbackPerCourse` in the `AppSettingProvider`.
In my `FeedbackAppService`, before creating a feedback, I inject `ISettingManager` to read this value for the current tenant.
I then count the existing feedbacks for that course. If the count exceeds the setting value, I throw an exception preventing the creation."

---

## 7. Behavioral & Problem Solving

**Q: What was the most challenging part of this project?**

**A:**
*Sample Answer (Choose one that fits you):*
"The most challenging part was understanding the **ABP Framework's strict layering**. Initially, I tried to put logic in the Controller or Entity, but I learned that business logic belongs in the **Domain** or **Application** services. Learning to use the `AsyncCrudAppService` correctly and overriding its methods to inject my custom validation logic took some time to master, but it resulted in much cleaner code."

**Q: How did you debug issues when the API wasn't working?**

**A:**
"I used a few techniques:
1.  **Swagger UI**: To test the raw API endpoints independently of the Angular frontend.
2.  **Logs**: Checked the `App_Data/Logs/` text file where ABP logs all server-side exceptions with stack traces.
3.  **Breakpoints**: Attached the Visual Studio debugger to the running process to step through the code line-by-line."
