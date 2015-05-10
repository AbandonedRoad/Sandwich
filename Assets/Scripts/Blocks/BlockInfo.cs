using System;

namespace Blocks
{
	public class BlockInfo
	{
		public string Key {get; set;}
		public float WallSize {get; set;}
		public int DoorWall {get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="Blocks.BlockInfo"/> class.
		/// </summary>
		/// <param name="key">The key of the entry.</param>
		/// <param name="wallSize">The size of the wall in meter</param>
		/// <param name="doorWall">This is the wall inwich the door is. -1 Means we have no door</param>
		public BlockInfo (string key, float wallSize, int doorWall)
		{
			Key = key;
			WallSize = wallSize;
			DoorWall = doorWall;
		}
	}
}