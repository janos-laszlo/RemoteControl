using System.Runtime.Serialization;

namespace ReceiverWinFormsApp.DTOs
{
    [DataContract]
    class ShutdownCommandDTO
    {
        [DataMember(Name = "seconds")]
        public int Seconds { get; set; }
    }
}