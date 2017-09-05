using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemeGenWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public enum BubblePosition
        {
            TopLeft, TopRight, BottomLeft, BottomRight,
            TopLeftStar, TopRightStar, BottomLeftStar
        }

        public class BubbleInfo
        {
            public Point Startpoint { get; set; }
            public string ImageName { get; set; }
            public Rectangle TextArea { get; set; }
        }

        public BubbleInfo GetBubbleInfo(BubblePosition position)
        {
            BubbleInfo info = new BubbleInfo();
            //Point p = new Point();
            switch (position)
            {
                case BubblePosition.TopLeft:
                    info.Startpoint = new Point(0, 0);
                    info.ImageName = "bubble-tl.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
                case BubblePosition.TopLeftStar:
                    info.Startpoint = new Point(0, 0);
                    info.ImageName = "bubble-star2tl.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
                case BubblePosition.TopRight:
                    info.Startpoint = new Point(200, 0);
                    info.ImageName = "bubble-tr.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
                case BubblePosition.TopRightStar:
                    info.Startpoint = new Point(200, 0);
                    info.ImageName = "bubble-star2tr.png";
                    info.TextArea = new Rectangle(70, 0, 200, 140);
                    return info;
                case BubblePosition.BottomLeft:
                    info.Startpoint = new Point(0, 400);
                    info.ImageName = "bubble-bl.png";
                    info.TextArea = new Rectangle(10, 60, 200, 120);
                    return info;
                case BubblePosition.BottomLeftStar:
                    info.Startpoint = new Point(0, 400);
                    info.ImageName = "bubble-star2bl.png";
                    info.TextArea = new Rectangle(0, 50, 200, 140);
                    return info;
                case BubblePosition.BottomRight:
                    info.Startpoint = new Point(200, 400);
                    info.ImageName = "bubble-star2br.png";
                    info.TextArea = new Rectangle(70, 60, 200, 140);
                    return info;
                default:
                    info.Startpoint = new Point(0, 0);
                    info.ImageName = "bubble-tl.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
            }
        }

        public ActionResult About(string t)
        {
            // 가정
            // 1. 이미지의 widht 는 400으로 한다. (resize 필요)
            // 2. 버블은 좌,우 및 상,하로 구분한다
            // 3. 
            string file = @"C:\Temp\images\tyson.jpg";
            byte[] photoBytes = System.IO.File.ReadAllBytes(file);

            var bubbleInfo = GetBubbleInfo(BubblePosition.BottomLeftStar);
            
            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            //Size size = new Size(150, 0);

            Bitmap bitmap = (Bitmap)Image.FromFile(Path.Combine(@"C:\Temp\images\", bubbleInfo.ImageName));

            //string firstText = "내 이름은";
            //string secondText = "빌 게이츠";

            //PointF firstLocation = new PointF(10f, 10f);
            //PointF secondLocation = new PointF(10f, 50f);

            //using (Graphics graphics = Graphics.FromImage(bitmap))
            //{
            //    using (Font arialFont = new Font("Malgun Gothic", 20, FontStyle.Bold))
            //    {
            //        graphics.DrawString(firstText, arialFont, Brushes.Black, firstLocation);
            //        graphics.DrawString(secondText, arialFont, Brushes.Black, secondLocation);
            //    }
            //}

            string textDiplay = "한방에 훅간다! 한방에 훅간다한방에 훅간다한방에 훅간다한방에 훅간다한방에 훅간다한방에 훅간다한방에 훅간다";

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (Font font1 = new Font("Segoe UI", 120, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Rectangle rect1 = bubbleInfo.TextArea;

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Font goodFont = FindFont(graphics, textDiplay, rect1.Size, font1);
                graphics.DrawString(textDiplay, goodFont, Brushes.Black, rect1, stringFormat);
            }

            //Image img = (Image)bitmap;
            Image img = bitmap as Image;

            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        //Image img = Image.FromFile(@"C:\Temp\images\bubble-tp.png");
                        ImageLayer layer = new ImageLayer();
                        layer.Image = img;
                        layer.Opacity = 90;
                        layer.Position = bubbleInfo.Startpoint;

                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    //.Resize(size)
                                    .Format(format)
                                    .Overlay(layer)
                                    .Save(outStream);
                    }

                    // Do something with the stream.
                    using (var fileStream = System.IO.File.Create(@"C:\Temp\images\bill2.jpg"))
                    {
                        outStream.Seek(0, SeekOrigin.Begin);
                        outStream.CopyTo(fileStream);
                    }
                }
            }

            //ViewBag.Message = "Your application description page.";

            return View();
        }

        private Font FindFont(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
            //you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            
            //font 길이 보정
            RealSize.Width += 150;

            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;

            if (ScaleFontSize < 12) ScaleFontSize = 12f;
            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}