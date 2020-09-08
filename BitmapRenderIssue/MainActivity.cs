using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Interop;
using System;
using static Android.Graphics.ImageDecoder;

namespace BitmapRenderIssue
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int CAPTURE_PHOTO = 100;
        private static Android.Net.Uri ImageUri;
        ImageView imageView1;
        private static Bitmap CurrImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            imageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            TakePicture();
        }

        public bool isCameraPermissionGranted()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.Camera)
                        == Permission.Granted)
                {
                    return true;
                }
                else
                {
                    RequestPermissions(new String[] { Android.Manifest.Permission.Camera }, 1);
                    return false;
                }
            }
            else
            { //permission is automatically granted on sdk<23 upon installation
                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (grantResults[0] == Permission.Granted)
            {
                CapturePhoto();
            }
        }

        public void CapturePhoto()
        {
            ImageUri = ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, new ContentValues());
            var i = new Intent(MediaStore.ActionImageCapture);
            i.PutExtra(MediaStore.ExtraOutput, ImageUri);
            StartActivityForResult(i, CAPTURE_PHOTO);
        }

        public void TakePicture()
        {

            if (isCameraPermissionGranted())
                CapturePhoto();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok && requestCode == CAPTURE_PHOTO && ImageUri != null)
            {
                Android.Util.Log.Info("CameraScanning", "Getting Picture");

                var src = ImageDecoder.CreateSource(ContentResolver, ImageUri);

                Bitmap img = ImageDecoder.DecodeBitmap(src, new OnHeaderDecodedListener());

                CurrImage = ImageProcessing.Grayscale_ColorMatrixColorFilter(img);

                img.Recycle();
                img.Dispose();

                imageView1.SetImageBitmap(CurrImage);

                Java.IO.File xFile = new Java.IO.File(ImageUri.Path);
                if (xFile.Exists())
                    xFile.Delete();
                ImageUri = null;
            }
        }

        private class OnHeaderDecodedListener : Java.Lang.Object, IOnHeaderDecodedListener
        {

            public void OnHeaderDecoded(ImageDecoder decoder, ImageInfo info, Source source)
            {
                decoder.MutableRequired = true;
            }

        }
    }
}
