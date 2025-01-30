using System.Linq;

namespace ConsoleApp12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SoldiersGenerator generator = new SoldiersGenerator();
            Squad squad = new Squad(generator);

            squad.ShowAllInfo();

            Console.ReadKey();

            squad.ShowNameAndRank();
        }
    }

    class Squad
    {
        private List<Soldier> _soldiers;

        public Squad(SoldiersGenerator soildersGenerator)
        {
            _soldiers = soildersGenerator.Generate();
        }

        public void ShowNameAndRank()
        {
            var soldiersName = _soldiers.Select(soldier => soldier.Name).ToList();
            var soldiersRank = _soldiers.Select(soldier => soldier.Rank).ToList();

            for (int i = 0; i < soldiersName.Count; i++)
            {
                Console.WriteLine($"Имя: {soldiersName[i]}. Звание: {soldiersRank[i]}");
            }
        }

        public void ShowAllInfo()
        {
            int number = 1;

            foreach (Soldier soldier in _soldiers)
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
            Weapon = generatorCharacteristics.GenerateWeapon();
            ServiceLifeInMonts = generatorCharacteristics.GenerateServiceLife();
            Rank = generatorCharacteristics.GenerateRank(ServiceLifeInMonts);
        }

        public string Name { get; private set; }
        public string Weapon { get; private set; }
        public string Rank { get; private set; }
        public int ServiceLifeInMonts { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Имя: {Name}");
            Console.WriteLine($"Оружие: {Weapon}");
            Console.WriteLine($"Звание: {Rank}");
            Console.WriteLine($"Месяцы службы: {ServiceLifeInMonts}\n");
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
        private List<string> _patronymics;
        private List<string> _weapons;
        private Dictionary<int, string> _monthServiceAndRanks;

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
            _weapons = new()
            {
                "Пистолет", "Пистолет - пулемет", "Штурмовая винтовка", "Снайперская винтовка", "Пулемет", "РПГ"
            };
            _monthServiceAndRanks = new()
            {
                { 6, "Рядовой"}, {12, "Ефрейтор"}, {36, "Сержант"}, {96, "Лейтенант"}, {180, "Капитан"}
            };
        }

        public string GenerateName()
        {
            string fullName = _surnames[UserUtils.GenerateRandomNumber(_surnames.Count)] + " " + _names[UserUtils.GenerateRandomNumber(_names.Count)] + " " + _patronymics[UserUtils.GenerateRandomNumber(_patronymics.Count)];

            return fullName;
        }

        public string GenerateWeapon()
        {
            return _weapons[UserUtils.GenerateRandomNumber(_weapons.Count)];
        }
        
        public int GenerateServiceLife()
        {
            int minMonth = 6;
            int maxMonth = 240;

            return UserUtils.GenerateRandomNumber(minMonth, maxMonth);
        }

        public string GenerateRank(int monthOfServiceLife)
        {
            string rank = "";

            foreach (int month in _monthServiceAndRanks.Keys)
            {
                if (monthOfServiceLife > month)
                {
                    rank = _monthServiceAndRanks[month];
                }
            }

            return rank;
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
