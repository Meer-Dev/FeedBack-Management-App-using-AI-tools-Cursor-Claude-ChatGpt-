FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/CourseFeedbackSystem.Web.Host/CourseFeedbackSystem.Web.Host.csproj", "src/CourseFeedbackSystem.Web.Host/"]
COPY ["src/CourseFeedbackSystem.Web.Core/CourseFeedbackSystem.Web.Core.csproj", "src/CourseFeedbackSystem.Web.Core/"]
COPY ["src/CourseFeedbackSystem.Application/CourseFeedbackSystem.Application.csproj", "src/CourseFeedbackSystem.Application/"]
COPY ["src/CourseFeedbackSystem.Core/CourseFeedbackSystem.Core.csproj", "src/CourseFeedbackSystem.Core/"]
COPY ["src/CourseFeedbackSystem.EntityFrameworkCore/CourseFeedbackSystem.EntityFrameworkCore.csproj", "src/CourseFeedbackSystem.EntityFrameworkCore/"]
WORKDIR "/src/src/CourseFeedbackSystem.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/CourseFeedbackSystem.Web.Host", "src/CourseFeedbackSystem.Web.Host"]
COPY ["src/CourseFeedbackSystem.Web.Core", "src/CourseFeedbackSystem.Web.Core"]
COPY ["src/CourseFeedbackSystem.Application", "src/CourseFeedbackSystem.Application"]
COPY ["src/CourseFeedbackSystem.Core", "src/CourseFeedbackSystem.Core"]
COPY ["src/CourseFeedbackSystem.EntityFrameworkCore", "src/CourseFeedbackSystem.EntityFrameworkCore"]
WORKDIR "/src/src/CourseFeedbackSystem.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "CourseFeedbackSystem.Web.Host.dll"]
