# Course Feedback Management System

A full-stack application built with ASP.NET Core (ABP Framework) and Angular for managing course feedback with role-based access control, file uploads, and automated background jobs.

---

## 📋 Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Running the Application](#running-the-application)
- [Default Credentials](#default-credentials)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [AI Tools Usage](#ai-tools-usage)
- [Screenshots](#screenshots)
- [Troubleshooting](#troubleshooting)

---

## ✨ Features

### Core Functionality
- ✅ **Course Management**: Full CRUD operations for courses
- ✅ **Feedback Management**: Students can provide feedback with ratings (1-5)
- ✅ **File Upload**: Support for PDF, JPG, PNG attachments (max 5MB)
- ✅ **Dashboard**: View total feedback count and top 5 courses by rating
- ✅ **Role-Based Access Control**: Granular permissions for different user roles
- ✅ **Multi-Tenancy**: Support for multiple organizations
- ✅ **Background Jobs**: Daily automated checks for inactive courses (Hangfire)

### Advanced Features
- 📊 Server-side pagination, sorting, and filtering
- 🔍 Advanced search capabilities
- 🎨 Modern UI with PrimeNG components
- 🔐 Secure authentication and authorization
- 📈 Real-time dashboard analytics
- 📧 Email notification support (configurable)
- 🌐 Tenant-specific settings (Max feedback per course)

---

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Architecture**: ABP Framework (Domain-Driven Design)
- **ORM**: Entity Framework Core
- **Database**: SQL Server 2019+
- **Background Jobs**: Hangfire
- **Authentication**: ABP Identity (JWT)
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: Angular 16+
- **UI Library**: PrimeNG
- **State Management**: RxJS
- **HTTP Client**: Angular HttpClient
- **Language**: TypeScript

### DevOps
- **Version Control**: Git
- **Package Manager**: NuGet, npm
- **Build Tool**: .NET CLI, Angular CLI

---

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (v8.0+)
- [Node.js](https://nodejs.org/) (v18.0+)
- [Angular CLI](https://angular.io/cli) (v16.0+)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (2019+ or LocalDB)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

**Optional:**
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Postman](https://www.postman.com/) for API testing

---

## 🚀 Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/FeedbackManagement.git
cd FeedbackManagement
```

### 2. Backend Setup

#### Install ABP CLI (if not already installed)
```bash
dotnet tool install -g Volo.Abp.Cli
```

#### Configure Database Connection
Update the connection string in `src/FeedbackManagement.HttpApi.Host/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=FeedbackManagement;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**For SQL Server Authentication:**
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=FeedbackManagement;User Id=sa;Password=YourPassword123;TrustServerCertificate=True"
  }
}
```

#### Run Database Migrations

**Option 1: Using Package Manager Console (Visual Studio)**
```powershell
# Set default project to EntityFrameworkCore
Update-Database
```

**Option 2: Using .NET CLI**
```bash
cd src/FeedbackManagement.EntityFrameworkCore
dotnet ef database update
```

#### Install Backend Dependencies
```bash
cd src/FeedbackManagement.HttpApi.Host
dotnet restore
```

### 3. Frontend Setup

```bash
cd angular
npm install
```

#### Configure API URL
Update `angular/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apis: {
    default: {
      url: 'https://localhost:44300',  // Backend API URL
      rootNamespace: 'FeedbackManagement'
    }
  }
};
```

---

## 🎯 Running the Application

### Start Backend (API)

**Option 1: Visual Studio**
1. Open `FeedbackManagement.sln`
2. Set `FeedbackManagement.HttpApi.Host` as startup project
3. Press F5 to run

**Option 2: Command Line**
```bash
cd src/FeedbackManagement.HttpApi.Host
dotnet run
```

The API will be available at:
- **HTTPS**: https://localhost:44300
- **Swagger UI**: https://localhost:44300/swagger

### Start Frontend (Angular)

```bash
cd angular
npm start
```

The Angular app will be available at:
- **URL**: http://localhost:4200

### Access Hangfire Dashboard

- **URL**: https://localhost:44300/hangfire
- **Note**: Only accessible to admin users

---

## 🔑 Default Credentials

### Admin User
- **Tenant**: Default
- **Username**: `admin`
- **Password**: `1q2w3E*`

### Test Users (Optional - Seed Data)
After running the application, you can create additional users through the admin panel.

---

## 📁 Project Structure

```
FeedbackManagement/
├── src/
│   ├── FeedbackManagement.Domain/              # Domain entities & business logic
│   │   ├── Entities/
│   │   │   ├── Course.cs
│   │   │   └── Feedback.cs
│   │   └── FeedbackManagementDomainModule.cs
│   │
│   ├── FeedbackManagement.Application/          # Application services
│   │   ├── Courses/
│   │   │   └── CourseAppService.cs
│   │   ├── Feedbacks/
│   │   │   └── FeedbackAppService.cs
│   │   ├── Jobs/
│   │   │   └── InactiveCourseCheckJob.cs
│   │   └── FeedbackManagementApplicationModule.cs
│   │
│   ├── FeedbackManagement.Application.Contracts/  # DTOs & Interfaces
│   │   ├── Courses/
│   │   │   ├── CourseDto.cs
│   │   │   └── ICourseAppService.cs
│   │   ├── Feedbacks/
│   │   │   ├── FeedbackDto.cs
│   │   │   └── IFeedbackAppService.cs
│   │   └── Permissions/
│   │       └── FeedbackManagementPermissions.cs
│   │
│   ├── FeedbackManagement.EntityFrameworkCore/   # EF Core configuration
│   │   ├── EntityFrameworkCore/
│   │   │   └── FeedbackManagementDbContext.cs
│   │   └── Migrations/
│   │
│   ├── FeedbackManagement.HttpApi.Host/         # Web API host
│   │   ├── Controllers/
│   │   ├── wwwroot/
│   │   │   └── uploads/feedback/                # File upload directory
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   └── FeedbackManagement.Domain.Shared/        # Shared constants
│       └── Settings/
│           └── FeedbackManagementSettings.cs
│
├── angular/                                      # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── courses/
│   │   │   │   ├── course-list.component.ts
│   │   │   │   └── course-list.component.html
│   │   │   ├── feedbacks/
│   │   │   │   ├── feedback-list.component.ts
│   │   │   │   └── feedback-list.component.html
│   │   │   ├── dashboard/
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   └── dashboard.component.html
│   │   │   └── services/
│   │   │       ├── course.service.ts
│   │   │       └── feedback.service.ts
│   │   └── environments/
│   │       └── environment.ts
│   ├── package.json
│   └── angular.json
│
├── test/
│   └── FeedbackManagement.Application.Tests/
│       ├── CourseAppService_Tests.cs
│       └── FeedbackAppService_Tests.cs
│
├── PRD.md                                        # Product Requirements Document
└── README.md                                     # This file
```

---

## 🔌 API Endpoints

### Course Management

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/app/course` | Get all courses (paginated) | Pages.Courses |
| GET | `/api/app/course/{id}` | Get course by ID | Pages.Courses |
| POST | `/api/app/course` | Create new course | Pages.Courses.Create |
| PUT | `/api/app/course/{id}` | Update course | Pages.Courses.Edit |
| DELETE | `/api/app/course/{id}` | Delete course | Pages.Courses.Delete |

### Feedback Management

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/app/feedback` | Get all feedbacks (paginated) | Pages.Feedbacks |
| GET | `/api/app/feedback/{id}` | Get feedback by ID | Pages.Feedbacks |
| POST | `/api/app/feedback` | Create new feedback | Pages.Feedbacks.Create |
| PUT | `/api/app/feedback/{id}` | Update feedback | Pages.Feedbacks.Edit |
| DELETE | `/api/app/feedback/{id}` | Delete feedback | Pages.Feedbacks.Delete |
| POST | `/api/app/feedback/upload-file` | Upload attachment | Pages.Feedbacks.Create |
| GET | `/api/app/feedback/top-courses` | Get top 5 courses by rating | Pages.Feedbacks |

### Dashboard

| Method | Endpoint | Description | Permission |
|--------|----------|-------------|------------|
| GET | `/api/app/dashboard/statistics` | Get dashboard statistics | Authenticated |

---

## 🧪 Testing

### Run Unit Tests

**Using Visual Studio:**
1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All Tests"

**Using .NET CLI:**
```bash
cd test/FeedbackManagement.Application.Tests
dotnet test
```

### Test Coverage

The project includes tests for:
- ✅ Course CRUD operations
- ✅ Feedback CRUD operations
- ✅ Validation rules (rating range, course active status)
- ✅ Permission-based access control
- ✅ Max feedback per course enforcement
- ✅ Top courses calculation

**Target Coverage**: 60%+ (Application layer)

### Manual Testing Checklist

- [ ] Create a new course
- [ ] Create feedback for active course
- [ ] Try creating feedback for inactive course (should fail)
- [ ] Upload a PDF/JPG/PNG file
- [ ] Try uploading invalid file type (should fail)
- [ ] Create feedback exceeding max limit (should fail)
- [ ] View dashboard with top courses
- [ ] Test pagination and filtering
- [ ] Check Hangfire job execution in dashboard
- [ ] Test role-based access (create test user without permissions)

---

## 🤖 AI Tools Usage

This project leveraged AI tools to accelerate development:

### Tools Used
- **ChatGPT (Claude)**: PRD generation, architecture design, code scaffolding
- **Cursor/Windsurf** (Optional): Code completion and debugging

### AI-Assisted Tasks
1. **PRD Generation**: Created comprehensive Product Requirements Document
2. **Entity Design**: Generated entity classes with proper validation
3. **DTO Mapping**: Scaffolded DTOs and AutoMapper profiles
4. **Service Implementation**: Generated application service boilerplate
5. **Angular Components**: Created component structure and templates
6. **Unit Tests**: Generated test cases and fixtures
7. **Documentation**: Created README, API documentation, and code comments

### Chat Logs
AI conversation logs are included in the `docs/ai-logs/` folder (if available).

### Benefits Observed
- ⏱️ **Time Saved**: ~30-40% reduction in development time
- 🐛 **Fewer Bugs**: AI-suggested best practices reduced common errors
- 📚 **Better Documentation**: Comprehensive docs generated automatically
- 🎯 **Consistent Code Style**: AI maintained consistent patterns throughout

---

## 📸 Screenshots

### Dashboard
![Dashboard](docs/screenshots/dashboard.png)
*Real-time analytics showing total feedback and top-rated courses*
<img width="1549" height="718" alt="image" src="https://github.com/user-attachments/assets/1f344b8b-53f6-4545-8833-c6d67cb9966b" />

### Course List
![Course List](docs/screenshots/course-list.png)
*Comprehensive course management with search and filters*
<img width="1446" height="644" alt="image" src="https://github.com/user-attachments/assets/9b9cd439-64a2-4e19-9ea1-c9a4b01acab5" />

### Feedback Form
![Feedback Form](docs/screenshots/feedback-form.png)
*User-friendly feedback submission with file upload*
<img width="1335" height="867" alt="image" src="https://github.com/user-attachments/assets/68cc4f65-afcd-4be8-8cf5-5f3b39bb6889" />

<img width="1569" height="645" alt="image" src="https://github.com/user-attachments/assets/eb6d3d2e-121f-4d11-83a2-2be9a957fec3" />


---

## 🛠 Troubleshooting

### Issue: Database migration fails

**Error**: `The connection string is invalid`

**Solution**:
```bash
# Verify SQL Server is running
# Check connection string in appsettings.json
# Try updating database again
cd src/FeedbackManagement.EntityFrameworkCore
dotnet ef database update
```

---

### Issue: CORS error in Angular

**Error**: `Access to XMLHttpRequest blocked by CORS policy`

**Solution**:
Update `appsettings.json` in HttpApi.Host:
```json
{
  "App": {
    "CorsOrigins": "http://localhost:4200"
  }
}
```

---

### Issue: File upload directory not found

**Error**: `Could not find a part of the path 'wwwroot/uploads/feedback'`

**Solution**:
```bash
# Create directory manually
cd src/FeedbackManagement.HttpApi.Host
mkdir -p wwwroot/uploads/feedback
```

---

### Issue: Hangfire dashboard returns 404

**Error**: `Cannot GET /hangfire`

**Solution**:
- Ensure you're logged in as admin user
- Check Hangfire is configured in `Program.cs`
- Verify URL: `https://localhost:44300/hangfire` (with HTTPS)

---

### Issue: Angular build fails

**Error**: `Module not found: PrimeNG`

**Solution**:
```bash
cd angular
rm -rf node_modules package-lock.json
npm install
npm start
```

---

## 📚 Additional Resources

### Documentation
- [ABP Framework Documentation](https://docs.abp.io)
- [PrimeNG Components](https://primeng.org/showcase)
- [Hangfire Documentation](https://docs.hangfire.io)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### Video Tutorials
- [ABP Framework Getting Started](https://www.youtube.com/watch?v=videoid)
- [Angular PrimeNG Tutorial](https://www.youtube.com/watch?v=videoid)

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👥 Authors

- **Meer Fareed** - *Initial work* - https://github.com/yourusername)](https://github.com/Meer-Dev/FeedBack-Management-App-using-AI-tools-Cursor-Claude-ChatGpt-

---

## 🙏 Acknowledgments

- ABP Framework team for the excellent architecture
- PrimeNG team for beautiful UI components
- Hangfire team for reliable background job processing
- AI tools (ChatGPT/Claude) for development assistance

---

## 📞 Support

For questions or issues:
- **Email**: support@feedbackmanagement.com
- **GitHub Issues**: [Create an issue](https://github.com/Meer-Dev/FeedBack-Management-App-using-AI-tools-Cursor-Claude-ChatGpt-/issues)
- **Documentation**: See `docs/` folder

---

**Happy Coding! 🚀**
