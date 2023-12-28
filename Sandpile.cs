using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Sandpile1
{
    public partial class Sandpile : ObservableObject
    {
        [ObservableProperty]
        int grains = 0;

        [ObservableProperty]
        BitmapImage image = new();

        [ObservableProperty]
        bool notBusy = true;

        [ObservableProperty]
        double progress = 0;

        static int[,] grid = new int[150, 150];

        private void GrainAdd(int x, int y)
        {
            switch (grid[x, y])
            {
                case 0:
                case 1:
                case 2:
                    grid[x, y] += 1;
                    break;
                case 3:
                    grid[x, y] = 0;
                    GrainAdd(x + 1, y);
                    GrainAdd(x - 1, y);
                    GrainAdd(x, y + 1);
                    GrainAdd(x, y - 1);
                    break;
                default:
                    break;
            }
            // generate here for Every possible image..
            //ImageGenerate();
        }

        private void GrainAddMiddle(int grains)
        {
            int centerX = 75;
            int centerY = 75;

            for (int x = 1; x <= grains; x++)
            {
                Progress = (double)x / grains * 100;
                GrainAdd(centerX, centerY);
                // add here for all STEPS, not recursive genratin.
                ImageGenerate();
            }
        }

        partial void OnGrainsChanged(int oldValue, int newValue)
        {
            // Optomization: if we are always dropping in the middle
            // if newValue > oldValue, just drop the difference onto exisiting pile
            // if newvalue < oldValue, recreate whole thing.
            // if equal, do nothing.

            // right now do whole thing each time.

            Task.Run(() =>
            {
                NotBusy = false;
                if (oldValue < newValue)
                {
                    GrainAddMiddle(newValue - oldValue); // add new number of grains
                }
                else
                {
                    grid = new int[150, 150];
                    GrainAddMiddle(newValue);
                }
                // ImageGenerate();
                // generatge once at the end or multiple times??
                NotBusy = true;
            });
        }

        private void ImageGenerate()
        {
            using Bitmap bmp = new Bitmap(150, 150);

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    switch (grid[x, y])
                    {
                        case 0:
                            bmp.SetPixel(x, y, Color.FromArgb(255, 255, 178));
                            break;
                        case 1:
                            bmp.SetPixel(x, y, Color.FromArgb(254, 204, 92));
                            break;
                        case 2:
                            bmp.SetPixel(x, y, Color.FromArgb(253, 141, 60));
                            break;
                        case 3:
                            bmp.SetPixel(x, y, Color.FromArgb(227, 26, 28));
                            break;
                        default:
                            bmp.SetPixel(x, y, Color.White);
                            break;
                    }
                }
            }
            Image = BmpImageFromBmp(bmp);
        }

        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using var memory = new System.IO.MemoryStream();

            bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

    }
}
