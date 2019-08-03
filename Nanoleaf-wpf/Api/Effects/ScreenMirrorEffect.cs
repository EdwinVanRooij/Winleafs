﻿using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api.Effects.ScreenMirrorEffects;

namespace Winleafs.Wpf.Api.Effects
{
    public class ScreenMirrorEffect : ICustomEffect
    {
        public static string Name => $"{CustomEffectsCollection.EffectNamePreface}Screen mirror";

        private readonly INanoleafClient _nanoleafClient;
        private readonly System.Timers.Timer _timer;
        private readonly ScreenMirrorAlgorithm _screenMirrorAlgorithm;
        private readonly IScreenMirrorEffect _screenMirrorEffect;

        public ScreenMirrorEffect(Device device, Orchestrator orchestrator, INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;
            _screenMirrorAlgorithm = device.ScreenMirrorAlgorithm;

            if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirror)
            {
                _screenMirrorEffect = new ScreenMirror(device, orchestrator, nanoleafClient);
            }
            else if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.Ambilight)
            {
                _screenMirrorEffect = new Ambilght(nanoleafClient, device);
            }

            var timerRefreshRate = 1000;

            if (device.ScreenMirrorRefreshRatePerSecond > 0 && device.ScreenMirrorRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / device.ScreenMirrorRefreshRatePerSecond;
            }

            if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.Ambilight && device.ScreenMirrorControlBrightness && timerRefreshRate > 1000 / 5)
            {
                timerRefreshRate = 1000 / 5; //When ambilight is on and controls brightness is enabled, we can update a maximum of 5 times per second since setting brightness is a different action
            }

            _timer = new System.Timers.Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => _screenMirrorEffect.ApplyEffect());
        }

        public async Task Activate()
        {
            if (_screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirror)
            {
                //For screen mirror, we need to enable external control
                await _nanoleafClient.ExternalControlEndpoint.PrepareForExternalControl();
            }

            _timer.Start();
        }

        /// <summary>
        /// Stops the timer and gives it 1 second to complete. Also stop the screengrabber if no other ambilight effects are active
        /// </summary>
        public async Task Deactivate()
        {
            _timer.Stop();
            Thread.Sleep(1000); //Give the last command the time to complete, 1000 is based on testing and a high value (better safe then sorry)
        }

        public bool IsContinuous()
        {
            return true;
        }

        public bool IsActive()
        {
            return _timer.Enabled;
        }
    }
}