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
using AlertDialog = Android.Support.V7.App.AlertDialog;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using HELPS.Model;
using Newtonsoft.Json;
using HELPS.Controllers;
using Android.Util;

namespace HELPS.Views.Activities
{
    [Activity(Label = "BookingDetailActivity")]
    public class BookingDetailActivity : AppCompatActivity
    {
        private SupportToolbar _Toolbar;
        private LinearLayout _Booked;
        private LinearLayout _NotBooked;
        private Booking _Booking;
        private Workshop _Workshop;
        private WorkshopBooking _WorkshopBooking;
        //private string studentId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Check request type and set appropriate variables
            SetVariables();

            // Set our view from the "BookingDetail" layout resource.
            SetContentView(Resource.Layout.BookingDetail);

            // Set the toolbar
            _Toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_Toolbar);

            // {Architecture} Pass the workshop title as the toolbar title
            // SupportActionBar.Title(<pass here>)

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _Booked = FindViewById<LinearLayout>(Resource.Id.bookedButtons);
            _Booked.Visibility = ViewStates.Gone;

            _NotBooked = FindViewById<LinearLayout>(Resource.Id.notBookedButtons);
            _NotBooked.Visibility = ViewStates.Gone;

            // Send an intent that tells the activity if the session is booked by the student or not.
            if (_Booking != null)
            {
                SetBookingView();
            }
            else if(_WorkshopBooking !=null)
            {
                SetBookingView();
            }
            else // _worshop != null
            {
                SetWorkshopView();
            }
        }

        private void SetWorkshopView()
        {
            // Display the booking button
            _NotBooked.Visibility = ViewStates.Visible;
            Button bookButton = FindViewById<Button>(Resource.Id.buttonBook);
            bookButton.Click += delegate
            {
                Book();
                Server.workshopBookingsAltered = true;
                Finish();
            };
        }

        private void SetBookingView()
        {
            _Booked.Visibility = ViewStates.Visible;

            // Set up notification button
            Button changeNotificationButton = FindViewById<Button>(Resource.Id.buttonChangeNotification);
            changeNotificationButton.Click += delegate
            {
                DisplayNotificationSettings();
            };

            //Set up cancel button
            Button cancelButton = FindViewById<Button>(Resource.Id.buttonCancelBooking);
            cancelButton.Click += delegate
            {
                CancelBooking();
            };
        }

        private void SetVariables()
        {
            //studentId = Intent.GetStringExtra("studentId");
            string requestType = Intent.GetStringExtra("requestType");

            if (requestType == "showAvailableWorkshop")
            {
                _Workshop = JsonConvert.DeserializeObject<Workshop>(Intent.GetStringExtra("workshop"));
                
            }
               
            else // requestType == "showBooking"
            {
                string bookingType = Intent.GetStringExtra("bookingType");
                string bookingString = Intent.GetStringExtra("booking");

                if (bookingType == "Session")
                {
                    SessionBooking sessionBooking = JsonConvert.DeserializeObject<SessionBooking>(bookingString);
                    _Booking = sessionBooking;
                }
                else // bookingType == "Workshop"
                {
                    WorkshopBooking workshopBooking = JsonConvert.DeserializeObject<WorkshopBooking>(bookingString);
                    _WorkshopBooking = workshopBooking;
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            OnBackPressed();
            return base.OnOptionsItemSelected(item);
        }

        // Sends the user to the notification settings
        private void DisplayNotificationSettings()
        {
            // Implement method
            // Send the booking to the notification activity to change the notification for the specific booking
        }

        // Warns user that they will delete this booking and removes the booking if user presses okay
        private void CancelBooking()
        {
            AlertDialog.Builder cancelAlert = new AlertDialog.Builder(this);

            cancelAlert.SetTitle(GetString(Resource.String.cancelBooking));
            cancelAlert.SetMessage(GetString(Resource.String.areYouSureCancel));
            cancelAlert.SetPositiveButton("YES", delegate
            {
                // Code to cancel booking.
                WorkshopController workshopController = new WorkshopController();

                if(!workshopController.CancelBooking(_WorkshopBooking.workshopID.ToString()))
                {
                    //show error, stay on page;
                }

                else
                {
                    //show dialog saying cancled
                    var SuccesDialog = new AlertDialog.Builder(this);
                    SuccesDialog.SetMessage("Booking has been Cancled!");
                    SuccesDialog.SetNeutralButton("OK", delegate { });
                    SuccesDialog.Show();
                }

            });
            cancelAlert.SetNegativeButton("NO", delegate { });
            cancelAlert.Show();
        }

        private void Book()
        {
            // Code to book the workshop
            WorkshopController workshopController = new WorkshopController();
            if (!workshopController.Book(_Workshop.WorkshopId))
            {
                //show error , stay on page;
            }
            else
            { //show dialog saying booked and return}
            }
        }
    }
}