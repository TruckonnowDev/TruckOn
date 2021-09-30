using System;
using System.Threading.Tasks;
using Foundation;
using MDispatch.iOS.NewRender.Direktory;
using MDispatch.NewElement.Directory;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(MediaService))]
namespace MDispatch.iOS.NewRender.Direktory
{
    public class MediaService : IMediaService
    {
        public async Task<bool> SaveImageFromByte(byte[] imageByte, string filename)
        {
            bool isSave = true;
            var imageData = new UIImage(NSData.FromArray(imageByte));
            imageData.SaveToPhotosAlbum((image, error) =>
            {
                //you can retrieve the saved UI Image as well if needed using  
                //var i = image as UIImage;  
                if (error != null)
                {
                    isSave = false;
                }
            });
            return isSave;
        }
    }
}
