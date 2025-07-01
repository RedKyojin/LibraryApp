# LibraryApp

## What it does

A simple console app to manage a small library of books and other resources. Data is stored in SQL Server LocalDB, and an in-memory list is used for fast lookups.

## How to set up

1. Clone this repo:

   ```bash
   git clone https://github.com/yourusername/LibraryApp.git
   cd LibraryApp
   ```
2. Install EF Core CLI if needed:

   ```bash
   dotnet tool install -g dotnet-ef
   ```
3. Create the database:

   ```bash
   dotnet ef database update
   ```
4. Build the project:

   ```bash
   dotnet build
   ```
5. Run the app:

   ```bash
   dotnet run --project LibraryApp
   ```

## Menu options

When you run the app, you�ll see a menu:

```
1) Add Resource
2) Remove Resource
3) Search by Title
4) Search by Author
5) Search by Genre
6) Check Out Resource
7) Return Resource
8) Overdue Report
9) Exit
```

Enter the number and follow prompts.

## Main files

* **Program.cs**: console UI and commands
* **LibraryCollection.cs**: simple array-backed list
* **Data/LibraryContext.cs**: EF Core setup
* **Models/Resource.cs**: resource properties
* **Migrations/**: database migrations

## Resource data model

Each resource has:

* **Id** (int)
* **Title**, **Author**, **Genre** (string)
* **PublicationYear** (int)
* **IsAvailable** (bool)
* **DueDate** (DateTime?)

## Requirements

* .NET 8.0 SDK
* SQL Server LocalDB (installed with Visual Studio)

Enjoy using LibraryApp!
