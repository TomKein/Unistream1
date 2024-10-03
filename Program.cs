using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Unistream1;

public class Transaction
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
}

public class TransactionContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("TransactionDB");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
            
        using var context = new TransactionContext();

        Console.WriteLine("Тестовое задание для Юнистрим #1");

        while (true)
        {
            Console.Write("Введите команду (add, get, exit): ");
            var command = Console.ReadLine()?.Trim().ToLower();

            switch (command)
            {
                case "add":
                    AddTransaction(context);
                    break;
                case "get":
                    GetTransaction(context);
                    break;
                case "exit":
                    Console.WriteLine("Выход из приложения. До свидания!");
                    return;
                default:
                    Console.WriteLine("[Ошибка] Неизвестная команда. Пожалуйста, введите 'add', 'get' или 'exit'.");
                    break;
            }
        }
    }

    static void AddTransaction(TransactionContext context)
    {
        try
        {
            Console.Write("Введите Id: ");
            var idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("[Ошибка] Неверный формат Id. Пожалуйста, введите целое число.");
                return;
            }

            Console.Write("Введите дату (dd.MM.yyyy): ");
            var dateInput = Console.ReadLine();
            if (!DateTime.TryParseExact(dateInput, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime transactionDate))
            {
                Console.WriteLine("[Ошибка] Неверный формат даты. Используйте формат dd.MM.yyyy.");
                return;
            }

            Console.Write("Введите сумму: ");
            var amountInput = Console.ReadLine();
            if (!decimal.TryParse(amountInput, out decimal amount))
            {
                Console.WriteLine("[Ошибка] Неверный формат суммы. Пожалуйста, введите число.");
                return;
            }

            if (context.Transactions.Any(t => t.Id == id))
            {
                Console.WriteLine("[Ошибка] Транзакция уже существует.");
                return;
            }

            var transaction = new Transaction
            {
                Id = id,
                TransactionDate = transactionDate,
                Amount = amount
            };

            context.Transactions.Add(transaction);
            context.SaveChanges();

            Console.WriteLine("[OK]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
    }

    static void GetTransaction(TransactionContext context)
    {
        try
        {
            Console.Write("Введите Id: ");
            var idInput = Console.ReadLine();
            if (!int.TryParse(idInput, out int id))
            {
                Console.WriteLine("[Ошибка] Неверный формат Id. Пожалуйста, введите целое число.");
                return;
            }

            var transaction = context.Transactions.FirstOrDefault(t => t.Id == id);
            if (transaction == null)
            {
                Console.WriteLine("[Ошибка] Транзакция не найдена.");
                return;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(transaction, options);
            Console.WriteLine(json);
            Console.WriteLine("[OK]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Ошибка] {ex.Message}");
        }
    }
}