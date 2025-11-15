#if DEBUG
#define LOGUPDATE
//#undef LOGUPDATE
#endif

using System;
using System.Linq;
using System.Collections.Generic;

using Android.OS;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using System.Text;

namespace door.Droid
{
    public class BLESearchCallbackAndroid5 : ScanCallback
    {
        public event EventHandler<LockStatusChangedEventArgs> LockStatusChanged;

        readonly List<AndroidDevice> bluetoothDevices;
        readonly BLEManager bleManager;
        readonly string identifier;

        public BLESearchCallbackAndroid5(List<AndroidDevice> devices, BLEManager manager, string id)
        {
            bluetoothDevices = devices;
            bleManager = manager;
            identifier = id;
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            if (result.ScanRecord != null && result.ScanRecord.GetBytes().Length > 0)
            {
                if (result.ScanRecord.DeviceName!= null && result.ScanRecord.DeviceName.ToLower().Contains("loockit"))
                {
                    //16th byte, 0 closed, 1 open
                    Console.WriteLine(result.ScanRecord.GetBytes()[16]);

                    LockStatusChangedEventArgs args = new LockStatusChangedEventArgs();
                    args.IsOpen = result.ScanRecord.GetBytes()[16] == 0;
                    args.DeviceId = 123;

                    LockStatusChanged?.Invoke(this, args);
                }
            }

            if (result.ScanRecord != null && result.ScanRecord.GetBytes().Length > 0 &&
                System.Text.Encoding.UTF8.GetString(result.ScanRecord.GetBytes()).ToUpper().Contains(identifier))
            {
                lock (bluetoothDevices)
                {
                    AndroidDevice androidDevice = bluetoothDevices.FirstOrDefault(x => x.BLEDevice.Address == result.Device.Address);
                    if (androidDevice == null)
                    {
                        androidDevice = new AndroidDevice();
                        androidDevice.Initialize(new BLEDevice(result.Device, androidDevice, bleManager));
                        bluetoothDevices.Add(androidDevice);
                    }
#if LOGUPDATE
                    Console.WriteLine("( onLeScan ) < device found > | {0}, {1}", androidDevice, TimeUtils.TimeString);
#endif
                    androidDevice.BLEDevice.Rssi = result.Rssi;
                    androidDevice.BLEDevice.LastScan = DateTime.UtcNow;
                }
            }
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {

        }
    }

    public class BLEDevices : Java.Lang.Object
    {
        public event EventHandler<LockStatusChangedEventArgs> LockStatusChanged;

        readonly List<AndroidDevice> bluetoothDevices;
        readonly BLEManager bleManager;

        public BLESearchCallbackAndroid5 BleSearchCallbackAndroid5 { get; }

        public BLEDevices(BLEManager manager)
        {
            bluetoothDevices = new List<AndroidDevice>();
            bleManager = manager;

            BleSearchCallbackAndroid5 = new BLESearchCallbackAndroid5(bluetoothDevices, bleManager, "PLITE");
            BleSearchCallbackAndroid5.LockStatusChanged += (sender, e) =>
            {
                LockStatusChanged?.Invoke(this, e);
            };
        }

        public void Create(AndroidDevice androidDevice, BluetoothDevice device)
        {
            lock (bluetoothDevices)
            {
                androidDevice.Initialize(new BLEDevice(device, androidDevice, bleManager));
                bluetoothDevices.Add(androidDevice);
            }
        }

        public AndroidDevice GetUnassignedDevice(DateTime dateTime)
        {
            lock (bluetoothDevices)
                return bluetoothDevices.Where(x => dateTime.EarlierOrSameAs(x.BLEDevice.LastScan))
                    .OrderByDescending(x => x.BLEDevice.LastScan).ThenByDescending(x => x.BLEDevice.Rssi)
                    .FirstOrDefault(x => !x.Assigned);
        }

        public int GetUnassignedDeviceFindCount(DateTime dateTime)
        {
            lock (bluetoothDevices)
                return bluetoothDevices.Count(x => !x.Assigned && dateTime.EarlierOrSameAs(x.BLEDevice.LastScan));
        }

        public int GetAssignedDeviceFindCount(DateTime dateTime)
        {
            lock (bluetoothDevices)
                return bluetoothDevices.Count(x => x.Assigned && dateTime.EarlierOrSameAs(x.BLEDevice.LastScan));
        }

        public int AssignedDeviceCount
        {
            get
            {
                lock (bluetoothDevices)
                    return bluetoothDevices.Count(x => x.Assigned);
            }
        }

        public List<AndroidDevice> GetFoundAssignedDevices(DateTime dateTime)
        {
            lock (bluetoothDevices)
                return AssignedDevices.Where(x => dateTime.EarlierOrSameAs(x.BLEDevice.LastScan)).ToList();
        }

        public List<AndroidDevice> AssignedDevices
        {
            get
            {
                lock (bluetoothDevices)
                    return bluetoothDevices.Where(x => x.Assigned).ToList();
            }
        }

        public int GetFoundCount(DateTime time)
        {
            lock (bluetoothDevices)
                return bluetoothDevices.Count(x => x.BLEDevice.LastScan >= time);
        }
    }
}

