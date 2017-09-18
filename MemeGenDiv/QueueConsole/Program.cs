using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsole
{
    class Program
    {
        static CloudQueue queue;

        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            queue = queueClient.GetQueueReference("meme-queue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            var userInfo = new UserInfo()
            {
                ID = "4711",
                Name = "TY",
                Title = "bjn"
            };

            SendMessage(userInfo).Wait();

            //Console.ReadKey();
        }

        private static async Task SendMessage(UserInfo userInfo)
        {
            // convert object to json string
            string messageJson = await JsonConvert.SerializeObjectAsync(userInfo);
            
            // send message to queue
            var message = new CloudQueueMessage(messageJson);
            await queue.AddMessageAsync(message);
        }
    }

    public class UserInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string Title { get; set; }
    }
}
