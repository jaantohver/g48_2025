using System;
using System.Linq;

using UIKit;
using Foundation;
using AVFoundation;
using CoreFoundation;

namespace door.iOS
{
    class QRScanController : UIViewController, IAVCaptureMetadataOutputObjectsDelegate
    {
        AVCaptureSession captureSession;
        AVCaptureVideoPreviewLayer previewLayer;

        public event EventHandler<string> QrFound;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Black;

            captureSession = new AVCaptureSession();

            AVCaptureDevice videoCaptureDevice = AVCaptureDevice.GetDefaultDevice(AVCaptureDeviceType.BuiltInWideAngleCamera, AVMediaTypes.Video, AVCaptureDevicePosition.Back);

            if (videoCaptureDevice == null)
            {
                Failed();
                return;
            }

            AVCaptureDeviceInput videoInput = AVCaptureDeviceInput.FromDevice(videoCaptureDevice);

            if (captureSession.CanAddInput(videoInput))
            {
                captureSession.AddInput(videoInput);
            }
            else
            {
                Failed();
                return;
            }

            AVCaptureMetadataOutput metadataOutput = new AVCaptureMetadataOutput();

            if (captureSession.CanAddOutput(metadataOutput))
            {
                captureSession.AddOutput(metadataOutput);
                metadataOutput.SetDelegate(this, DispatchQueue.MainQueue);
                metadataOutput.MetadataObjectTypes = AVMetadataObjectType.QRCode;
            }
            else
            {
                Failed();
                return;
            }

            previewLayer = new AVCaptureVideoPreviewLayer(captureSession);
            previewLayer.Frame = View.Layer.Bounds;
            previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            View.Layer.AddSublayer(previewLayer);

            captureSession.StartRunning();
        }

        public override void ViewWillAppear(bool animated) {
            base.ViewWillAppear(animated);

            if (captureSession != null && !captureSession.Running)
            {
                captureSession.StartRunning();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            if (captureSession != null && captureSession.Running)
            {
                captureSession.StopRunning();
            }
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.Portrait;
        }

        void Failed()
        {
            UIAlertController ac = UIAlertController.Create("Scanning not supported", "Your device does not support scanning a code from an item. Please use a device with a camera.", UIAlertControllerStyle.Alert);
            ac.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(ac, true, null);
            captureSession = null;
        }

        [Export("captureOutput:didOutputMetadataObjects:fromConnection:")]
        public void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            captureSession.StopRunning();

            foreach (AVMetadataObject o in metadataObjects)
            {
                if (o is AVMetadataMachineReadableCodeObject)
                {
                    if (!string.IsNullOrWhiteSpace((o as AVMetadataMachineReadableCodeObject).StringValue))
                    {
                        var haptic = new UINotificationFeedbackGenerator();
                        haptic.Prepare();
                        haptic.NotificationOccurred(UINotificationFeedbackType.Success);
                        haptic.Dispose();

                        Found((o as AVMetadataMachineReadableCodeObject).StringValue);

                        DismissViewController(true, null);

                        break;
                    }
                }
            }
        }

        void Found(string code)
        {
            QrFound?.Invoke(this, code);
        }
    }
}
