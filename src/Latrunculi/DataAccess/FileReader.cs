using Latrunculi.Entities;
using Newtonsoft.Json;
using System.IO;

namespace Latrunculi.DataAccess
{
    public interface IFileReader
    {
        Game GetSavedGame(string savedGameFilePath);
    }

    public class FileReader : IFileReader
    {
        public Game GetSavedGame(string savedGameFilePath)
        {
            Game game;
            using (var file = File.OpenRead(savedGameFilePath))
            {
                using(var streamReader = new StreamReader(file))
                {
                    var text = streamReader.ReadToEnd();
                    
                    game = (Game)JsonConvert.DeserializeObject(text, typeof(Game));
                }
            }

            return game;
        }
    }
}
