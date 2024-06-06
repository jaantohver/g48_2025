using System;

using Android.OS;
using Android.App;
using Android.Util;
using Android.Views;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using Android.Gms.Vision;
using AndroidX.AppCompat.App;
using Android.Gms.Vision.Barcodes;

namespace ttki.Droid
{
    [Activity]
    public class QrScannerActivity : AppCompatActivity, ISurfaceHolderCallback, Detector.IProcessor
    {
        CameraSource cameraSource;
        BarcodeDetector barcodeDetector;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_qrscanner);

            SetupControls();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            cameraSource.Stop();
        }

        void SetupControls()
        {
            barcodeDetector = new BarcodeDetector.Builder(this).SetBarcodeFormats(BarcodeFormat.QrCode).Build();
            barcodeDetector.SetProcessor(this);


            cameraSource = new CameraSource.Builder(this, barcodeDetector)
                    .SetRequestedPreviewSize(1920, 1080)
                    .SetAutoFocusEnabled(true)
                    .Build();

            SurfaceView surfaceView = FindViewById<SurfaceView>(Resource.Id.cameraSurfaceView);

            surfaceView.Holder.AddCallback(this);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                //Start preview after 1s delay
                cameraSource.Start(holder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            try
            {
                cameraSource.Start(holder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }

        public void ReceiveDetections(Detector.Detections detections)
        {
            if (detections.DetectedItems.Size() == 1)
            {
                SparseArray barcodes = detections.DetectedItems;

                RunOnUiThread(delegate
                {
                    cameraSource.Stop();

                    string value = (barcodes.ValueAt(0) as Barcode).DisplayValue;

                    //Remove byte order mark
                    if (value.StartsWith("%EF%BB%BF"))
                    {
                        value = value.Remove(0, 6);
                    }

                    value = value.Trim(new char[] { '\uFEFF', '\u200B' });

                    Intent i = new Intent();
                    i.PutExtra("qr", value);

                    SetResult(Result.Ok, i);

                    Finish();
                });
            }
        }

        public void Release()
        {
        }
    }
}
