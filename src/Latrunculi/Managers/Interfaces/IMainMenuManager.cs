using Latrunculi.Entities;

namespace Latrunculi.Managers.Interfaces
{
    public interface IMainMenuManager
    {
        IGameManager StartNewGame();

        IGameManager StartNewGame(Player black, Player white);

        IGameManager LoadGame(string path);

        void SaveGame(string path);

        bool CanPlay();

        IGameManager Play();
    }
}
