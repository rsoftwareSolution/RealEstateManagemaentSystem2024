using System;
using System.Collections.Generic;
using System.Linq;

public class AutoIdGenerator
{
    private int counter = 0;  // Keeps track of the ID number
    private string prefix;    // Prefix for the ID
    private int length;       // Length of the numeric part
    private List<string> existingIds; // Simulating database of existing IDs

    // Constructor to initialize the generator with a prefix and numeric length
    public AutoIdGenerator(string prefix, int length = 4)
    {
        this.prefix = prefix;
        this.length = length;
        existingIds = new List<string>(); // Simulating existing records in DB
    }

    // Method to simulate database check (replace with actual DB check)
    private bool IsIdExist(string id)
    {
        // Simulating a database check. Replace this with actual database query logic.
        return existingIds.Contains(id);
    }

    // Method to generate a unique ID based on prefix and counter
    public string GenerateNextId()
    {
        string newId;

        // Keep generating new IDs until a unique one is found
        do
        {
            // Increment counter to generate the next ID
            counter++;
            newId = $"{prefix}{counter.ToString().PadLeft(length, '0')}"; // Format the ID as prefix + counter

            // Check if this ID exists in the mock database
        } while (IsIdExist(newId));

        // Simulate inserting the ID into the database (add to list)
        existingIds.Add(newId);

        // Return the unique ID for use in the new record insertion
        return newId;
    }

    // Reset the counter to start from a specific value, e.g., for a new batch
    public void ResetCounter(int startValue = 0)
    {
        counter = startValue;
    }
}
