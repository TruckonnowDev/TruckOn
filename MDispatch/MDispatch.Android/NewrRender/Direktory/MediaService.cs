using System;
using System.Threading.Tasks;
using Android.Content;
using MDispatch.Droid.NewrRender.Direktory;
using MDispatch.NewElement.Directory;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(MediaService))]
namespace MDispatch.Droid.NewrRender.Direktory
{
    public class MediaService : IMediaService
    {
        Context CurrentContext => CrossCurrentActivity.Current.Activity;
        public async Task<bool> SaveImageFromByte(byte[] imageByte, string filename)
        {
            bool isSave = true;
            try
            {
                Java.IO.File storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                string path = System.IO.Path.Combine(storagePath.ToString(), filename);
                System.IO.File.WriteAllBytes(path, imageByte);
                var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
                CurrentContext.SendBroadcast(mediaScanIntent);
            }
            catch (Exception ex)
            {
                isSave = false;
            }
            return isSave;
        }
    }
}
