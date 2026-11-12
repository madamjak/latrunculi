using Latrunculi.Managers;
using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WpfApp.Navigation;

namespace WpfApp.ViewModels
{
    public class MainMenuViewModel : BaseViewModel, IPageViewModel
    {
        private readonly IMainMenuManager _mainMenuManager;

        public MainMenuViewModel(IMainMenuManager mainMenuManager)
        {
            _mainMenuManager = mainMenuManager;
        }

        public bool CanPlay
        {
            get
            {
                return _mainMenuManager != null && _mainMenuManager.CanPlay();
            }
        }

        public string UserMessage { get; set; }

        private ICommand _play;
        private ICommand _startNew;
        private ICommand _load;
        private ICommand _exit;
        private ICommand _save;

        public ICommand PlayGame
        {
            get
            {
                return _play ?? (_play = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToGameplay", _mainMenuManager.Play());
                }, y => CanPlay));
            }
        }

        public ICommand SaveGame
        {
            get
            {
                return _save ?? (_save = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToLoader", "save");
                }, y => CanPlay));
            }
        }

        public ICommand StartNewGame
        {
            get
            {
                return _startNew ?? (_startNew = new RelayCommand<string>(x =>
                {
                    Mediator.Notify("GoToSetupPlayers", "new");
                }));
            }
        }

        public ICommand LoadExistingGame
        {
            get
            {
                return _load ?? (_load = new RelayCommand<string>(x =>
                {
                    Mediator.Notify("GoToLoader", "load");
                }));
            }
        }

        
        public ICommand Exit
        {
            get
            {
                return _exit ?? (_exit = new RelayCommand(x =>
                {
                    Application curApp = Application.Current;
                    curApp.Shutdown();
                }));
            }
        }
    }
}
