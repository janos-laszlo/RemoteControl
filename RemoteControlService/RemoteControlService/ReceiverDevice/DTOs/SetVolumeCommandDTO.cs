using System.Runtime.Serialization;

namespace RemoteControlService.ReceiverDevice.DTOs
{
    [DataContract]
    class SetVolumeCommandDTO
    {
        [DataMember(Name = "percent")]
        public int Percent { get; set; }
    }
}