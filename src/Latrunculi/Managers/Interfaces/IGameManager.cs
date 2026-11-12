using Latrunculi.Entities;

namespace Latrunculi.Managers.Interfaces
{
    public interface IGameManager
    {
        void SaveGame(string path);

        IGameplayManager PlayGame();

        void SetBlackPlayer(Player newPlayer);

        void SetWhitePlayer(Player newPlayer);

        bool IsBlackPlayerSet();

        bool IsWhitePlayerSet();

        Player GetBlackPlayer();

        Player GetWhitePlayer();
    }
}
