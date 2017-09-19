using System.Drawing;

namespace MemeImgGen
{
    public class BubbleInfo
    {
        public Point StartPoint { get; set; }
        public string ImageName { get; set; }
        public Rectangle TextArea { get; set; }
    }


    public class Request
    {
        public string ID { get; set; }
        public string ImgPath { get; set; }
        public MemeInfo MemeInfo { get; set; }
        public BotInfo BotInfo { get; set; }
    }

    public class MemeInfo
    {
        public string Bubble { get; set; }
        public string Filter { get; set; }
        public string Text { get; set; }

    }
    public class BotInfo
    {
        public string ToID { get; set; }
        public string FromID { get; set; }
        public string ToName { get; set; }
        public string FromName { get; set; }
        public string ServiceURL { get; set; }
        public string ChannelID { get; set; }
        public string ConversationID { get; set; }
    }


}
