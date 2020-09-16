using System.Runtime.Serialization;

namespace RemoteControlService.ReceiverDevice.DTOs
{
    [DataContract]
    class ShutdownCommandDTO
    {
        [DataMember(Name = "seconds")]
        public int Seconds { get; set; }
    }
}