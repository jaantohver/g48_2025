using System;
using System.Threading.Tasks;

namespace Practilite
{
    public abstract class Device
    {
        public virtual string Id { get; set; }

        public int Index { get; set; }

        protected bool BrightnessLinked { get; set; }

        protected bool WhiteBalanceLinked { get; set; }

        public bool ShouldBeConnected { get; set; }

        public bool ShouldStandby { get; set; }

        public int CommandFailureCounter { get; set; }

        public int MaxFailuresBeforeReconnect { get; } = 2;

        public int ConnectFailureCounter { get; set; }

        public int MaxFailuresBeforeRestart { get; } = 2;

        public bool InitialConnectDone { get; set; }

        public EventHandler<EventArgs> BrightnessCharacteristicValueChanged, WhiteBalanceCharacteristicValueChanged,
            OverdriveCharacteristicValueChanged, DidConnect, DidDisconnect, IdentifyCharacteristicValueChanged,
            StandbyCharacteristicValueChanged, StandbyShutdown, BatteryStatusCharacteristicValueChanged;

        public EventHandler<EventArgs> ResignSliderControl, UpdateValue;

        protected Device()
        {
            
        }

        public void ResignSlider()
        {
            
        }

        #region read

        public abstract void ReadBrightness();

        public abstract void ReadWhiteBalance();

        public abstract void ReadBoost();

        public abstract void ReadStandby();

        public abstract void ReadBattery();

        #endregion

        #region write

        public abstract void WriteBrightness(short brightness);

        public abstract void WriteWhiteBalance(short whitebalance);

        public abstract void WriteBoost(bool enabled);

        public abstract void WriteIdentify();

        public abstract void WriteStandby(bool enabled);

        public abstract void WriteDisconnect();

        #endregion
    }
}

