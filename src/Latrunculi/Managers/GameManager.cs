using Latrunculi.DataAccess;
using Latrunculi.Entities;
using Latrunculi.Logic;
using Latrunculi.Managers.Interfaces;
using Latrunculi.UI.Managers;
using System;
using System.Collections.Generic;

namespace Latrunculi.Managers
{
    public class GameManager : IGameManager
    {
        private readonly IFileWriter _fileWriter;
        private readonly IGameplayManager _gameplayManager;

        private Game _game;

        public GameManager(Game game, IGameRules rules)
        {
            _game = game;
            _fileWriter = new FileWriter();
            _gameplayManager = new GameplayManager(_game, rules);

        }

              
        public void SetBlackPlayer(Player newPlayer)
        {
            _game.BlackPlayer = newPlayer;
        }

        public void SetWhitePlayer(Player newPlayer)
        {
            _game.WhitePlayer = newPlayer;
        }

        public IGameplayManager PlayGame()
        {
            return _gameplayManager;
        }

        public void SaveGame(string path)
        {
            _fileWriter.SaveGame(_game, path);
        }

        public bool IsBlackPlayerSet()
        {
            return _game.BlackPlayer != null;
        }

        public bool IsWhitePlayerSet()
        {
            return _game.WhitePlayer != null;
        }

        public Player GetBlackPlayer()
        {
            return _game.BlackPlayer;
        }

        public Player GetWhitePlayer()
        {
            return _game.WhitePlayer;
        }
    }
}
