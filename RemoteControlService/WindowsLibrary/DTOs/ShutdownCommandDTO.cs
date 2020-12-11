using System.Runtime.Serialization;

namespace WindowsLibrary.DTOs
{
    [DataContract]
    public class ShutdownCommandDTO
    {
        [DataMember(Name = "seconds")]
        public int Seconds { get; set; }
    }
}