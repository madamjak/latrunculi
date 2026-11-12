using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Helpers;
using Latrunculi.Managers;
using Latrunculi.Managers.Interfaces;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp
{
    class Program
    {
        private static readonly Regex _commandRegex = new Regex(@"^([a-h][0-7]-[a-h][0-7])$");

        static void Main(string[] args)
        {
            MainMenu();
        }

        private static void PrintMainMenu()
        {
            Console.Clear();
            Console.WriteLine("*************** LATRUNCULI MAIN MENU ***************");
            Console.WriteLine();

            Console.WriteLine("[1] to start new game");
            Console.WriteLine("[2] to load existing game");
            Console.WriteLine("[3] to exit game");

            Console.WriteLine();
        }

        private static void PrintGameMenu()
        {
            Console.Clear();
            Console.WriteLine("*************** LATRUNCULI GAME MENU ***************");
            Console.WriteLine();

            Console.WriteLine("[1] to play the game");
            Console.WriteLine("[2] to save game current game");
            Console.WriteLine("[3] to change black player settings");
            Console.WriteLine("[4] to change white player settings");
            Console.WriteLine("[5] to go back to main menu");

            Console.WriteLine();
        }

        private static void PrintGamePlayMenu(IGameplayManager playManager, bool clear)
        {
            if (clear)
            {
                Console.Clear();
            }
            
            Console.WriteLine("*************** LATRUNCULI GAMEPLAY MENU ***************");
            Console.WriteLine();

            Console.WriteLine("[1] to perform move");
            Console.WriteLine("[2] for best move hint");
            Console.WriteLine("[3] to undo last move");
            Console.WriteLine("[4] to redo last move");
            Console.WriteLine("[5] to print moves history");
            Console.WriteLine("[6] to go to game menu");

            Console.WriteLine();

            Console.WriteLine("Board:");

            PrintGamePosition(playManager);
        }

        private static void MainMenu()
        {
            IMainMenuManager mainMenuManager = new MainMenuManager();
            IGameManager gameManager;

            var exit = false;

            PrintMainMenu();

            while (true)
            {
                Console.Write("Please select your option: ");
                var key = Console.ReadKey();

                switch (key.KeyChar)
                {
                    case '1':
                        
                        var blackPlayer = CreatePlayer("Black");
                        var whitePlayer = CreatePlayer("White");

                        gameManager = mainMenuManager.StartNewGame(blackPlayer, whitePlayer);

                        GameMenu(gameManager);
                        break;

                    case '2':

                        Console.WriteLine("Enter path to game file:");
                        var path = Console.ReadLine();

                        try
                        {
                            gameManager = mainMenuManager.LoadGame(path);
                            GameMenu(gameManager);
                        }
                        catch
                        {
                            Console.WriteLine();
                            Console.WriteLine("Unexpected error when loading file. Please enter correct filepath.");
                        }

                        break;
                    case '3':
                        exit = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Please select valid option.");
                        break;
                }

                if (exit)
                {
                    break;
                }
            }
        }

        private static void GameMenu(IGameManager gameManager)
        {
            var exit = false;
            
            PrintGameMenu();

            while (true)
            {
                Console.Write("Please select your option: ");
                var key = Console.ReadKey();

                switch (key.KeyChar)
                {
                    case '1':
                        Play(gameManager.PlayGame());
                        break;
                    case '2':

                        Console.WriteLine();
                        Console.WriteLine("Please enter path to game file: ");
                        var path = Console.ReadLine();
                        try
                        {
                            gameManager.SaveGame(path);
                        }
                        catch 
                        {
                            Console.WriteLine();
                            Console.WriteLine("Unexpected error when saving the file. Please try again.");
                            Console.WriteLine();
                        }

                        break;
                    case '3':
                        var newBlack = CreatePlayer("Black");
                        gameManager.SetBlackPlayer(newBlack);

                        PrintGameMenu();
                        break;
                    case '4':
                        var newWhite = CreatePlayer("White");
                        gameManager.SetWhitePlayer(newWhite);

                        PrintGameMenu();
                        break;
                    case '5':
                        exit = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Please select valid option.");
                        break;
                }

                if (exit)
                {
                    break;
                }
            }

            PrintMainMenu();
        }

        private static void Play(IGameplayManager playManager)
        {
            var exit = false;

            PrintGamePlayMenu(playManager, true);

            while (true)
            {
                Console.WriteLine();
                Console.Write("Please select your option: ");
                var key = Console.ReadKey().KeyChar;

                switch (key)
                {
                    case '1':

                        TryToPerformMove(playManager);
                        break;
                    case '2':

                        Console.WriteLine();
                        Console.WriteLine("Thinking...");

                        var bestMove = playManager.GetCurrentPlayerBestMoveHint(new System.Threading.CancellationToken());

                        Console.WriteLine("Best move: {0}", bestMove.StringCommand);

                        break;
                    case '3':

                        if (playManager.IsUndoPossible())
                        {
                            playManager.Undo();

                            PrintGamePlayMenu(playManager, true);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Undo is not possible. No moves to undo.");
                        }

                        break;

                    case '4':

                        if (playManager.IsRedoPossible())
                        {
                            playManager.Redo();

                            PrintGamePlayMenu(playManager, true);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Redo is not possible. No moves to redo.");
                        }
                        
                        break;
                    case '5':

                        Console.Clear();
                        PrintMovesHistory(playManager);

                        PrintGamePlayMenu(playManager, false);

                        break;

                    case '6':
                        exit = true;
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Select valid option");
                        break;
                }

                if (exit)
                {
                    break;
                }
            }

            PrintGameMenu();
        }

        private static void TryToPerformMove(IGameplayManager gameplay)
        {
            if (gameplay.GetCurrentPosition().Status != BoardPositionStatus.Open)
            {
                Console.WriteLine();
                Console.WriteLine("Game is over.");
                return;
            }

            if (gameplay.GetCurrentPlayer().Type == PlayerType.Computer)
            {
                Console.WriteLine();
                Console.WriteLine("Computer thinking...");

                gameplay.PerformMove(null, new System.Threading.CancellationToken());

                PrintGamePlayMenu(gameplay, true);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Please enter valid coordinates in format <from-to> (e.g. e3-e4):");
            

            while (true)
            {
                var commandString = Console.ReadLine();

                if (!_commandRegex.IsMatch(commandString)) 
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter valid coordinates in format <from-to> (e.g. e3-e4):");
                }
                else
                {
                    var command = commandString.ToMoveStoneCommand();
                    if (gameplay.IsValidMove(command))
                    {
                        gameplay.PerformMove(command, new System.Threading.CancellationToken());

                        PrintGamePlayMenu(gameplay, true);
                        break;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Please enter valid move.");
                    }
                }
            }
        }

        private static void PrintMovesHistory(IGameplayManager playManager)
        {
            var history = playManager.GetHistory();

            Console.WriteLine();
            Console.WriteLine("Moves history: ");
            var historyNumber = 1;
            foreach(var record in history)
            {
                switch (record.UndoRedo)
                {
                    case UndoRedo.Undo:
                    case UndoRedo.Redo:
                        Console.WriteLine("{0}. {1} {2} player move: {3}", historyNumber, record.UndoRedo.ToString(), record.PlayerColor.ToString(), record.Command.StringCommand);
                        break;
                    default:
                        Console.WriteLine("{0}. {1} player move: {2}", historyNumber, record.PlayerColor.ToString(), record.Command.StringCommand);
                        break;
                }

                historyNumber++;
            }

            Console.WriteLine();
        }

        private static void PrintGamePosition(IGameplayManager gameManager)
        {
            var currentPosition = gameManager.GetCurrentPosition();

            var sb = new StringBuilder();

            sb.AppendLine("    a   b   c   d   e   f   g   h");
            sb.AppendLine("  |");
                         
            for (int i = 0; i < currentPosition.Board.GetLength(0); i++)
            {
                sb.Append(string.Format("{0} |", i));
                for (int j = 0; j < currentPosition.Board.GetLength(1); j++)
                {
                    if (currentPosition.Board[i, j] == SquareStatus.Black)
                    {
                        sb.Append(" B |");
                    }
                    else if (currentPosition.Board[i, j] == SquareStatus.White)
                    {
                        sb.Append(" W |");
                    }
                    else
                    {
                        sb.Append("   |");
                    }
                }

                sb.AppendLine();
            }
            sb.AppendLine();

            Console.Write(sb.ToString());

            Console.WriteLine("Last move was: {0}", currentPosition.LastMoveStoneCommand != null ? currentPosition.LastMoveStoneCommand.StringCommand : string.Empty);
            Console.WriteLine("Number of moves without removed stone: {0}", currentPosition.NumberOfMovesWithoutRemovedStoneCounter);
            Console.WriteLine("Board status: {0}", currentPosition.Status.ToString());

            Console.WriteLine("Current player color is {0}", gameManager.GetCurrentPlayerColor().ToString());
        }

        private static Player CreatePlayer(string color)
        {
            Console.Clear();
            Console.WriteLine("{0} player settings:", color);

            var player = new Player();

            Console.WriteLine();
            Console.WriteLine("Select player type:");
            Console.WriteLine("[1] for human");
            Console.WriteLine("[2] for computer");

            while (true)
            {
                var key = Console.ReadKey().KeyChar;
                PlayerType type = PlayerType.NotSpecified;
                switch (key)
                {
                    case '1':
                    case '2':
                        type = (PlayerType)(key - '0');
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Please select valid option.");
                        break;
                }

                if (type != PlayerType.NotSpecified)
                {
                    player.Type = type;
                    break;
                }
            }


            if(player.Type == PlayerType.Computer)
            {
                Console.WriteLine();
                Console.WriteLine("Select computer player level:");
                Console.WriteLine("[1] for easy");
                Console.WriteLine("[2] for medium");
                Console.WriteLine("[3] for hard");

                while (true)
                {
                    var key = Console.ReadKey().KeyChar;
                    PlayerLevel level = PlayerLevel.NotSpecified;
                    switch (key)
                    {
                        case '1':
                        case '2':
                        case '3':
                            level = (PlayerLevel)(key - '0');
                            break;
                        default:
                            Console.WriteLine("Please select valid option.");
                            break;
                    }

                    if (level != PlayerLevel.NotSpecified)
                    {
                        player.Level = level;
                        break;
                    }
                }
            }
            
            return player;
        }
    }
}
