using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Android.OS;
using Android.App;
using Android.Bluetooth;
using Android.Content.PM;
using Android.Bluetooth.LE;

namespace door.Droid
{
    public class BLEManager
    {
        public static event EventHandler<LockStatusChangedEventArgs> LockStatusChanged;

        static BLEManager current;

        public static BLEManager GetManager(MainActivity activity)
        {
            return (current != null && current.mainActivity == activity) ? current : Create(activity);
        }

        static BLEManager Create(MainActivity activity)
        {
            if (current == null)
                current = new BLEManager(activity);
            current.mainActivity = activity;
            return current;
        }

        readonly BluetoothManager manager;
        readonly BluetoothAdapter adapter;
        readonly BLEDevices devices;
        readonly BLECommandQueue bleCommandQueue;
        readonly BLESearchCallback bleSearchCallbacks;
        private MainActivity mainActivity;

        public BLEManager(MainActivity activity)
        {
            mainActivity = activity;
            manager = Application.Context.GetSystemService("bluetooth") as BluetoothManager;
            adapter = manager.Adapter;
            devices = new BLEDevices(this);
            devices.LockStatusChanged += (sender, e) =>
            {
                LockStatusChanged?.Invoke(this, e);
            };
            bleCommandQueue = new BLECommandQueue(this);
            bleSearchCallbacks = new BLESearchCallback();
        }

        public bool Available
        {
            get
            {
                return Application.Context.PackageManager.HasSystemFeature(PackageManager.FeatureBluetoothLe);
            }
        }

        public bool Enabled
        {
            get
            {
                return manager.Adapter != null && manager.Adapter.IsEnabled;
            }
        }

        public void CreateDevice(AndroidDevice androidDevice)
        {
            //devices.Create(androidDevice, adapter.GetRemoteDevice(androidDevice.Id));
        }

        public void FindNewDevice()
        {
            EnqueueScan(true);
        }

        public void CancelFindNewDevice()
        {
        }

        public bool Enqueue(Command command)
        {
            return bleCommandQueue.Enqueue(command);
        }

        public void EnqueueScan(bool priority)
        {
            bleCommandQueue.Enqueue(new ManagerCommand(this, ManagerMode.SCAN));
        }

        public void EnqueueCooldown()
        {
            bleCommandQueue.Enqueue(new ManagerCommand(this, ManagerMode.COOLDOWN), true);
        }

        public void Clearqueue(BLEDevice device)
        {
            bleCommandQueue.ClearQueue(device);
        }

        const int timeBetweenScansMs = 180000;

        public bool ScanValid => lastScanTime != default &&
                lastScanTime.Subtract(TimeSpan.FromMilliseconds(timeBetweenScansMs)).EarlierOrSameAs(DateTime.UtcNow);

        bool AssignedDevicesDisconnected()
        {
            return devices.AssignedDevices.All(x => x.BLEDevice.ProfileState == ProfileState.Disconnected);
        }

        volatile bool inLoop;

        public void Loop()
        {
            if (!inLoop)
            {
                inLoop = true;
                ThreadPool.QueueUserWorkItem(o => {
                    while (bleCommandQueue.CommandsAvailable())
                    {
                        if (Available && Enabled && /*mainActivity.LocationEnabled*/ true)
                        {
                            var command = bleCommandQueue.Dequeue();

                            if (command != null)
                            {
                                var waitEvent = command.Execute();
                                if (!waitEvent.WaitOne(command.Timeout))
                                    command.ExecuteFailed();
                                if ((command as GattCommand)?.InnerCommand != GattMode.OPEN)
                                    bleCommandQueue.CommandFinished(command); // open finishes in connectionattemptended
                                if (waitEvent != manualResetEvent)
                                    command.DisposeEvent(waitEvent); // don't dispose manager manualresetevent
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (!Enabled)
                            {
                                //mainActivity.EnableBluetooth().WaitOne();
                                //if (mainActivity.CancelBluetooth)
                                //{
                                //    bleCommandQueue.ClearQueue();
                                //    break;
                                //}
                            }
                            //if (!mainActivity.LocationEnabled)
                            //{
                                //mainActivity.EnableLocation().WaitOne();
                                //if (mainActivity.CancelBluetooth)
                                //{
                                //    bleCommandQueue.ClearQueue();
                                //    break;
                                //}
                            //}
                        }
                    }
                    inLoop = false;
                });
            }
        }

        public void ConnectionAttemptEnded(BLEDevice device)
        {
            bleCommandQueue.ConnectionAttemptEnded(device);
            Loop();
        }

        readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public ManualResetEvent ExecuteManagerCommand(ManagerCommand command)
        {
            manualResetEvent.Reset();
            switch (command.InnerCommand)
            {
                case ManagerMode.SCAN:
                    ExecuteScan(command);
                    break;
                case ManagerMode.CONNECTED:
                    break;
                case ManagerMode.COOLDOWN:
                    ExecuteCooldown(command);
                    break;
                case ManagerMode.RESTART:
                    ExecuteRestart(command);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return manualResetEvent;
        }

        DateTime lastScanTime;

        const int scanTimeMs = 10000;

        void ExecuteScan(ManagerCommand command)
        {
            ThreadPool.QueueUserWorkItem(o => { // scanning thread
                if (StartScan())
                {
                    lastScanTime = DateTime.UtcNow;

                    ThreadPool.QueueUserWorkItem(o1 => { // polling thread
                        while (!AllDevicesFoundOrScanTimePassed(lastScanTime))
                        {
                            Thread.Sleep(2000);
                        }
                        //StopScan();
                        Connect();
                        manualResetEvent.Set();
                    });
                }
                else
                {
                    // ble inoperational
                    EnqueueRestart();
                    manualResetEvent.Set();
                }
            });
        }

        bool StartScan()
        {
            ScanFilter filter = new ScanFilter.Builder().SetServiceUuid(ParcelUuid.FromString("6ba1b218-15a8-461f-9fa8-5dcae273eafd")).Build();
            //ScanFilter filter = new ScanFilter.Builder().SetServiceUuid(ParcelUuid.FromString("1234")).Build();
            new ScanFilter.Builder().SetManufacturerData(0, new byte[] { });

            List<ScanFilter> filters = new List<ScanFilter> { filter };
            ScanSettings settings = new ScanSettings.Builder().Build();
            ScanCallback callback = devices.BleSearchCallbackAndroid5;

            //adapter.BluetoothLeScanner.StartScan(filters, settings, callback);
            adapter.BluetoothLeScanner.StartScan(devices.BleSearchCallbackAndroid5);

            return true;
        }

        void StopScan()
        {
            if (adapter != null && adapter.BluetoothLeScanner != null && devices != null && devices.BleSearchCallbackAndroid5 != null) {
                adapter.BluetoothLeScanner.StopScan(devices.BleSearchCallbackAndroid5);
            }
        }

        bool AllDevicesFoundOrScanTimePassed(DateTime dateTime)
        {
            return AllDevicesFound(dateTime) ||
            dateTime.Add(TimeSpan.FromMilliseconds(scanTimeMs)).EarlierOrSameAs(DateTime.UtcNow);
        }

        bool AllDevicesFound(DateTime dateTime)
        {
            return UnassignedDevicesFound(dateTime) && AssignedDevicesFound(dateTime);
        }

        bool UnassignedDevicesFound(DateTime dateTime)
        {
            //return devices.GetUnassignedDeviceFindCount(dateTime) >= bleSearchCallbacks.CallBackCount;
            return true;
        }

        bool AssignedDevicesFound(DateTime dateTime)
        {
            return devices.GetAssignedDeviceFindCount(dateTime) >= devices.AssignedDeviceCount;
        }

        void ExecuteCooldown(ManagerCommand command)
        {
            ThreadPool.QueueUserWorkItem(o => {
                Thread.Sleep(1000);
                manualResetEvent.Set();
            });
        }

        void Connect()
        {
            //foreach (var device in devices.GetFoundAssignedDevices(lastScanTime))
                //device.Connect();

            var callBacks = bleSearchCallbacks.GetCallbacks();
            lock (callBacks)
                foreach (var callback in callBacks)
                    //callback.SetDevice(devices.GetUnassignedDevice(lastScanTime));

            bleSearchCallbacks.ClearCallbacks();
        }

        public void DisconnectAssignedDevices()
        {
            //foreach (var device in devices.GetFoundAssignedDevices(lastScanTime))
                //device.Disconnect(false);
        }

        public bool AnyDevicesConnected
        {
            get
            {
                return devices.AssignedDevices.Any(x => x.BLEDevice.ProfileState != ProfileState.Disconnected);
            }
        }

        public bool NoDevicesConnecting
        {
            get
            {
                return devices.AssignedDevices.All(x => x.BLEDevice.ProfileState != ProfileState.Connecting);
            }
        }

        public bool DeviceFoundLastScan(BLEDevice device)
        {
            return device.LastScan >= lastScanTime;
        }

        public bool NoDevicesFound
        {
            get
            {
                return devices.AssignedDevices.All(x => !DeviceFoundLastScan(x.BLEDevice));
            }
        }

        public void EnqueueRestart()
        {
            bleCommandQueue.Enqueue(new ManagerCommand(this, ManagerMode.RESTART));
        }

        void ExecuteRestart(ManagerCommand command)
        {
            //mainActivity.RestartBluetooth(this, adapter);
        }

        public void RestartBluetoothCallback(bool restarted)
        {
            Connect();
            manualResetEvent.Set();
        }
    }
}

