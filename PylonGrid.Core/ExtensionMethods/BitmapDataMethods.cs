using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ExtensionMethods
{
    public static class BitmapDataMethods
    {
        public static unsafe void SetAllBytes(this BitmapData data, byte value)
        {
            byte* posI = (byte*) data.Scan0;
            byte* posN = posI + data.Height*data.Stride;
            while (posI < posN)
            {
                *posI++ = value;
            }
        }

        public static unsafe void CopyTo(this BitmapData srcData, BitmapData dstData)
        {
            if (srcData.Width != dstData.Width) throw new ArgumentException("Width");
            if (srcData.Height != dstData.Height) throw new ArgumentException("Height");
            if (srcData.Stride != dstData.Stride) throw new ArgumentException("Stride");

            byte* srcPos = (byte*)srcData.Scan0;
            byte* dstPos = (byte*)dstData.Scan0;
            int opsCount = srcData.Height*srcData.Stride;

            for (int i = 0; i < opsCount; i++)
            {
                *dstPos = *srcPos;
                srcPos++;
                dstPos++;
            }
        }

        public static unsafe void CopyGray8ToArgb(BitmapData srcData, BitmapData dstData)
        {
            if (srcData.Width != dstData.Width) throw new ArgumentException("Width");
            if (srcData.Height != dstData.Height) throw new ArgumentException("Height");
            if (srcData.Stride != dstData.Stride / 4) throw new ArgumentException("Stride");

            byte* srcPos = (byte*)srcData.Scan0;
            byte* dstPos = (byte*)dstData.Scan0;
            int opsCount = srcData.Height*srcData.Stride;
            
            for (int i = 0; i < opsCount; i++)
            {
                byte srcVal = *srcPos;
                // ARGB is actually BGRA in byte order (ARGB for uint?)
                *dstPos++ = srcVal;  // B
                *dstPos++ = srcVal;  // G
                *dstPos++ = srcVal;  // R
                *dstPos++ = 255;     // A (skip)
                srcPos++;
            }            
        }
    }
}