using System;

namespace door
{
	public class LockStatusChangedEventArgs : EventArgs
	{
		public int DeviceId;

		public bool IsOpen;
	}
}
