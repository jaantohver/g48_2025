#define LOGUPDATE
//#undef LOGUPDATE

using System;
using System.Linq;
using System.Collections.Generic;

using Android.Bluetooth;

namespace door.Droid
{
    public class BLECommandQueue
    {
        readonly List<Command> commandsQueued;
        readonly BLEManager bleManager;
        readonly List<Command> commandsInProgress;

        public BLECommandQueue(BLEManager manager)
        {
            bleManager = manager;
            commandsQueued = new List<Command>();
            commandsInProgress = new List<Command>();
        }

        #region queue content

        bool CommandsInProgressContains(Command command)
        {
            lock (commandsInProgress)
                return commandsInProgress.Any(x => x.Equals(command));
        }

        bool CommandsQueuedContains(Command command)
        {
            lock (commandsQueued)
                return commandsQueued.Any(x => x.Equals(command));
        }

        public bool CommandsAvailable()
        {
            lock (commandsQueued)
                return commandsQueued.Count > 0;
        }

        bool ReconnectQueued(BLEDevice gattDevice)
        {
            return commandsQueued.Any(x => x is GattCommand && (x as GattCommand).InnerCommand == GattMode.OPEN &&
            (x as GattCommand).GattDevice == gattDevice);
        }

        bool ReconnectInProgress(BLEDevice gattDevice)
        {
            return commandsInProgress.Any(x => x is GattCommand && (x as GattCommand).InnerCommand == GattMode.OPEN &&
            (x as GattCommand).GattDevice == gattDevice);
        }

        bool ReconnectNeedsQueueing(BLEDevice gattDevice)
        {
            return !ReconnectQueued(gattDevice) && !ReconnectInProgress(gattDevice);
        }

        bool ScanQueued
        {
            get
            {
                lock (commandsQueued)
                    return commandsQueued.Any(x => x is ManagerCommand && (x as ManagerCommand).InnerCommand == ManagerMode.SCAN);
            }
        }

        bool ScanInProgress
        {
            get
            {
                lock (commandsInProgress)
                    return commandsInProgress.Any(x => x is ManagerCommand && (x as ManagerCommand).InnerCommand == ManagerMode.SCAN);
            }
        }

        bool ScanNeedsQueueing
        {
            get
            {
                return !ScanQueued && !ScanInProgress;
            }
        }

        Command CurrentConnectionAttempt
        {
            get
            {
                lock (commandsInProgress)
                    return commandsInProgress.FirstOrDefault(x => (x as GattCommand)?.InnerCommand == GattMode.OPEN);
            }
        }

        #endregion

        #region queue enqueue/dequeue

        bool CanEnqeueue(Command command)
        {
            if (!CommandsQueuedContains(command))
            {
                if (command is ManagerCommand ||
                    (command is GattCommand && (command as GattCommand).InnerCommand.EqualsAny(GattMode.OPEN, GattMode.CLOSE)))
                    return !CommandsInProgressContains(command);
                return true;
            }
            return false;
        }

        public bool Enqueue(Command command, bool priority = false)
        {
            lock (commandsQueued)
                lock (commandsInProgress)
                {
                    if (CanEnqeueue(command))
                    {
                        if (priority)
                        {
                            commandsQueued.Insert(0, command);
                        }
                        else
                        {
                            commandsQueued.Add(command);
                        }

                        bleManager.Loop();
                        return true;
                    }
                }
            bleManager.Loop();
            return false;
        }

        bool CanDequeue(Command command)
        {
            return command.DependsOn == null ||
            (command.DependsOn != null && !CommandsInProgressContains(command.DependsOn) && !CommandsQueuedContains(command.DependsOn));
        }

        bool CanExecute(Command command)
        {
            if (command is ManagerCommand managerCommand)
            {
                switch (managerCommand.InnerCommand)
                {
                    case ManagerMode.SCAN:
                        if (bleManager.AnyDevicesConnected)
                        {
                            bleManager.DisconnectAssignedDevices();
                            Enqueue(managerCommand);
                            return false;
                        }
                        return true;
                    case ManagerMode.CONNECTED:
                        return false;
                    case ManagerMode.COOLDOWN:
                        return true;
                    case ManagerMode.RESTART:
                        if (bleManager.AnyDevicesConnected)
                        {
                            bleManager.DisconnectAssignedDevices();
                            Enqueue(managerCommand);
                            return false;
                        }
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (command is GattCommand gattCommand)
            {
                switch (gattCommand.InnerCommand)
                {
                    case GattMode.OPEN:
                        if (/*gattCommand.GattDevice.AndroidDevice.ShouldBeConnected*/true)
                        {
                            if (gattCommand.GattDevice.ProfileState == ProfileState.Connected ||
                                gattCommand.GattDevice.ProfileState == ProfileState.Connecting)
                                return false;
                            var connectionAttempt = CurrentConnectionAttempt;
                            if (connectionAttempt != null)
                            {
                                gattCommand.DependsOn = connectionAttempt;
                            }
                            else
                            {
                                if (bleManager.ScanValid)
                                    return true;
                                if (ScanNeedsQueueing)
                                    bleManager.EnqueueScan(false);
                            }
                            Enqueue(gattCommand);
                        }
                        return false;
                    case GattMode.CLOSE:
                        return true;
                    case GattMode.DISCOVER:
                    case GattMode.NOTIFY:
                    case GattMode.READ:
                    case GattMode.WRITE:
                        if (/*gattCommand.GattDevice.AndroidDevice.ShouldBeConnected*/true)
                        {
                            if (gattCommand.GattDevice.ProfileState == ProfileState.Connected)
                                return true;
                            if (ReconnectNeedsQueueing(gattCommand.GattDevice))
                            {
                                gattCommand.GattDevice.EnqueueGattClose();
                                gattCommand.GattDevice.EnqueueGattOpen();
                            }
                            Enqueue(gattCommand);
                        }
                        else if (gattCommand.CharacteristicUuid == Constants.Characteristics.Disconnect)
                        {
                            if (gattCommand.GattDevice.ProfileState == ProfileState.Connected)
                                return true;
                        }
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }

        public Command Dequeue()
        {
            lock (commandsQueued)
                lock (commandsInProgress)
                {
                    for (var i = 0; i < Int16.MaxValue; i++)
                    {
                        var command = commandsQueued.FirstOrDefault(CanDequeue);
                        if (command != null)
                        {
                            commandsQueued.Remove(command);
                            if (CanExecute(command))
                            {
                                commandsInProgress.Add(command);
                                return command;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            return null;
        }

        public void ClearQueue()
        {
            lock (commandsQueued)
                commandsQueued.Clear();
        }

        #endregion

        #region command

        public void ClearQueue(BLEDevice device)
        {
            lock (commandsQueued)
            {
                var queuedCommands = commandsQueued.Where(x => (x as GattCommand)?.GattDevice == device).ToList();
                foreach (var command in queuedCommands)
                    commandsQueued.Remove(command);
            }
            lock (commandsInProgress)
            {
                var inProgressCommands = commandsInProgress.Where(x => (x as GattCommand)?.GattDevice == device).ToList();
                foreach (var command in inProgressCommands)
                    commandsInProgress.Remove(command);
            }
        }

        public void CommandFinished(Command command)
        {
            lock (commandsInProgress)
                commandsInProgress.Remove(command);
        }

        public void ConnectionAttemptEnded(BLEDevice device)
        {
            lock (commandsInProgress)
            {
                var command = commandsInProgress.FirstOrDefault(x => (x is GattCommand) &&
                              (x as GattCommand).InnerCommand == GattMode.OPEN &&
                              (x as GattCommand).GattDevice == device);
                if (command != null && commandsInProgress.Contains(command))
                    CommandFinished(command);
            }
        }

        #endregion
    }
}

