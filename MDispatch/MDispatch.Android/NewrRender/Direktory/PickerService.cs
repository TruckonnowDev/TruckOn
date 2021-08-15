using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using MDispatch.Droid.NewrRender.Direktory;
using MDispatch.NewElement.Directory;
using Xamarin.Forms;

[assembly: Dependency(typeof(PickerService))]
namespace MDispatch.Droid.NewrRender.Direktory
{
    public class PickerService: IPhotoPickerService
    {
        public Task<Stream> GetImageStreamAsync()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            MainActivity.mainActivity.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Photo"),
                MainActivity.PickImageId);
            MainActivity.mainActivity.PickImageTaskCompletionSource = new TaskCompletionSource<Stream>();
            return MainActivity.mainActivity.PickImageTaskCompletionSource.Task;
        }
    }
}
