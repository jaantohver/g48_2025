namespace door
{
	public class Lock
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public bool IsLocked { get; set; }

		public long LastSeenTimestamp { get; set; }
	}
}
