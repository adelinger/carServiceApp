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
using Java.Lang;

namespace carServiceApp.My_Classes.Sliding_tab
{
    public class slidingTabFragment : Fragment
    {
        private SlidingTabScrollView slidingTabScrollView;
        private ViewPager viewPager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            slidingTabScrollView = view.FindViewById<SlidingTabScrollView>(Resource.Id.sliding_tabs);
            viewPager            = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.Adapter = new samplePagerAdapter();
            slidingTabScrollView.viewPager= viewPager;
        }

       public class samplePagerAdapter :PagerAdapter
        {
            List<string> items = new List<string>();

            public samplePagerAdapter() :base()
            {
                items.Add("one");
                items.Add("two");
                items.Add("three");
            }

            public override int Count
            {
                get { return items.Count; }
            }

            public override bool IsViewFromObject(View view, Java.Lang.Object obj)
            {
                return view == obj;
            }
            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                if (position == 0)
                {
                    View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.carDetails, container, false);
                    container.AddView(view);



                    return view;
                }
                return null;
            }

            public string getHeaderTitle (int position)
            {
                return items[position];
            }

            public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
            {
                container.RemoveView((View)obj);
            }
        }
    }

    
}
