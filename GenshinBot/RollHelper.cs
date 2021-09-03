using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GenshinBot
{
    static class RollHelper
    {
        static readonly Random _rnd;

        static RollHelper()
        {
            _rnd = new Random(DateTime.Now.Millisecond);
        }

        public static WishInfo Wish()
        {
            WishInfo wish = new WishInfo();
            int stars = 3;
            int chance = _rnd.Next(1, 100);

            if (chance <= 5)
                stars = 5;
            else if (chance < 30)
                stars = 4;

            wish.WishPath = GetWishGifPath(stars);
            wish.CharacterPath = GetCharacterPath(stars);
            return wish;
        }

        private static string GetWishGifPath(int stars)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            //Console.WriteLine(projectDirectory);
            var gifPath = Path.Combine(projectDirectory, $"Doc/Wish/{stars}starwish-single.gif");
            //Console.WriteLine(gifPath);

            //var img = System.Drawing.Image.FromFile(gifPath);
            //var duration = GetGifDuration(img, 12);
            //Console.WriteLine(duration);

            return gifPath;
        }
        private static string GetCharacterPath(int stars)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            var charsPath = Path.Combine(projectDirectory, $"Doc/Chars/{stars}");
            return Directory.GetFiles(charsPath).OrderBy(x => _rnd.Next()).First();

        }
        private static TimeSpan? GetGifDuration(System.Drawing.Image image, int fps = 60)
        {
            var minimumFrameDelay = (1000.0 / fps);
            if (!image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif)) return null;
            if (!ImageAnimator.CanAnimate(image)) return null;

            var frameDimension = new FrameDimension(image.FrameDimensionsList[0]);

            var frameCount = image.GetFrameCount(frameDimension);
            var totalDuration = 0;

            for (var f = 0; f < frameCount; f++)
            {
                var delayPropertyBytes = image.GetPropertyItem(20736).Value;
                var frameDelay = BitConverter.ToInt32(delayPropertyBytes, f * 4) * 10;
                // Minimum delay is 16 ms. It's 1/60 sec i.e. 60 fps
                totalDuration += (frameDelay < minimumFrameDelay ? (int)minimumFrameDelay : frameDelay);
            }

            return TimeSpan.FromMilliseconds(totalDuration);
        }
    }

    public class WishInfo
    {
        public string WishPath { get; set; }
        public string CharacterPath { get; set; }
    }
}
