namespace ConsoleApp12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SoildersGenerator generator = new SoildersGenerator();
            Squad squad = new Squad(generator);

            squad.ShowNameAndRank();
        }
    }

    class Squad
    {
        private List<Soilder> _soilders;

        public Squad(SoildersGenerator soildersGenerator)
        {
            _soilders = soildersGenerator.Generate();
        }

        public void ShowNameAndRank()
        {
            var soildersName = _soilders.Select(soildet => soildet.Name).ToList();
            var soildersRank = _soilders.Select(soildet => soildet.Rank).ToList();

            for (int i = 0; i < soildersName.Count; i++)
            {
                Console.WriteLine($"Имя: {soildersName[i]}. Звание: {soildersRank[i]}");
            }
        }
    }

    class Soilder
    {
        public Soilder(GeneratorCharacteristics generatorCharacteristics)
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
    }

    class SoildersGenerator
    {
        private GeneratorCharacteristics _generatorCharacteristics;

        public SoildersGenerator()
        {
            _generatorCharacteristics = new GeneratorCharacteristics();
        }

        public List<Soilder> Generate()
        {
            int count = 5;

            List<Soilder> soilders = new List<Soilder>();

            for (int i = 0; i < count; i++)
            {
                soilders.Add(new Soilder(_generatorCharacteristics));
            }

            return soilders;
        }
    }

    class GeneratorCharacteristics
    {
        private List<string> _names;
        private List<string> _surnames;
        private List<string> _patronymics;
        private List<string> _weapons;
        private List<string> _ranks;

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
            _ranks = new()
            {
                "Рядовой", "Ефрейтор", "Сержант", "Лейтенант", "Капитан"
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
            int CorporalPromotionInMonth = 12;
            int SergantPromotionInMonth = 36;
            int LieutenantPromotionInMonth = 96;
            int CaptainPromotionInMonth = 180;

            if (monthOfServiceLife < CorporalPromotionInMonth)
            {
                return _ranks[0];
            }
            else if (monthOfServiceLife < SergantPromotionInMonth)
            {
                return _ranks[1];
            }
            else if (monthOfServiceLife < LieutenantPromotionInMonth)
            {
                return _ranks[2];
            }
            else if (monthOfServiceLife < CaptainPromotionInMonth)
            {
                return _ranks[3];
            }
            else
            {
                return _ranks[4];
            }
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
