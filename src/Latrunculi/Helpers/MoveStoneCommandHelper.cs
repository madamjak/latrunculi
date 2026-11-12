using Latrunculi.Entities;
using Latrunculi.Logic.Entities;
using System;

namespace Latrunculi.Helpers
{
    public static class MoveStoneCommandHelper
    {
        public static string GetStringCommand(this MoveStoneCommand command)
        {
            return string.Format("{0}{1}-{2}{3}", (char)(command.From.Item2 + 97), (char)(command.From.Item1 + 48), (char)(command.To.Item2 + 97), (char)(command.To.Item1 + 48));
        }

        public static string GetStringPosition(this MyPositionStats command)
        {
            return string.Format("{0}{1}-{2}{3}", (char)(command.MyPosition.Item2 + 97), (char)(command.MyPosition.Item1 + 48), (char)(command.MyPosition.Item2 + 97), (char)(command.MyPosition.Item1 + 48));
        }

        public static MoveStoneCommand ToMoveStoneCommand(this string stringCommand)
        {
            var splits = stringCommand.Split('-');
            var from = new Tuple<int, int>(splits[0][1] - 48, splits[0][0] - 97);
            var to = new Tuple<int, int>(splits[1][1] - 48, splits[1][0] - 97);

            return new MoveStoneCommand(from, to)
            {
                StringCommand = stringCommand
            };
        }
    }
}
