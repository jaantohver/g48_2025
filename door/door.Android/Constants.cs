using System;

namespace door.Droid
{
    public static class Constants
    {
        #region app settings

        public const int CellCount = 6;
        public const int MinCellSize = 105;

        public const float MinK = 3000f;
        public const float MaxK = 6000f;

        public const string SaveFile = "plite.data";

        #endregion

        #region bluetooth

        public const short PractiliteIdentifier = 513;

        public static class Commands
        {
            public const string StandbyDisabled = "0x31";
            public const string StandbyEnabled = "0x3E";
            public const string StandbyShutdown = "0xAB";
            public const string BoostEnabled = "0x51";
            public const string BoostDisabled = "0x5E";
            public const string Disconnect = "0x4F";
            public const string Identify = "0x77";
        }

        public static class Characteristics
        {
            public const string DeviceControlService = "000018A0-B87F-490C-92CB-11BA5EA5167C";
            public const string Brightness = "000018A2-B87F-490C-92CB-11BA5EA5167C";
            public const string WhiteBalance = "000018A3-B87F-490C-92CB-11BA5EA5167C";
            public const string Standby = "000018A1-B87F-490C-92CB-11BA5EA5167C";
            public const string Boost = "000018A5-B87F-490C-92CB-11BA5EA5167C";
            public const string Disconnect = "000018A8-B87F-490C-92CB-11BA5EA5167C";
            public const string Identify = "000018A9-B87F-490C-92CB-11BA5EA5167C";
            public const string BatteryStatus = "000018AA-B87F-490C-92CB-11BA5EA5167C";

            public static string GetCharacteristicName(string uuid)
            {
                switch (uuid)
                {
                    case (Characteristics.Brightness):
                        return "Brightness";
                    case (Characteristics.WhiteBalance):
                        return "WhiteBalance";
                    case (Characteristics.Standby):
                        return "Standby";
                    case (Characteristics.Boost):
                        return "Boost";
                    case (Characteristics.Disconnect):
                        return "Disconnect";
                    case (Characteristics.Identify):
                        return "Identify";
                    case (Characteristics.BatteryStatus):
                        return "Battery status";
                }
                return "unknown";
            }
        }

        #endregion
    }
}

