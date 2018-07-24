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

namespace carServiceApp.My_Classes
{
    public class onDatePickedEventArgs
    {
        public string datePicked { get; set; }

        public onDatePickedEventArgs(string DatePicked)
        {
            datePicked = DatePicked;
        }
    }

    class calendarView:DialogFragment
    {
        private View view;
        private CalendarView mCalendar;
        private Button saveButton;

        private string datePicked;

        public event EventHandler<onDatePickedEventArgs> onDatePickedEvent;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view       = inflater.Inflate(Resource.Layout.calendar, container, false);
            mCalendar  = view.FindViewById<CalendarView>(Resource.Id.calendarView1);
            saveButton = view.FindViewById<Button>(Resource.Id.calendarConfirm);

           // mCalendar.MinDate = DateTime.Now.ToFileTime();

            saveButton.Click += SaveButton_Click;
            mCalendar.DateChange += MCalendar_DateChange;

            return view;
        }

        private void MCalendar_DateChange(object sender, CalendarView.DateChangeEventArgs e)
        {
            int mjesec = e.Month + 1;
            datePicked = e.DayOfMonth + "/" + mjesec + "/" + e.Year;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(datePicked))
            {
                datePicked = DateTime.UtcNow.ToString("dd/M/yyyy");
            }
            onDatePickedEvent.Invoke(this, new onDatePickedEventArgs(datePicked));
            this.Dismiss();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

    }
}