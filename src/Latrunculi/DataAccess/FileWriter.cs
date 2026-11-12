using Latrunculi.Entities;
using Newtonsoft.Json;
using System.IO;

namespace Latrunculi.DataAccess
{
    public interface IFileWriter
    {
        void SaveGame(Game game, string path);
    }

    public class FileWriter : IFileWriter
    {
        public void SaveGame(Game game, string path)
        {
            string text = JsonConvert.SerializeObject(game);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (var file = File.Create(path))
            {
                using(var writer = new StreamWriter(file))
                {
                    writer.Write(text);
                }
            }
        }
    }
}
