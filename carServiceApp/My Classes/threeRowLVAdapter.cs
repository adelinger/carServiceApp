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
    class threeRowLVAdapter :BaseAdapter<string>
    {
        private List<string> items;
        private Context context;

        public threeRowLVAdapter(Context Context, List<string> Items)
        {
            items = Items;
            context = Context;
        }

        public override string this[int position] => items[position];

        public override int Count => items.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
             //   view = LayoutInflater.From(context).Inflate(Resource.Layout.customLV, null, false);
            }

           // TextView row1 = view.FindViewById<TextView>(Resource.Id.row1);
            return view;
        }
    }
}