using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using MDispatch.iOS.NewRender.CustomCamera;
using MDispatch.NewElement;
using UIKit;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CameraPage), typeof(CameraPageRender))]
namespace MDispatch.iOS.NewRender.CustomCamera
{
    public class CameraPageRender : PageRenderer
    {
        AVCaptureSession captureSession;
        AVCaptureDeviceInput captureDeviceInput;
        AVCaptureStillImageOutput stillImageOutput;
        AVCaptureVideoPreviewLayer videoPreviewLayer = null;
        UIButton takePhotoButton;
        UIButton takePhotoIspectionButton;
        UIButton takePhotoIspectionButton1;
        UIButton scanPhotoButton;
        UIView liveCameraStream;
        private Timer timer = null;
        private bool isTake = false;

        public CameraPageRender()
        {
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);
            ReSetOrientation(toInterfaceOrientation);
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupUserInterface();
            SetupEventHandlers();
            await AuthorizeCameraUse();
            SetupLiveCameraStream();
            timer = new Timer(new TimerCallback(WaiteBtn), null, 1500, Timeout.Infinite);
            ReSetOrientation(UIInterfaceOrientation.LandscapeRight);
        }

        private void WaiteBtn(object state)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    if(Element == null)
                    {
                        return;
                    }
                    if ((((CameraPage)Element).TypeCamera == null || ((CameraPage)Element).TypeCamera == "Photo"))
                    {
                        View.Add(takePhotoButton);
                    }
                    else if (((CameraPage)Element).TypeCamera == "DetectText")
                    {
                        View.Add(scanPhotoButton);
                    }
                    else if (((CameraPage)Element).TypeCamera == "PhotoIspection")
                    {
                        View.Add(takePhotoIspectionButton);
                        View.Add(takePhotoIspectionButton1);
                    }
                    else
                    {
                        View.Add(takePhotoButton);
                    }
                }
                catch { }
            });
        }

        private async Task AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        private void SetupLiveCameraStream()
        {
            captureSession = new AVCaptureSession();
            videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = UIScreen.MainScreen.Bounds,
                Orientation = GetCameraForOrientation()
            };
            liveCameraStream.Layer.AddSublayer(videoPreviewLayer);
            videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            ConfigureCameraForDevice(captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
            var dictionary = new NSMutableDictionary();
            dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
            stillImageOutput = new AVCaptureStillImageOutput()
            {
                OutputSettings = new NSDictionary(),
            };
            captureSession.AddOutput(stillImageOutput);
            captureSession.AddInput(captureDeviceInput);
            stillImageOutput.ConnectionFromMediaType(AVMediaType.Video).VideoOrientation = GetCameraForOrientation();
            captureSession.StartRunning();
        }

        public async Task<NSData> CapturePhoto()
        {
            CMSampleBuffer sampleBuffer = null;
            NSData jpegImageAsNsData = null;
            AVCaptureConnection videoConnection = null;
            try
            {
                videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
                sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);
                jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
            }
            catch (Exception e)
            {
                jpegImageAsNsData = null;
            }
            return jpegImageAsNsData;
        }

        public AVCaptureVideoOrientation GetCameraForOrientation()
        {
            var currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
            AVCaptureVideoOrientation aVCaptureVideoOrientation = AVCaptureVideoOrientation.Portrait;

            if (currentOrientation == UIInterfaceOrientation.Portrait)
            {
                aVCaptureVideoOrientation = AVCaptureVideoOrientation.Portrait;
            }
            else if (currentOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                aVCaptureVideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
            }
            return aVCaptureVideoOrientation;
        }

        public AVCaptureVideoOrientation GetCameraForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            AVCaptureVideoOrientation aVCaptureVideoOrientation = AVCaptureVideoOrientation.Portrait;

            if (toInterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                aVCaptureVideoOrientation = AVCaptureVideoOrientation.Portrait;
            }
            else if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                aVCaptureVideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
            }
            return aVCaptureVideoOrientation;
        }


        private void SetupEventHandlers()
        {
            takePhotoButton.TouchUpInside += async (s, e) =>
            {
                UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    takePhotoButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                }, () =>
                {
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                    {
                        takePhotoButton.Transform = CGAffineTransform.MakeScale(1f, 1f);
                    }, null);
                });
                if (!isTake)
                {
                    takePhotoButton.Enabled = false;
                    isTake = true;
                    var data = await CapturePhoto();
                    if (data != null)
                    {
                        UIImage originalImage = ImageFromByteArray(data.ToArray());
                        (Element as CameraPage).SetPhotoResult(originalImage.AsJPEG(.6f).ToArray(), (int)originalImage.Size.Width, (int)originalImage.Size.Height);
                    }
                    isTake = false;
                    takePhotoButton.Enabled = true;
                }
            };
            takePhotoIspectionButton1.TouchUpInside += async (s, e) =>
            {
                UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    takePhotoIspectionButton1.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                }, () =>
                {
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                    {
                        takePhotoIspectionButton1.Transform = CGAffineTransform.MakeScale(1f, 1f);
                    }, null);
                });
                if (!isTake)
                {
                    takePhotoIspectionButton1.Enabled = false;
                    isTake = true;
                    var data = await CapturePhoto();
                    if (data != null)
                    {
                        UIImage originalImage = ImageFromByteArray(data.ToArray());
                        (Element as CameraPage).SetPhotoResult(originalImage.AsJPEG(.6f).ToArray(), (int)originalImage.Size.Width, (int)originalImage.Size.Height);
                    }
                    isTake = false;
                    takePhotoIspectionButton1.Enabled = true;
                }
            };
            takePhotoIspectionButton.TouchUpInside += async (s, e) =>
            {
                UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    takePhotoIspectionButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                }, () =>
                {
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                    {
                        takePhotoIspectionButton.Transform = CGAffineTransform.MakeScale(1f, 1f);
                    }, null);
                });
                if (!isTake)
                {
                    takePhotoButton.Enabled = false;
                    isTake = true;
                    var data = await CapturePhoto();
                    if (data != null)
                    {
                        UIImage originalImage = ImageFromByteArray(data.ToArray());
                        (Element as CameraPage).SetPhotoinspectionResult(originalImage.AsJPEG(.6f).ToArray(), (int)originalImage.Size.Width, (int)originalImage.Size.Height);
                    }
                    isTake = false;
                    takePhotoButton.Enabled = true;
                }
            };
            scanPhotoButton.TouchUpInside += async (s, e) =>
            {
                UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    scanPhotoButton.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
                }, () =>
                {
                    UIView.Animate(0.2, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                    {
                        scanPhotoButton.Transform = CGAffineTransform.MakeScale(1f, 1f);
                    }, null);
                });
                if (!isTake)
                {
                    scanPhotoButton.Enabled = false;
                    isTake = true;
                    var data = await CapturePhoto();
                    if (data != null)
                    {
                        UIImage originalImage = ImageFromByteArray(data.ToArray());
                        (Element as CameraPage).SetScan(originalImage.AsJPEG(.7f).ToArray(), (int)originalImage.Size.Width, (int)originalImage.Size.Height);
                    }
                    isTake = false;
                    scanPhotoButton.Enabled = true;
                }
            };
        }

        public static byte[] ResizeImageIOS(UIImage originalImage, float width, float height)
        {
            UIImageOrientation orientation = originalImage.Orientation;
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                                                 (int)width, (int)height, 8,
                                                 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
                                                 CGImageAlphaInfo.PremultipliedFirst))
            {
                //RectangleF imageRect = new RectangleF(0, 0, width, height);
                //context.DrawImage(imageRect, originalImage.CGImage);
                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);
                return resizedImage.AsJPEG().ToArray();
            }
        }

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                return null;
            }
            return image;
        }

        private void ReSetOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            liveCameraStream.Frame = new CGRect(0f, 0f, View.Bounds.Width, View.Bounds.Height);
            videoPreviewLayer.Frame = UIScreen.MainScreen.Bounds;
            videoPreviewLayer.Connection.VideoOrientation = GetCameraForOrientation();
            videoPreviewLayer.Orientation = GetCameraForOrientation(toInterfaceOrientation);
            takePhotoButton.Frame = new CGRect(View.Bounds.Right - 124, (View.Bounds.Bottom / 2) - 50, 100, 100);
            stillImageOutput.ConnectionFromMediaType(AVMediaType.Video).VideoOrientation = GetCameraForOrientation();
        }

        private void SetupUserInterface()
        {
            liveCameraStream = new UIView()
            {
                Frame = UIScreen.MainScreen.Bounds
            };
            takePhotoButton = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(View.Bounds.Right - 124, (View.Bounds.Bottom / 2) - 50, 100, 100)
            };
            takePhotoButton.SetBackgroundImage(UIImage.FromBundle("Take.png"), UIControlState.Normal);

            takePhotoIspectionButton1 = new UIButton(UIButtonType.Custom)
            {
                 Frame = new CGRect(View.Bounds.Right - 104, View.Bounds.Bottom - 84, 60, 60)
            };
            takePhotoIspectionButton1.SetBackgroundImage(UIImage.FromBundle("NotDamage.png"), UIControlState.Normal);

            takePhotoIspectionButton = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(View.Bounds.Right - 124, (View.Bounds.Bottom / 2) - 50, 100, 100)
            };
            takePhotoIspectionButton.SetBackgroundImage(UIImage.FromBundle("Take.png"), UIControlState.Normal);

            scanPhotoButton = new UIButton()
            {
                Frame = new CGRect(View.Bounds.Right - 124, (View.Bounds.Bottom / 2) - 50, 100, 100),
            };
            scanPhotoButton.SetBackgroundImage(UIImage.FromBundle("scanPlate.png"), UIControlState.Normal);
            View.InsertSubview(liveCameraStream, 0);
        }

        public void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsFocusModeSupported(AVCaptureFocusMode.AutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.AutoFocus;
                device.UnlockForConfiguration();
            }
            if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.AutoExpose))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.AutoExpose;
                device.UnlockForConfiguration();
            }
            if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.AutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.AutoWhiteBalance;
                device.UnlockForConfiguration();
            }
        }
    }
}
