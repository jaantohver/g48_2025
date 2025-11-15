#if DEBUG
#define LOGUPDATE
//#undef LOGUPDATE
#endif

using System;
using System.Threading;

using Android.Bluetooth;
using Android.Media;
using door.Droid;

namespace door.Droid
{
    public class AndroidDevice// : Device
    {
        //public CellView ConnectionDelegate { get; set; }

        public BLEDevice BLEDevice { get; private set; }

        public bool Assigned { get; set; }

        public bool InitialCharacteristicReadDone { get; set; }

        DateTime lastConnectionStateChange;

        //public Devices Devices;

        public void Initialize(BLEDevice device)
        {
            BLEDevice = device;
        }

        #region implemented abstract members of Device

        //public override bool SetValueForMode(Mode m, short v)
        //{
        //    if (m == Mode.BRIGHTNESS)
        //    {
        //        if (Brightness.Value != v)
        //        {
        //            Brightness.Value = v;
        //            WriteBrightness(Brightness.Value);
        //            return true;
        //        }
        //    }
        //    else
        //    { /*if (m == Mode.WHITE_BALANCE)*/
        //        if (WhiteBalance.Value != v)
        //        {
        //            WhiteBalance.Value = v;
        //            WriteWhiteBalance(WhiteBalance.Value);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //public override void ReadBrightness()
        //{
        //    if (BLEDevice.EnqueueGattReadCharacteristic(Constants.Characteristics.Brightness))
        //    {
        //        Brightness.WillRead();
        //    }
        //}

        //public override void ReadWhiteBalance()
        //{
        //    if (BLEDevice.EnqueueGattReadCharacteristic(Constants.Characteristics.WhiteBalance))
        //    {
        //        WhiteBalance.WillRead();
        //    }
        //}

        //public override void ReadBoost()
        //{
        //    if (BLEDevice.EnqueueGattReadCharacteristic(Constants.Characteristics.Boost))
        //    {
        //        Boost.WillRead();
        //    }
        //}

        //public override void ReadStandby()
        //{
        //    if (BLEDevice.EnqueueGattReadCharacteristic(Constants.Characteristics.Standby))
        //    {
        //        Standby.WillRead();
        //    }
        //}

        //public override void ReadBattery()
        //{
        //    if (BLEDevice.EnqueueGattReadCharacteristic(Constants.Characteristics.BatteryStatus))
        //    {
        //        Battery.WillRead();
        //    }
        //}

        //public override void WriteBrightness(short brightness)
        //{
        //    if (BLEDevice.EnqueueGattWriteCharacteristic(Constants.Characteristics.Brightness))
        //    {
        //        Brightness.WillWrite();
        //    }
        //}

        //public override void WriteWhiteBalance(short whitebalance)
        //{
        //    if (BLEDevice.EnqueueGattWriteCharacteristic(Constants.Characteristics.WhiteBalance))
        //    {
        //        WhiteBalance.WillWrite();
        //    }
        //}

        //public override void WriteBoost(bool enabled)
        //{
        //    if (BLEDevice.EnqueueGattWriteCharacteristic(Constants.Characteristics.Boost))
        //    {
        //        Boost.WillWrite();
        //    }
        //}

        //public override void WriteIdentify()
        //{
        //    BLEDevice.EnqueueGattWriteCharacteristic(Constants.Characteristics.Identify);
        //}

        //public override void WriteStandby(bool enabled)
        //{
        //    if (BLEDevice.EnqueueGattWriteCharacteristic(Constants.Characteristics.Standby))
        //    {
        //        Standby.WillWrite();
        //    }
        //}

        //public override void WriteDisconnect()
        //{
        //    if (BLEDevice.EnqueueGattWriteCharacteristic(Constants.Characteristics.Disconnect))
        //    {

        //    }
        //}

        //public byte[] GetByteValue(string uuid)
        //{
        //    if (uuid == Constants.Characteristics.Brightness)
        //    {
        //        return BitConverter.GetBytes(Brightness.Value);
        //    }
        //    else if (uuid == Constants.Characteristics.WhiteBalance)
        //    {
        //        return BitConverter.GetBytes(WhiteBalance.Value);
        //    }
        //    else if (uuid == Constants.Characteristics.Boost)
        //    {
        //        return new[] { Convert.ToByte(Boost.Value ? Constants.Commands.BoostEnabled : Constants.Commands.BoostDisabled, 16) };
        //    }
        //    else if (uuid == Constants.Characteristics.Identify)
        //    {
        //        return new[] { Convert.ToByte(Constants.Commands.Identify, 16) };
        //    }
        //    else if (uuid == Constants.Characteristics.Standby)
        //    {
        //        return new[] { Convert.ToByte(Standby.Value ? Constants.Commands.StandbyEnabled : Constants.Commands.StandbyDisabled, 16) };
        //    }
        //    else if (uuid == Constants.Characteristics.Disconnect)
        //    {
        //        return new[] { Convert.ToByte(Constants.Commands.Disconnect, 16) };
        //    }
        //    else
        //    {
        //        return new byte[] { 0x0 };
        //    }

        //}

        //public bool DidWrite(string uuid)
        //{
        //    if (uuid == Constants.Characteristics.Brightness)
        //    {
        //        return Brightness.WroteValue;
        //    }
        //    else if (uuid == Constants.Characteristics.WhiteBalance)
        //    {
        //        return WhiteBalance.WroteValue;
        //    }
        //    else if (uuid == Constants.Characteristics.Boost)
        //    {
        //        return Boost.WroteValue;
        //    }
        //    else if (uuid == Constants.Characteristics.Standby)
        //    {
        //        return Standby.WroteValue;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        #endregion

        #region commands

        //public void Connect()
        //{
        //    ShouldBeConnected = true;
        //    BLEDevice.EnqueueGattOpen();
        //}

        //public void Disconnect(bool write)
        //{
        //    ShouldBeConnected = false;
        //    BLEDevice.ClearAllCommands();
        //    BLEDevice.GetAndReleaseAll();
        //    if (write && BLEDevice.ProfileState == ProfileState.Connected && CommandFailureCounter < MaxFailuresBeforeReconnect)
        //    {
        //        WriteDisconnect();
        //        BLEDevice.EnqueueGattClose();
        //    }
        //    else
        //    {
        //        BLEDevice.EnqueueGattClose();
        //    }
        //}

        //public void Reconnect()
        //{
        //    ThreadPool.QueueUserWorkItem(o => {
        //        var delayStart = DateTime.UtcNow;
        //        Thread.Sleep(10000);
        //        if (lastConnectionStateChange.EarlierOrSameAs(delayStart))
        //            DeviceDisconnected();
        //    });
        //    Disconnect(false);
        //    Connect();
        //}

        public void DiscoverServices()
        {
            BLEDevice.EnqueueGattDiscoverServices();
        }

        public void SetNotifyCharacteristic(string characteristicUuid)
        {
            BLEDevice.EnqueueGattSetNotifyCharacteristic(characteristicUuid);
        }

        #endregion

//        void UpdatedCharacteristicValue(BluetoothGattCharacteristic characteristic, bool updated)
//        {
//            var uuid = characteristic?.Uuid.ToString().ToUpper() ?? "unknown";
//            if (uuid == Constants.Characteristics.Brightness)
//            {
//                if (updated && GetLinkForMode(Mode.BRIGHTNESS))
//                {
//                    // clear link on characteristic notification
//                    SetLinkForMode(Mode.BRIGHTNESS, false);
//                    Devices.Save();
//                }
//                Brightness.DidRead();
//                Brightness.Value = BitConverter.ToInt16(characteristic.GetValue(), 0);
//#if LOGUPDATE
//                Console.WriteLine("[Log device update] {0} | update characteristic | brightness: {1} , {2}", this, Brightness.Value, TimeUtils.TimeString);
//#endif
//                BrightnessCharacteristicValueChanged.Handle(this, new EventArgs());
//            }
//            else if (uuid == Constants.Characteristics.WhiteBalance)
//            {
//                if (updated && GetLinkForMode(Mode.WHITE_BALANCE))
//                {
//                    // clear link on characteristic notification
//                    SetLinkForMode(Mode.WHITE_BALANCE, false);
//                    Devices.Save();
//                }
//                WhiteBalance.DidRead();
//                WhiteBalance.Value = BitConverter.ToInt16(characteristic.GetValue(), 0);
//                /*#if LOGUPDATE
//				Console.WriteLine ("[Log device update] {0} | update characteristic | whitebalance: {1} , {2}", this, WhiteBalance.Value, TimeUtils.TimeString);
//				#endif*/
//                WhiteBalanceCharacteristicValueChanged.Handle(this, new EventArgs());
//            }
//            else if (uuid == Constants.Characteristics.Standby)
//            {
//                Standby.DidRead();
//                if (characteristic.GetValue()[0] == Convert.ToByte(Constants.Commands.StandbyShutdown, 16))
//                {
//                    ShouldBeConnected = false;

//                    StandbyShutdown.Handle(this, new EventArgs());
//                }
//                else
//                {
//                    var temp = characteristic.GetValue()[0];
//                    if (temp == Convert.ToByte(Constants.Commands.StandbyEnabled, 16))
//                    {
//                        if (InitialConnectDone)
//                        {
//                            Standby.Value = true;
//                        }
//                        else
//                        {
//                            Standby.Value = false; // Connected and overriding standby
//                            WriteStandby(false);
//                        }
//                    }
//                    else if (temp == Convert.ToByte(Constants.Commands.StandbyDisabled, 16))
//                    {
//                        Standby.Value = false;
//                    }

//                    StandbyCharacteristicValueChanged.Handle(this, new EventArgs());
//                }
//                /*#if LOGUPDATE
//				Console.WriteLine ("[Log device update] {0} | update characteristic | standby: {1} , {2}", this, Standby.Value, TimeUtils.TimeString);
//				#endif*/
//            }
//            else if (uuid == Constants.Characteristics.Boost)
//            {
//                Boost.DidRead();
//                if (characteristic.GetValue()[0] == Convert.ToByte(Constants.Commands.BoostEnabled, 16))
//                {
//                    Boost.Value = true;
//                }
//                else
//                {
//                    Boost.Value = false;
//                }
//                /*#if LOGUPDATE
//				Console.WriteLine ("[Log device update] {0} | update characteristic | boost: {1} , {2}", this, Boost.Value, TimeUtils.TimeString);
//				#endif*/
//                OverdriveCharacteristicValueChanged.Handle(this, new EventArgs());
//            }
//            else if (uuid == Constants.Characteristics.BatteryStatus)
//            {
//                Battery.DidRead();
//                Battery.Value = BitConverter.ToInt16(characteristic.GetValue(), 0);

//                BatteryStatusCharacteristicValueChanged?.Invoke(this, EventArgs.Empty);
//            }

//            if (!InitialCharacteristicReadDone)
//            {
//                /*#if LOGUPDATE
//				Console.WriteLine ("[Log device update] {0} | count reads | brightness: {1}, whitebalance: {2}, boost: {3}, standby: {4}, {5}", this, Brightness.ValueRead, WhiteBalance.ValueRead, Boost.ValueRead, Standby.ValueRead, TimeUtils.TimeString);
//				#endif*/
//                if (Brightness.ValueRead && WhiteBalance.ValueRead && Boost.ValueRead && Standby.ValueRead)
//                {
//                    //Check if all characteristics have been read
//                    InitialCharacteristicReadDone = true;
//                    /*#if LOGUPDATE
//					Console.WriteLine ("[Log device update] {0} | device finished connecting, {1}", this, TimeUtils.TimeString);
//					#endif*/
//                    if (ConnectionDelegate != null)
//                    {
//                        ConnectionDelegate.DeviceConnected(this);
//                    }
//                }
//            }
//        }

        void WroteCharacteristicValue(BluetoothGattCharacteristic characteristic)
        {
            var uuid = characteristic.Uuid.ToString().ToUpper();
#if LOGUPDATE
            Console.WriteLine("( wroteCharacteristicValue ) < {0} > | {1}",
                Constants.Characteristics.GetCharacteristicName(uuid), TimeUtils.TimeString);
#endif
            if (uuid == Constants.Characteristics.Brightness)
            {
                //Brightness.DidWrite();
            }
            else if (uuid == Constants.Characteristics.WhiteBalance)
            {
                //WhiteBalance.DidWrite();
            }
            else if (uuid == Constants.Characteristics.Standby)
            {
                //InitialConnectDone = true;
                //Standby.DidWrite();
            }
            else if (uuid == Constants.Characteristics.Boost)
            {
                //Boost.DidWrite();
            }
        }

        bool HandleGattCommandFailureCounter()
        {
            //CommandFailureCounter++;
            //if (CommandFailureCounter > MaxFailuresBeforeReconnect)
            //{
            //    Reconnect();
            //    return false;
            //}
            return true;
        }

        public void ReadFailed(string characteristicUuid)
        {
#if LOGUPDATE
            Console.WriteLine("( readFailed ) < {0} > | {1}",
                Constants.Characteristics.GetCharacteristicName(characteristicUuid), TimeUtils.TimeString);
#endif
            if (HandleGattCommandFailureCounter())
            {
                if (characteristicUuid == Constants.Characteristics.Brightness)
                {
                    //ReadBrightness();
                }
                else if (characteristicUuid == Constants.Characteristics.WhiteBalance)
                {
                    //ReadWhiteBalance();
                }
                else if (characteristicUuid == Constants.Characteristics.Standby)
                {
                    //ReadStandby();
                }
                else if (characteristicUuid == Constants.Characteristics.Boost)
                {
                    //ReadBoost();
                }
            }
        }

        public void WriteFailed(string characteristicUuid)
        {
#if LOGUPDATE
            Console.WriteLine("( writeFailed ) < {0} > | {1}",
                Constants.Characteristics.GetCharacteristicName(characteristicUuid), TimeUtils.TimeString);
#endif
            if (HandleGattCommandFailureCounter())
            {
                if (characteristicUuid == Constants.Characteristics.Brightness)
                {
                    //WriteBrightness(Brightness.Value);
                }
                else if (characteristicUuid == Constants.Characteristics.WhiteBalance)
                {
                    //WriteWhiteBalance(WhiteBalance.Value);
                }
                else if (characteristicUuid == Constants.Characteristics.Standby)
                {
                    //WriteStandby(Standby.Value);
                }
                else if (characteristicUuid == Constants.Characteristics.Boost)
                {
                    //WriteBoost(Boost.Value);
                }
                else if (characteristicUuid == Constants.Characteristics.Identify)
                {
                    //WriteIdentify();
                }
            }
        }

        public void DiscoveryFailed()
        {
#if LOGUPDATE
            Console.WriteLine("( discoveryFailed ) < - > | {0}", TimeUtils.TimeString);
#endif
            if (HandleGattCommandFailureCounter())
                DiscoverServices();
        }

        public void SetNotifyFailed(string characteristicUuid)
        {
#if LOGUPDATE
            Console.WriteLine("( setNotifyFailed ) < {0} > | {1}",
                Constants.Characteristics.GetCharacteristicName(characteristicUuid), TimeUtils.TimeString);
#endif
            if (HandleGattCommandFailureCounter())
                SetNotifyCharacteristic(characteristicUuid);
        }

        #region gatt overrides

        public void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            //CommandFailureCounter = 0;
            //UpdatedCharacteristicValue(characteristic, true);
        }

        public void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            //CommandFailureCounter = 0;
            //UpdatedCharacteristicValue(characteristic, false);
        }

        public void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            //CommandFailureCounter = 0;
            WroteCharacteristicValue(characteristic);
        }

        public void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            lastConnectionStateChange = DateTime.UtcNow;
            if (newState == ProfileState.Connected && status == GattStatus.Success)
            {
                BLEDevice.ProfileState = ProfileState.Connected;
                //CommandFailureCounter = 0;
                //ConnectFailureCounter = 0;
                DiscoverServices();
            }
            else if (newState == ProfileState.Disconnected || status != GattStatus.Success)
            {
                BLEDevice.ProfileState = ProfileState.Disconnected;
//                if (ShouldBeConnected)
//                {
//#if LOGUPDATE
//                    Console.WriteLine("( onConnectionStateChange ) < open > | connect failure#{0}: newstate:{1}, status:{2}, {3}",
//                        ConnectFailureCounter, newState, status, TimeUtils.TimeString);
//#endif
//                    ConnectFailureCounter++;
//                    if (ConnectFailureCounter <= MaxFailuresBeforeRestart)
//                    {
//                        Reconnect();
//                    }
//                    else
//                    {
//                        BLEDevice.BleManager.EnqueueRestart();
//                    }
//                }
            }
        }

        public void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            //CommandFailureCounter = 0;

            //SetNotifyCharacteristic(Constants.Characteristics.Brightness);
            //SetNotifyCharacteristic(Constants.Characteristics.WhiteBalance);
            //SetNotifyCharacteristic(Constants.Characteristics.Standby);
            //SetNotifyCharacteristic(Constants.Characteristics.Boost);
            //SetNotifyCharacteristic(Constants.Characteristics.BatteryStatus);

            //if (Brightness.ShouldRead() || WhiteBalance.ShouldRead() || Boost.ShouldRead() || Standby.ShouldRead())
            //{
            //    InitialCharacteristicReadDone = false;
            //}

            //if (Brightness.ShouldRead())
            //{
            //    ReadBrightness();
            //}
            //else
            //{
            //    Brightness.DidRead();
            //    WriteBrightness(Brightness.Value);
            //}
            //if (WhiteBalance.ShouldRead())
            //{
            //    ReadWhiteBalance();
            //}
            //else
            //{
            //    WhiteBalance.DidRead();
            //    WriteWhiteBalance(WhiteBalance.Value);
            //}
            //if (Boost.ShouldRead())
            //{
            //    ReadBoost();
            //}
            //else
            //{
            //    Boost.DidRead();
            //    WriteBoost(Boost.Value);
            //}
            //if (Standby.ShouldRead())
            //{
            //    ReadStandby();
            //}
            //else
            //{
            //    Standby.DidRead();
            //    WriteStandby(Standby.Value);
            //}
            //if (Battery.ShouldRead())
            //{
            //    ReadBattery();
            //}
            //else
            //{
            //    Battery.DidRead();
            //}

            //if (InitialCharacteristicReadDone)
            //{
            //    if (ConnectionDelegate != null)
            //    {
            //        ConnectionDelegate.DeviceConnected(this);
            //    }
            //}
        }

        #endregion

        public void DeviceDisconnected()
        {
            //if (ConnectionDelegate != null)
            //{
            //    ConnectionDelegate.DeviceDisconnected(this);
            //}
        }

        public void ResetValues()
        {
            //BrightnessLinked = false;
            //WhiteBalanceLinked = false;
            //Brightness.Value = 0;
            //Brightness.WillRead();
            //WhiteBalance.Value = 0;
            //WhiteBalance.WillRead();
            //Boost.Value = false;
            //Boost.WillRead();
            //Standby.Value = false;
            //Standby.WillRead();
            //Battery.Value = 0;
            //Battery.WillRead();
        }

        //public override string ToString()
        //{
        //    return string.Format("{0}, connection state: {1}", Id, BLEDevice.ProfileState);
        //}
    }
}