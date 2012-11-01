using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using PylonGrid.Core;

namespace PylonGrid.CUI
{
    public sealed class ConsoleRunner
    {
		public static void Main(string[] args)
        {	
			StereoBitmap frame = new StereoBitmap();
			frame.SceneImage = new Bitmap("../../../frame1-scene.bmp");
			frame.RangeImage = new Bitmap("../../../frame1-range.bmp");
			frame.InitDebugImage();
			
			PylonGridDetector detector = new PylonGridDetector(
				frame.RangeImage.Width, 
				frame.RangeImage.Height,
				5,
				new FastMedian(7, 0.2, 2));

			List<Head> heads = detector.DetectHeads(frame.RangeImage);

            detector.DebugVisualize(frame.DebugImage, heads);
			frame.DebugImage.Save("../../../frame1-result.bmp");
			
            Console.WriteLine("heads (size={0}):", heads.Count);
			foreach (Head head in heads)
			{
				Console.WriteLine("  {0}", head);
			}
		}
		
    }
}
