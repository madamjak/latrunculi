using Latrunculi.DataAccess;
using Latrunculi.Entities;
using Latrunculi.Logic;
using Latrunculi.Managers.Interfaces;

namespace Latrunculi.Managers
{
    public class MainMenuManager : IMainMenuManager
    {
        private readonly IFileReader _fileReader;

        private readonly IGameRules _rules;

        private IGameManager _gameManager;

        public MainMenuManager()
        {
            _rules = new LatrunculiRules();
            _fileReader = new FileReader();
        }

        public bool CanPlay()
        {
            return _gameManager != null && _gameManager.IsBlackPlayerSet() && _gameManager.IsWhitePlayerSet();
        }

        public IGameManager LoadGame(string path)
        {
            var game = _fileReader.GetSavedGame(path);

            _gameManager = new GameManager(game, _rules);

            return _gameManager;
        }

        public IGameManager Play()
        {
            return _gameManager;
        }

        public void SaveGame(string path)
        {
            _gameManager.SaveGame(path);
        }

        public IGameManager StartNewGame(Player black, Player white)
        {
            _gameManager = new GameManager(new Game()
            {
                BlackPlayer = black,
                WhitePlayer = white,
                CurrentPosition = _rules.GetStartingPosition(),
                CurrentPlayer = _rules.GetStartingPlayer()
            }, _rules);

            return _gameManager;
        }

        public IGameManager StartNewGame()
        {
            _gameManager = new GameManager(new Game() 
            {
                CurrentPosition = _rules.GetStartingPosition(),
                CurrentPlayer = _rules.GetStartingPlayer()
            }, _rules);

            return _gameManager;
        }
    }
}
