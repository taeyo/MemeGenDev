using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using ImageProcessor;
using ImageProcessor.Imaging;
using System.Configuration;
using ImageProcessor.Imaging.Filters.Photo;
using MemeImgGen;
using System;

namespace MemeGen
{
    public static class MemeGen
    {
        //public class VisionInfo
        //{
        //    public string id { get; set; }
        //    public string BlobName { get; set; }
        //    public string BubbleFigure { get; set; }
        //    public string BubblePosition { get; set; }
        //    public string BubbleFilter { get; set; }
        //    public string BubbleText { get; set; }
        //}

        public class VisionInfo
        {
            public string id { get; set; }
            public string BlobName { get; set; }
            public BubbleInfo BubbleInfo { get; set; }
        }

        public class BubbleInfo
        {
            public string Figure { get; set; }
            public string Position { get; set; }
            public string Filter { get; set; }
            public string Text { get; set; }
        }

        [StorageAccount("AzureWebJobsStorage")]
        [FunctionName("MemeBlob")]
        public static async void Run(
            [QueueTrigger("meme-que")] VisionInfo info,
            string BlobName,
            [Blob("meme-input/{BlobName}", FileAccess.Read)]byte[] inputBlob,
            [Blob("meme-output/{BlobName}", FileAccess.Write)]Stream outputBlob,
            TraceWriter log)
        {
            string assetsPath = ConfigurationManager.AppSettings["ASSETS_ROOT"] ?? @"C:\Temp\images\";

            string bubbleFigure = info.BubbleInfo.Figure.ToUpper();
            string bubblePos = info.BubbleInfo.Position.ToUpper();
            string filter = info.BubbleInfo.Filter.ToUpper();
            string textDisplay = info.BubbleInfo.Text;
            var imgGen = new MemeImgGen.ImageProcessor(assetsPath);

            BubbleFigure figure = (BubbleFigure)Enum.Parse(typeof(BubbleFigure), bubbleFigure);
            BubblePosition position = (BubblePosition)Enum.Parse(typeof(BubblePosition), bubblePos);
            BubbleFilter bFilter = (BubbleFilter)Enum.Parse(typeof(BubbleFilter), filter);
            IMatrixFilter bubbleFilter = imgGen.GetFilterByName(bFilter);

            // 이미지 변환 메서드 호출
            byte[] outBytes = imgGen.ImageGenerate(inputBlob, textDisplay, figure, position, bubbleFilter);
            await outputBlob.WriteAsync(outBytes, 0, outBytes.Length);
        }
    }
}
