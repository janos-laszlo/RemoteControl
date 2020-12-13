using System.Runtime.Serialization;

namespace WindowsLibrary.DTOs
{
    [DataContract]
    class SetVolumeCommandDTO
    {
        [DataMember(Name = "percent")]
        public int Percent { get; set; }
    }
}