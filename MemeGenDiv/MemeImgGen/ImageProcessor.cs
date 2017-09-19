using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeImgGen
{
    public class ImageProcessor
    {
        string assetsPath = @"C:\Temp\images\";
        string fontName = "Kristen ITC";
        int opacity = 90;

        int figure_normal_revise_w = 20;
        int figure_normal_revise_h = 60;

        public ImageProcessor(string assetsPath)
        {
            this.assetsPath = assetsPath;
        }

        public IMatrixFilter GetFilterByName(BubbleFilter filter)
        {
            // MatrixFilters.Comic는 Enum이 아니라 Property다.
            // 동적으로 가져오려면 Reflection을 사용해야 하는데 그건 좀 무겁다.
            switch (filter)
            {
                case BubbleFilter.NONE:
                    return null;
                case BubbleFilter.COMIC:
                    return MatrixFilters.Comic;
                case BubbleFilter.BLACKWHITE:
                    return MatrixFilters.BlackWhite;
                case BubbleFilter.GOTHAM:
                    return MatrixFilters.Gotham;
                case BubbleFilter.GREYSCALE:
                    return MatrixFilters.GreyScale;
                case BubbleFilter.LOMOGRAPH:
                    return MatrixFilters.Lomograph;
                case BubbleFilter.POLAROID:
                    return MatrixFilters.Polaroid;
                case BubbleFilter.HISATCH:
                    return MatrixFilters.HiSatch;
                case BubbleFilter.SEPIA:
                    return MatrixFilters.Sepia;
                default:
                    return MatrixFilters.Lomograph;
            }
        }

        private BubbleInfo GetBubbleInfo(BubblePosition position, BubbleFigure bubbleFigure)
        {
            BubbleInfo info = new BubbleInfo();
            // ie : bubble-TOPLEFT-STAR.png
            info.ImageName = $"BUBBLE-{position.ToString()}-{bubbleFigure.ToString()}.png";
            info.StartPoint = new Point(0, 0); // Right position is calculated after automatically 

            //Point p = new Point();
            switch (position)
            {
                case BubblePosition.TOPLEFT:
                    switch (bubbleFigure)
                    {
                        case BubbleFigure.STAR:
                            info.TextArea = new Rectangle(10, 0, 200, 120);
                            break;
                        case BubbleFigure.CLOUD:

                        case BubbleFigure.NORMAL:
                        default:
                            info.TextArea = new Rectangle(10, 10, 200, 110);
                            break;
                    }
                    return info;
                case BubblePosition.TOPRIGHT:
                    switch (bubbleFigure)
                    {
                        case BubbleFigure.STAR:
                            info.TextArea = new Rectangle(70, 0, 200, 140);
                            break;
                        case BubbleFigure.CLOUD:

                        case BubbleFigure.NORMAL:
                        default:
                            info.TextArea = new Rectangle(10, 10, 200, 120);
                            break;
                    }
                    return info;
                //info.STARtpoint = new Point(200, 0);
                //info.ImageName = "bubble-tr.png";
                //info.TextArea = new Rectangle(10, 10, 200, 120);
                //break;
                case BubblePosition.BOTTOMLEFT:
                    switch (bubbleFigure)
                    {
                        case BubbleFigure.STAR:
                            info.TextArea = new Rectangle(5, 60, 200, 130);
                            break;
                        case BubbleFigure.CLOUD:

                        case BubbleFigure.NORMAL:
                        default:
                            info.TextArea = new Rectangle(10, 60, 200, 120);
                            break;
                    }
                    return info;
                //info.STARtpoint = new Point(0, 400);
                //info.ImageName = "bubble-bl.png";
                //info.TextArea = new Rectangle(10, 60, 200, 120);
                //break;
                case BubblePosition.BOTTOMRIGHT:
                    switch (bubbleFigure)
                    {
                        case BubbleFigure.STAR:
                            info.TextArea = new Rectangle(70, 50, 200, 160);
                            break;
                        case BubbleFigure.CLOUD:

                        case BubbleFigure.NORMAL:
                        default:
                            info.TextArea = new Rectangle(0, 50, 200, 140);
                            break;
                    }
                    return info;
                //info.STARtpoint = new Point(200, 400);
                //info.ImageName = "bubble-STAR2br.png";
                //info.TextArea = new Rectangle(70, 60, 200, 140);
                //return info;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Main method of this class for processing image
        /// provide filtering, image overlaying etc 
        /// </summary>
        /// <param name="photoBytes"></param>
        /// <returns></returns>
        public byte[] ImageGenerate(byte[] photoBytes, string textDiplay, 
            BubbleFigure bubbleFigure, BubblePosition bubblePosition, IMatrixFilter appliedFilter)
        {
            // Get width and height of original image
            // I know it's not good solution but now... remain 
            int originImgWidth, originImgHeight;
            using (Image image = Image.FromStream(new MemoryStream(photoBytes)))
            {
                originImgWidth = image.Width;
                originImgHeight = image.Height;
            }

            // 가정
            // 1. 이미지의 widht 는 400으로 한다. (resize 필요)
            // 2. 버블은 좌,우 및 상,하로 구분한다

            var bubbleInfo = GetBubbleInfo(bubblePosition, bubbleFigure);

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };

            Bitmap bitmap = (Bitmap)Image.FromFile(Path.Combine(assetsPath, bubbleInfo.ImageName));

            #region 강제 텍스트 출력예
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
            #endregion

            //Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            // create bitmap for overlaying
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                using (Font font1 = new Font(this.fontName, 120, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    Rectangle rect1 = bubbleInfo.TextArea;

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    Font goodFont = FindFont(graphics, textDiplay, rect1.Size, font1);
                    graphics.DrawString(textDiplay, goodFont, Brushes.Black, rect1, stringFormat);
                }
            }
            //Image img = (Image)bitmap;
            Image overlayImage = (Image)bitmap;

            //get width and height of layer for overlay 
            int overlayWidth, overlayHeight;
            overlayWidth = overlayImage.Width;
            overlayHeight = overlayImage.Height;

            // revise TextArea width and Height
            Rectangle rec = bubbleInfo.TextArea;
            if(bubbleFigure == BubbleFigure.NORMAL)
            {
                bubbleInfo.TextArea = new Rectangle(rec.X, rec.Y, rec.Width - figure_normal_revise_w, rec.Height - figure_normal_revise_h);
            }

            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                // if bubblePosition is right side, start X is imgWidth - overlayWidth
                if (bubblePosition == BubblePosition.TOPRIGHT || bubblePosition ==  BubblePosition.BOTTOMRIGHT)
                {
                    bubbleInfo.StartPoint = new Point(originImgWidth - overlayWidth, bubbleInfo.StartPoint.Y);
                }

                // if bubblePosition is bottom side, start y is imgHeight - overlayHeight
                if (bubblePosition == BubblePosition.BOTTOMLEFT || bubblePosition == BubblePosition.BOTTOMRIGHT)
                {
                    bubbleInfo.StartPoint = new Point(bubbleInfo.StartPoint.X, originImgHeight - overlayHeight);
                }

                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        //Image img = Image.FromFile(@"C:\Temp\images\bubble-tp.png");
                        ImageLayer layer = new ImageLayer();
                        layer.Image = overlayImage;
                        layer.Opacity = this.opacity;
                        layer.Position = bubbleInfo.StartPoint;

                        //필터 적용?
                        // Comic, Gotham, GreyScale, Lomograph, Polaroid, HiSatch, Sepia
                        var filter = appliedFilter;

                        // Load, resize, set the format and quality and save an image.
                        var factory = imageFactory.Load(inStream)
                                                //.Resize(size)
                                                .Format(format);

                        if (appliedFilter != null)
                            factory.Filter(filter);

                        factory
                            .Overlay(layer)
                            .Save(outStream);
                    }

                    return outStream.ToArray();
                }
            }
        }

        private Font FindFont(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
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
    }
}
