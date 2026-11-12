using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Extensions;
using Latrunculi.Helpers;
using Latrunculi.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Latrunculi.Logic
{
    public static class LatrunculiBoardStats
    {
        private const int BoardWidth = 8;

        private const int BoardHeight = 7;

        public static IList<MoveStoneCommand> GetPossibleMoves(SquareStatus[,] board, SquareStatus squareStatus)
        {
            var stats = GetMyBoardPositionStats(board, squareStatus, true, true, true);

            //1 - attack
            //2 - try escape
            //3 - try attack
            //4 - random
            var movesByRank = new Dictionary<int, HashSet<MoveStoneCommand>>();

            var result = new List<MoveStoneCommand>();

            foreach (var position in stats.OponentStats)
            {
                foreach (var c in position.CommandsForBeingRemoved)
                {
                    if (movesByRank.ContainsKey(1))
                    {
                        movesByRank[1].Add(c);
                    }
                    else
                    {
                        movesByRank.Add(1, new HashSet<MoveStoneCommand>() { c });
                    }

                }
            }

            foreach (var position in stats.PositionsStats)
            {
                foreach (var c in position.CommandsToTryEscape)
                {
                    if (!movesByRank.DictionaryByRankContainsMove(1, c))
                    {
                        if (movesByRank.ContainsKey(2))
                        {
                            movesByRank[2].Add(c);
                        }
                        else
                        {
                            movesByRank.Add(2, new HashSet<MoveStoneCommand>() { c });
                        }
                    }
                }

                foreach (var m in position.CommandsToTryToAttack)
                {
                    if (!movesByRank.DictionaryByRankContainsMove(1, m) &&
                        !movesByRank.DictionaryByRankContainsMove(2, m))
                    {
                        if (movesByRank.ContainsKey(3))
                        {
                            movesByRank[3].Add(m);
                        }
                        else
                        {
                            movesByRank.Add(3, new HashSet<MoveStoneCommand>() { m });
                        }
                    }
                }

                foreach (var m in position.LegalMoves)
                {
                    var c = new MoveStoneCommand()
                    {
                        From = position.MyPosition,
                        To = m
                    };

                    if (!movesByRank.DictionaryByRankContainsMove(1, c) &&
                        !movesByRank.DictionaryByRankContainsMove(2, c) &&
                        !movesByRank.DictionaryByRankContainsMove(3, c))
                    {
                        if (movesByRank.ContainsKey(4))
                        {
                            movesByRank[4].Add(c);
                        }
                        else
                        {
                            movesByRank.Add(4, new HashSet<MoveStoneCommand>() { c });
                        }
                    }
                }
            }

            if (movesByRank.ContainsKey(1))
            {
                var l = movesByRank[1].ToList();
                l.Shuffle();
                result.AddRange(l);

                //Console.WriteLine();
                //Console.WriteLine("rank 1 moves:");

                //foreach(var m in l)
                //{
                //    Console.WriteLine(m.GetStringCommand());
                //}
            }

            if (movesByRank.ContainsKey(2))
            {
                var l = movesByRank[2].ToList();
                l.Shuffle();
                result.AddRange(l);

                //Console.WriteLine();
                //Console.WriteLine("rank 2 moves:");

                //foreach (var m in l)
                //{
                //    Console.WriteLine(m.GetStringCommand());
                //}
            }

            if (movesByRank.ContainsKey(3))
            {
                var l = movesByRank[3].ToList();
                l.Shuffle();
                result.AddRange(l);

                //Console.WriteLine();
                //Console.WriteLine("rank 3 moves:");

                //foreach (var m in l)
                //{
                //    Console.WriteLine(m.GetStringCommand());
                //}
            }

            if (movesByRank.ContainsKey(4))
            {
                var l = movesByRank[4].ToList();
                l.Shuffle();
                result.AddRange(l);

                //Console.WriteLine();
                //Console.WriteLine("rank 3 moves:");

                //foreach (var m in l)
                //{
                //    Console.WriteLine(m.GetStringCommand());
                //}
            }

            //Console.WriteLine();
            //Console.WriteLine("result:");

            //foreach (var m in result)
            //{
            //    Console.WriteLine(m.GetStringCommand());
            //}

            return result;
        }

        public static MyBoardPositionStats GetMyBoardPositionStats(SquareStatus[,] board, SquareStatus myColor, bool getCommands, bool getEscapeCommand, bool getPositionStats)
        {
            var result = new MyBoardPositionStats()
            {
                PositionsStats = new List<MyPositionStats>(),
                OponentStats = new List<MyPositionStats>()
            };

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == myColor)
                    {
                        result.MyCount++;

                        if (getPositionStats)
                        {
                            var myPosition = new Tuple<int, int>(i, j);

                            var optionsOfBeingRemoved = GetOptionsOfBeingRemoved(board, myPosition, myColor, getCommands, getEscapeCommand);

                            var optionsToAttack = GetOptionsToAttack(board, myPosition, myColor, getCommands);

                            result.PositionsStats.Add(new MyPositionStats()
                            {
                                MyPosition = myPosition,
                                LegalMoves = board.GetNeighbours(myPosition, SquareStatus.Empty),
                                //MyNeighbours = GetNeighbours(board, myPositions, myPosition),
                                //OponentNeightbours = GetNeighbours(board, myPositions, myPosition.GetOponentColor())
                                OptionsOfBeingRemoved = optionsOfBeingRemoved.NumberOfOptions,
                                CommandsForBeingRemoved = optionsOfBeingRemoved.CommandsToRemove,
                                OptionsToTryToAttack = optionsToAttack.NumberOfOptions,
                                CommandsToTryToAttack = optionsToAttack.Commands

                            });
                        }
                    }

                    if (board[i, j] == myColor.GetOponentColor())
                    {
                        result.OponentCount++;

                        if (getPositionStats)
                        {
                            var myPosition = new Tuple<int, int>(i, j);

                            var optionsOfBeingRemoved = GetOptionsOfBeingRemoved(board, myPosition, myColor.GetOponentColor(), getCommands, getEscapeCommand);

                            var optionsToAttack = GetOptionsToAttack(board, myPosition, myColor.GetOponentColor(), getCommands);

                            result.OponentStats.Add(new MyPositionStats()
                            {
                                MyPosition = myPosition,
                                LegalMoves = board.GetNeighbours(myPosition, SquareStatus.Empty),
                                //MyNeighbours = GetNeighbours(board, myPositions, myPosition),
                                //OponentNeightbours = GetNeighbours(board, myPositions, myPosition.GetOponentColor())
                                OptionsOfBeingRemoved = optionsOfBeingRemoved.NumberOfOptions,
                                CommandsForBeingRemoved = optionsOfBeingRemoved.CommandsToRemove,
                                OptionsToTryToAttack = optionsToAttack.NumberOfOptions,
                                CommandsToTryToAttack = optionsToAttack.Commands
                            });
                        }
                    }
                }
            }

            return result;
        }

        private static OptionsToAttack GetOptionsToAttack(SquareStatus[,] board, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands)
        {
            var result = new OptionsToAttack()
            {
                Commands = new List<MoveStoneCommand>()
            };

            result = GetOptionsToAttackHorizontal(result, board, myPosition, myColor, getCommands);

            result = GetOptionsToAttackVertical(result, board, myPosition, myColor, getCommands);

            return result;
        }

        private static OptionsToAttack GetOptionsToAttackVertical(OptionsToAttack result, SquareStatus[,] board, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands)
        {
            if (board.GetBottomLeftNeighbour(myPosition, 1, myColor) != null
                && board.GetLeftNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetTopLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetTopLeftNeighbour(myPosition, 1, myColor) != null
                && board.GetLeftNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetBottomLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetTopRightNeighbour(myPosition, 1, myColor) != null
                && board.GetRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetBottomRightNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetBottomRightNeighbour(myPosition, 1, myColor) != null
                && board.GetRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetTopRightNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetBottomLeftNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetTopLeftNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetTopRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetBottomRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            return result;
        }

        private static OptionsToAttack GetOptionsToAttackHorizontal(OptionsToAttack result, SquareStatus[,] board, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands)
        {
            if (board.GetBottomLeftNeighbour(myPosition, 1, myColor) != null
                && board.GetBottomNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetBottomRightNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetBottomRightNeighbour(myPosition, 1, myColor) != null
                && board.GetBottomNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetBottomLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetTopLeftNeighbour(myPosition, 1, myColor) != null
                && board.GetTopNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetTopRightNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetRightNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetTopRightNeighbour(myPosition, 1, myColor) != null
                && board.GetTopNeighbour(myPosition, 1, myColor.GetOponentColor()) != null
                && board.GetTopLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetLeftNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetBottomRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetBottomLeftNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetBottomNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetTopRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            if (board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty) != null
                && board.GetTopRightNeighbour(myPosition, 1, myColor.GetOponentColor()) != null)
            {
                result.NumberOfOptions++;

                if (getCommands)
                {
                    result.Commands.Add(new MoveStoneCommand(myPosition, board.GetTopNeighbour(myPosition, 1, SquareStatus.Empty)));
                }
            }

            return result;
        }

        private static OptionsOfBeingRemoved GetOptionsOfBeingRemoved(SquareStatus[,] board, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands, bool getEscapeCommand)
        {
            var result = new OptionsOfBeingRemoved()
            {
                CommandsToRemove = new List<MoveStoneCommand>()
            };

            result = GetOptionsOfBeingRemovedInCorner(result, board, myPosition, myColor, getCommands, getEscapeCommand);

            result = GetOptionsOfBeingRemovedHorizontal(result, board, myPosition, myColor, getCommands, getEscapeCommand);

            result = GetOptionsOfBeingRemovedVertical(result, board, myPosition, myColor, getCommands, getEscapeCommand);

            return result;
        }

        private static OptionsOfBeingRemoved GetOptionsOfBeingRemovedInCorner(OptionsOfBeingRemoved result, SquareStatus[,] currentBoard, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands, bool getEscapeCommand)
        {
            if (myPosition.Item1 == 0 && myPosition.Item2 == 0)
            {
                if (currentBoard.GetBottomNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetRightNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetRightNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetRightNeighbourCoords(myPosition, 2), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }

                        //wouldn't escape by moving my current position
                        if (getEscapeCommand && currentBoard.GetBottomRightNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetBottomRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetBottomRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }

                        //wouldn't escape if there would be oponent color there so don't return it as it would affect how 'good' moves are sorted
                        if (getEscapeCommand && currentBoard.GetRightNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                if (currentBoard.GetRightNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetBottomNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetBottomNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomNeighbourCoords(myPosition, 2), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetBottomRightNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetBottomRightNeighbourCoords(myPosition, 1), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetBottomRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetBottomNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                return result;
            }

            if (myPosition.Item1 == 0 && myPosition.Item2 == BoardWidth - 1)
            {
                if (currentBoard.GetBottomNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetLeftNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetLeftNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetLeftNeighbourCoords(myPosition, 2), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetBottomLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetBottomLeftNeighbourCoords(myPosition, 1), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetBottomLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomLeftNeighbourCoords(myPosition, 1), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetLeftNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                if (currentBoard.GetLeftNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetBottomNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetBottomNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomNeighbourCoords(myPosition, 2), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetBottomLeftNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetBottomLeftNeighbourCoords(myPosition, 1), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetBottomLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomLeftNeighbourCoords(myPosition, 1), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetBottomNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                return result; ;
            }

            if (myPosition.Item1 == BoardHeight - 1 && myPosition.Item2 == 0)
            {
                if (currentBoard.GetRightNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetTopNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetTopNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopNeighbourCoords(myPosition, 2), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetTopRightNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetTopRightNeighbourCoords(myPosition, 1), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetTopRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopRightNeighbourCoords(myPosition, 1), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetTopNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                if (currentBoard.GetTopNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetRightNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetRightNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetRightNeighbourCoords(myPosition, 2), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetTopRightNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetTopRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetTopRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetRightNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                return result;
            }

            if (myPosition.Item1 == BoardHeight - 1 && myPosition.Item2 == BoardWidth - 1)
            {
                if (currentBoard.GetLeftNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetTopNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetTopNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopNeighbourCoords(myPosition, 2), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetTopLeftNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetTopLeftNeighbourCoords(myPosition, 1), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetTopLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopLeftNeighbourCoords(myPosition, 1), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetTopNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                if (currentBoard.GetTopNeighbour(myPosition, 1) == myColor.GetOponentColor() &&
                    currentBoard.GetLeftNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (currentBoard.GetLeftNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetLeftNeighbourCoords(myPosition, 2), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetTopLeftNeighbour(myPosition, 1) == myColor)
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(currentBoard.GetTopLeftNeighbourCoords(myPosition, 1), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (currentBoard.GetTopLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopLeftNeighbourCoords(myPosition, 1), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }

                        if (getEscapeCommand && currentBoard.GetLeftNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        {
                            result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                return result;
            }

            return result;
        }

        private static OptionsOfBeingRemoved GetOptionsOfBeingRemovedHorizontal(OptionsOfBeingRemoved result, SquareStatus[,] currentBoard, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands, bool getEscapeCommand)
        {
            if (myPosition.Item2 - 1 >= 0 && myPosition.Item2 + 1 < BoardWidth)
            {
                if (currentBoard.GetLeftNeighbour(myPosition, 1) == myColor.GetOponentColor()
                    && currentBoard.GetRightNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (myPosition.Item2 + 2 < BoardWidth && currentBoard.GetRightNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetRightNeighbourCoords(myPosition, 2), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                        //TODO
                        //if (getEscapeCommand && currentBoard.GetLeftNeighbour(myPosition, 2) != myColor.GetOponentColor())
                        //{
                        //    result.CommandsToTryEscape.Add(new MoveStoneCommand(myPosition, currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        //}
                    }

                    if (myPosition.Item1 - 1 >= 0 && currentBoard.GetTopRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item1 + 1 < BoardHeight && currentBoard.GetBottomRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomRightNeighbourCoords(myPosition, 1), currentBoard.GetRightNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                if (currentBoard.GetRightNeighbour(myPosition, 1) == myColor.GetOponentColor()
                    && currentBoard.GetLeftNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (myPosition.Item2 - 2 >= 0 && currentBoard.GetLeftNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetLeftNeighbourCoords(myPosition, 2), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item1 - 1 >= 0 && currentBoard.GetTopLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopLeftNeighbourCoords(myPosition, 1), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item1 + 1 < BoardHeight && currentBoard.GetBottomLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomLeftNeighbourCoords(myPosition, 1), currentBoard.GetLeftNeighbourCoords(myPosition, 1)));
                        }
                    }
                }
            }

            return result;
        }

        private static OptionsOfBeingRemoved GetOptionsOfBeingRemovedVertical(OptionsOfBeingRemoved result, SquareStatus[,] currentBoard, Tuple<int, int> myPosition, SquareStatus myColor, bool getCommands, bool getEscapeCommand)
        {
            if (myPosition.Item1 - 1 >= 0 && myPosition.Item1 + 1 < BoardHeight)
            {
                if (currentBoard.GetTopNeighbour(myPosition, 1) == myColor.GetOponentColor()
                    && currentBoard.GetBottomNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (myPosition.Item1 + 2 < BoardHeight && currentBoard.GetBottomNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomNeighbourCoords(myPosition, 2), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item2 - 1 >= 0 && currentBoard.GetBottomLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomLeftNeighbourCoords(myPosition, 1), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item2 + 1 < BoardWidth && currentBoard.GetBottomRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetBottomRightNeighbourCoords(myPosition, 1), currentBoard.GetBottomNeighbourCoords(myPosition, 1)));
                        }
                    }
                }

                if (currentBoard.GetBottomNeighbour(myPosition, 1) == myColor.GetOponentColor()
                    && currentBoard.GetTopNeighbour(myPosition, 1) == SquareStatus.Empty)
                {
                    if (myPosition.Item1 - 2 >= 0 && currentBoard.GetTopNeighbour(myPosition, 2) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopNeighbourCoords(myPosition, 2), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item2 - 1 >= 0 && currentBoard.GetTopLeftNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopLeftNeighbourCoords(myPosition, 1), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }

                    if (myPosition.Item2 + 1 < BoardHeight && currentBoard.GetTopRightNeighbour(myPosition, 1) == myColor.GetOponentColor())
                    {
                        result.NumberOfOptions++;

                        if (getCommands)
                        {
                            result.CommandsToRemove.Add(new MoveStoneCommand(currentBoard.GetTopRightNeighbourCoords(myPosition, 1), currentBoard.GetTopNeighbourCoords(myPosition, 1)));
                        }
                    }
                }
            }

            return result;
        }
    }
}
