using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Extensions;
using Latrunculi.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Latrunculi.Logic
{
    public interface IGameAi
    {
        MoveStoneCommand GetBestPossibleMove(BoardPosition currentPosition, PlayerColor currentPlayer, PlayerLevel level, bool isBestMoveScenario, CancellationToken cancellationToken);
    }

    /// <summary>
    /// calculate best possible move using alfa-beta pruning 
    /// https://soubor.inf.upol.cz/ws-public/Studium/Kombinovane/Projektovy-seminar
    /// https://www.mygreatlearning.com/blog/alpha-beta-pruning-in-ai/
    /// 
    /// </summary>
    public class AlphaBetaMinMax : IGameAi
    {
        private readonly IGameRules _rules;

        public AlphaBetaMinMax(IGameRules gameRules)
        {
            _rules = gameRules;
        }

        public MoveStoneCommand GetBestPossibleMove(BoardPosition currentPosition, PlayerColor currentPlayer, PlayerLevel level, bool isBestMoveScenario, CancellationToken cancellationToken)
        {
            var position = currentPosition.CopyObject<BoardPosition>();

            cancellationToken.ThrowIfCancellationRequested();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var possibleMoves = _rules.GetPossibleMovesSorted(position, currentPlayer);

            var depth = GetDepth(level, isBestMoveScenario);
            var maxElapsed = GetMaxElapsedMs(level, isBestMoveScenario);

            MoveStoneCommand bestMove = possibleMoves.First();
            var bestValue = double.NegativeInfinity;
            var alfa = double.NegativeInfinity;

            foreach (var move in possibleMoves)
            {
                var updatedPosition = _rules.GetUpdatedPosition(position, move);

                var childValue = AlfabetaMinimax(updatedPosition, depth - 1, double.NegativeInfinity, double.PositiveInfinity, false, currentPlayer, cancellationToken);

                position.Board = BoardHelper.GetPreviousBoardPosition(updatedPosition, updatedPosition.LastMoveStoneCommand);

                if (stopWatch.ElapsedMilliseconds >= maxElapsed)
                {
                    break;
                }

                if (bestValue < childValue)
                {
                    bestValue = childValue;
                    bestMove = move;
                    alfa = Math.Max(alfa, bestValue);
                }
            }

            stopWatch.Stop();
            //Console.WriteLine(string.Format("elapsed ms: {0}", stopWatch.ElapsedMilliseconds));

            return bestMove;
        }

        private long GetMaxElapsedMs(PlayerLevel level, bool isBestMoveScenario)
        {
            if (isBestMoveScenario)
            {
                return 10000;
            }

            switch (level)
            {
                case PlayerLevel.Easy:
                    return 3000;
                case PlayerLevel.Medium:
                    return 6000;
                case PlayerLevel.Hard:
                    return 10000;
            }

            throw new ApplicationException("not supported level");
        }

        private int GetDepth(PlayerLevel level, bool isBestMoveScenario)
        {
            if (isBestMoveScenario)
            {
                return 4;
            }

            switch (level)
            {
                case PlayerLevel.Easy:
                    return 2;
                case PlayerLevel.Medium:
                    return 3;
                case PlayerLevel.Hard:
                    return 4;
            }

            throw new ApplicationException("not supported level");
        }

        private double AlfabetaMinimax(BoardPosition position, int depth, double alfa, double beta, bool isMaxsTurn, PlayerColor maxsColor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (depth == 0 || _rules.IsTerminalPosition(position, maxsColor))
            {
                return _rules.EvaluatePosition(position, maxsColor);
            }

            var bestValue = isMaxsTurn ? double.NegativeInfinity : double.PositiveInfinity;
            var playerColor = _rules.GetPlayerOnTurn(position);
            var possibleMoves = _rules.GetPossibleMovesSorted(position, playerColor);

            foreach (var move in possibleMoves)
            {
                var newPosition = _rules.GetUpdatedPosition(position, move);

                var childValue = AlfabetaMinimax(newPosition, depth - 1, alfa, beta, !isMaxsTurn, maxsColor, cancellationToken);

                position.Board = BoardHelper.GetPreviousBoardPosition(newPosition, newPosition.LastMoveStoneCommand);

                if (isMaxsTurn && bestValue < childValue)
                {
                    bestValue = childValue;
                    alfa = Math.Max(alfa, bestValue);
                    if (beta <= alfa)
                    {
                        break;
                    }
                }

                if (!isMaxsTurn && bestValue > childValue)
                {
                    bestValue = childValue;
                    beta = Math.Min(beta, bestValue);
                    if (beta <= alfa)
                    {
                        break;
                    }
                }
            }

            return bestValue;
        }
    }
}
