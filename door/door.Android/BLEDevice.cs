#if DEBUG
#define LOGUPDATE
//#undef LOGUPDATE
#endif

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Android.App;
using Android.Bluetooth;
using Android.Telecom;
using door.Droid;

namespace door.Droid
{
    public class BLEDevice : BluetoothGattCallback
    {
        public BLEManager BleManager { get; private set; }

        readonly BluetoothDevice bluetoothDevice;

        BluetoothGatt bluetoothGatt;

        public int Rssi { get; set; }

        public string Address
        {
            get
            {
                return bluetoothDevice.Address;
            }
        }

        public DateTime LastScan { get; set; }

        public AndroidDevice AndroidDevice { get; private set; }

        public BLEDevice(BluetoothDevice bluetoothdevice, AndroidDevice androiddevice, BLEManager manager)
        {
            bluetoothDevice = bluetoothdevice;
            AndroidDevice = androiddevice;
            //AndroidDevice.Id = Address;
            BleManager = manager;
        }

        readonly List<BluetoothGattCharacteristic> characteristicList = new List<BluetoothGattCharacteristic>();

        public BluetoothGattCharacteristic GetCharacteristic(string uuid)
        {
            if (!string.IsNullOrWhiteSpace(uuid))
            {
                lock (characteristicList)
                {
                    return characteristicList.FirstOrDefault(x => x.Uuid.ToString().ToUpper() == uuid.ToUpper());
                }
            }
            else
            {
                return null;
            }
        }

        #region callbacks

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);

            AndroidDevice.OnCharacteristicChanged(gatt, characteristic);
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);

            if (status == GattStatus.Success)
            {
                AndroidDevice.OnCharacteristicRead(gatt, characteristic, status);
            }
            else
            {
                AndroidDevice.ReadFailed(characteristic.Uuid.ToString().ToUpper());
            }

            GetAndRelease(GattMode.READ, characteristic.Uuid.ToString().ToUpper());
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);

            if (status == GattStatus.Success)
            {
                AndroidDevice.OnCharacteristicWrite(gatt, characteristic, status);
            }
            else
            {
                AndroidDevice.WriteFailed(characteristic.Uuid.ToString().ToUpper());
            }

            GetAndRelease(GattMode.WRITE, characteristic.Uuid.ToString().ToUpper());
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);

            ConnectionTimedOut = false;
            BleManager.ConnectionAttemptEnded(this);
            AndroidDevice.OnConnectionStateChange(gatt, status, newState);
            GetAndReleaseAll();
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);

            GetAndRelease(GattMode.NOTIFY, descriptor.Characteristic.Uuid.ToString().ToUpper());
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);

            if (status == GattStatus.Success)
            {
                lock (characteristicList)
                    foreach (var service in gatt.Services)
                        foreach (var characteristic in service.Characteristics)
                            if (!characteristicList.Contains(characteristic))
                                characteristicList.Add(characteristic);

                AndroidDevice.OnServicesDiscovered(gatt, status);
            }
            else
            {
                // Error discovering
                AndroidDevice.DiscoveryFailed();
            }

            GetAndRelease(GattMode.DISCOVER, default);
        }

        #endregion

        readonly Dictionary<GattCommand, ManualResetEvent> resetEventDictionary = new Dictionary<GattCommand, ManualResetEvent>();

        void GetAndRelease(GattMode command, string characteristicUuid)
        {
            lock (resetEventDictionary)
            {
                var kvp = resetEventDictionary.FirstOrDefault(x => x.Key.InnerCommand == command && x.Key.CharacteristicUuid == characteristicUuid);
                if (kvp.Key != null)
                {
                    try
                    { //HACK
                        kvp.Value?.Set();
                    }
                    catch
                    {
                    }
                    resetEventDictionary.Remove(kvp.Key);
                }
            }
        }

        public void GetAndReleaseAll()
        {
            lock (resetEventDictionary)
            {
                foreach (var kvp in resetEventDictionary)
                {
                    if (kvp.Key != null && kvp.Value != null)
                    {
                        try
                        { //HACK
                            kvp.Value?.Set();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                resetEventDictionary.Clear();
            }
        }

        public void DisposeEvent(ManualResetEvent ev)
        {
            lock (resetEventDictionary)
            {
                var kvp = resetEventDictionary.FirstOrDefault(x => x.Value == ev);
                if (kvp.Key != null)
                {
                    resetEventDictionary.Remove(kvp.Key);
                }
            }

            try
            { //HACK
                ev.Dispose();
            }
            catch
            {
                return;
            }
        }

        public bool EnqueueGattOpen()
        {
            //if (AndroidDevice.ShouldBeConnected)
            //    return BleManager.Enqueue(new GattCommand(this, GattMode.OPEN));
            return false;
        }

        public bool EnqueueGattClose()
        {
            return BleManager.Enqueue(new GattCommand(this, GattMode.CLOSE));
        }

        public bool EnqueueGattDiscoverServices()
        {
            return BleManager.Enqueue(new GattCommand(this, GattMode.DISCOVER));
        }

        public bool EnqueueGattSetNotifyCharacteristic(string characteristicUuid)
        {
            return BleManager.Enqueue(new GattCommand(this, GattMode.NOTIFY, characteristicUuid));
        }

        public bool EnqueueGattReadCharacteristic(string characteristicUuid)
        {
            return BleManager.Enqueue(new GattCommand(this, GattMode.READ, characteristicUuid));
        }

        public bool EnqueueGattWriteCharacteristic(string characteristicUuid)
        {
            return BleManager.Enqueue(new GattCommand(this, GattMode.WRITE, characteristicUuid));
        }

        public void ClearAllCommands()
        {
            BleManager.Clearqueue(this);
        }

        public ProfileState ProfileState { get; set; }

        public bool ConnectionTimedOut { get; set; }

        readonly Java.Util.UUID characteristicNotifyDescriptorUUID = Java.Util.UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");

        public ManualResetEvent ExecuteGattCommand(GattCommand command)
        {
            var manualResetEvent = new ManualResetEvent(false);
            var handleReset = false;
            var characteristic = GetCharacteristic(command.CharacteristicUuid);

            Application.SynchronizationContext.Post(async o => {
                switch (command.InnerCommand)
                {
                    case GattMode.OPEN:
                        ProfileState = ProfileState.Connecting;
                        ConnectionTimedOut = false;
                        if (bluetoothGatt != null)
                        {
                            bluetoothGatt?.Close(); //HACK
                            await Task.Delay(TimeSpan.FromMilliseconds(100));
                        }
                        bluetoothGatt = bluetoothDevice.ConnectGatt(Application.Context, false, this); // autoconnect takes a long time
                        break;
                    case GattMode.CLOSE:
                        lock (characteristicList)
                            characteristicList.Clear();
                        bluetoothGatt?.Close();
                        ProfileState = ProfileState.Disconnected;
                        handleReset = true;
                        break;
                    case GattMode.DISCOVER:
                        lock (characteristicList)
                            characteristicList.Clear();
                        bluetoothGatt.DiscoverServices();
                        break;
                    case GattMode.NOTIFY:
                        if (characteristic == null)
                        {
#if LOGUPDATE
                            Console.WriteLine("( executeGattCommand ) < error > | characteristics not discovered, {0}", TimeUtils.TimeString);
#endif

                            EnqueueGattDiscoverServices();
                            EnqueueGattSetNotifyCharacteristic(command.CharacteristicUuid);
                            handleReset = true;
                        }
                        else
                        {
                            bluetoothGatt.SetCharacteristicNotification(characteristic, true);
                            var descriptor = characteristic.GetDescriptor(characteristicNotifyDescriptorUUID);
                            descriptor.SetValue(BluetoothGattDescriptor.EnableIndicationValue.ToArray());
                            bluetoothGatt.WriteDescriptor(descriptor);
                        }
                        break;
                    case GattMode.READ:
                        if (characteristic == null)
                        {
#if LOGUPDATE
                            Console.WriteLine("( executeGattCommand ) < error > | characteristics not discovered, {0}", TimeUtils.TimeString);
#endif

                            EnqueueGattDiscoverServices();
                            EnqueueGattReadCharacteristic(command.CharacteristicUuid);
                            handleReset = true;
                        }
                        else
                        {
                            bluetoothGatt.ReadCharacteristic(characteristic);
                        }
                        break;
                    case GattMode.WRITE:
                        if (characteristic == null)
                        {
#if LOGUPDATE
                            Console.WriteLine("( executeGattCommand ) < error > | characteristics not discovered, {0}", TimeUtils.TimeString);
#endif

                            EnqueueGattDiscoverServices();
                            EnqueueGattWriteCharacteristic(command.CharacteristicUuid);
                            handleReset = true;
                        }
                        else
                        {
                            //characteristic.SetValue(AndroidDevice.GetByteValue(command.CharacteristicUuid));
                            characteristic.WriteType = GattWriteType.Default;
                            bluetoothGatt.WriteCharacteristic(characteristic);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (handleReset)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(30));
                    manualResetEvent.Set();
                }
                else
                {
                    lock (resetEventDictionary)
                        resetEventDictionary.Add(command, manualResetEvent);
                }
            }, handleReset);
            return manualResetEvent;
        }

        public void GattCommandExecuteTimedOut(GattCommand command)
        {
#if LOGUPDATE
            Console.WriteLine("( gattCommandTimeout ) < fixing > | {0}, {1}", command, TimeUtils.TimeString);
#endif

            BleManager.EnqueueCooldown();

//            switch (command.InnerCommand)
//            {
//                case GattMode.OPEN:
//#if LOGUPDATE
//                    Console.WriteLine("( gattCommandTimeout ) < open > | ConnectionTimedOut = true, {0}", TimeUtils.TimeString);
//#endif

//                    ConnectionTimedOut = true;
//                    ThreadPool.QueueUserWorkItem(o => {
//#if LOGUPDATE
//                        Console.WriteLine("( gattCommandTimeout ) < open > | starting timeout timer {0}", TimeUtils.TimeString);
//#endif
//                        var gattCopy = bluetoothGatt;
//                        Thread.Sleep(31000);
//#if LOGUPDATE
//                        Console.WriteLine("( gattCommandTimeout ) < open > | timeout finished#{0}, reconnect: {1}, {2}",
//                            AndroidDevice.ConnectFailureCounter, (gattCopy == bluetoothGatt && ProfileState == ProfileState.Connecting &&
//                        ConnectionTimedOut && AndroidDevice.ShouldBeConnected), TimeUtils.TimeString);
//#endif
//                        if (gattCopy == bluetoothGatt && ProfileState == ProfileState.Connecting && ConnectionTimedOut &&
//                            AndroidDevice.ShouldBeConnected)
//                        {
//                            AndroidDevice.ConnectFailureCounter++;
//                            if (AndroidDevice.ConnectFailureCounter <= AndroidDevice.MaxFailuresBeforeRestart)
//                            {
//                                AndroidDevice.Reconnect();
//                            }
//                            else
//                            {
//                                BleManager.EnqueueRestart();
//                            }
//                        }
//                    });
//                    break;
//                case GattMode.CLOSE:
//#if LOGUPDATE
//                    Console.WriteLine("( gattCommandTimeout ) < close > | queueing new close = {0}, {1}",
//                        AndroidDevice.ShouldBeConnected, TimeUtils.TimeString);
//#endif

//                    if (AndroidDevice.ShouldBeConnected)
//                        EnqueueGattClose();
//                    break;
//                case GattMode.DISCOVER:
//#if LOGUPDATE
//                    Console.WriteLine("( gattCommandTimeout ) < discover > | queueing new discover = {0}, {1}",
//                        AndroidDevice.ShouldBeConnected, TimeUtils.TimeString);
//#endif

//                    if (AndroidDevice.ShouldBeConnected)
//                        AndroidDevice.DiscoveryFailed();
//                    break;
//                case GattMode.NOTIFY:
//#if LOGUPDATE
//                    Console.WriteLine("( gattCommandTimeout ) < notify > | queueing new notify = {0}, {1}",
//                        AndroidDevice.ShouldBeConnected, TimeUtils.TimeString);
//#endif

//                    if (AndroidDevice.ShouldBeConnected)
//                        AndroidDevice.SetNotifyFailed(command.CharacteristicUuid);
//                    break;
//                case GattMode.READ:
//#if LOGUPDATE
//                    Console.WriteLine("( gattCommandTimeout ) < read > | queueing new read = {0}, {1}",
//                        AndroidDevice.ShouldBeConnected, TimeUtils.TimeString);
//#endif

//                    if (AndroidDevice.ShouldBeConnected)
//                        AndroidDevice.ReadFailed(command.CharacteristicUuid);
//                    break;
//                case GattMode.WRITE:
//#if LOGUPDATE
//                    Console.WriteLine("( gattCommandTimeout ) < write > | queueing new write = {0}, {1}",
//                        AndroidDevice.ShouldBeConnected, TimeUtils.TimeString);
//#endif

//                    if (AndroidDevice.ShouldBeConnected)
//                        AndroidDevice.WriteFailed(command.CharacteristicUuid);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
        }

        public override string ToString()
        {
            return string.Format("{0}, state: {1}", Address, ProfileState);
        }
    }
}

