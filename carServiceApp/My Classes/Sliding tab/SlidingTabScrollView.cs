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

        public ViewPager.IOnPageChangeListener OnPageListener
        {
            set { mViewPagerPageChangeListener = value; }
        }
        public ViewPager viewPager
        {
            set
            {
                mTabStrip.RemoveAllViews();
                mViewPager = value;
                if (value != null)
                {
                    value.PageSelected += Value_PageSelected;
                    value.PageScrollStateChanged += Value_PageScrollStateChanged;
                    value.PageScrolled += Value_PageScrolled;

                    populateTabStrip();
                }
            }
        }

        private void Value_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            int tabCount = mTabStrip.ChildCount;

            if ((tabCount == 0) || (e.Position < 0) || (e.Position >= tabCount))
            {
                //return if any of these conditions apply, don't scroll
                return;
            }
            mTabStrip.onViewPagerPageChanged(e.Position, e.PositionOffset);

            View selectedTitle = mTabStrip.GetChildAt(e.Position);
            int extraOffSet = (selectedTitle != null ? (int) (e.Position * selectedTitle.Width) :0);

            scrollToTab(e.Position, extraOffSet);

            if (mViewPagerPageChangeListener != null)
            {
                mViewPagerPageChangeListener.OnPageScrolled(e.Position, e.PositionOffset, e.PositionOffsetPixels);
            }
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (mViewPager != null)
            {
                scrollToTab(mViewPager.CurrentItem, 0);
            }
        }

        private void populateTabStrip ()
        {
            PagerAdapter adapter = mViewPager.Adapter;
            for (int i = 0; i < adapter.Count; i++)
            {
                TextView tabView = createDefaultTabView(Context);
                tabView.Text     = ((slidingTabFragment.samplePagerAdapter)adapter).getHeaderTitle(i);
                tabView.SetTextColor(Android.Graphics.Color.Black);
                tabView.Tag      = i;
                tabView.Click += TabView_Click;
                mTabStrip.AddView(tabView);
            }
        }

        private TextView createDefaultTabView(Context context)
        {
            TextView textView = new TextView(context);
            textView.Gravity = GravityFlags.Center;
            textView.SetTextSize(ComplexUnitType.Sp, TAB_VIEW_TEXT_SIZE_SP);
            textView.Typeface = Android.Graphics.Typeface.DefaultBold;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb)
            {
                TypedValue outValue = new TypedValue();
                context.Theme.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground, outValue, false);
                textView.SetBackgroundResource(outValue.ResourceId);
            }

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.IceCreamSandwich)
            {
                textView.SetAllCaps(true);
            }

            int padding = (int)(TAB_VIEW_PADDING_DIPS * Resources.DisplayMetrics.Density);
            textView.SetPadding(padding, padding, padding, padding);

            return textView;
        }

        private void TabView_Click(object sender, EventArgs e)
        {
            TextView clickTab      = (TextView)sender;
            int pageToScrollTo     = (int)clickTab.Tag;
            mViewPager.CurrentItem = pageToScrollTo;
        }

        private void scrollToTab(int tabIndex, int extraOffSet)
        {
            int tabCount = mTabStrip.ChildCount;
            
            if (tabCount == 0 || tabIndex <0 || tabIndex >= tabCount)
            {
                //don't scroll
            }

            View selectedChild = mTabStrip.GetChildAt(tabIndex);
            if (selectedChild != null)
            {
                int scrollAmountX = selectedChild.Left + extraOffSet;

                if (tabIndex > 0 || extraOffSet > 0)
                {
                    scrollAmountX -= mTitleOffset;
                }
                this.ScrollTo(scrollAmountX, 0);
            }
        }

        private void Value_PageScrollStateChanged(object sender, ViewPager.PageScrollStateChangedEventArgs e)
        {
            mScrollState = e.State;

            if (mViewPagerPageChangeListener != null)
            {
                mViewPagerPageChangeListener.OnPageScrollStateChanged(e.State);
            }
        }

        private void Value_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (mScrollState == ViewPager.ScrollStateIdle)
            {
                mTabStrip.onViewPagerPageChanged(e.Position, 0f);
                scrollToTab(e.Position, 0);
            }
            
            if (mViewPagerPageChangeListener != null)
            {
                mViewPagerPageChangeListener.OnPageSelected(e.Position);
            }
        }
    }
}