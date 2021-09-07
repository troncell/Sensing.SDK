using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Chromagic
{
    public class ChromaKey
    {
        public float Hue { get; set; } = 120.0f;
        public float Tolerance { get; set; } = 45.0f;
        public float Saturation { get; set; } = 0.2f;
        public float MinValue { get; set; } = 0.35f;
        public float MaxValue { get; set; } = 0.95f;

        public int leftSpill { get; set; } = 1;
        public int rightSpill { get; set; } = 1;

        /// <summary>
        /// Margin will be filtered in chromaKey process.
        /// </summary>
        public int Margin_Left { get; set; } = 0;
        public int Margin_Top { get; set; } = 0;
        public int Margin_Right { get; set; } = 0;
        public int Margin_Bottom { get; set; } = 0;

        public ChromaKey()
        {
            //Hue = 120.0f;
            //Tolerance = 45.0f;
            //Saturation = 0.2f;
            //MinValue = 0.35f;
            //MaxValue = 0.95f;

            //leftSpill = 1;
            //rightSpill = 1;
        }

        private unsafe void RGB_to_HSV(float[] color, float[] hsv)
        {
            float r, g, b, delta;
            float colorMax, colorMin;
            float h = 0.0f, s = 0.0f, v = 0.0f;
            r = color[0];
            g = color[1];
            b = color[2];
            colorMax = System.Math.Max(r, g);
            colorMax = System.Math.Max(colorMax, b);
            colorMin = System.Math.Min(r, g);
            colorMin = System.Math.Min(colorMin, b);
            v = colorMax;

            if (colorMax != 0.0f)
            {
                s = (colorMax - colorMin) / colorMax;
            }
            if (s != 0.0f) // if not achromatic
            {
                delta = colorMax - colorMin;
                if (r == colorMax)
                {
                    h = (g - b) / delta;
                }
                else if (g == colorMax)
                {
                    h = 2.0f + (b - r) / delta;
                }
                else // b is max
                {
                    h = 4.0f + (r - g) / delta;
                }
                h *= 60.0f;

                if (h < 0.0f)
                {
                    h += 360.0f;
                }

            }

            hsv[0] = h / 360.0f; // moving h to be between 0 and 1.
            hsv[1] = s;
            hsv[2] = v;
            hsv[3] = color[3];
        }

        private unsafe void HSV_to_RGB(float[] hsv, float[] rgb)
        {
            float[] color = new float[4];
            color[0] = color[1] = color[2] = color[3] = 0.0f;
            float f, p, q, t;
            float h, s, v;
            float r = 0.0f, g = 0.0f, b = 0.0f;
            float i;

            if (hsv[1] == 0.0f)
            {
                if (hsv[2] != 0.0f)
                {
                    color[0] = color[1] = color[2] = color[3] = hsv[2];
                }
            }
            else
            {
                h = hsv[0] * 360.0f;
                s = hsv[1];
                v = hsv[2];

                if (h == 360.0)
                {
                    h = 0.0f;
                }

                h /= 60.0f;

                i = (float)((int)h);

                f = h - i;

                p = v * (1.0f - s);
                q = v * (1.0f - (s * f));
                t = v * (1.0f - (s * (1.0f - f)));
                if (i < 0.01f)
                {
                    r = v;
                    g = t;
                    b = p;
                }
                else if (i < 1.01f)
                {
                    r = q;
                    g = v;
                    b = p;
                }
                else if (i < 2.01f)
                {
                    r = p;
                    g = v;
                    b = t;
                }
                else if (i < 3.01f)
                {
                    r = p;
                    g = q;
                    b = v;
                }
                else if (i < 4.01f)
                {
                    r = t;
                    g = p;
                    b = v;
                }
                else if (i < 5.01f)
                {
                    r = v;
                    g = p;
                    b = q;
                }

                color[0] = r;
                color[1] = g;
                color[2] = b;
            }
            color[3] = hsv[3];

            rgb[0] = color[0];
            rgb[1] = color[1];
            rgb[2] = color[2];
            rgb[3] = color[3];
        }

        private unsafe bool ProcessSpill(byte* tmp, byte* data, int width, int height, int stride)
        {
            float[] rgb = new float[4];
            float[] hsv = new float[4];

            float h1 = Hue - Tolerance;
            float h2 = Hue + Tolerance;

            float hue_tolerance = Tolerance;
            hue_tolerance /= 360.0f;

            h1 /= 360.0f;
            h2 /= 360.0f;

            float s = Saturation;

            for (int y = 0; y < height; y++)
            {
                byte* bits = (byte*)tmp + (y * stride);
                for (int x = 0; x < width; x++)
                {
                    rgb[2] = *(bits + x * 4 + 0) / 255.0f;
                    rgb[1] = *(bits + x * 4 + 1) / 255.0f;
                    rgb[0] = *(bits + x * 4 + 2) / 255.0f;
                    rgb[3] = *(bits + x * 4 + 3) / 255.0f;

                    RGB_to_HSV(rgb, hsv);

                    if (hsv[0] >= h1 && hsv[0] <= h2)
                    {
                        if (hsv[1] >= s)
                        {
                            if (hsv[2] >= MinValue && hsv[2] <= MaxValue)
                            {
                                hsv[3] = 0.0f;
                                hsv[1] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] < MinValue)
                            {
                                // make an attempt to preserve the shadows
                                hsv[3] = System.Math.Min(1.0f, MinValue + 1.0f - (hsv[2] / MinValue));
                                hsv[1] = 0.0f;
                                hsv[2] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] > MaxValue)
                            {
                                // make an attempt to preserve the highlights
                                hsv[3] = System.Math.Min(1.0f, ((hsv[2] - MaxValue) / (1.0f - MaxValue)));
                                hsv[1] = 0.0f;
                                hsv[2] = 1.0f;

                                HSV_to_RGB(hsv, rgb);
                            }

                        }
                        else
                        {
                            hsv[3] = 1.0f;
                            hsv[1] = 0.0f;
                            HSV_to_RGB(hsv, rgb);
                        }

                        *(bits + x * 4 + 0) = (byte)(rgb[2] * 255.0f);
                        *(bits + x * 4 + 1) = (byte)(rgb[1] * 255.0f);
                        *(bits + x * 4 + 2) = (byte)(rgb[0] * 255.0f);
                        *(bits + x * 4 + 3) = (byte)(rgb[3] * 255.0f);
                    }

                }
            }

            for (int y = 0; y < height; y++)
            {
                byte* src = (byte*)tmp + (y * stride);
                byte* dst = (byte*)data + (y * stride);

                for (int x = 0; x < width; x++)
                {
                    rgb[2] = *(src + x * 4 + 0) / 255.0f;
                    rgb[1] = *(src + x * 4 + 1) / 255.0f;
                    rgb[0] = *(src + x * 4 + 2) / 255.0f;
                    rgb[3] = *(src + x * 4 + 3) / 255.0f;

                    if ((x - leftSpill) >= 0)
                    {
                        float left = *(src + (x - leftSpill) * 4 + 3) / 255.0f;
                        if (left < rgb[3]) rgb[3] = left;
                    }

                    if ((x + rightSpill) < width)
                    {
                        float right = *(src + (x + rightSpill) * 4 + 3) / 255.0f;
                        if (right < rgb[3]) rgb[3] = right;
                    }

                    *(dst + x * 4 + 0) = (byte)(rgb[2] * 255.0f);
                    *(dst + x * 4 + 1) = (byte)(rgb[1] * 255.0f);
                    *(dst + x * 4 + 2) = (byte)(rgb[0] * 255.0f);
                    *(dst + x * 4 + 3) = (byte)(rgb[3] * 255.0f);
                }
            }

            return true;
        }

        public unsafe bool ProcessData(byte[] data, int width, int height, int stride,int taskCount)
        {
            var run1 = Task.Run(() =>
            {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, 0, height / 8);
                }
            });
            var run2 = Task.Run(() => {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height / 8, height / 8);
                }
            });
            var run3 = Task.Run(() =>
            {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height / 4, height / 8);
                }
            });


            var run4 = Task.Run(() =>
            {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height*3/ 8, height / 8);
                }
            });


            var run5 = Task.Run(() =>
            {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height / 2, height / 8);
                }
            });
            var run6 = Task.Run(() => {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height * 5 / 8, height / 8);
                }
            });
            var run7 = Task.Run(() =>
            {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height * 6 / 8, height / 8);
                }
            });


            var run8 = Task.Run(() =>
            {
                fixed (byte* p = &data[0])
                {
                    ProcessWithMargin(p, width, height, stride, height * 7 / 8, height / 4);
                }
            });




            run1.Wait();
            run2.Wait();
            run3.Wait();
            run4.Wait();

            run5.Wait();
            run6.Wait();
            run7.Wait();
            run8.Wait();
            return true;
            //var run3 = Task.Run(() => {
            //    fixed (byte* p = &data[0])
            //    {
            //        ProcessWithMargin(p, width, height, stride);
            //    }
            //});




        }

        private unsafe bool Process(byte* data, int width, int height, int stride)
        {
            float[] rgb = new float[4];
            float[] hsv = new float[4];

            float h1 = Hue - Tolerance;// / 2.0f;
            float h2 = Hue + Tolerance;// / 2.0f;

            float hue_tolerance = Tolerance;// / 2.0f;
            hue_tolerance /= 360.0f;

            h1 /= 360.0f;
            h2 /= 360.0f;

            float s = Saturation;

            for (int y = 0; y < height; y++)
            {
                byte* bits = (byte*)data + (y * stride);
                for (int x = 0; x < width; x++)
                {
                    rgb[2] = *(bits + x * 4 + 0) / 255.0f;
                    rgb[1] = *(bits + x * 4 + 1) / 255.0f;
                    rgb[0] = *(bits + x * 4 + 2) / 255.0f;
                    rgb[3] = 0xff;

                    *(bits + x * 4 + 3) = 0xff;

                    RGB_to_HSV(rgb, hsv);

                    if (hsv[0] >= h1 && hsv[0] <= h2)
                    {
                        if (hsv[1] >= s)
                        {
                            if (hsv[2] >= MinValue && hsv[2] <= MaxValue)
                            {
                                hsv[3] = 0.0f;
                                hsv[1] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] < MinValue)
                            {
                                // make an attempt to preserve the shadows
                                hsv[3] = System.Math.Min(1.0f, MinValue + 1.0f - (hsv[2] / MinValue));
                                hsv[1] = 0.0f;
                                hsv[2] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] > MaxValue)
                            {
                                // make an attempt to preserve the highlights
                                hsv[3] = System.Math.Min(1.0f, ((hsv[2] - MaxValue) / (1.0f - MaxValue)));
                                hsv[1] = 0.0f;
                                hsv[2] = 1.0f;

                                HSV_to_RGB(hsv, rgb);
                            }

                        }
                        else
                        {
                            hsv[3] = 1.0f;
                            hsv[1] = 0.0f;
                            HSV_to_RGB(hsv, rgb);
                        }

                        *(bits + x * 4 + 0) = (byte)(rgb[2] * 255.0f);
                        *(bits + x * 4 + 1) = (byte)(rgb[1] * 255.0f);
                        *(bits + x * 4 + 2) = (byte)(rgb[0] * 255.0f);
                        *(bits + x * 4 + 3) = (byte)(rgb[3] * 255.0f);
                    }

                }
            }

            return true;
        }


        private unsafe bool ProcessWithMargin(byte* data, int width, int height, int stride,int startHeight,int availbeHeight)
        {
            float[] rgb = new float[4];
            float[] hsv = new float[4];

            float h1 = Hue - Tolerance;// / 2.0f;
            float h2 = Hue + Tolerance;// / 2.0f;

            float hue_tolerance = Tolerance;// / 2.0f;
            hue_tolerance /= 360.0f;

            h1 /= 360.0f;
            h2 /= 360.0f;

            float s = Saturation;

            var aH = Math.Min(height - Margin_Bottom, availbeHeight + startHeight);
            var aF = Math.Max(Margin_Top, startHeight);
            for (int y = aF; y < aH; y++)
            {
                byte* bits = (byte*)data + (y * stride);
                for (int x = Margin_Left; x < width  - Margin_Right; x++)
                {
                    rgb[2] = *(bits + x * 4 + 0) / 255.0f;
                    rgb[1] = *(bits + x * 4 + 1) / 255.0f;
                    rgb[0] = *(bits + x * 4 + 2) / 255.0f;
                    rgb[3] = 0xff;

                    *(bits + x * 4 + 3) = 0xff;

                    RGB_to_HSV(rgb, hsv);

                    if (hsv[0] >= h1 && hsv[0] <= h2)
                    {
                        if (hsv[1] >= s)
                        {
                            if (hsv[2] >= MinValue && hsv[2] <= MaxValue)
                            {
                                hsv[3] = 0.0f;
                                hsv[1] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] < MinValue)
                            {
                                // make an attempt to preserve the shadows
                                hsv[3] = System.Math.Min(1.0f, MinValue + 1.0f - (hsv[2] / MinValue));
                                hsv[1] = 0.0f;
                                hsv[2] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] > MaxValue)
                            {
                                // make an attempt to preserve the highlights
                                hsv[3] = System.Math.Min(1.0f, ((hsv[2] - MaxValue) / (1.0f - MaxValue)));
                                hsv[1] = 0.0f;
                                hsv[2] = 1.0f;

                                HSV_to_RGB(hsv, rgb);
                            }

                        }
                        else
                        {
                            hsv[3] = 1.0f;
                            hsv[1] = 0.0f;
                            HSV_to_RGB(hsv, rgb);
                        }

                        *(bits + x * 4 + 0) = (byte)(rgb[2] * 255.0f);
                        *(bits + x * 4 + 1) = (byte)(rgb[1] * 255.0f);
                        *(bits + x * 4 + 2) = (byte)(rgb[0] * 255.0f);
                        *(bits + x * 4 + 3) = (byte)(rgb[3] * 255.0f);
                    }
                }
            }

            return true;
        }

        private unsafe bool ProcessWithMargin(byte* data, int width, int height, int stride)
        {
            float[] rgb = new float[4];
            float[] hsv = new float[4];

            float h1 = Hue - Tolerance;// / 2.0f;
            float h2 = Hue + Tolerance;// / 2.0f;

            float hue_tolerance = Tolerance;// / 2.0f;
            hue_tolerance /= 360.0f;

            h1 /= 360.0f;
            h2 /= 360.0f;

            float s = Saturation;

            for (int y = Margin_Top; y < height - Margin_Top - Margin_Bottom; y++)
            {
                byte* bits = (byte*)data + (y * stride);
                for (int x = Margin_Left; x < width - Margin_Left - Margin_Right; x++)
                {
                    rgb[2] = *(bits + x * 4 + 0) / 255.0f;
                    rgb[1] = *(bits + x * 4 + 1) / 255.0f;
                    rgb[0] = *(bits + x * 4 + 2) / 255.0f;
                    rgb[3] = 0xff;

                    *(bits + x * 4 + 3) = 0xff;

                    RGB_to_HSV(rgb, hsv);

                    if (hsv[0] >= h1 && hsv[0] <= h2)
                    {
                        if (hsv[1] >= s)
                        {
                            if (hsv[2] >= MinValue && hsv[2] <= MaxValue)
                            {
                                hsv[3] = 0.0f;
                                hsv[1] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] < MinValue)
                            {
                                // make an attempt to preserve the shadows
                                hsv[3] = System.Math.Min(1.0f, MinValue + 1.0f - (hsv[2] / MinValue));
                                hsv[1] = 0.0f;
                                hsv[2] = 0.0f;

                                HSV_to_RGB(hsv, rgb);
                            }
                            else if (hsv[2] > MaxValue)
                            {
                                // make an attempt to preserve the highlights
                                hsv[3] = System.Math.Min(1.0f, ((hsv[2] - MaxValue) / (1.0f - MaxValue)));
                                hsv[1] = 0.0f;
                                hsv[2] = 1.0f;

                                HSV_to_RGB(hsv, rgb);
                            }

                        }
                        else
                        {
                            hsv[3] = 1.0f;
                            hsv[1] = 0.0f;
                            HSV_to_RGB(hsv, rgb);
                        }

                        *(bits + x * 4 + 0) = (byte)(rgb[2] * 255.0f);
                        *(bits + x * 4 + 1) = (byte)(rgb[1] * 255.0f);
                        *(bits + x * 4 + 2) = (byte)(rgb[0] * 255.0f);
                        *(bits + x * 4 + 3) = (byte)(rgb[3] * 255.0f);
                    }
                }
            }

            return true;
        }

        //public unsafe bool Chroma(Bitmap bitmap)
        //{
        //    if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) return false;

        //    System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
        //        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //        System.Drawing.Imaging.ImageLockMode.ReadWrite,
        //        bitmap.PixelFormat);

        //    Process((byte*)data.Scan0, data.Width, data.Height, data.Stride);

        //    bitmap.UnlockBits(data);

        //    return true;
        //}

        public unsafe bool Chroma(IntPtr data, int width, int height, int stride)
        {
            ProcessWithMargin((byte*)data, width, height, stride);
            return true;
        }

        //public unsafe bool ChromaWithSpill(Bitmap bitmap)
        //{
        //    if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) return false;

        //    Bitmap tmpBitmap = new Bitmap(bitmap);

        //    System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
        //        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //        System.Drawing.Imaging.ImageLockMode.ReadWrite,
        //        bitmap.PixelFormat);

        //    System.Drawing.Imaging.BitmapData tmpdata = tmpBitmap.LockBits(
        //        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        //        System.Drawing.Imaging.ImageLockMode.ReadWrite,
        //        bitmap.PixelFormat);

        //    ProcessSpill((byte*)tmpdata.Scan0, (byte*)data.Scan0, data.Width, data.Height, data.Stride);

        //    bitmap.UnlockBits(data);
        //    tmpBitmap.UnlockBits(data);

        //    return true;
        //}

    }
}
