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
using carServiceApp.My_Classes.Database;

namespace carServiceApp.My_Classes
{
    class listViewAdapter : BaseAdapter<orders>
    {
        public List<orders> mOrders = new List<orders>();
        private Context context;

        public listViewAdapter(Context Context, List<orders> m_Orders)
        {
            mOrders = m_Orders;
            context = Context;
        }

        public override orders this[int position] => mOrders[position];

        public override int Count => mOrders.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.listViewRows, null, false);
            }

                TextView brojNarudzbe  = row.FindViewById<TextView>(Resource.Id.brojNarudzbe);
                brojNarudzbe.Text      = mOrders[position].uid.ToString();
                TextView nazivAuta     = row.FindViewById<TextView>(Resource.Id.nazivAuta);
                nazivAuta.Text         = mOrders[position].carName;
                TextView datumNarudzbe = row.FindViewById<TextView>(Resource.Id.datumNarudzbe);
                datumNarudzbe.Text     = mOrders[position].datumKreiranja;

            return row;
        }
    }
}