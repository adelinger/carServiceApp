using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace carServiceApp.My_Classes.Sliding_tab
{
  public class slidingTabStrip : LinearLayout
    {
        private const int DEFAULT_BOTTOM_BORDER_THICKNESS_DIPS = 2;
        private const byte DEFAULT_BOTTOM_BORDER_COLOR_ALPHA = 0X26;
        private const int SELECTED_INDICATOR_THICKNESS_DIPS = 8;
        private int[] INDICATOR_COLORS = { 0x19A319, 0x0000FC };
        private int[] DIVIDER_COLORS = { 0xC5C5C5 };

        private const int DEFAULT_DIVIDER_THICKNESS_DIPS = 1;
        private const float DEFAULT_DIVIDER_HEIGHT = 0.5f;

        //Bottom border
        private int mBottomBorderThickness;
        private Paint mBottomBorderPaint;
        private int mDefaultBottomBorderColor;

        //Indicator
        private int mSelectedIndicatorThickness;
        private Paint mSelectedIndicatorPaint;

        //Divider
        private Paint mDividerPaint;
        private float mDividerHeight;

        //Selected position and offset
        private int mSelectedPosition;
        private float mSelectionOffset;

        //Tab colorizer
        private SlidingTabScrollView.TabColorizer mCustomTabColorizer;
        private SimpleTabColorizer mDefaultTabColorizer;

        public slidingTabStrip(Context context) : this(context, null)
        {

        }
        public slidingTabStrip(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetWillNotDraw(false);

            float density = Resources.DisplayMetrics.Density;
            TypedValue outValue = new TypedValue();
            context.Theme.ResolveAttribute(Android.Resource.Attribute.ColorForeground, outValue, true);
            int themeForeGround = outValue.Data;
            mDefaultBottomBorderColor = setColorAlpha(themeForeGround, DEFAULT_BOTTOM_BORDER_COLOR_ALPHA);

            mDefaultTabColorizer = new simpleTabColorizer();
            mDefaultTabColorizer.indicatorColors = INDICATOR_COLORS;
            mDefaultTabColorizer.dividerColors = INDICATOR_COLORS;

            mBottomBorderThickness = (int)(DEFAULT_BOTTOM_BORDER_THICKNESS_DIPS * density);
            mBottomBorderPaint = new Paint();
            mBottomBorderPaint.Color = getColorFromInteger(0xC5C5C5);

            mSelectedIndicatorThickness = (int)(SELECTED_INDICATOR_THICKNESS_DIPS * density);
            mSelectedIndicatorPaint = new Paint();

            mDividerHeight = DEFAULT_DIVIDER_HEIGHT;
            mDividerPaint = new Paint();
            mDividerPaint.StrokeWidth = (int)(DEFAULT_DIVIDER_THICKNESS_DIPS * density);
        }

        public SlidingTabScrollView.tabColorizer customTabColorizer
        {
            set
            {
                mCustomTabColorizer = value;
                this.Invalidate();
            }
        }

        public int [] selectedIndicatorColors
        {
            set
            {
                mCustomTabColorizer = null;
                mDefaultTabColorizer.indicatorColors = value;
                this.Invalidate();
            }
        }

        public int [] dividerColors
        {
            set
            {
                mDefaultTabColorizer = null;
                mDefaultTabColorizer.dividerColors = null;
                this.Invalidate();
            }
        }

        private Color getColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetRedComponent(color), Color.GetBlueComponent(color));
        }

        private int setColorAlpha(int color, byte alpha)
        {
            return Color.Argb(alpha, Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        public void onViewPagerPageChanged(int position, float positionOffset)
        {
            mSelectedPosition = position;
            mSelectionOffset = positionOffset;
            this.Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            int height = Height;
            int childCount = ChildCount;
            int dividerHeightPx = (int)(Math.Min(Math.Max(0f, mDividerHeight), 1f) * height);
        }

        private class simpleTabColorizer : SlidingTabScrollView.tabColorizer
        {
            private int [] mIndicatorColors;
            private int [] mDividerColors;

            public int getIndicatorColors(int position)
            {
                return mIndicatorColors[position % mIndicatorColors.Length];
            }

            public int getDividerColors (int position)
            {
                return mDividerColors[position % mDividerColors.Length];
            }

            public int [] indicatorColors
            {
                set { mIndicatorColors = value; }
            }

            public int [] dividerColors
            {
                set { mDividerColors = value;  }
            }
        }
           
    }


}