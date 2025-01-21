namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DetailsGenerator detailsGenerator = new DetailsGenerator();
            BrokenCarsGenerator brokenCarsGenerator = new BrokenCarsGenerator();
            CellsGenerator cellsGenerator = new CellsGenerator();

            Autoservice autoservice = new Autoservice(brokenCarsGenerator, detailsGenerator, cellsGenerator);
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

    class DetailsGenerator
    {
        public List<Detail> Generate()
        {
            List<Detail> details = new List<Detail>()
            {
                new Detail(Details.Двигатель),
                new Detail(Details.Трансмиссия),
                new Detail(Details.Тормоза),
                new Detail(Details.Подвеска),
                new Detail(Details.Бензобак)
            };

            return details;
        }
    }

    class Car
    {
        private List<Detail> _details = new List<Detail>();

        public Car(DetailsGenerator detailsGenerator)
        {
            _details = detailsGenerator.Generate();
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

    class BrokenCarsGenerator
    {
        public List<Car> Generate(DetailsGenerator detailsGenerator)
        {
            List<Car> cars = new List<Car>();

            int minimalCount = 3;
            int maximalCount = 7;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                Car car = new Car(detailsGenerator);

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
        private Dictionary<Details, int> _detailPriceList;
        private Dictionary<string, int> _servisePriceList;
        private int _money;

        public Autoservice(BrokenCarsGenerator brokenCars, DetailsGenerator detailsGenerator, CellsGenerator cellsGenerator)
        {
            _brokenCars = new List<Car>(brokenCars.Generate(detailsGenerator));
            _warehouse = new Warehouse(cellsGenerator);
            _detailPriceList = new Dictionary<Details, int>
            {
                {Details.Двигатель, 300},
                {Details.Трансмиссия, 120},
                {Details.Тормоза, 80},
                {Details.Подвеска, 220},
                {Details.Бензобак, 150}
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

            string[] commands = [CommandYes, CommandNo];

            string penaltyForRejectionName = "Штраф за отказ в ремонте";

            bool isService = true;
            bool isBadCondition = false;

            Array detailsNames = Enum.GetValues(typeof(Details));

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

                    int userInput = ServiceCommand(detailsNames);

                    if (userInput > 0 && userInput <= _detailPriceList.Keys.Count)
                    {
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

        private int ServiceCommand(Array details)
        {
            int number = 1;
            int numberOfEnd = 9;

            foreach (Details detail in details)
            {
                Console.WriteLine($"{number} - {detail}");
                number++;
            }

            Console.WriteLine($"{numberOfEnd} - Закончить ремонт\n");
            Console.WriteLine("Выберите деталь для замены или прекратите ремонт:");

            int.TryParse(Console.ReadLine(), out int userCommand);

            return userCommand;
        }
    }

    class Warehouse
    {
        private List<Cell> _cells;

        public Warehouse(CellsGenerator cellsGenerator)
        {
            _cells = cellsGenerator.Generate();
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

    class CellsGenerator
    {
        public List<Cell> Generate()
        {
            int count = 4;
            List<Cell> cells = new List<Cell>()
            {
                new Cell(new Detail(Details.Двигатель), count),
                new Cell(new Detail(Details.Трансмиссия), count),
                new Cell(new Detail(Details.Тормоза), count),
                new Cell(new Detail(Details.Подвеска), count),
                new Cell(new Detail(Details.Бензобак), count)
            };

            return cells;
        }
    }

    enum Details
    {
        Двигатель,
        Трансмиссия,
        Тормоза,
        Подвеска,
        Бензобак
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
