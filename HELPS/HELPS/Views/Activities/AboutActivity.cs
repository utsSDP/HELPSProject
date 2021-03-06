using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content.PM;

namespace HELPS.Views.Activities
{
    // Displays the information about UTS:HELPS
    [Activity(Label = "AboutActivity", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AboutActivity : AppCompatActivity
    {
        private SupportToolbar _Toolbar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.About);

            // Set the toolbar
            _Toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_Toolbar);
            SupportActionBar.Title = GetString(Resource.String.whatHelps);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Sets the toolbar back button to act like a back press.
            OnBackPressed();
            return base.OnOptionsItemSelected(item);
        }
    }
}