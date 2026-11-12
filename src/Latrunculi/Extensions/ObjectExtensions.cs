using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Latrunculi.Extensions
{
    public static class ObjectExtensions
    {
        public static T CopyObject<T>(this object objSource)
        {
            string text = JsonConvert.SerializeObject(objSource);

            return (T)JsonConvert.DeserializeObject(text, typeof(T));
        }
    }
}
