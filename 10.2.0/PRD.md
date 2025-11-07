# Product Requirements Document (PRD)
## Course Feedback Management System

**Version**: 1.0  
**Date**: 2024  
**Author**: Trainee ASP.NET & Angular Developer  
**Status**: Complete

---

## 1. Executive Summary

The Course Feedback Management System is a full-stack web application designed to manage courses and collect student feedback. The system provides comprehensive CRUD operations, role-based access control, file uploads, background job processing, and analytics dashboards.

## 2. Project Overview

### 2.1 Purpose
Create a robust feedback management system that allows:
- Course administrators to manage courses
- Students to submit feedback with ratings and optional file attachments
- System administrators to monitor feedback trends and course performance

### 2.2 Target Users
- **Administrators**: Manage courses, view all feedbacks, access dashboard
- **Students**: Submit feedback for courses
- **Instructors**: View feedback for their courses

### 2.3 Technology Stack
- **Backend**: ASP.NET Core 9.0 with ABP Framework 10.2.0
- **Frontend**: Angular 19 with PrimeNG
- **Database**: SQL Server with Entity Framework Core
- **Background Jobs**: Hangfire
- **File Storage**: Local file system (wwwroot/uploads)

## 3. Functional Requirements

### 3.1 Course Management

#### 3.1.1 Create Course
- **Description**: Administrators can create new courses
- **Inputs**:
  - Course Name (required, max 250 characters)
  - Instructor Name (optional, max 250 characters)
  - Is Active (boolean, default: true)
- **Validation**:
  - Course Name is required
  - Course Name must not exceed 250 characters
- **Output**: New course created with auto-generated ID

#### 3.1.2 Read Courses
- **Description**: View list of courses with pagination
- **Features**:
  - Search by course name or instructor name
  - Filter by active/inactive status
  - Sort by any column
  - Pagination (configurable page size)
- **Output**: Paginated list of courses

#### 3.1.3 Update Course
- **Description**: Modify existing course details
- **Validation**: Same as Create Course
- **Output**: Updated course information

#### 3.1.4 Delete Course
- **Description**: Remove course from system
- **Constraints**: 
  - Should check for existing feedbacks (optional: prevent deletion if feedbacks exist)
- **Output**: Course removed from database

### 3.2 Feedback Management

#### 3.2.1 Create Feedback
- **Description**: Students can submit feedback for courses
- **Inputs**:
  - Student Name (required, max 200 characters)
  - Course ID (required, must exist and be active)
  - Comment (optional, unlimited text)
  - Rating (required, integer 1-5)
  - File (optional, PDF/JPG/PNG, max 10MB)
- **Validation**:
  - Student Name is required
  - Course must exist and be active
  - Rating must be between 1 and 5
  - File must be PDF, JPG, or PNG
  - File size must not exceed 10MB
  - Check tenant setting: MaxFeedbackPerCourse
- **Business Rules**:
  - If feedback count for course >= MaxFeedbackPerCourse, disallow creation
  - CreatedDate is auto-filled with UTC timestamp
- **Output**: New feedback created with file URL if uploaded

#### 3.2.2 Read Feedbacks
- **Description**: View list of feedbacks with advanced filtering
- **Features**:
  - Search by student name or comment
  - Filter by course
  - Filter by rating range (min/max)
  - Sort by any column (default: CreatedDate DESC)
  - Pagination
- **Output**: Paginated list of feedbacks with course names

#### 3.2.3 Update Feedback
- **Description**: Modify existing feedback
- **Validation**: Same as Create Feedback
- **Output**: Updated feedback information

#### 3.2.4 Delete Feedback
- **Description**: Remove feedback from system
- **Output**: Feedback removed from database

### 3.3 File Upload

#### 3.3.1 Upload File
- **Description**: Upload file attachment for feedback
- **Supported Formats**: PDF, JPG, PNG
- **Size Limit**: 10MB
- **Storage**: Local file system (wwwroot/uploads/feedbacks/)
- **Security**: 
  - Permission check (Pages.Feedbacks)
  - File type validation
  - File size validation
  - Unique filename generation (GUID)
- **Output**: File URL for storage in database

#### 3.3.2 Download File
- **Description**: Download/view uploaded file
- **Security**: Verify file path is within uploads directory
- **Output**: File download or display

### 3.4 Roles & Permissions

#### 3.4.1 Permission Definitions
- **Pages.Courses**: Access to Courses management
- **Pages.Feedbacks**: Access to Feedbacks management

#### 3.4.2 Authorization
- **Backend**: Application services protected with `[AbpAuthorize]` attribute
- **Frontend**: Route guards check permissions before navigation
- **UI**: Buttons and menus hidden based on permissions

### 3.5 Tenant Settings

#### 3.5.1 MaxFeedbackPerCourse Setting
- **Key**: `App.Feedback.MaxFeedbackPerCourse`
- **Type**: Integer
- **Scope**: Tenant
- **Default**: 100
- **Purpose**: Limit number of feedbacks per course
- **Usage**: Checked when creating new feedback

### 3.6 Background Jobs

#### 3.6.1 Feedback Monitoring Job
- **Schedule**: Daily at 2:00 AM UTC
- **Description**: Monitor active courses for feedback activity
- **Logic**:
  - Get all active courses
  - For each course, check if any feedbacks exist in last 10 days
  - If no feedbacks found, log warning: "No feedback received for Course [CourseName] in last 10 days."
- **Future Enhancement**: Send email notification to admin

### 3.7 Dashboard

#### 3.7.1 Statistics Cards
- **Total Feedbacks**: Count of all feedbacks
- **Total Courses**: Count of all courses
- **Active Courses**: Count of active courses
- **Average Rating**: Average of all feedback ratings

#### 3.7.2 Top Courses by Rating
- **Description**: Display top 5 courses ranked by average rating
- **Columns**:
  - Rank
  - Course Name
  - Instructor Name
  - Average Rating (calculated from feedbacks)
  - Feedback Count
- **Sorting**: By average rating (descending)

## 4. Non-Functional Requirements

### 4.1 Performance
- Page load time < 2 seconds
- API response time < 500ms
- Support 1000+ courses and 10,000+ feedbacks

### 4.2 Security
- Authentication required for all operations
- Role-based authorization
- File upload validation
- SQL injection prevention (EF Core parameterized queries)
- XSS protection (Angular sanitization)

### 4.3 Usability
- Responsive design (mobile-friendly)
- Intuitive navigation
- Clear error messages
- Loading indicators
- Confirmation dialogs for destructive actions

### 4.4 Scalability
- Multi-tenant architecture (ABP Framework)
- Database indexing on foreign keys
- Pagination for large datasets
- Background job processing

## 5. Database Schema

### 5.1 Courses Table
```sql
CREATE TABLE Courses (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  CourseName NVARCHAR(250) NOT NULL,
  InstructorName NVARCHAR(250) NULL,
  IsActive BIT NOT NULL DEFAULT 1
);
```

### 5.2 Feedbacks Table
```sql
CREATE TABLE Feedbacks (
  Id INT IDENTITY(1,1) PRIMARY KEY,
  StudentName NVARCHAR(200) NOT NULL,
  CourseId INT NOT NULL,
  Comment NVARCHAR(MAX) NULL,
  Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
  CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FileUrl NVARCHAR(500) NULL,
  CONSTRAINT FK_Feedbacks_Courses FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);
```

## 6. User Interface Requirements

### 6.1 Courses Page
- List view with PrimeNG table
- Search box
- Active/Inactive filter dropdown
- Create button (opens modal)
- Edit button per row (opens modal)
- Delete button per row (with confirmation)
- Pagination controls

### 6.2 Feedbacks Page
- List view with PrimeNG table
- Search box
- Course filter dropdown
- Min/Max rating filters
- Create button (opens modal with file upload)
- Edit button per row (opens modal)
- Delete button per row (with confirmation)
- File download button (if file exists)
- Pagination controls

### 6.3 Dashboard Page
- 4 statistics cards in a row
- Top 5 courses table below cards
- Auto-refresh on page load

### 6.4 Modal Dialogs
- Create/Edit Course modal
- Create/Edit Feedback modal (with file upload)
- Confirmation dialogs for delete operations

## 7. API Specifications

### 7.1 Course API
- `GET /api/services/app/Course/GetAll` - Paginated list
- `GET /api/services/app/Course/Get?id={id}` - Get by ID
- `POST /api/services/app/Course/Create` - Create
- `PUT /api/services/app/Course/Update` - Update
- `DELETE /api/services/app/Course/Delete?id={id}` - Delete

### 7.2 Feedback API
- `GET /api/services/app/Feedback/GetAll` - Paginated list
- `GET /api/services/app/Feedback/Get?id={id}` - Get by ID
- `POST /api/services/app/Feedback/Create` - Create
- `PUT /api/services/app/Feedback/Update` - Update
- `DELETE /api/services/app/Feedback/Delete?id={id}` - Delete

### 7.3 Dashboard API
- `GET /api/services/app/Dashboard/GetStatistics` - Get statistics
- `GET /api/services/app/Dashboard/GetTopCoursesByRating?count=5` - Get top courses

### 7.4 File Upload API
- `POST /api/services/app/FileUpload/UploadFile` - Upload file (multipart/form-data)
- `GET /api/services/app/FileUpload/DownloadFile?fileUrl={url}` - Download file

## 8. Testing Requirements

### 8.1 Unit Tests
- Application service methods
- Validation logic
- Business rules

### 8.2 Integration Tests
- API endpoints
- Database operations
- File upload/download

### 8.3 UI Tests
- Component rendering
- Form validation
- User interactions

## 9. Deployment Requirements

### 9.1 Environment Setup
- SQL Server database
- .NET 9.0 runtime
- Node.js 18+ for Angular build
- IIS or Kestrel web server

### 9.2 Configuration
- Connection string
- CORS origins
- JWT settings
- File upload path

## 10. Future Enhancements

1. **Email Notifications**: Send email when no feedbacks received
2. **Export Functionality**: Export feedbacks to Excel/CSV
3. **Advanced Analytics**: Charts and graphs for feedback trends
4. **Comment Threading**: Reply to feedbacks
5. **File Storage**: Move to Azure Blob Storage or AWS S3
6. **Real-time Updates**: SignalR for live dashboard updates
7. **Mobile App**: React Native or Flutter mobile application

## 11. Acceptance Criteria

✅ All CRUD operations work correctly  
✅ Server-side validation implemented  
✅ File upload/download functional  
✅ Permissions enforced on backend and frontend  
✅ Tenant setting prevents feedback creation when limit reached  
✅ Background job runs daily and logs warnings  
✅ Dashboard displays accurate statistics  
✅ Search, pagination, and filters work on all lists  
✅ UI is responsive and user-friendly  
✅ All code follows ABP Framework conventions  

## 12. AI Tools Usage

This PRD and implementation were created with assistance from:
- **Cursor AI**: Code generation, refactoring, error fixing
- **ChatGPT**: Architecture decisions, best practices, documentation

AI was used for:
- Generating PRD structure
- Creating entity classes and DTOs
- Writing application services
- Scaffolding Angular components
- Writing documentation
- Code review and optimization

---

**Document Status**: Complete  
**Last Updated**: 2024  
**Next Review**: As needed

