using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RemoteControlService.ReceiverDevice
{
    static class JSONUtils
    {
        public static T Parse<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var message = (T)serializer.ReadObject(ms);

            return message;
        }
    }
}