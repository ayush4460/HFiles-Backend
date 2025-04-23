# HFiles Medical Dashboard Backend API

This is the backend API for the Medical Record Dashboard, built with ASP.NET Core 8, Entity Framework Core, and MySQL. It handles user authentication, profile management, file uploads, and session-based authorization.


## 🔧 Local Setup

### 1. Clone the Repository
• git clone https://github.com/ayush4460/HFiles-Backend.git

### 2. Set Up MySQL Database
Create a database named medical_db in MySQL:

• CREATE DATABASE medical_db;

Add the connection string to appsettings.json:

In appsettings.json, configure the connection string to connect to your local MySQL database:

{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;user=root;password=yourpassword;database=medical_db"
  }
}

Replace yourpassword with the actual password for your MySQL root user.

### 3. Run the App
• dotnet restore

• dotnet build

• dotnet run

The API will be available at http://localhost:5138

### 4. Run Migrations
• dotnet ef migrations add InitialCreate

• dotnet ef database update

This will create the required tables in the medical_db database.


## 📁 API Endpoints

### 🔐 Auth

#### POST /api/auth/signup

Create a new user account.

Request body:

{

  "fullName": "John Doe",
  
  "email": "john@example.com",
  
  "password": "yourpassword",
  
  "phoneNumber": "1234567890",
  
  "gender": "Male"
  
}


#### POST /api/auth/login

Authenticate and log in an existing user.

Request body:

{

  "email": "john@example.com",
  
  "password": "yourpassword"
  
}


#### POST /api/auth/logout

Log out the currently authenticated user.


### 👤 User

#### GET /api/user/me

Get the profile information of the currently authenticated user.


#### PUT /api/user/me

Update the profile information of the currently authenticated user.

Request body:

{

  "fullName": "John Doe",
  
  "email": "john@example.com",
  
  "phoneNumber": "0987654321",
  
  "gender": "Male"
  
}


### 📄 Files

#### POST /api/file/upload

Upload a new file (PDF or image).

Request body:

The file should be uploaded as multipart/form-data. You can use Postman or any HTTP client to test file uploads.


#### GET /api/file

Get a list of all uploaded files.


#### DELETE /api/file/{id}

Delete a file by its ID.



## 🛠️ Tools and Technologies Used

• ASP.NET Core 8 for building the backend API
• Entity Framework Core for interacting with the database
• MySQL for the database
• JWT Authentication for secure API access
• Postman for testing APIs


