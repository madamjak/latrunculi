using Latrunculi.Entities;
using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp.Adapters;
using WpfApp.Adapters.Interfaces;
using WpfApp.Models;
using WpfApp.Navigation;

namespace WpfApp.ViewModels
{
    public class GamePlayViewModel : BaseViewModel, IPageViewModel
    {
        private readonly IGameplayManagerAdapter _gameplayManagerAdapter;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Square _selectedFrom { get; set; }
        private Square _selectedTo { get; set; }

        private RelayCommand _selected;
        private RelayCommand _mainMenu;
        private RelayCommand _undo;
        private RelayCommand _redo;
        private RelayCommand _play;
        private RelayCommand _pause;
        private RelayCommand _bestMove;
        private RelayCommand<string> _editPlayers;

        public GameplayModel Model { get; set; }

        public GamePlayViewModel(IGameManager gameManager)
        {
            _gameplayManagerAdapter = new GameplayManagerAdapter(gameManager);

            SetupModel();
        }

        private void SetupModel()
        {
            Model = _gameplayManagerAdapter.GetStartingGamePlayModel(Model);
        }

        public ICommand PositionSelected
        {
            get
            {
                return _selected ?? (_selected = new RelayCommand(x =>
                {
                    OnPositionSelected(x);
                }, y => CanSelectPosition(y)));
            }
        }

        public ICommand MainMenu
        {
            get
            {
                return _mainMenu ?? (_mainMenu = new RelayCommand(x =>
                {
                    _cancellationTokenSource.Cancel();
                    Mediator.Notify("GoToMainMenu");
                }, y => !Model.IsAutoplayRunning));
            }
        }

        public ICommand EditPlayers
        {
            get
            {
                return _editPlayers ?? (_editPlayers = new RelayCommand<string>(x =>
                {
                    _cancellationTokenSource.Cancel();
                    Mediator.Notify("GoToSetupPlayers", "edit");
                }, y => !Model.IsAutoplayRunning));
            }
        }

        public ICommand Undo
        {
            get
            {
                return _undo ?? (_undo = new RelayCommand(x =>
                {
                    _cancellationTokenSource.Cancel();
                    _gameplayManagerAdapter.HidePreviouslyBestPossibleMove(Model);
                    _gameplayManagerAdapter.HidePreviouslySuggestedPossibleMoves(Model);
                    _gameplayManagerAdapter.Undo(Model);
                }, y => Model.IsUndoPossible && !Model.IsAutoplayRunning));
            }
        }

        public ICommand Redo
        {
            get
            {
                return _redo ?? (_redo = new RelayCommand(x =>
                {
                    _cancellationTokenSource.Cancel();
                    _gameplayManagerAdapter.HidePreviouslyBestPossibleMove(Model);
                    _gameplayManagerAdapter.HidePreviouslySuggestedPossibleMoves(Model);
                    _gameplayManagerAdapter.Redo(Model);
                }, y => Model.IsRedoPossible && !Model.IsAutoplayRunning));
            }
        }

        public ICommand Play
        {
            get
            {
                return _play ?? (_play = new RelayCommand(x =>
                {
                    Autoplay();
                }, y => Model.IsAutoplayPossible && !Model.IsAutoplayRunning));
            }
        }

        private async void Autoplay()
        {
            Model.IsAutoplayRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                while (Model.IsAutoplayPossible)
                {
                    MoveCommandHistoryItem history = null;
                    history = await Task.Run(() =>
                    {
                        history = _gameplayManagerAdapter.Autoplay(_cancellationTokenSource.Token);
                        Thread.Sleep(200);
                        return history;
                    });

                    _gameplayManagerAdapter.UpdateModel(Model, history);
                }
            }
            catch (OperationCanceledException)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            Model.IsAutoplayRunning = false;
            EnsureCanExecuteCommandsIsSetCorrectly();
        }

        private void EnsureCanExecuteCommandsIsSetCorrectly()
        {
            _selected.RaiseCanExecuteChanged();
            _mainMenu.RaiseCanExecuteChanged();
            _undo.RaiseCanExecuteChanged();
            _redo.RaiseCanExecuteChanged();
            _play.RaiseCanExecuteChanged();
            _pause.RaiseCanExecuteChanged();
            _bestMove.RaiseCanExecuteChanged();
            _editPlayers.RaiseCanExecuteChanged();
        }

        public ICommand Pause
        {
            get
            {
                return _pause ?? (_pause = new RelayCommand(x =>
                {
                    _cancellationTokenSource.Cancel();
                }, y => Model.IsAutoplayRunning));
            }
        }

        public ICommand BestMove
        {
            get
            {
                return _bestMove ?? (_bestMove = new RelayCommand(x =>
                {
                    _gameplayManagerAdapter.HidePreviouslyBestPossibleMove(Model);
                    GetBestMove();
                }, y => Model.IsBestMovePossible && !Model.IsAutoplayRunning));
            }
        }

        private async void GetBestMove()
        {
            Model.IsBestMovePossible = false;
            Model.BestMoveButtonContent = "Thinking...";
            _cancellationTokenSource = new CancellationTokenSource();

            MoveStoneCommand result = null;
            try
            {
                result = await Task.Run(() =>
                {
                    return _gameplayManagerAdapter.GetCurrentPlayerBestMove(_cancellationTokenSource.Token);
                });
            }
            catch (OperationCanceledException)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            //ifit wasn't cancelled
            if (result != null)
            {
                _gameplayManagerAdapter.DisplayBestPossibleMove(result, Model);
            }

            Model.IsBestMovePossible = true;
            Model.BestMoveButtonContent = "Best move";

            EnsureCanExecuteCommandsIsSetCorrectly();
        }

        private bool CanSelectPosition(object y)
        {
            return !Model.IsAutoplayRunning && !_gameplayManagerAdapter.IsCurrentPlayerComputer() && _gameplayManagerAdapter.IsGameOpen();
        }

        private void OnPositionSelected(object x)
        {
            var square = x as Square;
            
            if(Model.ValidPositionSelections.ContainsKey(square.Coords))
            {
                _selectedFrom = square;
                OnSelectedFromStone();
                return;
            }

            if (_selectedFrom != null && Model.ValidPositionSelections[_selectedFrom.Coords].Contains(square.Coords))
            {
                _selectedTo = square;
                OnSelectedToStone();

                _selectedFrom = null;
                _selectedTo = null;
                Model.GameplayErrorMessage = string.Empty;
                return;
            }

            Model.GameplayErrorMessage = "Invalid move!";
        }

        private void OnSelectedToStone()
        {
            if(!Model.IsBestMovePossible) //bestMove calc is running so cancel it as we decided to make move 
            {
                _cancellationTokenSource.Cancel();
                Thread.Sleep(100);
            }

            _cancellationTokenSource = new CancellationTokenSource();

            _gameplayManagerAdapter.HidePreviouslyBestPossibleMove(Model);
            _gameplayManagerAdapter.HidePreviouslySuggestedPossibleMoves(Model);

            _gameplayManagerAdapter.PerformMove(new MoveStoneCommand()
            {
                From = _selectedFrom.Coords,
                To = _selectedTo.Coords
            }, Model, _cancellationTokenSource.Token);

            if (_gameplayManagerAdapter.IsCurrentPlayerComputer())
            {
                Autoplay();
            }
        }

        private void OnSelectedFromStone()
        {
            _gameplayManagerAdapter.HidePreviouslySuggestedPossibleMoves(Model);
            _gameplayManagerAdapter.DisplayPossibleMoves(Model, _selectedFrom);
        }
    }
}
