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
        public Detail(Details name)
        {
            Name = name;
            IsBroken = true;
        }

        public Details Name { get; private set; }
        public bool IsBroken { get; private set; }

        public void Break()
        {
            IsBroken = false;
        }

        public void ShowInfo()
        {
            string conditionServiceable = "Исправный";
            string conditionBroken = "Сломанный";

            if (IsBroken)
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

        public bool InspectDetailCondition(Details name)
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

        public int ReceiveBadDetailsCount()
        {
            int count = 0;

            foreach (Detail detail in _details)
            {
                if (detail.IsBroken == false)
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

                if (_details[randomIndex].IsBroken != false)
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
        private Dictionary<Details, int> _detailPriceList;
        private Dictionary<string, int> _servisePriceList;
        private int _money;

        public Autoservice(Generator generator)
        {
            _brokenCars = new List<Car>(generator.GenerateCars());
            _warehouse = new Warehouse(generator.GenerateCells());
            _detailPriceList = new Dictionary<Details, int>
            {
                {Details.Engine, 300},
                {Details.Transmission, 120},
                {Details.Brakes, 80},
                {Details.Suspension, 220},
                {Details.FuelTank, 150}
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

            Details[] detailsNames = (Details[])Enum.GetValues(typeof(Details));

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
                        Repair(car, userInput, detailsNames, isService);
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
                isService = false;
            }
        }

        private void Repair(Car car, int userInput, Details[] detailsNames, bool isService)
        {
            bool isBadCondition = false;

            Details detailName = (Details)detailsNames.GetValue(userInput - 1);

            if (car.InspectDetailCondition(detailName) == isBadCondition)
            {
                Detail newDetail = _warehouse.GiveDetail(detailName);

                isService = IsSetDetailInCar(car, newDetail);
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

        private bool IsSetDetailInCar(Car car, Detail detail)
        {
            string repairServiseName = "Услуги по ремонту";
            string penaltyForCanNotFixName = "Штраф за непочиненную деталь";

            if (detail != null)
            {
                car.ChangeDetail(detail);

                int profit = _detailPriceList[detail.Name] + _servisePriceList[repairServiseName];
                _money += profit;

                Console.WriteLine($"Деталь успешно установлена. Прибыль: {profit}");

                return true;
            }
            else
            {
                Console.WriteLine($"Деталь закончилась на складе. Дальнейший ремонт невозможен. Штраф: {_servisePriceList[penaltyForCanNotFixName]}");

                _money -= _servisePriceList[penaltyForCanNotFixName];

                if (_money < 0)
                {
                    Console.WriteLine("Пришлось брать в долг, чтобы выплтить деньги клиенту");
                    _money = 0;
                }

                return false;
            }
        }

        private void VerifySuccessRateRepairing(Car car)
        {
            string penaltyForCanNotFixName = "Штраф за непочиненную деталь";

            if (car.ReceiveBadDetailsCount() > 0)
            {
                int overallPenalty = _servisePriceList[penaltyForCanNotFixName] * car.ReceiveBadDetailsCount();
                _money -= overallPenalty;

                Console.WriteLine($"Штраф за недоработку: {overallPenalty}");
            }
        }

        private int ServiceCommand(Array details, int commandEnd)
        {
            int number = 1;

            foreach (Details detail in details)
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

        public Detail GiveDetail(Details name)
        {
            int index = 0;
            Detail detail;
            bool isDetailExist;

            for (int i = 0; i < _cells.Count; i++)
            {
                if (_cells[i].ShowName() == name)
                {
                    index = i;
                }
            }

            isDetailExist = _cells[index].TryGetOne(out detail);

            if (isDetailExist == true)
            {
                return detail;
            }

            return null;
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
        }

        public Details ShowName()
        {
            return _detail.Name;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Деталь {_detail.Name} - осталось {_count} штук.");
        }

        public bool TryGetOne(out Detail detail)
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
                new Cell(new Detail(Details.Engine), count),
                new Cell(new Detail(Details.Transmission), count),
                new Cell(new Detail(Details.Brakes), count),
                new Cell(new Detail(Details.Suspension), count),
                new Cell(new Detail(Details.FuelTank), count)
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
                new Detail(Details.Engine),
                new Detail(Details.Transmission),
                new Detail(Details.Brakes),
                new Detail(Details.Suspension),
                new Detail(Details.FuelTank)
            };

            return details;
        }
    }

    enum Details
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
