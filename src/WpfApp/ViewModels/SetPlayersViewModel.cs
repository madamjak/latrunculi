using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WpfApp.Models;
using WpfApp.Navigation;

namespace WpfApp.ViewModels
{
    public class SetPlayersViewModel : BaseViewModel, IPageViewModel
    {
        private readonly IGameManager _gameManager;
        private readonly string _caller;
        private ICommand _play;
        private ICommand _mainMenu;
        private PlayerType blackPlayerType;
        private PlayerLevel blackPlayerLevel;
        private PlayerType whitePlayerType;
        private PlayerLevel whitePlayerLevel;
        private bool whitePlayerLevelVisible;
        private bool blackPlayerLevelVisible;

        public SetPlayersViewModel(IMainMenuManager mainMenuManager, object obj)
        {
            _caller = obj as string;

            if (_caller.Equals("edit"))
            {
                _gameManager = mainMenuManager.Play();

                BlackPlayerLevel = _gameManager.GetBlackPlayer().Level;
                BlackPlayerType = _gameManager.GetBlackPlayer().Type;
                WhitePlayerLevel = _gameManager.GetWhitePlayer().Level;
                WhitePlayerType = _gameManager.GetWhitePlayer().Type;
            }
            else
            {
                _gameManager = mainMenuManager.StartNewGame();
            }
        }

        public PlayerType BlackPlayerType 
        {
            get 
            {
                return blackPlayerType;
            }
            set
            { 
                blackPlayerType = value;
                BlackPlayerLevelVisible = blackPlayerType == PlayerType.Computer;
            }
        }

        public PlayerType WhitePlayerType
        {
            get
            {
                return whitePlayerType;
            }
            set
            {
                whitePlayerType = value;
                WhitePlayerLevelVisible = whitePlayerType == PlayerType.Computer;
            }
        }

        public PlayerLevel BlackPlayerLevel { get => blackPlayerLevel; set => blackPlayerLevel = value; }

        public PlayerLevel WhitePlayerLevel { get => whitePlayerLevel; set => whitePlayerLevel = value; }

        public bool WhitePlayerLevelVisible
        {
            get
            {
                return whitePlayerLevelVisible;
            }
            set
            {
                whitePlayerLevelVisible = value;
                OnPropertyChanged("WhitePlayerLevelVisible");
            }
        }

        public bool BlackPlayerLevelVisible
        {
            get
            {
                return blackPlayerLevelVisible;
            }
            set
            {
                blackPlayerLevelVisible = value;
                OnPropertyChanged("BlackPlayerLevelVisible");
            }
        }

        public bool IsPlayButtonEnabled
        {
            get
            {
                return BlackPlayerType != PlayerType.NotSpecified && WhitePlayerType != PlayerType.NotSpecified &&
                    (BlackPlayerType == PlayerType.Human || BlackPlayerLevel != PlayerLevel.NotSpecified) &&
                    (WhitePlayerType == PlayerType.Human || WhitePlayerLevel != PlayerLevel.NotSpecified);
            }
        }

        public ICommand PlayGame
        {
            get
            {
                return _play ?? (_play = new RelayCommand(x =>
                {
                    SetPlayers();
                    Mediator.Notify("GoToGameplay");
                }, y => IsPlayButtonEnabled));
            }
        }

        public ICommand MainMenu
        {
            get
            {
                return _mainMenu ?? (_mainMenu = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToMainMenu");
                }));
            }
        }

        private void SetPlayers()
        {
            _gameManager.SetBlackPlayer(new Player()
            {
                Level = BlackPlayerLevel,
                Type = BlackPlayerType
            });

            _gameManager.SetWhitePlayer(new Player()
            {
                Level = WhitePlayerLevel,
                Type = WhitePlayerType
            });
        }
    }
}
