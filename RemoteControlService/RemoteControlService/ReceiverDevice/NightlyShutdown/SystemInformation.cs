using System;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
{
    public class SystemInformation : ISystemInformation
    {
        public DateTime GetLastSystemShutdown()
        {
            string sKey = @"System\CurrentControlSet\Control\Windows";
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey);

            string sValueName = "ShutdownTime";
            byte[] val = (byte[])key.GetValue(sValueName);
            long valueAsLong = BitConverter.ToInt64(val, 0);
            DateTime exactShutdownDateTime = DateTime.FromFileTime(valueAsLong);
            return new DateTime(exactShutdownDateTime.Year,
                                exactShutdownDateTime.Month,
                                exactShutdownDateTime.Day,
                                exactShutdownDateTime.Hour,
                                exactShutdownDateTime.Minute,
                                exactShutdownDateTime.Second,
                                0);
        }
    }
}
