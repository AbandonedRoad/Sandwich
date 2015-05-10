using System;

namespace Blocks
{
	public class ObjectInfo
	{
		public string Key {get; set;}
		public string ReduceSide {get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="Blocks.ObjectInfo"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="reduceSide">Reduce side.</param>
		public ObjectInfo (string key, string reduceSide)
		{
			Key = key;
			ReduceSide = reduceSide;
		}
	}
}