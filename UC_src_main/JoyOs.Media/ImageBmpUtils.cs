using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using Color = System.Windows.Media.Color;

namespace JoyOs.Media
{
    public static class ImageBmpUtils
    {

        public static BitmapImage LoadBitmapImage(string imageName)
        {
            Bitmap bitmap;

            using (FileStream fs = File.Open(imageName, FileMode.Open))
                bitmap = new Bitmap(fs);

            return Convert(bitmap);
        }

        public static BitmapImage LoadAndColorizeBitmapImage(string imageName, Color color)
        {
            using (FileStream fs = File.Open(imageName, FileMode.Open))
            {
                var bitmap = new Bitmap(fs);
                ColorizeBitmap(bitmap, color);

                return Convert(bitmap);
            }
        }

        public static BitmapImage LoadAndColorizeBitmapImage(BitmapImage bit, Color color)
        {   
                Bitmap bitmap = Convert(bit);
                ColorizeBitmap(bitmap, color);
                return Convert(bitmap);
        }

        public static Color Colorize(Color originalColor, Color color)
        {
            int strength = (originalColor.R + originalColor.G + originalColor.B);
            return
                Color.FromArgb(
                    originalColor.A,
                    System.Convert.ToByte(color.R*strength/255/3),
                    System.Convert.ToByte(color.G*strength/255/3),
                    System.Convert.ToByte(color.B*strength/255/3));
        }

        public static Color ToMediaColor(this System.Drawing.Color color)
        {
            return
                Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Drawing.Color ToSystemColor(this Color color)
        {
            return
                System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static BitmapImage Convert(Bitmap bitmap)
        {
            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            var bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static Bitmap Convert(BitmapImage bitmap)
        {
            //TODO: фэйл какойто
            var memoryStream = new MemoryStream();
            var bitmapEncoder = new BmpBitmapEncoder();

            bitmapEncoder.Frames.Add(BitmapFrame.Create(memoryStream));
            bitmapEncoder.Save(memoryStream);

            return new Bitmap(memoryStream);
        }

        public static void ColorizeBitmap(Bitmap bitmap, Color tintColor)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color clearPixel = bitmap.GetPixel(x, y).ToMediaColor();
                    Color tintedPixel = Colorize(clearPixel, tintColor);

                    bitmap.SetPixel(x, y, tintedPixel.ToSystemColor());
                }
            }
        }

        //public static Bitmap ResizeBitmapWithHigthCvolity(Graphics e,Rectangle rectangle)
        //{
        //    Graphics g = e;
        //    Bitmap bmp = new Bitmap("rama.jpg");
        //    g.FillRectangle(Brushes.White, rectangle);

        //    int width = bmp.Width;
        //    int height = bmp.Height;

        //    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    g.DrawImage(
        //      bmp,
        //      new Rectangle(130, 10, 120, 120),   // source rectangle
        //      new Rectangle(0, 0, width, height), // destination rectangle
        //      GraphicsUnit.Pixel);
        //    return bmp;
        //}

        //public static Bitmap ScaleByPercent(this Bitmap imgPhoto, int Percent)
        //{
        //    float nPercent = ((float)Percent / 100);

        //    int sourceWidth = imgPhoto.Width;
        //    int sourceHeight = imgPhoto.Height;
        //    var destWidth = (int)(sourceWidth * nPercent);
        //    var destHeight = (int)(sourceHeight * nPercent);

        //    var bmPhoto = new Bitmap(destWidth, destHeight,
        //                             PixelFormat.Format24bppRgb);
        //    bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
        //                          imgPhoto.VerticalResolution);

        //    Graphics grPhoto = Graphics.FromImage(bmPhoto);
        //    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

        //    grPhoto.DrawImage(imgPhoto,
        //                      new Rectangle(0, 0, destWidth, destHeight),
        //                      new Rectangle(0, 0, sourceWidth, sourceHeight),
        //                      GraphicsUnit.Pixel);

        //    grPhoto.Dispose();
        //    return bmPhoto;
        //}

        //public static Bitmap DrawShapes(object sender, System.Windows.Forms.PaintEventArgs e)
        //{

        //    Graphics gForm = e.Graphics;
        //    gForm.FillRectangle(Brushes.White, this.ClientRectangle);

        //    // Create a Bitmap image in memory and set its CompositingMode
        //    Bitmap bmp = new Bitmap(260, 260, PixelFormat.Format32bppArgb);
        //    Graphics gBmp = Graphics.FromImage(bmp);
        //    gBmp.CompositingMode = CompositingMode.SourceCopy;

        //    // draw a red circle to the bitmap in memory
        //    Color red = Color.FromArgb(0x60, 0xff, 0, 0);
        //    Brush redBrush = new SolidBrush(red);
        //    gBmp.FillEllipse(redBrush, 70, 70, 160, 160);

        //    // draw a green rectangle to the bitmap in memory
        //    Color green = Color.FromArgb(0x40, 0, 0xff, 0);
        //    Brush greenBrush = new SolidBrush(green);
        //    gBmp.FillRectangle(greenBrush, 10, 10, 140, 140);

        //    // draw the bitmap on our window
        //    gForm.DrawImage(bmp, 20, 20, bmp.Width, bmp.Height);

        //    bmp.Dispose();
        //    gBmp.Dispose();
        //    redBrush.Dispose();
        //    greenBrush.Dispose();

        //    return bmp;
        //}
        //    /// <summary>
        ///// Metoda nacte pole barev z bitmapy
        ///// </summary>
        ///// <param name="bmp">Bitmapa, ze ktere se budou cist data</param>
        ///// <returns>Pole barev</returns>
        //public unsafe static Color[,] BitmapToColorArray(Bitmap bmp)
        //{
        //    Color[,] img = new Color[bmp.Width, bmp.Height];
        //    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        //    for (int y = 0; y < data.Height; y++)
        //    {
        //        // vypocte ukazatel na zacatek y-teho radku
        //        int* retPos = (int*)((int)data.Scan0 + (y * data.Stride));

        //        int x = 0;
        //        while (x < data.Width)
        //        {
        //            // vyplni pixel nahodnou barvou
        //            img[x, y] = Color.FromArgb(*retPos);

        //            // posun na dalsi pixel
        //            retPos++; x++;
        //        }
        //    }
        //    bmp.UnlockBits(data);

        //    return img;
        //}

        ///// <summary>
        ///// Metoda vytvori Bitmapu z pole barev
        ///// </summary>
        ///// <param name="pixels">Zadane pole barev</param>
        ///// <returns>Vytvorena bitmapa</returns>
        //public unsafe static Bitmap ColorArrayToBitmap(Color[,] pixels)
        //{
        //    Bitmap bmp = new Bitmap(pixels.GetLength(0), pixels.GetLength(1));
        //    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        //    for (int y = 0; y < data.Height; y++)
        //    {
        //        // vypocte ukazatel na zacatek y-teho radku
        //        int* retPos = (int*)((int)data.Scan0 + (y * data.Stride));

        //        int x = 0;
        //        while (x < data.Width)
        //        {
        //            // vyplni pixel nahodnou barvou
        //            *retPos = pixels[x, y].ToArgb();

        //            // posun na dalsi pixel
        //            retPos++; x++;
        //        }
        //    }
        //    bmp.UnlockBits(data);

        //    return bmp;
        //}


        //public static Color[,] Blur(Color[,] img)
        //{
        //    Color[,] nMatrix = new Color[img.GetLength(0), img.GetLength(1)];

        //    for (int i = 0; i < nMatrix.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < nMatrix.GetLength(1); j++)
        //        {
        //            Color i1j1 = (j + 1 < img.GetLength(1) && (i + 1) < img.GetLength(0)) ? img[i + 1, j + 1] : Color.FromArgb(0, 0, 0);
        //            Color ij1 = (j + 1 < img.GetLength(1)) ? img[i, j + 1] : Color.FromArgb(0, 0, 0);
        //            Color i_1j1 = (j + 1 < img.GetLength(1) && (i - 1) > 0) ? img[i - 1, j + 1] : Color.FromArgb(0, 0, 0);

        //            Color i1j = (i + 1 < img.GetLength(0)) ? img[i + 1, j] : Color.FromArgb(0, 0, 0);
        //            Color ij = img[i, j];
        //            Color i_1j = ((i - 1) > 0) ? img[i - 1, j] : Color.FromArgb(0, 0, 0);

        //            Color i1j_1 = ((j - 1) > 0 && (i + 1) < img.GetLength(0)) ? img[i + 1, j - 1] : Color.FromArgb(0, 0, 0);
        //            Color ij_1 = (j - 1 > 0) ? img[i, j - 1] : Color.FromArgb(0, 0, 0);
        //            Color i_1j_1 = (j - 1 > 0 && (i - 1) > 0) ? img[i - 1, j - 1] : Color.FromArgb(0, 0, 0);



        //            int blurR = i1j1.R + ij1.R + i_1j1.R + i1j.R + ij.R + i_1j.R + i1j_1.R + ij_1.R + i_1j_1.R;
        //            int blurG = i1j1.G + ij1.G + i_1j1.G + i1j.G + ij.G + i_1j.G + i1j_1.G + ij_1.G + i_1j_1.G;
        //            int blurB = i1j1.B + ij1.B + i_1j1.B + i1j.B + ij.B + i_1j.B + i1j_1.B + ij_1.B + i_1j_1.B;

        //            blurR /= 9;
        //            blurG /= 9;
        //            blurB /= 9;

        //            nMatrix[i, j] = Color.FromArgb(blurR, blurG, blurB);
        //        }
        //    }


        //    return nMatrix;
        //}
   // }
    }
}