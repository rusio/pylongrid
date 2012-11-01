using System;
using System.Diagnostics;
using System.Linq;
using ExtensionMethods;

namespace PylonGrid.Core
{
    /// <summary>
    /// A Median-Filter that uses linear sorting for faster performance.
    /// It is well suited for noise filtering, but also for surface convexisation.
    /// </summary>
    /// 
    /// <remarks>
    /// It is possible to process only every n-th pixel in the 
    /// kernel instead of all pixels, for tradeoff between accuracy and
    /// performance. This comes to play when the kernel radius is big.
    /// 
    /// The filter also supports specifying which quantile value to take
    /// 0.5 being default for median. Values like 0.75 or 0.25 have 
    /// different effects, and can be used for convexisation of a 3D surface.
    /// 
    /// This filter processes 8-bit grayscale images.
    /// </remarks>
    public sealed class FastMedian : IMedianFilter
    {
        public int Radius { get; set; }
        public double Quantile { get; set; }
        public int Step { get; set; }

        private readonly int[] _Histogram;
        private readonly int _QuantileCount;
        private readonly int _Side;

        public FastMedian(int radius) 
            : this(radius, 0.5)
        {
        }

        public FastMedian(int radius, double quantile) 
            : this(radius, quantile, 1)
        {
        }

        public FastMedian(int radius, double quantile, int step)
        {
            Radius = radius;
            Quantile = quantile;
            Step = step;

            _Side = Radius*2 + 1;

            _Histogram = new int[256];
            int kernelArea = _Side*_Side;
            int probedArea = kernelArea/Step/Step;
            _QuantileCount = (int)Math.Round(probedArea*quantile);
        }

        public unsafe byte Calculate(int imgX, int imgY, byte* scan0, int stride)
        {
            Array.Clear(_Histogram, 0, _Histogram.Length); // reset histogram

            // start and end points of kernel (both inclusive)
            int minX = imgX - Radius;
            int maxX = imgX + Radius;
            int minY = imgY - Radius;
            int maxY = imgY + Radius;

            int lineInc = Step*stride - _Side;
            byte* imgPtr = scan0 + minY*stride + minX;  // top-left of the kernel

            // process the kernel pixels
            for (int y = minY; y <= maxY; y += Step, imgPtr += lineInc)
            {
                for (int x = minX; x <= maxX; x += Step, imgPtr += Step)
                {
                    byte value = *imgPtr;
                    _Histogram[value]++;
                }
            }

            // select the median value (using pointers for a little more performance)
            int count = 0;
            fixed (int* pos0 = _Histogram)
            {
                int* posN = pos0 + _Histogram.Length;
                int* posI = pos0;
                while (posI < posN)
                {
                    count += *posI;
                    if (count > _QuantileCount)
                        return (byte)(posI - pos0);
                    posI++;
                }
            }

            Debug.Fail("Median value not found");
            return 0;
        }
    }
}