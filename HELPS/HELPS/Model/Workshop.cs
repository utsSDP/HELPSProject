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

namespace HELPS.Model
{
    class Workshop
    {
        
        public int WorkshopId { get; set; }
        public string topic { get; set; }
        public string description { get; set; }
        public string targetingGroup { get; set; }
        public string campus { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> maximum { get; set; }
        public Nullable<int> WorkShopSetID { get; set; }
        public Nullable<int> cutoff { get; set; }
        public string type { get; set; }
        public Nullable<int> reminder_num { get; set; }
        public Nullable<int> reminder_sent { get; set; }
        public string DaysOfWeek { get; set; }
        public Nullable<int> BookingCount { get; set; }
        public Nullable<System.DateTime> archived { get; set; }
    }
}