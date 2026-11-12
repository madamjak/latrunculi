using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Helpers;
using Latrunculi.Logic;
using Latrunculi.Logic.Entities;
using System;
using System.Linq;
using Xunit;

namespace Latrunculi.Tests
{
    public class BasicTests
    {
        private readonly IGameRules _rules;

        public BasicTests()
        {
            _rules = new LatrunculiRules();
        }

        [Fact]
        public void Should_return_expected_highest_rank_move()
        {
            //var firstMove = _rules.GetPossibleMovesSorted(GetCurrentPosition(), GetCurrentPlayer()).First();
            //Assert.Equal(0, rank);
        }

        private PlayerColor GetCurrentPlayer()
        {
            throw new NotImplementedException();
        }

        private BoardPosition GetCurrentPosition()
        {
            throw new NotImplementedException();
        }
    }
}
