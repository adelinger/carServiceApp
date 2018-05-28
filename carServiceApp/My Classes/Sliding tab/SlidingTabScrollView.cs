using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace carServiceApp.My_Classes.Sliding_tab
{
   public class SlidingTabScrollView :HorizontalScrollView
    {
        private const int TITLE_OFFSET_DIPS = 24;
        private const int TAB_VIEW_TEXT_SIZE_SP = 12;
        private const int TAB_VIEW_PADDING_DIPS = 16;

        private int mTitleOffset;

        private int mTabViewLayoutID;
        private int mTabViewTextViewID;

        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mViewPagerPageChangeListener;

        public static slidingTabStrip mTabStrip;

        private int mScrollState;
        private Context context;
        private object p;

        public interface tabColorizer
        {
            int getIndicatorColors(int position);
            int getDividerColors(int position);
        }

        public SlidingTabScrollView(Context context) :this (context, null) 
        {

        }

        public SlidingTabScrollView(Context context, IAttributeSet attrs) :this (context, attrs, 0)
        {
            
        }
        public SlidingTabScrollView(Context context, IAttributeSet attrs, int defaultStyle) :base(context, attrs, defaultStyle)
        {
            HorizontalScrollBarEnabled = false;
            FillViewport = true;
            this.SetBackgroundColor(Android.Graphics.Color.Rgb(0xE4, 0xE5, 0xE5));

            mTitleOffset = (int)(TITLE_OFFSET_DIPS * Resources.DisplayMetrics.Density);
            mTabStrip = new slidingTabStrip(context);
            this.AddView(mTabStrip, LayoutParams.MatchParent, LayoutParams.MatchParent);
        }

        public tabColorizer customTabColorizer
        {
            set { mTabStrip.customTabColorizer = value; }
        }
        public int [] selectedIndicatorColor
        {
            set { mTabStrip.selectedIndicatorColors = value;  }
        }
        public int [] dividerColors
        {
            set { mTabStrip.dividerColors = value; }
        }
    }
}