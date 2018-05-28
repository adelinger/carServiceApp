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
        private SlidingTabScrollView.tabColorizer mCustomTabColorizer;
        private simpleTabColorizer mDefaultTabColorizer;

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
            get
            {
                return null;
            }
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
            int tabCount = ChildCount;
            int dividerHeightPx = (int)(Math.Min(Math.Max(0f, mDividerHeight), 1f) * height);
            SlidingTabScrollView.tabColorizer tabColorizer = customTabColorizer != null ? mCustomTabColorizer : mDefaultTabColorizer;

            if (tabCount > 0)
            {
                View selectedTitle = GetChildAt(mSelectedPosition);
                int left = selectedTitle.Left;
                int right = selectedTitle.Right;
                int color = tabColorizer.getIndicatorColors(mSelectedPosition);

                if (mSelectionOffset > 0f && mSelectedPosition < (tabCount-1))
                {
                    int nextColor = tabColorizer.getIndicatorColors(mSelectedPosition + 1);
                    if (color != nextColor)
                    {
                        color = blendColor(nextColor, color, mSelectionOffset);

                    }

                    View nextTitle = GetChildAt(mSelectedPosition + 1);
                    left = (int)(mSelectionOffset * nextTitle.Left + (1.0f - mSelectionOffset) * left);
                    right = (int)(mSelectionOffset * nextTitle.Right +  (1.0f - mSelectionOffset) * right);
                }

                mSelectedIndicatorPaint.Color = getColorFromInteger(color);
                canvas.DrawRect(left, height - mSelectedIndicatorThickness, right, height, mSelectedIndicatorPaint);

                int separatorTop = (height - dividerHeightPx) / 2;
                for (int i = 0; i < ChildCount; i++)
                {
                    View child = GetChildAt(i);
                    mDividerPaint.Color = getColorFromInteger(tabColorizer.getDividerColors(i));
                    canvas.DrawLine(child.Right, separatorTop, child.Right, separatorTop + dividerHeightPx, mDividerPaint);
                }

                canvas.DrawRect(0, height - mBottomBorderThickness, Width, height, mBottomBorderPaint);
            }
        }

        private int blendColor(int color1, int color2, float ratio)
        {
            float inverseRatio = 1f - ratio;
            float r = (Color.GetRedComponent(color1) * ratio) + (Color.GetRedComponent(color2) * inverseRatio);
            float g = (Color.GetGreenComponent(color1) * ratio) + (Color.GetRedComponent(color2) * inverseRatio);
            float b = (Color.GetBlueComponent(color1) * ratio) + (Color.GetRedComponent(color2) * inverseRatio);

            return Color.Rgb((int)r, (int)g, (int)b);
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