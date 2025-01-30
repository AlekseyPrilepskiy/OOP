using System.Linq;

namespace ConsoleApp12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SoldiersGenerator generator = new SoldiersGenerator();
            Headquarters headquarters = new Headquarters(generator);

            headquarters.HoldMeeting();
        }
    }

    class Headquarters
    {
        private List<Soldier> _firstSquad;
        private List<Soldier> _secondSquad;

        public Headquarters(SoldiersGenerator soildersGenerator)
        {
            _firstSquad = soildersGenerator.Generate();
            _secondSquad = soildersGenerator.Generate();
        }

        public void HoldMeeting()
        {
            Console.WriteLine("Отряд номер 1.\n");
            ShowAllSoldiers(_firstSquad);
            
            Console.WriteLine("Отряд номер 2.\n");
            ShowAllSoldiers(_secondSquad);

            Console.WriteLine("Провести обмен солдатами - Enter");
            Console.ReadKey();

            TransferSoilders();

            Console.WriteLine("Отряд номер 1.\n");
            ShowAllSoldiers(_firstSquad);

            Console.WriteLine("Отряд номер 2.\n");
            ShowAllSoldiers(_secondSquad);
        }

        private void TransferSoilders()
        {
            string symbol = "Б";

            var transferedSoldiers = _firstSquad.Where(soldier => soldier.Surname.StartsWith(symbol)).ToList();

            _firstSquad.RemoveAll(soldier => soldier.Surname.StartsWith(symbol));

            _secondSquad = _secondSquad.Union(transferedSoldiers).ToList();
        }

        private void ShowAllSoldiers(List<Soldier> soldiers)
        {
            int number = 1;

            foreach (Soldier soldier in soldiers)
            {
                Console.WriteLine($"Номер {number++}");
                soldier.ShowInfo();
            }
        }
    }

    class Soldier
    {
        public Soldier(GeneratorCharacteristics generatorCharacteristics)
        {
            Name = generatorCharacteristics.GenerateName();
            Surname = generatorCharacteristics.GenerateSurname();
        }

        public string Name { get; private set; }
        public string Surname { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя: {Name}");
            Console.WriteLine($"Фамилия: {Surname}\n");
        }
    }

    class SoldiersGenerator
    {
        private GeneratorCharacteristics _generatorCharacteristics;

        public SoldiersGenerator()
        {
            _generatorCharacteristics = new GeneratorCharacteristics();
        }

        public List<Soldier> Generate()
        {
            int count = 5;

            List<Soldier> soldiers = new List<Soldier>();

            for (int i = 0; i < count; i++)
            {
                soldiers.Add(new Soldier(_generatorCharacteristics));
            }

            return soldiers;
        }
    }

    class GeneratorCharacteristics
    {
        private List<string> _names;
        private List<string> _surnames;

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
                "Биванов", "Баранов", "Бабушкин", "Бобров", "Баринов", "Березов", "Башкаков",
                "Смирнов", "Кузнецов", "Попов", "Васильев", "Петров", "Соколов",
                "Михайлов", "Новиков", "Федоров", "Морозов", "Волков", "Алексеев"
            };
        }

        public string GenerateName()
        {
            return _names[UserUtils.GenerateRandomNumber(_names.Count)];
        }

        public string GenerateSurname()
        {
            return _surnames[UserUtils.GenerateRandomNumber(_surnames.Count)];
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int maxNumber)
        {
            return s_random.Next(maxNumber);
        }
    }
}
