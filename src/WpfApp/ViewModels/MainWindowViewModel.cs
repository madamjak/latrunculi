using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfApp.Navigation;

namespace WpfApp.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IMainMenuManager _mainMenuManager;

        private IPageViewModel _currentPageViewModel;

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            CurrentPageViewModel = viewModel;
        }

        private void OnGoToMainMenu(object obj)
        {
            ChangeViewModel(new MainMenuViewModel(_mainMenuManager));
        }

        private void OnGoToSetupPlayers(object obj)
        {
            ChangeViewModel(new SetPlayersViewModel(_mainMenuManager, obj));
        }

        private void GoToGameplay(object obj)
        {
            ChangeViewModel(new GamePlayViewModel(_mainMenuManager.Play()));
        }

        private void GoToGameLoader(object obj)
        {
            ChangeViewModel(new GameLoaderViewModel(_mainMenuManager, obj));
        }

        public MainWindowViewModel(IMainMenuManager mainMenuManager)
        {
            _mainMenuManager = mainMenuManager;


            CurrentPageViewModel = new MainMenuViewModel(mainMenuManager);

            Mediator.Subscribe("GoToMainMenu", OnGoToMainMenu);
            Mediator.Subscribe("GoToSetupPlayers", OnGoToSetupPlayers);
            Mediator.Subscribe("GoToGameplay", GoToGameplay);
            Mediator.Subscribe("GoToLoader", GoToGameLoader);
        }
    }
}
