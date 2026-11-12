using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Extensions;
using Latrunculi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Latrunculi.Logic
{
    public interface IGameRules
    {
        BoardPosition GetStartingPosition();

        PlayerColor GetStartingPlayer();

        PlayerColor GetPlayerOnTurn(BoardPosition position);

        bool IsValidPlayerMove(BoardPosition position, MoveStoneCommand command, PlayerColor currentPlayer);

        BoardPosition GetUpdatedPosition(BoardPosition position, MoveStoneCommand command);

        IList<MoveStoneCommand> GetPossibleMovesSorted(BoardPosition position, PlayerColor currentPlayer);

        IDictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetCurrentPlayerPossibleMoves(BoardPosition position, PlayerColor currentPlayer);

        bool IsTerminalPosition(BoardPosition position, PlayerColor playerColor);

        double EvaluatePosition(BoardPosition position, PlayerColor playerColor);
    }

    public class LatrunculiRules : IGameRules
    {
        private const int BoardWidth = 8;

        private const int BoardHeight = 7;

        private const int MaxNumberOfMovesWithoutRemovingStone = 30;

        public IList<MoveStoneCommand> GetPossibleMovesSorted(BoardPosition position, PlayerColor currentPlayer)
        {
            return LatrunculiBoardStats.GetPossibleMoves(position.Board, currentPlayer.GetBoardColor());
        }

        public BoardPosition GetStartingPosition()
        {
            var board = new SquareStatus[BoardHeight, BoardWidth];

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (i == 0 || i == 1)
                    {
                        board[i, j] = SquareStatus.Black;
                    }
                    else if (i == 5 || i == 6)
                    {
                        board[i, j] = SquareStatus.White;
                    }
                    else
                    {
                        board[i, j] = SquareStatus.Empty;
                    }
                }
            }

            return new BoardPosition()
            {
                Board = board,
                Status = BoardPositionStatus.Open,
                LastPlayerColor = SquareStatus.Black,
                BlackPlayerStonesCount = 16,
                WhitePlayerStonesCount = 16
        };
        }

        public BoardPosition GetUpdatedPosition(BoardPosition position, MoveStoneCommand command)
        {
            var updatedBoard = position.Board;

            var currentColor = updatedBoard[command.From.Item1, command.From.Item2];
            var opponentColor = currentColor.GetOponentColor();

            updatedBoard[command.From.Item1, command.From.Item2] = SquareStatus.Empty;
            updatedBoard[command.To.Item1, command.To.Item2] = currentColor;

            var neighbours = updatedBoard.GetNeighbours(command.To, opponentColor);

            var removedStones = new List<Tuple<int, int>>();
            foreach (var oponent in neighbours)
            {
                if (ShouldBeRemoved(updatedBoard, oponent, command.To, currentColor))
                {
                    updatedBoard[oponent.Item1, oponent.Item2] = SquareStatus.Empty;
                    removedStones.Add(oponent);
                }
            }

            var newPosition = new BoardPosition()
            {
                Board = updatedBoard,
                LastMoveStoneCommand = command,
                NumberOfMovesWithoutRemovedStoneCounter = removedStones.Count > 0 ? 0 : position.NumberOfMovesWithoutRemovedStoneCounter + 1,
                LastPlayerColor = currentColor,
                RemovedStonesInLastMove = removedStones,
                BlackPlayerStonesCount = position.BlackPlayerStonesCount,
                WhitePlayerStonesCount = position.WhitePlayerStonesCount
            };

            newPosition.Status = GetPositionStatus(newPosition, currentColor);

            return newPosition;
        }

        private BoardPositionStatus GetPositionStatus(BoardPosition newPosition, SquareStatus currentColor)
        {
            var stats = LatrunculiBoardStats.GetMyBoardPositionStats(newPosition.Board, currentColor, false, false, false);

            if (currentColor == SquareStatus.Black && stats.OponentCount == 0)
            {
                return BoardPositionStatus.BlackWin;
            }

            if (currentColor == SquareStatus.White && stats.OponentCount == 0)
            {
                return BoardPositionStatus.WhiteWin;
            }

            if (newPosition.NumberOfMovesWithoutRemovedStoneCounter == MaxNumberOfMovesWithoutRemovingStone)
            {
                if (stats.MyCount == stats.OponentCount)
                {
                    return BoardPositionStatus.Draw;
                }

                if (stats.MyCount > stats.OponentCount)
                {
                    return currentColor == SquareStatus.White ? BoardPositionStatus.WhiteWin : BoardPositionStatus.BlackWin;
                }
                else
                {
                    return currentColor == SquareStatus.White ? BoardPositionStatus.BlackWin : BoardPositionStatus.WhiteWin;
                }
            }

            return BoardPositionStatus.Open;
        }

        private bool ShouldBeRemoved(SquareStatus[,] currentBoard, Tuple<int, int> oponentNeighbour, Tuple<int, int> to, SquareStatus currentColor)
        {
            //corner scenarios
            if (
                oponentNeighbour.Item1 == 0 && oponentNeighbour.Item2 == 0 && currentBoard[1, 0] == currentColor && currentBoard[0, 1] == currentColor ||
                oponentNeighbour.Item1 == 0 && oponentNeighbour.Item2 == BoardWidth - 1 && currentBoard[1, BoardWidth - 1] == currentColor && currentBoard[0, BoardWidth - 2] == currentColor ||
                oponentNeighbour.Item1 == BoardHeight - 1 && oponentNeighbour.Item2 == 0 && currentBoard[BoardHeight - 2, 0] == currentColor && currentBoard[BoardHeight - 1, 1] == currentColor ||
                oponentNeighbour.Item1 == BoardHeight - 1 && oponentNeighbour.Item2 == BoardWidth - 1 && currentBoard[BoardHeight - 1, BoardWidth - 2] == currentColor && currentBoard[BoardHeight - 2, BoardWidth - 1] == currentColor)
            {
                return true;
            }

            //between left - right
            if (oponentNeighbour.Item2 - 1 >= 0 && oponentNeighbour.Item2 + 1 < BoardWidth &&
                currentBoard[oponentNeighbour.Item1, oponentNeighbour.Item2 - 1] == currentColor &&
                currentBoard[oponentNeighbour.Item1, oponentNeighbour.Item2 + 1] == currentColor &&
                oponentNeighbour.Item1 == to.Item1)
            {
                return true;
            }

            //between top - bottom
            if (oponentNeighbour.Item1 - 1 >= 0 && oponentNeighbour.Item1 + 1 < BoardHeight &&
                currentBoard[oponentNeighbour.Item1 - 1, oponentNeighbour.Item2] == currentColor &&
                currentBoard[oponentNeighbour.Item1 + 1, oponentNeighbour.Item2] == currentColor &&
                oponentNeighbour.Item2 == to.Item2)
            {
                return true;
            }

            return false;
        }

        public bool IsValidPlayerMove(BoardPosition position, MoveStoneCommand command, PlayerColor playerColor)
        {
            return command.From.Item1 >= 0 && command.From.Item1 < BoardHeight && command.From.Item2 >= 0 && command.From.Item2 < BoardWidth && //isFromOnBoard
                command.To.Item1 >= 0 && command.To.Item1 < BoardHeight && command.To.Item2 >= 0 && command.To.Item2 < BoardWidth && //isToOnBoard
                position.Board[command.From.Item1, command.From.Item2] == GetCurrentPlayerColor(playerColor) && //isMovingCorrectColor
                position.Board[command.To.Item1, command.To.Item2] == SquareStatus.Empty && //isTargetEmpty
                (command.From.Item1 == command.To.Item1 && Math.Abs(command.To.Item2 - command.From.Item2) == 1 || //moving only one block orthogonally
                command.From.Item2 == command.To.Item2 && Math.Abs(command.To.Item1 - command.From.Item1) == 1);
        }

        public SquareStatus GetCurrentPlayerColor(PlayerColor playerColor)
        {
            return playerColor.GetBoardColor();
        }

        private bool IsWin(BoardPosition position, PlayerColor playerColor)
        {
            return playerColor == PlayerColor.White && position.Status == BoardPositionStatus.WhiteWin ||
                playerColor == PlayerColor.Black && position.Status == BoardPositionStatus.BlackWin;
        }

        private bool IsLoose(BoardPosition position, PlayerColor playerColor)
        {
            return playerColor == PlayerColor.White && position.Status == BoardPositionStatus.BlackWin ||
                playerColor == PlayerColor.Black && position.Status == BoardPositionStatus.WhiteWin;
        }

        private bool IsDraw(BoardPosition position)
        {
            return position.Status == BoardPositionStatus.Draw;
        }

        public double EvaluatePosition(BoardPosition position, PlayerColor playerColor)
        {
            if (IsWin(position, playerColor))
            {
                return double.PositiveInfinity;
            }

            if (IsLoose(position, playerColor))
            {
                return double.NegativeInfinity;
            }

            if (IsDraw(position))
            {
                return 0;
            }

            var squareColor = playerColor.GetBoardColor();

            var myStats = LatrunculiBoardStats.GetMyBoardPositionStats(position.Board, squareColor, false, false, true);

            var rank = 0;
            foreach (var stone in myStats.PositionsStats)
            {
                var posrank = stone.LegalMoves.Count * 10 + stone.OptionsOfBeingRemoved * 30 * -1 + stone.OptionsToTryToAttack * 20;
                rank += posrank;
            }

            foreach (var stone in myStats.OponentStats)
            {
                var posrank = stone.LegalMoves.Count * 10 * -1 + stone.OptionsOfBeingRemoved * 30 + -1 * stone.OptionsToTryToAttack * 20;
                rank += posrank;

            }

            var rcrank = 100 * (myStats.MyCount - myStats.OponentCount);
            rank += rcrank;

            rank += -1 * 50 * position.NumberOfMovesWithoutRemovedStoneCounter;

            return rank;
        }

        public PlayerColor GetStartingPlayer()
        {
            return PlayerColor.White;
        }

        public bool IsTerminalPosition(BoardPosition position, PlayerColor playerColor)
        {
            return IsWin(position, playerColor) || IsLoose(position, playerColor) || IsDraw(position);
        }

        public PlayerColor GetPlayerOnTurn(BoardPosition position)
        {
            return position.LastPlayerColor == SquareStatus.Black ? PlayerColor.White : PlayerColor.Black;
        }

        public IDictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetCurrentPlayerPossibleMoves(BoardPosition position, PlayerColor currentPlayer)
        {
            var result = new Dictionary<Tuple<int, int>, HashSet<Tuple<int, int>>>();
            
            for(int i = 0; i < position.Board.GetLength(0); i++)
            {
                for (int j = 0; j < position.Board.GetLength(1); j++)
                {
                    if (position.Board[i, j] == currentPlayer.GetBoardColor())
                    {
                        var myPos = new Tuple<int, int>(i, j);
                        var possibleMoves = new HashSet<Tuple<int, int>>(position.Board.GetNeighbours(myPos, SquareStatus.Empty));

                        if(possibleMoves.Count > 0)
                        {
                            result.Add(myPos, possibleMoves);
                        }
                    }
                }
            }

            return result;
        }
    }
}
