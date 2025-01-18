using System.ComponentModel.Design;

namespace ConsoleApp7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BrokenCarsGenerator brokenCarsGenerator = new BrokenCarsGenerator();
            Autoservice autoservice = new Autoservice(brokenCarsGenerator);
            autoservice.Work();
        }
    }

    class Detail
    {
        public Detail(string name)
        {
            Name = name;
            IsGood = true;
        }

        public string Name { get; private set; }
        public bool IsGood { get; private set; }

        public void Damaged()
        {
            IsGood = false;
        }

        public void ShowInfo()
        {
            string conditionServiceable = "Исправный";
            string conditionBroken = "Сломанный";

            if (IsGood)
            {
                Console.WriteLine($"Деталь: {Name}. Состояние: {conditionServiceable}");
            }
            else
            {
                Console.WriteLine($"Деталь: {Name}. Состояние: {conditionBroken}");
            }
        }
    }

    class Car
    {
        private List<Detail> _details = new List<Detail>();

        public Car()
        {
            _details.Add(new Detail("Двигатель"));
            _details.Add(new Detail("Трансмиссия"));
            _details.Add(new Detail("Тормоза"));
            _details.Add(new Detail("Подвеска"));
            _details.Add(new Detail("Топливный бак"));
        }

        public void ShowInfo()
        {
            Console.WriteLine("Детали машины:");

            foreach (Detail detail in _details)
            {
                detail.ShowInfo();
            }

            Console.WriteLine();
        }

        public bool InspectDetailCondition(string name)
        {
            int index = 9;

            for (int i = 0; i < _details.Count; i++)
            {
                if (name.ToUpper() == _details[i].Name.ToUpper())
                {
                    index = i;
                    break;
                }
            }

            return _details[index].IsGood;
        }

        public int ReceiveBadDetailsCount()
        {
            int count = 0;

            foreach (Detail detail in _details)
            {
                if (detail.IsGood == false)
                {
                    count++;
                }
            }

            return count;
        }

        public void TakeNewDetail(Detail newDetail)
        {
            for (int i = 0; i < _details.Count; i++)
            {
                if (newDetail.Name == _details[i].Name)
                {
                    _details[i] = newDetail;
                }
            }
        }

        public void BreakDown()
        {
            int count = UserUtils.GenerateRandomNumber(1, _details.Count);
            int randomIndex;

            while (count > 0)
            {
                randomIndex = UserUtils.GenerateRandomNumber(0, _details.Count - 1);

                if (_details[randomIndex].IsGood != false)
                {
                    _details[randomIndex].Damaged();
                    count--;
                }
            }
        }
    }

    class BrokenCarsGenerator
    {
        public List<Car> Generate()
        {
            List<Car> cars = new List<Car>();

            int minimalCount = 3;
            int maximalCount = 6;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                Car car = new Car();

                car.BreakDown();
                cars.Add(car);
            }

            return cars;
        }
    }

    class Autoservice
    {
        private List<Car> _brokenCars;
        private Warehouse _warehouse;
        private Dictionary<string, int> _detailPriceList;
        private Dictionary<string, int> _servisePriceList;
        private int _money;

        public Autoservice(BrokenCarsGenerator brokenCars)
        {
            _brokenCars = new List<Car>(brokenCars.Generate());
            _warehouse = new Warehouse();
            _detailPriceList = new Dictionary<string, int>
            {
                {"ДВИГАТЕЛЬ", 300},
                {"ТРАНСМИССИЯ", 120},
                {"ТОРМОЗА", 80},
                {"ПОДВЕСКА", 220},
                {"ТОПЛИВНЫЙ БАК", 150},
            };
            _servisePriceList = new Dictionary<string, int>
            {
                {"Услуги по ремонту", 100},
                {"Штраф за непочиненную деталь", 100},
                {"Штраф за отказ в ремонте", 50}
            };
            _money = 500;
        }

        public void Work()
        {
            int carsCount = _brokenCars.Count;

            foreach (Car car in _brokenCars)
            {
                Console.WriteLine($"Всего в очереди {carsCount} машин");
                Console.WriteLine($"Денег в кассе: {_money}");

                Service(car);

                carsCount--;

                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить.");
                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine($"Итого денег: {_money}");
        }

        private void Service(Car car)
        {
            const string CommandYes = "Y";
            const string CommandNo = "N";

            string[] commands = [CommandYes, CommandNo];

            string repairServiseName = "Услуги по ремонту";
            string penaltyForCanNotFixName = "Штраф за непочиненную деталь";
            string penaltyForRejectionName = "Штраф за отказ в ремонте";

            bool isService = true;
            bool isBadCondition = false;

            Console.WriteLine();
            _warehouse.ShowInfo();
            Console.WriteLine();

            Console.WriteLine("Информация о следующем в очереди автомобиле:");
            car.ShowInfo();

            Console.Write($"Начать ремонт автомобиля? {CommandYes}/{CommandNo} (Да/Нет): ");
            string userInputToServiceCar = Console.ReadLine();

            while (commands.Contains(userInputToServiceCar.ToUpper()) == false)
            {
                Console.Write("Повторите ввод снова: ");
                userInputToServiceCar = Console.ReadLine();
            }

            if (userInputToServiceCar.ToUpper() == CommandYes)
            {
                while (isService)
                {
                    Console.Clear();

                    car.ShowInfo();

                    Console.WriteLine($"Желаете провести замену детали? {CommandYes}/{CommandNo} (Да/Нет)");
                    string userInput = Console.ReadLine();

                    while (commands.Contains(userInput.ToUpper()) == false)
                    {
                        Console.Write("Повторите ввод снова: ");
                        userInput = Console.ReadLine();
                    }

                    if (userInput.ToUpper() == CommandYes)
                    {
                        Console.WriteLine("Введите название детали для починки: ");
                        string detailName = Console.ReadLine();
                        detailName = detailName.ToUpper();

                        if (_detailPriceList.ContainsKey(detailName))
                        {
                            if (car.InspectDetailCondition(detailName) == isBadCondition)
                            {
                                Detail newDetail = _warehouse.GiveDetail(detailName);

                                if (newDetail != null)
                                {
                                    car.TakeNewDetail(newDetail);
                                    _money += _detailPriceList[detailName] + _servisePriceList[repairServiseName];
                                }
                                else
                                {
                                    Console.WriteLine($"Деталь закончилась на складе. Ремонт невозможен. Штраф: {_servisePriceList[penaltyForCanNotFixName]}");
                                    _money -= _servisePriceList[penaltyForCanNotFixName];

                                    if (_money < 0)
                                    {
                                        Console.WriteLine("Пришлось брать в долг, чтобы выплтить деньги клиенту");
                                        _money = 0;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Зачем чинить исправную деталь?");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Таких деталей не существует. Проверьте правильность ввода");
                        }
                    }
                    else
                    {
                        isService = false;

                        Console.WriteLine("Ремонт данного автомобиля окончен.");

                        if (car.ReceiveBadDetailsCount() > 0)
                        {
                            int overallPenalty = _servisePriceList[penaltyForCanNotFixName] * car.ReceiveBadDetailsCount();

                            _money -= overallPenalty;

                            Console.WriteLine($"Штраф за недоработку: {overallPenalty}");
                        }
                    }

                    Console.WriteLine("Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine($"Штраф за отказ в ремонте: {_servisePriceList[penaltyForRejectionName]}");

                _money -= _servisePriceList[penaltyForRejectionName];
                isService = false;
            }
        }
    }

    class Warehouse
    {
        private List<Detail> _details;
        private List<int> _counts;

        public Warehouse()
        {
            _details = new List<Detail> { new Detail("Двигатель"), new Detail("Трансмиссия"), new Detail("Тормоза"), new Detail("Подвеска"), new Detail("Топливный бак") };
            _counts = new List<int> { 4, 4, 4, 4, 4 };
        }

        public Detail GiveDetail(string name)
        {
            for (int i = 0; i < _details.Count; i++)
            {
                if (_details[i].Name.ToUpper() == name && _counts[i] != 0)
                {
                    _counts[i]--;
                    return new Detail(_details[i].Name);
                }
            }

            return null;
        }

        public void ShowInfo()
        {
            Console.WriteLine("Складские запасы:");

            for (int i = 0; i < _details.Count; i++)
            {
                Console.WriteLine($"Деталь: {_details[i].Name} - {_counts[i]} штук осталось");
            }
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }
    }
}
