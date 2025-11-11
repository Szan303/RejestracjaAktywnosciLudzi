using System.Text.Json;

class AktywnoscLudzi
{
    public string Nick { get; set; }
    public DateTime Date { get; set; }
    public bool NaUrlopie { get; set; }
    public DateTime? KoniecUrlopu { get; set; }
    public AktywnoscLudzi(string nick, DateTime date)
    {
        Nick = nick;
        Date = date;
        NaUrlopie = false;
        KoniecUrlopu = null;
    }
}

class Program
{
    static List<AktywnoscLudzi> aktywnosc = new List<AktywnoscLudzi>();
    static void Main()
    {
        OdczytajZPliku(); //wczytywanie z pliku przy starcie programu
        while (true)
        {

            Console.Clear();
            Console.WriteLine("Wybierz opcję:");
            Console.WriteLine("1. Zarejestruj aktywność");
            Console.WriteLine("2. Wyświetl zarejestrowane aktywności");
            Console.WriteLine("3. Dodaj urlop");
            Console.WriteLine("4. Zakończ program");
            string wybor = Console.ReadLine();
            switch (wybor)
            {
                case "1":
                    RejestrujAktywnosc();
                    break;
                case "2":
                    WyswietlAktywnosci();
                    break;
                case "3":
                    UstawUrlop();
                    break;
                case "4":
                    ZapiszDoPliku(); // zapisywanie do pliku przy zamknięciu programu
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                    Thread.Sleep(1000); // chwilka, żeby użytkownik zobaczył komunikat
                    break;
            }
        }
    }
    public static void RejestrujAktywnosc()
    {
        Console.Clear();
        Console.Write("Podaj nick: ");
        string nick = Console.ReadLine();
        DateTime date;

        Console.Write("Czy chcesz wpisać datę ręcznie? T / N : ");
        string odpowiedz = Console.ReadLine();
        if (odpowiedz == "T")
        {
            Console.Write("Podaj datę (rrrr-mm-dd): ");
            string input = Console.ReadLine();
            if (DateTime.TryParse(input, out DateTime dt))
            {
                date = dt;
                Console.WriteLine($"Wprowadzono datę: {date}");
            }
            else
            {
                Console.WriteLine("Nieprawidłowy format daty!");
                Thread.Sleep(1000);
                return;
            }
        }
        else if (odpowiedz == "N" || string.IsNullOrWhiteSpace(odpowiedz))
        {
            date = DateTime.Now;
        }
        else
        {
            Console.WriteLine("Zła odpowiedź");
            Thread.Sleep(1000);
            return;
        }

        if (string.IsNullOrWhiteSpace(nick))
        {
            Console.WriteLine("Nick nie może być pusty!");
            Thread.Sleep(1000);
            return;
        }

        var istniejacy = aktywnosc.FirstOrDefault(a => a.Nick.Equals(nick, StringComparison.OrdinalIgnoreCase));
        if (istniejacy != null)
        {
            aktywnosc.Remove(istniejacy);
            Console.WriteLine($"Zaktualizowano aktywność użytkownika: {nick}");
            Thread.Sleep(1000);
        }
        else
        {
            Console.WriteLine($"Zarejestrowano nową aktywność użytkownika: {nick}");
            Thread.Sleep(1000);
        }

        aktywnosc.Add(new AktywnoscLudzi(nick, date));
        ZapiszDoPliku();
        Console.WriteLine("Aktywność zapisana!");
        Thread.Sleep(1000); // chwilka, żeby użytkownik zobaczył komunikat


    }
    public static void WyswietlAktywnosci()
    {
        if (aktywnosc.Count == 0)
        {
            Console.WriteLine("Brak zarejestrowanych aktywności.");
            Thread.Sleep(1000); // chwilka, żeby użytkownik zobaczył komunikat
            return;
        }
        foreach (var a in aktywnosc)
        {
            if (a.NaUrlopie && a.KoniecUrlopu.HasValue && a.KoniecUrlopu.Value < DateTime.Now)
            {
                a.NaUrlopie = false;
                a.KoniecUrlopu = null;
            }
            string status = a.NaUrlopie ? $" (Na urlopie do {a.KoniecUrlopu.Value.ToShortDateString()})" : "";
            TimeSpan roznica = DateTime.Now - a.Date;
            int dniTemu = (int)roznica.TotalDays;
            Console.WriteLine($"Nick: {a.Nick}, Aktywny: {dniTemu} dni temu {status}");
        }
        Console.WriteLine("\nNaciśnij dowolny klawisz, by wrócić do menu");
        Console.ReadKey();
    }
    public static void ZapiszDoPliku()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "aktywnosc.json");
        string json = JsonSerializer.Serialize(aktywnosc, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
    public static void OdczytajZPliku()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "aktywnosc.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText("aktywnosc.json");
            aktywnosc = JsonSerializer.Deserialize<List<AktywnoscLudzi>>(json);
        }
    }
    public static void UstawUrlop()
    {
        Console.Clear();
        Console.Write("Podaj nick użytkownika: ");
        string nick = Console.ReadLine();

        var osoba = aktywnosc.FirstOrDefault(a => a.Nick.Equals(nick, StringComparison.OrdinalIgnoreCase));
        if (osoba == null) 
        {
            Console.WriteLine("Nie znaleziono użytkownika o podanym nicku.");
            Thread.Sleep(1000);
            return;
        }

        Console.Write("Podaj datę zakończenia urlopu (rrrr-mm-dd): ");
        string input = Console.ReadLine();

        if (DateTime.TryParse(input, out DateTime koniecUrlopu))
        {
            osoba.NaUrlopie = true;
            osoba.KoniecUrlopu = koniecUrlopu;
            Console.WriteLine($"Użytkownik {nick} jest na urlopie do {koniecUrlopu}");
            ZapiszDoPliku();
        }
        else
        {
            Console.WriteLine("Nieprawidłowy format daty!");
        }

        Thread.Sleep(1500);
    }
}