using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker Interface
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            using var writer = new StreamWriter(_filePath);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_log, options);
            writer.Write(json);
            Console.WriteLine($"Successfully saved {_log.Count} items to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No existing data file found.");
                return;
            }

            using var reader = new StreamReader(_filePath);
            var json = reader.ReadToEnd();
            var items = JsonSerializer.Deserialize<List<T>>(json);

            if (items != null)
            {
                _log.Clear();
                _log.AddRange(items);
                Console.WriteLine($"Successfully loaded {items.Count} items from {_filePath}");
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// Integration Layer
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp()
    {
        _logger = new InventoryLogger<InventoryItem>("inventory.json");
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-30)));
        _logger.Add(new InventoryItem(2, "Monitor", 15, DateTime.Now.AddDays(-15)));
        _logger.Add(new InventoryItem(3, "Keyboard", 25, DateTime.Now.AddDays(-7)));
        _logger.Add(new InventoryItem(4, "Mouse", 50, DateTime.Now.AddDays(-3)));
        _logger.Add(new InventoryItem(5, "Headphones", 20, DateTime.Now));
        
        Console.WriteLine("Added 5 sample inventory items");
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        Console.WriteLine("\n=== Current Inventory ===");
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"[{item.Id}] {item.Name} - Qty: {item.Quantity} (Added: {item.DateAdded:yyyy-MM-dd})");
        }
    }
}

// Main Program
class Program
{
    static void Main()
    {
        var app = new InventoryApp();

        // Seed and save data
        app.SeedSampleData();
        app.SaveData();

        // Clear memory (simulate new session)
        Console.WriteLine("\nSimulating application restart...\n");

        // Create new instance and load data
        var newApp = new InventoryApp();
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}