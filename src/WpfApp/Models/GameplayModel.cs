using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace WpfApp.Models
{
    public class GameplayModel : INotifyPropertyChanged
    {
        public GameplayModel()
        {
            ValidPositionSelections = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            MovesHistory = new ObservableCollection<UiMoveHistoryItem>();
            BestMoveButtonContent = "Best move";
        }

        public ObservableCollection<ObservableCollection<Square>> Board { get; set; }

        public ObservableCollection<UiMoveHistoryItem> MovesHistory { get; set; }

        public bool IsUndoPossible { get { return isUndoPossible; } set { isUndoPossible = value; NotifyPropertyChanged("IsUndoPossible"); } }

        public bool IsRedoPossible { get { return isRedoPossible; } set { isRedoPossible = value; NotifyPropertyChanged("IsRedoPossible"); } }

        public bool IsBestMovePossible { get { return isBestMovePossible; } set { isBestMovePossible = value; NotifyPropertyChanged("IsBestMovePossible"); } }

        public bool IsAutoplayPossible { get { return isAutoplayPossible; } set { isAutoplayPossible = value; NotifyPropertyChanged("IsAutoplayPossible"); } }

        public bool IsAutoplayRunning { get { return isAutoplayRunning; } set { isAutoplayRunning = value; NotifyPropertyChanged("IsAutoplayRunning"); } }

        public string BestMoveButtonContent { get { return bestMoveButtonContent; } set { bestMoveButtonContent = value; NotifyPropertyChanged("BestMoveButtonContent"); } }

        private string bestMoveButtonContent;
        private string gameplayUserMessage;
        private string gameplayErrorMessage;
        private bool isUndoPossible;
        private bool isRedoPossible;
        private bool isBestMovePossible;
        private bool isAutoplayPossible;
        private bool isAutoplayRunning;

        public string GameplayErrorMessage
        {
            get
            {
                return gameplayErrorMessage;
            }
            set
            {
                gameplayErrorMessage = value;
                NotifyPropertyChanged("GameplayErrorMessage");
            }
        }

        public string GameplayUserMessage
        {
            get
            {
                return gameplayUserMessage;
            }
            set
            {
                gameplayUserMessage = value;
                NotifyPropertyChanged("GameplayUserMessage");
            }
        }

        public PlayerDetails BlackPlayer { get; set; }

        public PlayerDetails WhitePlayer { get; set; }

        public bool HelpVisible
        {
            get
            {
                return MovesHistory.Count == 0;
            }
        }

        public bool WhiteLevelVisible 
        { 
            get
            {
                return WhitePlayer.Type == PlayerType.Computer.ToString();
            } 
        }

        public bool BlackLevelVisible
        {
            get
            {
                return BlackPlayer.Type == PlayerType.Computer.ToString();
            }
        }

        public Tuple<Square, IList<Tuple<int, int>>> PreviouslySuggestedPositions { get; set; }

        public IDictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> ValidPositionSelections { get; set; }

        public Tuple<Square, Tuple<int, int>> PreviousSuggestedBestMove { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _numberOfMovesWithoutRemovedStone;
        public int NumberOfMovesWithoutRemovedStone
        {
            get
            {
                return _numberOfMovesWithoutRemovedStone;
            }

            set
            {
                _numberOfMovesWithoutRemovedStone = value;
                NotifyPropertyChanged("NumberOfMovesWithoutRemovedStone");
            }
        }
    }

    public class PlayerDetails : INotifyPropertyChanged
    {
        public string Type { get; set; }

        public string Level { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _remainingStones;
        public int RemainingStones
        {
            get
            {
                return _remainingStones;
            }

            set
            {
                _remainingStones = value;
                NotifyPropertyChanged("RemainingStones");
            }
        }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Square : INotifyPropertyChanged
    {
        private BitmapImage _imageSource;

        public BitmapImage ImageSource 
        {
            get
            {
                return _imageSource;
            }

            set 
            {
                _imageSource = value;
                NotifyPropertyChanged("ImageSource");
            }
        }

        public Tuple<int, int> Coords { get; set; }

        public SquareStatus SquareStatus { get; set; }

        public UiSquareStatus UiSquareStatus { get; set; }

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum UiSquareStatus
    {
        Empty,
        Black,
        White,
        BlackSelected,
        WhiteSelected,
        BlackBestMoveFrom,
        WhiteBestMoveFrom,
        PossibleMove,
        BestPossibleMove
    }

    public class UiMoveHistoryItem
    {
        public int MoveNumber { get; set; }

        public string UndoRedo { get; set; }

        public string Black { get; set; }

        public string White { get; set; }
    }
}
