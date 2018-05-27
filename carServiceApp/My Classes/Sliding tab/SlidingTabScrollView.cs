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

namespace carServiceApp.My_Classes.Sliding_tab
{
   public class SlidingTabScrollView
    {
        public interface tabColorizer
        {
            int getIndicatorColors(int position);
            int getDividerColors(int position);
        }
    }
}