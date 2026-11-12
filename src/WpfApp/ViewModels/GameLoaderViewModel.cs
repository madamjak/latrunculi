using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WpfApp.Navigation;

namespace WpfApp.ViewModels
{
    public class GameLoaderViewModel : BaseViewModel, IPageViewModel
    {
        private readonly IMainMenuManager _mainMenuManager;

        private readonly string _caller;

        public GameLoaderViewModel(IMainMenuManager mainMenuManager, object obj)
        {
            _mainMenuManager = mainMenuManager;

            _caller = obj as string;
        }

        public bool SaveVisible 
        {
            get
            {
                return _caller.Equals("save");
            }
        }

        public bool LoadVisible
        {
            get
            {
                return _caller.Equals("load");
            }
        }

        public string PathToFile { get; set; }

        private string _userMessage;
        public string UserMessage 
        {
            get 
            {
                return _userMessage;
            }
            set 
            {
                _userMessage = value;
                OnPropertyChanged("UserMessage");
            }
        }


        private ICommand _save;
        private ICommand _load;
        private ICommand _mainMenu;

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

        public ICommand Save
        {
            get
            {
                return _save ?? (_save = new RelayCommand(x =>
                {
                    SaveGame();
                }, y => !string.IsNullOrWhiteSpace(PathToFile)));
            }
        }

        public ICommand Load
        {
            get
            {
                return _load ?? (_load = new RelayCommand(x =>
                {
                    LoadGame();
                }, y => !string.IsNullOrWhiteSpace(PathToFile)));
            }
        }

        private void LoadGame()
        {
            try
            {
                _mainMenuManager.LoadGame(PathToFile);
            }
            catch
            {
                UserMessage = "Unexpected error when loading game file. Please try again. Make sure you select valid file.";
                return;
            }

            Mediator.Notify("GoToGameplay");
        }

        private void SaveGame()
        {
            try
            {
                _mainMenuManager.SaveGame(PathToFile);
            }
            catch
            {
                UserMessage = "Unexpected error when saving game. Please try again.";
                return;
            }

            Mediator.Notify("GoToMainMenu");
        }

    }
}
