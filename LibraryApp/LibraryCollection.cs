using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryApp.Models;

namespace LibraryApp
{
    // An array-backed list to hold Resources in memory
    public class LibraryCollection
    {
        private Resource[] items;
        private int count;

        // Constructor: set up the array with a capacity, default 100
        public LibraryCollection(int capacity = 100)
        {
            items = new Resource[capacity];
            count = 0;
        }
        // Add a Resource at the end, if there’s room
        public void Add(Resource r)
        {
            if (count >= items.Length)
            {
                Console.WriteLine("Collection is full, more items cannot be added.");
                return;
            }

            items[count] = r;   // put it in the next free slot
            count++;
            Console.WriteLine($"Added '{r.Title}' (ID={r.Id}).");
        }
        // Remove a Resource by its ID
        public bool Remove(int id)
        {
            for (int i = 0; i < count; i++)
            {
                if (items[i].Id == id)
                {
                    // swap the last item into this spot
                    items[i] = items[count - 1];
                    items[count - 1] = null;  // clear out reference
                    count--;                  // shrink the list
                    Console.WriteLine($"Removed item ID={id}.");
                    return true;
                }
            }
            Console.WriteLine($"No item found with ID={id}.");
            return false;
        }

        // Find the first Resource whose Title matches
        public Resource SearchByTitle(string title)
        {
            for (int i = 0; i < count; i++)
            {
                if (items[i].Title.Equals(
                        title, StringComparison.OrdinalIgnoreCase))
                {
                    return items[i];
                }
            }
            return null;
        }

        // Find all Resources by a given author
        public Resource[] SearchByAuthor(string author)
        {
            var results = new List<Resource>();
            for (int i = 0; i < count; i++)
            {
                if (items[i].Author.Equals(
                        author, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(items[i]);
                }
            }
            return results.ToArray();
        }

        // Count for how many items are loaded
        public int Count => count;
    }
}

