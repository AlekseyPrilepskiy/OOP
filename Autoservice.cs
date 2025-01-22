namespace ConsoleApp7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator();

            Autoservice autoservice = new Autoservice(generator);
            autoservice.Work();
        }
    }

    class Detail
    {
        public Detail(DetailNames name)
        {
            Name = name;
            IsBroken = false;
        }

        public DetailNames Name { get; private set; }
        public bool IsBroken { get; private set; }

        public void Break()
        {
            IsBroken = true;
        }

        public void ShowInfo()
        {
            string conditionServiceable = "Исправный";
            string conditionBroken = "Сломанный";
            string condition;

            if (IsBroken)
            {
                condition = conditionBroken;
            }
            else
            {
                condition = conditionServiceable;
            }

            Console.WriteLine($"Деталь: {Name}. Состояние: {condition}");
        }
    }

    class Car
    {
        private List<Detail> _details;

        public Car(List<Detail> details)
        {
            _details = new List<Detail>(details);
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

        public bool IsDetailConditionBad(DetailNames name)
        {
            int index = 9;

            for (int i = 0; i < _details.Count; i++)
            {
                if (name == _details[i].Name)
                {
                    index = i;
                    break;
                }
            }

            return _details[index].IsBroken;
        }

        public int GiveBadDetailsCount()
        {
            int count = 0;

            foreach (Detail detail in _details)
            {
                if (detail.IsBroken == true)
                {
                    count++;
                }
            }

            return count;
        }

        public void ChangeDetail(Detail newDetail)
        {
            for (int i = 0; i < _details.Count; i++)
            {
                if (newDetail.Name == _details[i].Name)
                {
                    _details[i] = newDetail;
                    break;
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

                if (_details[randomIndex].IsBroken != true)
                {
                    _details[randomIndex].Break();
                    count--;
                }
            }
        }
    }

    class Autoservice
    {
        private List<Car> _brokenCars;
        private Warehouse _warehouse;
        private Dictionary<DetailNames, int> _detailPriceList;
        private Dictionary<string, int> _servisePriceList;
        private int _money;

        public Autoservice(Generator generator)
        {
            _brokenCars = new List<Car>(generator.GenerateCars());
            _warehouse = new Warehouse(generator.GenerateCells());
            _detailPriceList = new Dictionary<DetailNames, int>
            {
                {DetailNames.Engine, 300},
                {DetailNames.Transmission, 120},
                {DetailNames.Brakes, 80},
                {DetailNames.Suspension, 220},
                {DetailNames.FuelTank, 150}
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

            const int CommandEndServise = 9;

            string penaltyForRejectionName = "Штраф за отказ в ремонте";

            bool isService = true;

            DetailNames[] detailsNames = (DetailNames[])Enum.GetValues(typeof(DetailNames));

            Console.WriteLine();
            _warehouse.ShowInfo();
            Console.WriteLine();

            Console.WriteLine("Информация о следующем в очереди автомобиле:");
            car.ShowInfo();

            string userCommand = InputUserCommand(CommandYes, CommandNo);

            if (userCommand.ToUpper() == CommandYes)
            {
                while (isService)
                {
                    Console.Clear();

                    car.ShowInfo();

                    int userInput = ServiceCommand(detailsNames, CommandEndServise);

                    if (userInput > 0 && userInput <= _detailPriceList.Keys.Count)
                    {
                        Repair(car, userInput, detailsNames, ref isService);
                    }
                    else if (userInput == CommandEndServise)
                    {
                        isService = false;

                        Console.WriteLine("Ремонт данного автомобиля окончен.");

                        VerifySuccessRateRepairing(car);
                    }
                    else
                    {
                        Console.WriteLine("Неверная команда. Попробуйте снова.");
                    }

                    Console.WriteLine("Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine($"Штраф за отказ в ремонте: {_servisePriceList[penaltyForRejectionName]}");

                _money -= _servisePriceList[penaltyForRejectionName];
            }
        }

        private void Repair(Car car, int userInput, DetailNames[] detailsNames, ref bool isService)
        {
            DetailNames detailName = (DetailNames)detailsNames.GetValue(userInput - 1);

            if (car.IsDetailConditionBad(detailName))
            {
                if (_warehouse.GiveDetail(detailName, out Detail newDetail))
                {
                    SetDetailInCar(car, newDetail);
                }
                else
                {
                    Console.WriteLine($"Деталь закончилась на складе. Дальнейший ремонт невозможен.");

                    VerifySuccessRateRepairing(car);

                    if (_money < 0)
                    {
                        Console.WriteLine("Пришлось брать в долг, чтобы выплтить деньги клиенту");
                        _money = 0;
                    }

                    isService = false;
                }
            }
            else
            {
                Console.WriteLine("Зачем чинить исправную деталь?");
            }
        }

        private string InputUserCommand(string command1, string command2)
        {
            string[] commands = [command1, command2];

            Console.Write($"Начать ремонт автомобиля? {command1}/{command2} (Да/Нет): ");
            string userCommand = Console.ReadLine();

            while (commands.Contains(userCommand.ToUpper()) == false)
            {
                Console.Write("Повторите ввод снова: ");
                userCommand = Console.ReadLine();
            }

            return userCommand;
        }

        private void SetDetailInCar(Car car, Detail detail)
        {
            string repairServiseName = "Услуги по ремонту";

            car.ChangeDetail(detail);

            int profit = _detailPriceList[detail.Name] + _servisePriceList[repairServiseName];
            _money += profit;

            Console.WriteLine($"Деталь успешно установлена. Прибыль: {profit}");
        }

        private void VerifySuccessRateRepairing(Car car)
        {
            string penaltyForCanNotFixName = "Штраф за непочиненную деталь";

            if (car.GiveBadDetailsCount() > 0)
            {
                int overallPenalty = _servisePriceList[penaltyForCanNotFixName] * car.GiveBadDetailsCount();
                _money -= overallPenalty;

                Console.WriteLine($"Штраф за недоработку: {overallPenalty}");
            }
        }

        private int ServiceCommand(DetailNames[] details, int commandEnd)
        {
            int number = 1;

            foreach (DetailNames detail in details)
            {
                Console.WriteLine($"{number} - {detail}");
                number++;
            }

            Console.WriteLine($"{commandEnd} - Закончить ремонт\n");
            Console.WriteLine("Выберите деталь для замены или прекратите ремонт:");

            int.TryParse(Console.ReadLine(), out int userCommand);

            return userCommand;
        }
    }

    class Warehouse
    {
        private List<Cell> _cells;

        public Warehouse(List<Cell> cells)
        {
            _cells = new List<Cell>(cells);
        }

        public bool GiveDetail(DetailNames name, out Detail detail)
        {
            int index = 0;

            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i].Name == name)
                {
                    index = i;
                }
            }

            if (_cells[index].TryGetDetail(out detail))
            {
                return true;
            }

            return false;
        }

        public void ShowInfo()
        {
            Console.WriteLine("Складские запасы:");

            foreach (Cell cell in _cells)
            {
                cell.ShowInfo();
            }
        }
    }

    class Cell
    {
        private Detail _detail;
        private int _count;

        public Cell(Detail detail, int count)
        {
            _detail = detail;
            _count = count;
            Name = _detail.Name;
        }

        public DetailNames Name { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Деталь {_detail.Name} - осталось {_count} штук.");
        }

        public bool TryGetDetail(out Detail detail)
        {
            if (_count > 0)
            {
                _count--;
                detail = _detail;

                return true;
            }
            else
            {
                detail = null;

                return false;
            }
        }
    }

    class Generator
    {
        public List<Cell> GenerateCells()
        {
            int count = 4;
            List<Cell> cells = new List<Cell>()
            {
                new Cell(new Detail(DetailNames.Engine), count),
                new Cell(new Detail(DetailNames.Transmission), count),
                new Cell(new Detail(DetailNames.Brakes), count),
                new Cell(new Detail(DetailNames.Suspension), count),
                new Cell(new Detail(DetailNames.FuelTank), count)
            };

            return cells;
        }

        public List<Car> GenerateCars()
        {
            List<Car> cars = new List<Car>();

            int minimalCount = 3;
            int maximalCount = 7;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                Car car = new Car(GenerateDetails());

                car.BreakDown();
                cars.Add(car);
            }

            return cars;
        }

        private List<Detail> GenerateDetails()
        {
            List<Detail> details = new List<Detail>()
            {
                new Detail(DetailNames.Engine),
                new Detail(DetailNames.Transmission),
                new Detail(DetailNames.Brakes),
                new Detail(DetailNames.Suspension),
                new Detail(DetailNames.FuelTank)
            };

            return details;
        }
    }

    enum DetailNames
    {
        Engine,
        Transmission,
        Brakes,
        Suspension,
        FuelTank
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
