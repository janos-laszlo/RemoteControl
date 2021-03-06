﻿using Domain.Controllers;
using NAudio.CoreAudioApi;
using System;

namespace WindowsLibrary.Controllers
{
    public class CmdLineVolumeController : IVolumeController
    {
        private readonly MMDevice defaultDevice;

        public CmdLineVolumeController()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        public int VolumeInPercent
        {
            get => Convert.ToInt32(defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100f);
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(VolumeInPercent),
                        "Expecting a value in the interval [0, 100]");
                }

                defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = value / 100f;
            }
        }
    }
}
