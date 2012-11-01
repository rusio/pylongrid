using System;

namespace PylonGrid.Core
{
	/// <summary>
	/// A group of Pylons, all on the same plane, at the top of the head.
	/// </summary>
    internal struct PylonGroup
    {
        internal int SumPylonX;
        internal int SumPylonY;
        internal int PylonCount;
        internal byte GroupRange;

        internal void Register(Pylon pylon)
        {
			pylon.IsGrouped = true;
            PylonCount++;
            GroupRange = pylon.Range;
            SumPylonX += pylon.ImgX;
            SumPylonY += pylon.ImgY;
        }

		internal Head ToHead()
        {
			int headCenterX = SumPylonX / PylonCount;
			int headCenterY = SumPylonY / PylonCount;
            return new Head(GroupRange, headCenterX, headCenterY);
        }
    }
}

