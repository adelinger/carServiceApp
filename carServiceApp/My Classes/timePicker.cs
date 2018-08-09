using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace carServiceApp.My_Classes
{
    public class timePicker : DialogFragment
    {
        View view;
        TimePicker time;
        Button addTime;

        private string hour;
        private string minute;

        public event EventHandler<OnTimeSelectedArgs> OnTimePickedEvent;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.timePicker, container, false);
            time = view.FindViewById<TimePicker>(Resource.Id.timePicker1);
            addTime = view.FindViewById<Button>(Resource.Id.timePickerConfirm);

            time.SetIs24HourView(Java.Lang.Boolean.True);
            time.Hour = DateTime.UtcNow.Hour;

            time.TimeChanged += Time_TimeChanged;
            addTime.Click += AddTime_Click;

            return view;
        }

        private void AddTime_Click(object sender, EventArgs e)
        {
            OnTimePickedEvent.Invoke(this, new OnTimeSelectedArgs(hour, minute));
            this.Dismiss();
        }

        private void Time_TimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            hour   = e.HourOfDay.ToString();
            minute = e.Minute.ToString();         
        }

    }

    public class OnTimeSelectedArgs
    {
        public string hourSelected { get; set; }
        public string minutesSelected { get; set; }

        public OnTimeSelectedArgs(string hour, string minute)
        {
            hourSelected = hour;
            minutesSelected = minute;
        }
    }
}