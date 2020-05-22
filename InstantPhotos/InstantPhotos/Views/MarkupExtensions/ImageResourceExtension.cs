using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InstantPhotos
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        /// <summary>
        /// String source path for embedded image
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Converts Source path to image
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;               
            }
            ImageSource tmp = ImageSource.FromResource(Source);
            return tmp;
        }
    }
}
