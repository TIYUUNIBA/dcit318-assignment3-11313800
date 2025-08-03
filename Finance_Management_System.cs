using System;
using System.Collections.Generic;

// Core immutable transaction record
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Transaction processing interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Concrete processor implementations
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer: ${transaction.Amount} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing mobile money: ${transaction.Amount} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto transaction: ${transaction.Amount} for {transaction.Category}");
    }
}

// Base Account class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Applied transaction. New balance: ${Balance}");
    }
}

// Sealed specialized account
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance) 
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            base.ApplyTransaction(transaction);
        }
    }
}

// Main application
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        // Create account with initial balance
        var account = new SavingsAccount("SAV-12345", 1000m);
        Console.WriteLine($"Initial balance: ${account.Balance}\n");

        // Create sample transactions
        var transactions = new[]
        {
            new Transaction(1, DateTime.Now, 150m, "Groceries"),
            new Transaction(2, DateTime.Now, 75m, "Utilities"),
            new Transaction(3, DateTime.Now, 200m, "Entertainment")
        };

        // Create processors
        var processors = new ITransactionProcessor[]
        {
            new MobileMoneyProcessor(),
            new BankTransferProcessor(),
            new CryptoWalletProcessor()
        };

        // Process and apply transactions
        for (int i = 0; i < transactions.Length; i++)
        {
            Console.WriteLine($"\nProcessing Transaction {transactions[i].Id}:");
            processors[i].Process(transactions[i]);
            account.ApplyTransaction(transactions[i]);
            _transactions.Add(transactions[i]);
        }

        // Final balance
        Console.WriteLine($"\nFinal balance: ${account.Balance}");
    }

    public static void Main()
    {
        new FinanceApp().Run();
    }
}