using System.Runtime.Serialization;

namespace RemoteControlService.DTOs
{
    [DataContract]
    class SetVolumeCommandDTO
    {
        [DataMember(Name = "percent")]
        public int Percent { get; set; }
    }
}