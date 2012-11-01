using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PylonGrid.Core
{
    /// <summary>
    /// An IHeadDetector detects the human heads in an image.
    /// </summary>
    public interface IHeadDetector
    {
        /// <summary>
        /// Detects the positions of all the heads in the given image.
        /// </summary>
        /// <param name="rangeImage">The image to process.</param>
        /// <returns>A list of the detected heads in the image.</returns>
        List<Head> DetectHeads(Bitmap rangeImage);
    }
}