using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace ConsoleApp10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CriminalsGenerator generator = new CriminalsGenerator();
            DetectiveDataBase detectiveDataBase = new DetectiveDataBase(generator);

            detectiveDataBase.Work();
        }
    }

    class DetectiveDataBase
    {
        private List<Criminal> _criminals;

        public DetectiveDataBase(CriminalsGenerator criminalsGenerator)
        {
            _criminals = new List<Criminal>(criminalsGenerator.Generate());
        }

        public void Work()
        {
            const int CommandShowAllCriminals = 1;
            const int CommandShowWantedCriminals = 2;
            const int CommandExit = 3;

            bool isWork = true;

            Console.WriteLine("Добро пожаловать в детективное досье.");

            while (isWork)
            {
                Console.WriteLine($"{CommandShowAllCriminals} - Показать всех в досье.");
                Console.WriteLine($"{CommandShowWantedCriminals} - Поиск по характеристикам.");
                Console.WriteLine($"{CommandExit} - Закончить работу.");

                Console.Write("Введите команду: ");
                int.TryParse(Console.ReadLine(), out int userCommand);

                switch (userCommand)
                {
                    case CommandShowAllCriminals:
                        ShowAllCriminals();
                        break;

                    case CommandShowWantedCriminals:
                        ShowWantedCriminals();
                        break;

                    case CommandExit:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Неверная команда.");
                        break;
                }
            }
        }

        private void ShowAllCriminals()
        {
            int number = 1;

            foreach (Criminal criminal in _criminals)
            {
                Console.WriteLine($"\nНомер: {number++}");
                criminal.ShowInfo();
            }
        }

        private void ShowWantedCriminals()
        {
            int inaccuracy = 3;

            Console.WriteLine("Введите данные подозреваемого.");

            Console.Write("Национальность: ");
            string nationality = Console.ReadLine().ToUpper();

            Console.Write("Рост: ");
            int.TryParse(Console.ReadLine(), out int height);

            int minHeight = height - inaccuracy;
            int maxHeight = height + inaccuracy;

            Console.Write("Вес: ");
            int.TryParse(Console.ReadLine(), out int weight);

            int minWeight = weight - inaccuracy;
            int maxWeight = weight + inaccuracy;

            var wantedCriminals = _criminals.Where(criminal => 
            criminal.Height >= minHeight
            && criminal.Height <= maxHeight
            && criminal.Weight >= minWeight
            && criminal.Weight <= maxWeight
            && criminal.Nationality.ToUpper() == nationality
            && criminal.IsPrisioner == false).ToList();

            if (wantedCriminals.Count > 0)
            {
                foreach (Criminal criminal in wantedCriminals)
                {
                    Console.WriteLine();
                    criminal.ShowInfo();
                }
            }
            else
            {
                Console.WriteLine("По запросу ничего не найдено.");
            }
        }
    }

    class Criminal
    {
        private string _name;

        public Criminal(GeneratorCharacteristics generatorCharacteristics)
        {
            _name = generatorCharacteristics.GenerateName();
            Nationality = generatorCharacteristics.GenerateNationality();
            Height = generatorCharacteristics.GenerateHeight();
            Weight = generatorCharacteristics.GenerateWeight();
            IsPrisioner = IsImprison();
        }

        public string Nationality { get; private set; }
        public int Height { get; private set; }
        public int Weight { get; private set; }
        public bool IsPrisioner { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"ФИО: {_name}");
            Console.WriteLine($"Национальность: {Nationality}");
            Console.WriteLine($"Рост: {Height}");
            Console.WriteLine($"Вес: {Weight}");
            Console.WriteLine($"Заключен под стражу: {IsPrisioner}");
        }

        private bool IsImprison()
        {
            int lowerRandomBorder = 1;
            int upperRandomBorder = 11;
            int chanceOfRandom = 3;

            if (UserUtils.GenerateRandomNumber(lowerRandomBorder, upperRandomBorder) <= chanceOfRandom)
            {
                return true;
            }

            return false;
        }
    }

    class CriminalsGenerator
    {
        private GeneratorCharacteristics _generatorCharacteristics;

        public CriminalsGenerator()
        {
            _generatorCharacteristics = new GeneratorCharacteristics();
        }

        public List<Criminal> Generate()
        {
            List<Criminal> _criminals = new List<Criminal>();

            int minimalCount = 8;
            int maximalCount = 20;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                _criminals.Add(new Criminal(_generatorCharacteristics));
            }

            return _criminals;
        }
    }

    class GeneratorCharacteristics
    {
        private List<string> _names;
        private List<string> _surnames;
        private List<string> _patronymics;
        private List<string> _nationalities;

        public GeneratorCharacteristics()
        {
            _names = new()
            {
                "Андрей", "Алексей", "Асланбек", "Бикек", "БигМак", "ВладиSlave", "Генадий", "Дмитрий",
                "Даниил", "Егор", "Евгений", "Иван", "Игнат", "Ичпочмак", "Константин", "Киргиз", "Леонид",
                "Леонтий", "Максим", "Марат", "Никита", "Олег", "Павел", "Пётр", "Руслан", "СтаниSlave", "Хабиб", "Ян"
            };
            _surnames = new()
            {
                "Иванов", "Смирнов", "Кузнецов", "Попов", "Васильев", "Петров", "Соколов",
                "Михайлов", "Новиков", "Федоров", "Морозов", "Волков", "Алексеев", "Лебедев",
                "Семенов", "Егоров", "Павлов", "Козлов", "Степанов", "Николаев",
            };
            _patronymics = new()
            {
                "Александрович", "Владимирович", "Сергеевич", "Николаевич", "Алексеевич", "Михайлович", "Павлович",
                "Иванович", "Егорович", "Васильевич", "Дмитриевич", "Григорьевич", "Анатольевич", "Юрьевич", "Борисович",
                "Константинович", "Викторович", "Олегович", "Романович", "Станиславович"
            };
            _nationalities = new()
            {
                "Русский", "Украинец", "Белорус", "Казах", "Татарин", "Узбек", "Армянин", "Грузин", "Азербайджанец", "Молдаванин"
            };
        }

        public string GenerateName()
        {
            string fio = _surnames[UserUtils.GenerateRandomNumber(_surnames.Count)] + " " + _names[UserUtils.GenerateRandomNumber(_names.Count)] + " " + _patronymics[UserUtils.GenerateRandomNumber(_patronymics.Count)];

            return fio;
        }

        public string GenerateNationality()
        {
            return _nationalities[UserUtils.GenerateRandomNumber(_nationalities.Count)];
        }

        public int GenerateHeight()
        {
            int minHeight = 155;
            int maxHeight = 210;

            return UserUtils.GenerateRandomNumber(minHeight, maxHeight);
        }

        public int GenerateWeight()
        {
            int minWeight = 55;
            int maxWeight = 135;

            return UserUtils.GenerateRandomNumber(minWeight, maxWeight);
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int maxNumber)
        {
            return s_random.Next(maxNumber);
        }

        public static int GenerateRandomNumber(int minNumber, int maxNumber)
        {
            return s_random.Next(minNumber, maxNumber);
        }
    }
}
