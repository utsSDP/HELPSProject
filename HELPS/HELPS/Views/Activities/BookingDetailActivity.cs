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
using System.Threading.Tasks;
using Android.Content.PM;

namespace HELPS.Views.Activities
{
    [Activity(Label = "BookingDetailActivity", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class BookingDetailActivity : AppCompatActivity
    {
        private SupportToolbar _Toolbar;
        private LinearLayout _Booked, _NotBooked, _DisplayBar;
        private TextView _Title, _Date, _Location, _Tutor, _Type, _Description;
        private ProgressBar _Full;
        private Booking _Booking;
        private Workshop _Workshop;
        private string bookingType;
        private ProgressDialog progressDialog;

        //private WorkshopBooking _WorkshopBooking;
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

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            // Set up the TextViews
            _Title = FindViewById<TextView>(Resource.Id.detailTitle);
            _Date = FindViewById<TextView>(Resource.Id.detailDate);
            _Location = FindViewById<TextView>(Resource.Id.detailLocation);
            _Tutor = FindViewById<TextView>(Resource.Id.detailTutor);
            _Type = FindViewById<TextView>(Resource.Id.detailType);
            _Description = FindViewById<TextView>(Resource.Id.detailDescription);

            _Full = FindViewById<ProgressBar>(Resource.Id.fullBar);

            _DisplayBar = FindViewById<LinearLayout>(Resource.Id.filledLayout);
            _DisplayBar.Visibility = ViewStates.Gone;

            _Booked = FindViewById<LinearLayout>(Resource.Id.bookedButtons);
            _Booked.Visibility = ViewStates.Gone;

            _NotBooked = FindViewById<LinearLayout>(Resource.Id.notBookedButtons);
            _NotBooked.Visibility = ViewStates.Gone;

            // Send an intent that tells the activity if the session is booked by the student or not.
            if (_Booking != null)
            {
                SetBookingView();
            }
            //else if(_WorkshopBooking !=null)
            //{
              //  SetBookingView();
            //}
            else // _workshop != null
            {
                SetWorkshopView();
            }
        }

        private void SetWorkshopView()
        {
            // Display the booking button
            _NotBooked.Visibility = ViewStates.Visible;

            // Display the filled bar
            _DisplayBar.Visibility = ViewStates.Visible;
            _Full.Max = _Workshop.maximum - 1;
            _Full.Progress = _Workshop.BookingCount;

            // Populate the TextViews
            SupportActionBar.Title = "Book Workshop";
            _Title.Text = _Workshop.Topic();
            DateTime? date = _Workshop.Date();
            _Date.Text = "Date: " + ((date == null) ? "Not available" : date.ToString());
            _Location.Text = "Location: " + _Workshop.Location();
            _Tutor.Text = "Tutor: " + _Workshop.Tutor();
            _Type.Text = "Type: " + _Workshop.Type();
            _Description.Text = _Workshop.Description();

            Button bookButton = FindViewById<Button>(Resource.Id.buttonBook);
            Button waitlistButton = FindViewById<Button>(Resource.Id.buttonWaitlist);

            bookButton.Click += async delegate
            {
                ShowProgressDialog(progressDialog, "Booking. Please wait...", true);
                await Book();
                Server.workshopBookingsAltered = true;

                ShowProgressDialog(progressDialog, "Booking. Please wait...", false);
            };

            waitlistButton.Click += async delegate
            {
                ShowProgressDialog(progressDialog, "Waitlisting. Please wait...", true);
                await Waitlist();
                Server.workshopBookingsAltered = true;

                ShowProgressDialog(progressDialog, "Waitlisting. Please wait...", false);
            };

            if (_Workshop.BookingCount < _Workshop.cutoff)
                waitlistButton.Visibility = ViewStates.Gone;
            else
                bookButton.Visibility = ViewStates.Gone;
        }

        private void ShowProgressDialog(ProgressDialog progressDialog, string message, bool show)
        {
            if (show)
            {
                progressDialog.Indeterminate = true;
                progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                progressDialog.SetMessage(message);
                progressDialog.SetCancelable(false);
                progressDialog.Show();
            }
            else
                progressDialog.Hide();
        }

        private async Task Waitlist()
        {
            WorkshopController workshopController = new WorkshopController();
            await workshopController.Waitlist(_Workshop.WorkshopId);

            //show dialog saying canceled
            var SuccesDialog = new AlertDialog.Builder(this);
            SuccesDialog.SetMessage("You have successfully been waitlisted!");
            SuccesDialog.SetNeutralButton("OK", delegate { Finish(); });
            SuccesDialog.Show();
        }

        private void SetBookingView()
        {
            _Booked.Visibility = ViewStates.Visible;

            // Set up the title
            SupportActionBar.Title = "View Booking";
            _Booking.Title();
            // Populate TextViews
            _Title.Text = _Booking.Topic();
            DateTime? date = _Booking.Date();
            _Date.Text = "Date: " + ((date == null) ? "Not available" : date.ToString());
            _Location.Text = "Location: " + _Booking.Location();
            _Tutor.Text = "Tutor: " + _Booking.Tutor();
            _Type.Text = "For: " + _Booking.Type();
            _Description.Text = _Booking.Description();

            Button cancelButton = FindViewById<Button>(Resource.Id.buttonCancelBooking);
            cancelButton.Click += delegate
            {
                CancelBooking();

            };
            if (date < DateTime.Now)
                cancelButton.Visibility = ViewStates.Gone;
        }

        private void SetVariables()
        {
            progressDialog = new ProgressDialog(this);

            string requestType = Intent.GetStringExtra("requestType");

            if (requestType == "showAvailableWorkshop")
            {
                _Workshop = JsonConvert.DeserializeObject<Workshop>(Intent.GetStringExtra("workshop"));
                
            }
               
            else // requestType == "showBooking"
            {
                bookingType = Intent.GetStringExtra("bookingType");
                string bookingString = Intent.GetStringExtra("booking");

                if (bookingType == "Session")
                {
                    SessionBooking sessionBooking = JsonConvert.DeserializeObject<SessionBooking>(bookingString);
                    _Booking = sessionBooking;
                }
                else // bookingType == "Workshop"
                {
                    WorkshopBooking workshopBooking = JsonConvert.DeserializeObject<WorkshopBooking>(bookingString);
                    _Booking = workshopBooking;
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

                if(_Booking.GetType() == typeof(SessionBooking))
                {
                    SessionController sessionController = new SessionController();

                    if (!sessionController.CancelSession(_Booking.ID()))
                    {
                        //show error, stay on page
                    }

                    else
                    {
                        //show dialog saying canceled
                        var SuccesDialog = new AlertDialog.Builder(this);
                        SuccesDialog.SetMessage("Booking has been Canceled!");
                        SuccesDialog.SetNeutralButton("OK", delegate { Finish(); });
                        SuccesDialog.Show();

                        if (bookingType.Equals("Session"))
                            Server.sessionBookingsAltered = true;
                        else
                            Server.workshopBookingsAltered = true;
                    }
                }

                else if (_Booking.GetType() == typeof(WorkshopBooking))
                {
                    // Code to cancel booking.
                    WorkshopController workshopController = new WorkshopController();

                    if (!workshopController.CancelBooking(_Booking.ID()))
                    {
                        //show error, stay on page;
                    }

                    else
                    {
                        //show dialog saying cancled
                        var SuccesDialog = new AlertDialog.Builder(this);
                        SuccesDialog.SetMessage("Booking has been Canceled!");
                        SuccesDialog.SetNeutralButton("OK", delegate { Finish(); });
                        SuccesDialog.Show();

                        if (bookingType.Equals("Session"))
                            Server.sessionBookingsAltered = true;
                        else
                            Server.workshopBookingsAltered = true;
                    }

                }

            });
            cancelAlert.SetNegativeButton("NO", delegate { });
            cancelAlert.Show();
        }

        private async Task Book()
        {
            // Code to book the workshop
            WorkshopController workshopController = new WorkshopController();
            if (! await workshopController.Book(_Workshop.WorkshopId))
            {
                //show error , stay on page;
            }
            else
            {
                //show dialog saying booked
                var SuccesDialog = new AlertDialog.Builder(this);
                SuccesDialog.SetMessage("You have successfully been booked!");
                SuccesDialog.SetNeutralButton("OK", delegate { Finish(); });
                SuccesDialog.Show();
            }
        }
    }
}