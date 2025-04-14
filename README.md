# Web Application Code Generator

The **Web Application Code Generator** is a sophisticated tool designed to **streamline the application development process**, specifically targeting the generation of Razor engine pages and essential database methods. It helps developers accelerate their workflow and reduce the need for repetitive coding tasks.

## 🚀 Features

- **Razor Page Generator**  
  Automatically creates Razor engine pages to help quickly scaffold UI components.

- **CRUD Method Generator**  
  Simplifies the creation of standard Create, Read, Update, and Delete (CRUD) operations for database tables.

- **SQL Code Generation**  
  Generates SQL queries and stored procedures (e.g., `SaveOrUpdateTableName`) in a clean, copyable format.

- **Text-Based Output**  
  Outputs generated SQL artifacts and Razor components into a user-friendly textarea for easy copy-paste integration into your project.

## 🛠️ Benefits

- Save time by automating repetitive coding tasks.
- Avoid human errors in boilerplate SQL and Razor code.
- Easily integrate generated code into existing projects.
- Speed up prototyping and development phases.

## 📦 Usage

1. Open the Web Application Code Generator.
2. Choose the type of code to generate (Razor page, CRUD methods, SQL procedure).
3. Provide necessary inputs (e.g., table name, column names).
4. Click "Generate."
5. Copy the generated code from the output textarea.
6. Paste the code into your project.

## 📋 Example Output

### Razor Page Example
```cshtml
@model YourProject.Models.TableName
<form method="post">
    <!-- Input fields for each property -->
    <button type="submit">Save</button>
</form>
```

### SQL Stored Procedure
```sql
CREATE PROCEDURE SaveOrUpdateTableName
    @Id INT,
    @Name NVARCHAR(100)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM TableName WHERE Id = @Id)
        UPDATE TableName SET Name = @Name WHERE Id = @Id
    ELSE
        INSERT INTO TableName (Name) VALUES (@Name)
END
```

## 💡 Tip

You can use the output directly in your ASP.NET MVC projects or any other .NET-based application that uses Razor and SQL Server.

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
