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
            string amnestyCrime = "Антиправительственное";

            Prison prison = new Prison(generator);

            prison.ShowInfo();

            Console.WriteLine("Власть сменилась. Нажмите любую клавишу для амнистии.\n");
            Console.ReadKey();

            prison.Amnesty(amnestyCrime);

            prison.ShowInfo();
        }
    }

    class Prison
    {
        private List<Criminal> _criminals;

        public Prison(CriminalsGenerator criminalsGenerator)
        {
            _criminals = criminalsGenerator.Generate();
        }

        public void ShowInfo()
        {
            int number = 1;

            foreach (Criminal criminal in _criminals)
            {
                Console.WriteLine($"Номер: {number++}");
                criminal.ShowInfo();
            }
        }

        public void Amnesty(string crime)
        {
            var updatedCriminals = _criminals.Where(criminal => criminal.Crime != crime).Select(criminal => criminal).ToList();

            _criminals = new List<Criminal>(updatedCriminals);
        }
    }

    class Criminal
    {
        private string _name;

        public Criminal(GeneratorCharacteristics generatorCharacteristics)
        {
            _name = generatorCharacteristics.GenerateName();
            Crime = generatorCharacteristics.GenerateCrime();
        }

        public string Crime { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"ФИО: {_name}");
            Console.WriteLine($"Преступление: {Crime}\n");
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
            int maximalCount = 15;
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
        private List<string> _criminalTypes;

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
            _criminalTypes = new()
            {
                "Воровство", "Мошенничество", "Убийство", "Антиправительственное", "Шантаж", "Вымогательство"
            };
        }

        public string GenerateName()
        {
            string fio = _surnames[UserUtils.GenerateRandomNumber(_surnames.Count)] + " " + _names[UserUtils.GenerateRandomNumber(_names.Count)] + " " + _patronymics[UserUtils.GenerateRandomNumber(_patronymics.Count)];

            return fio;
        }

        public string GenerateCrime()
        {
            return _criminalTypes[UserUtils.GenerateRandomNumber(_criminalTypes.Count)];
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
