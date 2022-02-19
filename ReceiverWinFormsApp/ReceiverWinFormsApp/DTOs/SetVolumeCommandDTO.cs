using System.Runtime.Serialization;

namespace ReceiverWinFormsApp.DTOs
{
    [DataContract]
    class SetVolumeCommandDTO
    {
        [DataMember(Name = "percent")]
        public int Percent { get; set; }
    }
}