using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiscordAstroBot.Objects.Config
{
    /// <summary>
    /// Serializable objects to represent the mod queue
    /// </summary>
    [Serializable, XmlRoot("SocialMediaModQueue"), XmlType("SocialMediaModQueue")]
    public class SocialMediaModQueueConfig
    {
        /// <summary>
        /// Holds all the servers with a mod queue
        /// </summary>
        [XmlElement("Server")]
        public List<SocialMediaModQueueServerEntry> Servers { get; set; }
    }

    /// <summary>
    /// Represents the mod queue on a specific server
    /// </summary>
    public class SocialMediaModQueueServerEntry
    {
        /// <summary>
        /// The ID of the server
        /// </summary>
        [XmlAttribute("ID")]
        public ulong ID { get; set; }

        /// <summary>
        /// The list of queue entries for this server
        /// </summary>
        [XmlElement("QueueEntry")]
        public List<SocialMediaModQueueEntry> QueueEntries { get; set; }
    }

    /// <summary>
    /// Represents a single mod queue entry
    /// </summary>
    public class SocialMediaModQueueEntry
    {
        /// <summary>
        /// The id of the queue entry (equals the message ID from the server)
        /// </summary>
        [XmlAttribute("ID")]
        public ulong ID { get; set; }

        /// <summary>
        /// The user ID of the author of this post
        /// </summary>
        [XmlElement("Author")]
        public ulong Author { get; set; }

        /// <summary>
        /// Url to the image which should be shared to social media
        /// </summary>
        [XmlElement("Image")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The content (a short message) describing the post, written by the author
        /// </summary>
        [XmlElement("Content")]
        public string Content { get; set; }

        /// <summary>
        /// The status of the queue entry
        /// </summary>
        [XmlAttribute("Status")]
        public SocialMediaModQueueStatus Status { get; set; }

        /// <summary>
        /// Override the ToString method to yiel proper output for Discord
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ID: {this.ID}\r\nStatus: {this.Status}\r\nImage: {this.ImageUrl}\r\nContents:\r\n```\r\n{this.Content}\r\n```";
        }
    }

    public enum SocialMediaModQueueStatus
    {
        AWAITINGREVIEW = 1,
        APPROVED = 2,
        DECLINED = 3,
        PUBLISHED = 4
    }
}