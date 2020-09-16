using System.Runtime.Serialization;

namespace RemoteControlService.ReceiverDevice.DTOs
{
    [DataContract(Name = "Command")]
    class CommandDTO
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "command")]
        public string Command { get; set; }
    }
}
