using System.Text.Json;
using System.Text.Json.Serialization;
using Bank;
class Program
{
    static async Task Main()
    {
        // int[] numbers = { 1 };

        // //tipos anuláveis
        // string s = null;

        // Console.WriteLine(string.IsNullOrWhiteSpace(s));


        // //tratamento de erros
        // try
        // {
        //   //Console.WriteLine(numbers[1]);
        // }
        // catch (IndexOutOfRangeException)
        // {
        //   //Console.WriteLine("Erro IndexOutOfRangeException");
        // }
        // catch (Exception exception)
        // {
        //   //Console.WriteLine($"Eu tenteiiii {exception}");
        // }


        //tipos de referência e de valor
        Teste t = new Teste();
        t.x = 10;

        Teste t2 = t;
        t2.x = 20;
        //Console.WriteLine(t);

        //chamda objeto Conta
        ILogger logger = new FileLogger("mylog.txt");
        BankAccount account1 = new BankAccount("Fred", 100, logger);
        BankAccount account2 = new BankAccount("Mariana", 100, logger);

        string json = JsonSerializer.Serialize(account1);
        Console.WriteLine($"Meu Json -> {json}");

        account1 = JsonSerializer.Deserialize<BankAccount>(json);

        List<BankAccount> accounts = new List<BankAccount>
        {
            account1,
            account2
        };

        List<int> numbers = new List<int>() { 1, 2, 3, 4 };

        foreach (int number in numbers)
        {
            Console.WriteLine(number);
        }

        foreach (BankAccount account in accounts)
        {
            Console.WriteLine(account.Balance);
        }

        // account1.Deposit(-1400);
        // account2.Deposit(1400);

        //Console.WriteLine(account2.GetBalance());
        // Console.WriteLine(account2.Balance);

        DataStore<string> store = new DataStore<string>();

        store.Value = "fred";
        Console.WriteLine(store.Value.Length);

        var calculate = new Calculate(Sum);
        var result = calculate(10, 20);
        Console.WriteLine(result);

        //Run(calculate);

        var multiply = new Calculate(Multiply);
        //Run(multiply);

        //func anonima 
        var dividir = delegate (int x, int y) { return x / y; };
        //expressão lambda 
        var dividir2 = (int x, int y) => x / y;

        Console.WriteLine($"Minha divisão = {calculate(10, 5)}");



        Func<decimal> test2 = delegate () { return 4.2m; };
        Func<decimal> test3 = () => 4.2m;

        Console.WriteLine(test2());

        Run((x, y) => x * y);

        "Testando".WriteLine(ConsoleColor.Red);

        var a = "bastardo";

        a.WriteLine(ConsoleColor.Cyan);

        var logger2 = new ConsoleLogger();
        logger2.Test();

        int[] numbersTest = { 1, 4, 10, 42, 56 };
        var query = from number in numbersTest
                    where number < 10
                    select number;
        //sintaxe de método
        var query2 = numbersTest.Where(number => number < 10);

        var result2 = query2.ToList();

        Console.WriteLine(result2.Count());

        foreach (var numeros in query2)
        {
            Console.WriteLine(numeros);
        }

        var query3 = numbersTest.Where(number => number < 8)
                                .First();
        query3 = numbersTest.First(number => number < 8);
        query3 = numbersTest.FirstOrDefault(number => number < 8);

        var query4 = numbersTest.OrderByDescending(number => number);

        var accounts2 = new List<BankAccount>
        {
            new BankAccount("Fred", 100)
            {
                Branch = "123"
            },
            new BankAccount("Mariana", 50)
            {
                Branch = "321"
            },
            new BankAccount("Teste", 200)
            {
                Branch = "321"
            },
        };

        var acc = accounts2.OrderBy(account => account.Balance).ToArray();
        var acc2 = accounts2.GroupBy(account => account.Branch);

        foreach (var group in acc2)
        {
            Console.WriteLine($"Agencia: {group.Key}");
            Console.WriteLine("---");
            foreach (var accountIn in group)
            {
                Console.WriteLine($"{accountIn.Name} possui ${accountIn.Balance}");
            }
            Console.WriteLine("---");
        }

        var namesQuery = accounts2.Select(account => $"{account.Name} ${account.Balance}");
        var namesQuery2 = accounts2.Select(account => new BranchCustomer { Name = account.Name, Branch = account.Branch });
        var namesQuery3 = accounts2.Select(account => new { account.Name, account.Branch });

        var test = Enumerable.Empty<int>();

        var random = new Random();
        var range = Enumerable.Range(0, 5).Select(_ => random.Next(1, 100));

        var range2 = Enumerable.Range(0, 26).Select(number => (char)('a' + number));

        Console.WriteLine("Executando...");
        //Task.Run(() => Thread.Sleep(5000));//async e sync

        await Task.Run(() =>
        {
            Thread.Sleep(5000);
            Console.WriteLine("Acordou...");
        });
        Console.WriteLine("Pronto!");
    }

    static void Run(Func<int, int, int> calc) //Calcular calc
    {
        Console.WriteLine($"Cálculo -> {calc(20, 30)}");
    }

    static int Sum(int a, int b)
    {
        return a + b;
    }

    static int Multiply(int x, int y)
    {
        return x * y;
    }
}

class BranchCustomer
{
    public string Name { get; set; }
    public string Branch { get; set; }
}
namespace Bank
{
    public static class Extension
    {
        public static void WriteLine(this string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public static void Test(this ILogger logger)
        {
        }
    }
    public delegate int Calculate(int x, int y);

    public class FileLogger : ILogger
    {
        private readonly string filePath;

        public FileLogger(string filePath)
        {
            this.filePath = filePath;
        }
        public void Log(string message)
        {
            File.AppendAllText(filePath, $"{message}{Environment.NewLine}");
        }
    }

    public class DataStore<T>
    {
        public T Value
        {
            get;
            set;
        }
    }

    public class ConsoleLogger : ILogger
    {
    }

    public interface ILogger
    {
        void Log(string message)
        {
            Console.WriteLine($"LOGGER: {message}");
        }
    }

    public class BankAccount
    {
        private readonly ILogger logger;
        public string Name
        {
            get; private set;
        }
        //prop
        public decimal Balance
        {
            get; private set;
        }

        public string Branch;

        [JsonConstructor]

        public BankAccount(string name, decimal balance) : this(name, balance, new ConsoleLogger())
        {

        }
        public BankAccount(string name, decimal balance, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Nome inválido.", nameof(name));
            }
            if (balance < 0)
            {
                throw new Exception("Saldo não pode ser negativo.");
            }
            Name = name;
            Balance = balance;
            this.logger = logger;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                logger.Log($"Não é possível depositar {amount} na conta de {Name}");
                return;
            }
            //balance += amount;
            Balance += amount;
        }
        //uma maneira alternativa ao prop
        public decimal GetBalance()
        {
            return Balance;
        }
    }

    public class Teste
    {
        public int x;
    }
}
