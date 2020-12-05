using System.Runtime.Serialization;

namespace RemoteControlService.DTOs
{
    [DataContract]
    class ShutdownCommandDTO
    {
        [DataMember(Name = "seconds")]
        public int Seconds { get; set; }
    }
}