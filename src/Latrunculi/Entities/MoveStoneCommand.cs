using System;

namespace Latrunculi.Entities
{
    [Serializable]
    public class MoveStoneCommand
    {
        public MoveStoneCommand()
        {
        }

        public MoveStoneCommand(Tuple<int, int> from, Tuple<int, int> to)
        {
            From = from;
            To = to;
        }

        public Tuple<int, int> From { get; set; }

        public Tuple<int, int> To { get; set; }

        public string StringCommand { get; set; }

        public override bool Equals(object obj)
        {
            var o = obj as MoveStoneCommand;
            return o != null && o.From.Equals(From) && o.To.Equals(To);
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() ^ To.GetHashCode();
        }
    }
}
