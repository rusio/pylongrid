using System;

namespace PylonGrid.Core
{
	/// <summary>
	/// A Pylon is placed above a pixel in the range image and knows the pixel's range.
	/// </summary>
    internal class Pylon
    {
        internal readonly int ImgX;
        internal readonly int ImgY;
        internal readonly bool IsBorder;

        internal byte Range;
        internal bool IsHead;
        internal bool IsGrouped;

        internal Pylon(int imgX, int imgY, bool isBorder)
        {
            ImgX = imgX;
            ImgY = imgY;
			IsBorder = isBorder;
			ResetAt(255);
        }
		
		internal void ResetAt(byte range)
		{
		    Range = range;
            IsHead = !IsBorder;
            IsGrouped = false;
		}
		
        internal bool IsDiscardedBy(Pylon adjacent)
        {
            if (adjacent.IsBorder)
                return false;			
            if (adjacent.Range < this.Range)
				return true;
            if (adjacent.Range == this.Range && !adjacent.IsHead)
				return true;
			return false;
        }

        internal bool IsInsideImage(int radius, int imgSizeX, int imgSizeY)
        {
            return 
                (ImgX - radius >= 0) &&
                (ImgX + radius < imgSizeX) &&
                (ImgY - radius >= 0) &&
                (ImgY + radius < imgSizeY);
        }
    }

}

