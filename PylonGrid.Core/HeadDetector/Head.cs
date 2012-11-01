using System;

namespace PylonGrid.Core
{
    /// <summary>
    /// Represents a human head somewhere in the image.
    /// </summary>
    public struct Head
    {
        /// <summary>
        /// The range of the head from the camera plane (z-range).
        /// </summary>
        public readonly byte Range;

        /// <summary>
        /// The x-coordinate of the head's center in the image.
        /// </summary>
        public readonly int CenterX;

        /// <summary>
        /// The y-coordinate of the head's center in the image.
        /// </summary>
        public readonly int CenterY;
		
		public Head(byte range, int centerX, int centerY)
		{
			Range = range;
			CenterX = centerX;
			CenterY = centerY;
		}

        public override string ToString()
        {
            return string.Format("Head(Range={0},Center=({1},{2}))", Range, CenterX, CenterY);
        }
    }
}

