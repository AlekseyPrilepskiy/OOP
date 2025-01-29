namespace ConsoleApp11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int currentYear = 2025;

            StewedMeatsGenerator generator = new StewedMeatsGenerator();
            Warehouse warehouse = new Warehouse(generator);

            warehouse.ShowInfo();

            Console.ReadKey();

            warehouse.FindBadMeat(currentYear);
        }
    }

    class Warehouse
    {
        private List<StewedMeat> _stewedMeats;

        public Warehouse(StewedMeatsGenerator stewedMeatsGenerator)
        {
            _stewedMeats = stewedMeatsGenerator.Generate();
        }

        public void ShowInfo()
        {
            ShowList(_stewedMeats);
        }

        public void FindBadMeat(int currentYear)
        {
            var badMeats = _stewedMeats.Where(stewedMeat => (currentYear - stewedMeat.YearOfBirth) > stewedMeat.YearsOfLife).ToList();

            Console.WriteLine("Список просрочки: \n");
            ShowList(badMeats);
        }

        private void ShowList(List<StewedMeat> stewedMeats)
        {
            int number = 1;

            foreach (StewedMeat stewedMeat in stewedMeats)
            {
                Console.WriteLine($"Номер {number++}");
                stewedMeat.ShowInfo();
            }
        }
    }

    class StewedMeatsGenerator
    {
        private GeneratorCharacteristics _generatorCharacteristics;

        public StewedMeatsGenerator()
        {
            _generatorCharacteristics = new GeneratorCharacteristics();
        }

        public List<StewedMeat> Generate()
        {
            List<StewedMeat> stewedMeats = new List<StewedMeat>();

            int minimalCount = 10;
            int maximalCount = 20;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                stewedMeats.Add(new StewedMeat(_generatorCharacteristics));
            }

            return stewedMeats;
        }
    }

    class StewedMeat
    {
        public StewedMeat(GeneratorCharacteristics generatorCharacteristics)
        {
            Name = generatorCharacteristics.GenerateName();
            YearOfBirth = generatorCharacteristics.GenerateYearOfBirth();
            YearsOfLife = generatorCharacteristics.GenerateYearsOfLife();
        }

        public string Name { get; private set; }
        public int YearOfBirth { get; private set; }
        public int YearsOfLife { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Название: {Name}.");
            Console.WriteLine($"Год изготовления: {YearOfBirth}.");
            Console.WriteLine($"Срок годности: {YearsOfLife} лет.\n");
        }
    }

    class GeneratorCharacteristics
    {
        private List<string> _names;

        public GeneratorCharacteristics()
        {
            _names = new()
            {
                "Свиная", "Говяжья", "Индейка", "Куриная", "Мясная"
            };
        }

        public string GenerateName()
        {
            return _names[UserUtils.GenerateRandomNumber(_names.Count)];
        }

        public int GenerateYearOfBirth()
        {
            int minYear = 1980;
            int maxYear = 2025;

            return UserUtils.GenerateRandomNumber(minYear, maxYear);
        }

        public int GenerateYearsOfLife()
        {
            int minLife = 2;
            int maxLife = 10;

            return UserUtils.GenerateRandomNumber(minLife, maxLife);
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int max)
        {
            return s_random.Next(max);
        }

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}
