using System;
using System.Diagnostics;
using System.Linq;
using ExtensionMethods;

namespace PylonGrid.Core
{
    public sealed class NullMedianFilter : IMedianFilter
    {
		public int Radius 
		{ 
			get { return 1; } 
		}
		
        public double Quantile 
		{ 
			get { return 0.5; }
		}
		
        public int Step 
		{ 
			get { return 1; }
		}
		
        public unsafe byte Calculate(int imgX, int imgY, byte* scan0, int stride)
		{
			byte* pixelPtr = scan0 + imgY*stride + imgX;
			return *pixelPtr;
		}
    }
}