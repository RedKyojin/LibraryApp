using System;
using System.Linq;
using System.Collections.Generic;
using LibraryApp.Data;
using LibraryApp.Models;

namespace LibraryApp
{
    class Program
    {
        static void Main()
        {
            using var db = new LibraryContext();
            var library = new LibraryCollection();

            //Load existing resources from the database to array-list
            var all = db.Resources.ToList();
            foreach (var r in all)
                library.Add(r);

            Console.WriteLine($"Loaded {library.Count} resources from DB.");

            //Main console loop
            while (true)
            {
                Console.WriteLine("\n--- Library Menu ---");
                Console.WriteLine("1) Add Resource");
                Console.WriteLine("2) Remove Resource");
                Console.WriteLine("3) Search by Title");
                Console.WriteLine("4) Search by Author");
                Console.WriteLine("5) Search by Genre");
                Console.WriteLine("6) Check Resource");
                Console.WriteLine("7) Return Resource");
                Console.WriteLine("8) Overdue Report");
                Console.WriteLine("9) Exit");
                Console.Write("Choice> ");
                var choice = Console.ReadLine();

                if (choice == "1")
                    AddResource(db, library);
                else if (choice == "2")
                    RemoveResource(db, library);
                else if (choice == "3")
                    SearchByTitle(library);
                else if (choice == "4")
                    SearchByAuthor(library);
                else if (choice == "5")
                    ReportByGenre(db);
                else if (choice == "6")
                    CheckOut(db, library);
                else if (choice == "7")
                    ReturnResource(db, library);
                else if (choice == "8")
                    OverdueReport(db);
                else if (choice == "9")
                    break;
                else
                    Console.WriteLine("Invalid option, try again.");
            }
        }

        //Add a new resource
        static void AddResource(LibraryContext db, LibraryCollection library)
        {
            var r = new Resource();
            string input;
            // Title
            do
            {
                Console.Write("Title: ");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Title cannot be empty.");
            } while (string.IsNullOrWhiteSpace(input));
            r.Title = input;
            // Author
            do
            {
                Console.Write("Author: ");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Author cannot be empty.");
            } while (string.IsNullOrWhiteSpace(input));
            r.Author = input;
            // Year
            int year;
            while (true)
            {
                Console.Write("Year: ");
                if (int.TryParse(Console.ReadLine(), out year) && year > 0)
                {
                    r.PublicationYear = year;
                    break;
                }
                Console.WriteLine("Please enter a valid year (number greater than 0).");
            }
            // Genre
            do
            {
                Console.Write("Genre: ");
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    Console.WriteLine("Genre cannot be empty.");
            } while (string.IsNullOrWhiteSpace(input));
            r.Genre = input;
            r.IsAvailable = true;
            db.Resources.Add(r);
            db.SaveChanges();
            library.Add(r);
        }

        //Remove resource
        static void RemoveResource(LibraryContext db, LibraryCollection library)
        {
            Console.Write("Enter ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }
            bool removed = library.Remove(id);
            if (removed)
            {
                var toRemove = db.Resources.Find(id);
                if (toRemove != null)
                {
                    db.Resources.Remove(toRemove);
                    db.SaveChanges();
                }
            }
        }

        //Search by title
        static void SearchByTitle(LibraryCollection library)
        {
            Console.Write("Title to search: ");
            var title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Search title cannot be empty.");
                return;
            }
            var found = library.SearchByTitle(title);
            if (found == null)
                Console.WriteLine("No resource found with that title.");
            else
                Console.WriteLine($"Found: {found.Id}) {found.Title} by {found.Author}");
        }

        //Search by author
        static void SearchByAuthor(LibraryCollection library)
        {
            Console.Write("Author to search: ");
            var author = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("Search author cannot be empty.");
                return;
            }
            var list = library.SearchByAuthor(author);
            if (list.Length == 0)
                Console.WriteLine("No resources found for that author.");
            else
            {
                Console.WriteLine($"Found {list.Length} items:");
                foreach (var r in list)
                    Console.WriteLine($" {r.Id}) {r.Title} ({r.PublicationYear})");
            }
        }

        static void CheckOut(LibraryContext db, LibraryCollection library)
        {
            Console.WriteLine("Enter ID to check out: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }
            var item = db.Resources.Find(id);
            if (item == null || !item.IsAvailable)
            {
                Console.WriteLine("Cannot check out: item not found or already checked out.");
                return;
            }
            item.IsAvailable = false;
            item.DueDate = DateTime.Now.AddDays(14);
            db.SaveChanges();
            library.Remove(id);
            library.Add(item);
            Console.WriteLine($"Checked out '{item.Title}'. Due on {item.DueDate:yyyy-MM-dd}.");
        }

        static void ReturnResource(LibraryContext db, LibraryCollection library)
        {
            Console.Write("Enter ID to return: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }
            var item = db.Resources.Find(id);
            if (item == null || item.IsAvailable)
            {
                Console.WriteLine("Cannot return: item not found or not checked out.");
                return;
            }
            item.IsAvailable = true;
            item.DueDate = null;
            db.SaveChanges();
            library.Remove(id);
            library.Add(item);
            Console.WriteLine($"Returned '{item.Title}'. Thank you!");
        }

        static void OverdueReport(LibraryContext db)
        {
            Console.WriteLine("Overdue items:");
            bool any = false;

            // Manually loop through each resource
            foreach (var r in db.Resources)
            {
                if (r.DueDate != null && r.DueDate < DateTime.Now)
                {
                    Console.WriteLine("  " + r.Id
                        + ") " + r.Title
                        + " - due " + r.DueDate.Value.ToString("yyyy-MM-dd"));
                    any = true;
                }
            }

            if (!any)
            {
                Console.WriteLine("No overdue items. All good!");
            }
        }
        //To list all resources of a specific genre
        static void ReportByGenre(LibraryContext db)
        {
            Console.Write("Enter genre: ");
            string genre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(genre))
            {
                Console.WriteLine("Please type a genre.");
                return;
            }
            genre = genre.ToLower();
            List<Resource> matches = new List<Resource>();
            foreach (Resource r in db.Resources)
            {
                if (r.Genre != null && r.Genre.ToLower() == genre)
                {
                    matches.Add(r);
                }
            }
            if (matches.Count == 0)
            {
                Console.WriteLine("No resources found in genre '" + genre + "'.");
            }
            else
            {
                Console.WriteLine("Resources in '" + genre + "':");
                for (int i = 0; i < matches.Count; i++)
                {
                    Resource r2 = matches[i];
                    string status;
                    if (r2.IsAvailable)
                        status = "Available";
                    else
                        status = "Checked out (due " + r2.DueDate + ")";
                    Console.WriteLine(
                        r2.Id + ") "
                      + r2.Title + " by "
                      + r2.Author + " ("
                      + r2.PublicationYear + ") - "
                      + status
                    );
                }
            }
        }
    }
}
