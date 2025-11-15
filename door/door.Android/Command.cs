using System.Threading;

namespace door.Droid
{
    public class Command
    {
        public int Timeout { get; protected set; }

        public Command DependsOn { get; set; }

        public virtual ManualResetEvent Execute()
        {
            return new ManualResetEvent(false);
        }

        public virtual void DisposeEvent(ManualResetEvent ev)
        {
        }

        public virtual void ExecuteFailed()
        {
        }

        public virtual bool Equals(Command other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return string.Format("[Timeout={0}, DependsOn={1}]", Timeout, DependsOn);
        }
    }

    public enum ManagerMode
    {
        SCAN,
        CONNECTED,
        COOLDOWN,
        RESTART
    }

    public class ManagerCommand : Command
    {
        public ManagerMode InnerCommand { get; private set; }

        public BLEManager BleManager { get; private set; }

        public ManagerCommand(BLEManager bleManager, ManagerMode innerCommand)
        {
            Timeout = -1;
            BleManager = bleManager;
            InnerCommand = innerCommand;
        }

        public override ManualResetEvent Execute()
        {
            return BleManager.ExecuteManagerCommand(this);
        }

        public override bool Equals(Command other)
        {
            return (other as ManagerCommand)?.InnerCommand == InnerCommand;
        }

        public override string ToString()
        {
            return string.Format("[InnerCommand={1}, BleManager={2}]{0}", base.ToString(), InnerCommand, BleManager);
        }
    }

    public enum GattMode
    {
        OPEN,
        CLOSE,
        DISCOVER,
        NOTIFY,
        READ,
        WRITE
    }

    public class GattCommand : Command
    {
        public BLEDevice GattDevice { get; private set; }

        public GattMode InnerCommand { get; private set; }

        public string CharacteristicUuid { get; private set; }

        public GattCommand(BLEDevice bledevice, GattMode command, string characteristicUuid = default)
        {
            Timeout = (command == GattMode.OPEN || command == GattMode.CLOSE) ? 500 : 3500;
            //Timeout = (command == GattMode.OPEN || command == GattMode.CLOSE) ? -1 : 3333;
            //Timeout = 3333;
            GattDevice = bledevice;
            InnerCommand = command;
            CharacteristicUuid = characteristicUuid;
        }

        public override ManualResetEvent Execute()
        {
            return GattDevice.ExecuteGattCommand(this);
        }

        public override void DisposeEvent(ManualResetEvent ev)
        {
            GattDevice.DisposeEvent(ev);
        }

        public override void ExecuteFailed()
        {
            GattDevice.GattCommandExecuteTimedOut(this);
        }

        public override bool Equals(Command other)
        {
            if (other is GattCommand otherGattCommand)
            {
                return otherGattCommand.GattDevice == GattDevice &&
                otherGattCommand.InnerCommand == InnerCommand &&
                otherGattCommand.CharacteristicUuid == CharacteristicUuid;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("[InnerCommand={2}, Characteristic={3}, GattDevice={1}]{0}",
                base.ToString(), GattDevice, InnerCommand, Constants.Characteristics.GetCharacteristicName(CharacteristicUuid));
        }
    }
}