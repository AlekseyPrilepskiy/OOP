using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace ConsoleApp10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PlayersGenerator playersGenerator = new PlayersGenerator();

            GameServer gameServer = new GameServer(playersGenerator);

            int count = 3;

            gameServer.ShowAllPlayers();

            Console.WriteLine($"Чтобы вывести топ {count} игроков по уровню нажмите любую клавишу.");
            Console.ReadKey();

            gameServer.ShowTopLevelPlayers(count);

            Console.WriteLine($"Чтобы вывести топ {count} игроков по силе нажмите любую клавишу.");
            Console.ReadKey();

            gameServer.ShowTopPowerPlayers(count);
        }
    }

    class GameServer
    {
        private List<Player> _players;

        public GameServer(PlayersGenerator playersGenerator)
        {
            _players = playersGenerator.Generate();
        }

        public void ShowAllPlayers()
        {
            Console.WriteLine("Список всех игроков:");
            int number = 1;

            foreach (Player player in _players)
            {
                Console.WriteLine($"Номер {number++}");
                player.ShowInfo();
            }
        }

        public void ShowTopLevelPlayers(int count)
        {
            Console.WriteLine($"Топ {count} игроков по уровню");

            var topLevelPlayers = _players.OrderByDescending(player => player.Level).Take(count);

            foreach (Player player in topLevelPlayers)
            {
                player.ShowInfo();
            }
        }

        public void ShowTopPowerPlayers(int count)
        {
            Console.WriteLine($"Топ {count} игроков по силе");

            var topPowerPlayers = _players.OrderByDescending(player => player.Power).Take(count);

            foreach (Player player in topPowerPlayers)
            {
                player.ShowInfo();
            }
        }
    }

    class Player
    {
        public Player(GeneratorCharacteristics generatorCharacteristics)
        {
            Name = generatorCharacteristics.GenerateName();
            Level = generatorCharacteristics.GenerateLevel();
            Power = generatorCharacteristics.GenetatePower();
        }

        public string Name { get; private set; }
        public int Level { get; private set; }
        public int Power { get; private set; }

        public void ShowInfo()
        {
            Console.WriteLine($"Никнейм: {Name}");
            Console.WriteLine($"Уровень: {Level}");
            Console.WriteLine($"Сила: {Power}\n");
        }
    }

    class PlayersGenerator
    {
        private GeneratorCharacteristics _generatorCharacteristics;

        public PlayersGenerator()
        {
            _generatorCharacteristics = new GeneratorCharacteristics();
        }

        public List<Player> Generate()
        {
            List<Player> players = new List<Player>();

            int minimalCount = 10;
            int maximalCount = 20;
            int count = UserUtils.GenerateRandomNumber(minimalCount, maximalCount);

            for (int i = 0; i < count; i++)
            {
                players.Add(new Player(_generatorCharacteristics));
            }

            return players;
        }
    }

    class GeneratorCharacteristics
    {
        private Queue<string> _names;

        public GeneratorCharacteristics()
        {
            _names = new Queue<string>(
            [
                "Andrew777", "Alex", "BigKek5", "BigMak", "VladAndSlave", "Dimitr",
                "Egor228", "EuWGen", "Ivan", "Cheburek", "BoneJa", "Shaurma",
                "Lev", "Makssssim", "Nick", "Ghost", "Pyosik", "RUSlan", "StasAndSlave", "Jan"
            ]);
        }

        public string GenerateName()
        {
            return _names.Dequeue();
        }

        public int GenerateLevel()
        {
            int minLevel = 1;
            int maxLevel = 100;

            return UserUtils.GenerateRandomNumber(minLevel, maxLevel);
        }

        public int GenetatePower()
        {
            int minPower = 100;
            int maxPower = 5000;

            return UserUtils.GenerateRandomNumber(minPower, maxPower);
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int minNumber, int maxNumber)
        {
            return s_random.Next(minNumber, maxNumber);
        }
    }
}
