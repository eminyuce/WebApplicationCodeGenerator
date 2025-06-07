# DAO Generator 

## **📌 Overview**
This **DAO (Data Access Object) Generator** is a web-based tool that automates the creation of database-related code for .NET applications. It connects to a database, extracts table schemas, and generates:
- **C# Model Classes** (Entities)
- **Repository Classes** (Data Access Layer)
- **Stored Procedures** (SQL)
- **ASP.NET MVC Controllers & Views**
- **Database Utility Methods**

Designed to **speed up development** and **reduce manual coding errors**.

---

## **🚀 Features**
✅ **Supports SQL Server & MySQL**  
✅ **Generates CRUD operations** (Create, Read, Update, Delete)  
✅ **Automatic detection of primary/foreign keys**  
✅ **Customizable UI controls per column** (TextBox, DropDown, CheckBox, etc.)  
✅ **Built-in validation rules** (Required, Range, Regex)  
✅ **Export generated code as `.txt`**  

---

## **🛠 Setup & Usage**
### **1. Prerequisites**
- **.NET Framework 4.5+** (for the generator itself)
- **SQL Server / MySQL Database** (to connect and generate code)

### **2. Running the Generator**
1. **Launch the Web Application** (hosted in IIS or run locally).
2. **Enter Connection String** (e.g., `Server=.;Database=MyDB;Integrated Security=True;`).
3. **Select a Table** from the dropdown.
4. **Customize** (optional):
   - Namespace (`ProjectNameSpace` by default).
   - Entity name (defaults to table name).
   - UI controls for each column.
5. **Click "Generate"** → Code appears in textboxes.
6. **Download** or copy the generated code.

---

## **📂 Generated Code Structure**
### **1. Entity Class (Model)**
```csharp
[Table("Products")]
public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public decimal Price { get; set; }
}
```

### **2. Repository Class (DAL)**
```csharp
public class ProductRepository
{
    public List<Product> GetProducts() { ... }
    public int SaveOrUpdateProduct(Product item) { ... }
    public void DeleteProduct(int id) { ... }
}
```

### **3. Stored Procedure (SQL)**
```sql
CREATE PROCEDURE SaveOrUpdateProduct(
    @ProductId INT = NULL,
    @Name NVARCHAR(100),
    @Price DECIMAL(18,2)
)
AS
BEGIN
    -- MERGE or INSERT/UPDATE logic
END
```

### **4. ASP.NET MVC Controller**
```csharp
public class ProductController : Controller
{
    public ActionResult Index() { ... }
    public ActionResult Edit(int id) { ... }
    [HttpPost]
    public ActionResult Edit(Product product) { ... }
}
```

### **5. Razor Views (`Index.cshtml`, `Edit.cshtml`)**
```html
@model List<Product>
<table>
    @foreach (var item in Model)
    {
        <tr>
            <td>@item.Name</td>
            <td>@Html.ActionLink("Edit", "Edit", new { id = item.ProductId })</td>
        </tr>
    }
</table>
```

---

## **🔧 Customization**
### **Modifying Generated Code**
- **Change UI Controls**: Adjust `DropDownList_Control` in the GridView.
- **Add Validation**: Use `DropDownList_Validator` (e.g., RequiredFieldValidator).
- **Edit Stored Procedure Logic**: Modify the generated SQL before execution.

### **Extending Functionality**
- **Support More Databases**: Extend `GetirTabloları()` for PostgreSQL/Oracle.
- **Add More Templates**: Modify `generateASpNetMvcEditOrCreate()` for Blazor/Web API.

---

## **📜 License**
MIT License - Free for personal/commercial use.

---

## **🎯 Why Use This?**
✔ **Saves hours** of manual coding  
✔ **Reduces errors** in database operations  
✔ **Consistent code structure** across projects  



## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
**Happy Coding! 🚀**
