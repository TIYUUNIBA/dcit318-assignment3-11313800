using System;
using System.Collections.Generic;

// Marker Interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Custom Exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// Product Classes
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString() => 
        $"[{Id}] {Name} (Brand: {Brand}, Warranty: {WarrantyMonths} months) - Qty: {Quantity}";
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString() => 
        $"[{Id}] {Name} (Expires: {ExpiryDate:yyyy-MM-dd}) - Qty: {Quantity}";
}

// Generic Inventory Repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items.Add(item.Id, item);
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        _items.Remove(id);
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// Warehouse Manager
public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        try
        {
            // Add electronic items
            _electronics.AddItem(new ElectronicItem(1, "Smartphone", 50, "Samsung", 24));
            _electronics.AddItem(new ElectronicItem(2, "Laptop", 30, "Dell", 36));
            
            // Add grocery items
            _groceries.AddItem(new GroceryItem(101, "Milk", 200, DateTime.Now.AddDays(14)));
            _groceries.AddItem(new GroceryItem(102, "Bread", 150, DateTime.Now.AddDays(7)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine($"=== {typeof(T).Name}s ===");
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
        Console.WriteLine();
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Updated {typeof(T).Name} {id}: Added {quantity} units");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed {typeof(T).Name} with ID {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        var manager = new WareHouseManager();
        
        // Seed initial data
        manager.SeedData();
        
        // Print all items
        manager.PrintAllItems(manager._groceries);
        manager.PrintAllItems(manager._electronics);

        // Test exception scenarios
        Console.WriteLine("=== Testing Exception Handling ===");
        
        // Try to add duplicate item
        try
        {
            manager._electronics.AddItem(new ElectronicItem(1, "Duplicate Phone", 10, "Apple", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Duplicate Test: {ex.Message}");
        }

        // Try to remove non-existent item
        try
        {
            manager.RemoveItemById(manager._groceries, 999);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Non-existent Test: {ex.Message}");
        }

        // Try to update with invalid quantity
        try
        {
            manager._groceries.UpdateQuantity(101, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Invalid Quantity Test: {ex.Message}");
        }

        // Successful operations
        Console.WriteLine("\n=== Testing Valid Operations ===");
        manager.IncreaseStock(manager._electronics, 2, 10);  // Add 10 laptops
        manager.RemoveItemById(manager._groceries, 101);    // Remove milk
        
        // Print updated inventories
        Console.WriteLine("\n=== Updated Inventories ===");
        manager.PrintAllItems(manager._groceries);
        manager.PrintAllItems(manager._electronics);
    }
}