using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace ConsoleApp10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PatientsGenerator generator = new PatientsGenerator();

            Hospital hospital = new Hospital(generator);

            hospital.Work();
        }
    }

    class Hospital
    {
        private List<Patient> _patients;

        public Hospital(PatientsGenerator patientsGenerator)
        {
            _patients = patientsGenerator.Generate();
        }

        public void Work()
        {
            const int CommandSortByName = 1;
            const int CommandSortByAge = 2;
            const int CommandFindPatientsByIll = 3;
            const int CommandEnd = 4;

            bool isWork = true;

            while (isWork)
            {
                Console.WriteLine("Список больных:\n");
                ShowInfo();

                Console.WriteLine($"{CommandSortByName} - Отсортировать по имени.");
                Console.WriteLine($"{CommandSortByAge} - Отсортировать по возрасту.");
                Console.WriteLine($"{CommandFindPatientsByIll} - Найти пациентов с конкретной болезнью.");
                Console.WriteLine($"{CommandEnd} - Закончить работу.");

                int.TryParse(Console.ReadLine(), out int userInput);

                switch (userInput)
                {
                    case CommandSortByName:
                        SortByName();
                        break;

                    case CommandSortByAge:
                        SortByAge();
                        break;

                    case CommandFindPatientsByIll:
                        FindPatientsByIll();
                        break;

                    case CommandEnd:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Некорректная команда. Попробуйте снова");
                        break;
                }

                Console.WriteLine("Для продолжения введите любую клавишу.");
                Console.ReadKey();

                Console.Clear();
            }
        }

        private void SortByName()
        {
            _patients = _patients.OrderBy(patient => patient.Name).ToList();
        }

        private void SortByAge()
        {
            _patients = _patients.OrderBy(patient => patient.Age).ToList();
        }

        private void FindPatientsByIll()
        {
            Console.Write("Введите название болезни: ");
            string ill = Console.ReadLine().ToUpper();

            var patientsWithIll = _patients.Where(patient => patient.Ilness.ToUpper() == ill).ToList();

            foreach (var patient in patientsWithIll)
            {
                patient.ShowInfo();
            }

            if (patientsWithIll.Count == 0)
            {
                Console.WriteLine("Пациентов с этой болезнью не найдено.");
            }
        }

        private void ShowInfo()
        {
            int number = 1;

            foreach (Patient patient in _patients)
            {
                Console.WriteLine($"Номер: {number++}");
                patient.ShowInfo();
            }
        }
    }

    class Patient
    {
        public Patient(GeneratorCharacteristics generatorCharacteristics)
        {
            Name = generatorCharacteristics.GenerateName();
            Ilness = generatorCharacteristics.GenerateIlness();
            Age = generatorCharacteristics.GenetateAge();
        }

        public string Name { get; private set; }
        public string Ilness { get; private set; }
        public int Age { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"ФИО: {Name}");
            Console.WriteLine($"Возраст: {Age}");
            Console.WriteLine($"Болезнь: {Ilness}\n");
        }
    }

    class PatientsGenerator
    {
        private GeneratorCharacteristics _generatorCharacteristics;

        public PatientsGenerator()
        {
            _generatorCharacteristics = new GeneratorCharacteristics();
        }

        public List<Patient> Generate()
        {
            List<Patient> patients = new List<Patient>();

            int minimalCount = 10;
            int maximalCount = 18;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                patients.Add(new Patient(_generatorCharacteristics));
            }

            return patients;
        }
    }

    class GeneratorCharacteristics
    {
        private List<string> _names;
        private List<string> _surnames;
        private List<string> _patronymics;
        private List<string> _illnessTypes;

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
            _illnessTypes = new()
            {
                "ОРВИ", "Аллергия", "Диабет", "Гепатит", "Перелом", "Альцгеймер"
            };
        }

        public string GenerateName()
        {
            string fullName = _surnames[UserUtils.GenerateRandomNumber(_surnames.Count)] + " " + _names[UserUtils.GenerateRandomNumber(_names.Count)] + " " + _patronymics[UserUtils.GenerateRandomNumber(_patronymics.Count)];

            return fullName;
        }

        public string GenerateIlness()
        {
            return _illnessTypes[UserUtils.GenerateRandomNumber(_illnessTypes.Count)];
        }

        public int GenetateAge()
        {
            int minAge = 18;
            int maxAge = 90;

            return UserUtils.GenerateRandomNumber(minAge, maxAge);
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
