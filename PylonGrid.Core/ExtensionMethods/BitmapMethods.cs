using System.Drawing;
using System.Drawing.Imaging;

namespace ExtensionMethods
{
    public static class BitmapMethods
    {
        public static BitmapData LockReadOnly(this Bitmap bitmap)
        {
            BitmapData result = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);
            return result;
        }

        public static BitmapData LockWriteOnly(this Bitmap bitmap)
        {
            BitmapData result = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                bitmap.PixelFormat);
            return result;
        }

        public static BitmapData LockReadWrite(this Bitmap bitmap)
        {
            BitmapData result = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            return result;
        }

        public static void CopyGray8ToArgb(Bitmap srcImage, Bitmap dstImage)
        {
            BitmapData srcData = srcImage.LockReadWrite();
            BitmapData dstData = dstImage.LockReadWrite();
            BitmapDataMethods.CopyGray8ToArgb(srcData, dstData);
            srcImage.UnlockBits(srcData);
            dstImage.UnlockBits(dstData);
        }


    }
}