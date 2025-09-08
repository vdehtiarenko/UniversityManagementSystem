
# **UniversityManagementSystem**

An application for university management that allows you to manage courses, groups, students, and teachers, organizing the educational process.

# Table of Contents

- [Description](#description)
- [Technologies](#technologies)
- [Project Setup](#project-setup)
- [User Interface](#user-interface)

# Description

UniversityManagementSystem is a WPF application based on MVVM architecture designed for effective university management. Users can manage courses, groups, students, and teachers by performing CRUD operations and organizing the educational process. The program allows you to easily manage course and group structures, as well as control information about students and teachers. A separate section has been implemented for managing teachers with the ability to assign them to groups. This ensures flexible planning and coordination of the educational process. The program facilitates university administration and stores all data in a structured form.

# Technologies

- C# / .NET 8
- WPF (MVVM)
- SQL Server
- Entity Framework Core

# Project Setup

1. **Cloning a repository**  
Clone the repository and open the project in **Visual Studio**:
```bash
git clone <your-repository-url>
```

2. **Creating the database**
Create a new database in your SQL Server with the desired name for your project.

3. **Configuring the database connection**
Open the ApplicationDbContext.cs file and locate the OnConfiguring method. Replace the connection string with your own.

4. **Applying migrations**
Open the Package Manager Console and run:
```bash
Update-Database
```

# User Interface
<img width="1291" height="760" alt="Image" src="https://github.com/user-attachments/assets/3c312d16-a6bf-4978-b7e8-3f7b8342a63f" />
<img width="1294" height="759" alt="Image" src="https://github.com/user-attachments/assets/1abddeee-75fb-4f69-9c2d-9fef0b6e9ead" />
<img width="380" height="359" alt="Image" src="https://github.com/user-attachments/assets/8e551a7f-f740-40d4-b8a1-a6b862f47f4d" /> <img width="381" height="238" alt="Image" src="https://github.com/user-attachments/assets/c9d24892-2aae-4fa8-8e64-e64091d32302" />
<img width="1292" height="750" alt="Image" src="https://github.com/user-attachments/assets/c23cc406-0116-4e59-8ad6-1933af606b0c" />
<img width="1291" height="761" alt="Image" src="https://github.com/user-attachments/assets/ac8620de-8a9b-40f6-ac13-1d7031341bfb" />
