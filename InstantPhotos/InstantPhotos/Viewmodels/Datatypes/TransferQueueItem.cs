using System.ComponentModel;
using System.IO;
using Xamarin.Forms;

namespace InstantPhotos
{
    /// <summary>
    /// Represents a transfer queue item both for display and processing
    /// </summary>
    class TransferQueueItem
    {
        /// <summary>
        /// Contains name of the image
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contains raw image data
        /// </summary>
        public ImageSource Image { get; set; }

        /*/// <summary>
        /// Represents tranfer progress
        /// </summary>
        public double TransferProgress { get; set; } = 0;*/

        /// <summary>
        /// Contains filestream of image
        /// </summary>
        public FileStream Stream { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="s"></param>
        /// <param name="img"></param>
        /// <param name="name"></param>
        public TransferQueueItem(FileStream stream ,ImageSource img, string name)
        {
            this.Name = name;
            this.Image = img;
            this.Stream = stream;
        }

    }
}
