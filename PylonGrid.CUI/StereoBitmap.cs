using System;
using System.Drawing;
using System.Drawing.Imaging;

using ExtensionMethods;

namespace PylonGrid.Core
{
    public sealed class StereoBitmap
    {
        /// <summary>
        /// The basic image of the scene, suitable for human viewing.
        /// It is rectified and is based on the reference camera.
        /// </summary>
        public Bitmap SceneImage;

        /// <summary>
        /// The range-image that corresponds to the scene-image.
        /// This image contains the range values calculated
        /// after the stereo processing.
        /// </summary>
        public Bitmap RangeImage;

        /// <summary>
        /// An image for drawing and viewing debugging info.
        /// </summary>
        public Bitmap DebugImage;

        public int ID = -1;

        public void Save(string prefix)
        {
            SceneImage.Save(prefix + "-scene.bmp", ImageFormat.Bmp);
            RangeImage.Save(prefix + "-range.bmp", ImageFormat.Bmp);
            DebugImage.Save(prefix + "-debug.bmp", ImageFormat.Bmp);
        }

        public void Save()
        {
            Save(ID.ToString("D9"));  // 000000123
        }

        public void Dispose()
        {
            SceneImage.Dispose();
            RangeImage.Dispose();
        }

        public void InitDebugImage()
        {
            Bitmap srcImage = RangeImage ?? SceneImage;
            DebugImage = new Bitmap(srcImage.Width, srcImage.Height);
            BitmapMethods.CopyGray8ToArgb(srcImage, DebugImage);
        }

    }
}