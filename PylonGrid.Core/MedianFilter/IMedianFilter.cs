using System;
using System.Diagnostics;
using System.Linq;
using ExtensionMethods;

namespace PylonGrid.Core
{
    public interface IMedianFilter
    {
		int Radius { get; }
		double Quantile { get; }
		int Step { get; }
		
        unsafe byte Calculate(int imgX, int imgY, byte* scan0, int stride);
    }
}