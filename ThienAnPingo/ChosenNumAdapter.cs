using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ThienAnPingo
{
    public class NumberModel : Java.Lang.Object
    {
        public NumberModel()
        {

        }
        public bool IsChosen { get; set; }
        public int Value { get; set; }
    }
    public class ChosenNumAdapter : BaseAdapter<NumberModel>
    {

        Context _context;
        IList<NumberModel> _listItem;

        public ChosenNumAdapter(Context context)
        {
            this._context = context;
            _listItem = new List<NumberModel>();
        }

        public void AddItem(int value,bool isChosen)
        {              
            _listItem.Add(new NumberModel() { IsChosen = isChosen, Value = value});
            NotifyDataSetChanged(); 
        }

        public void UpdateNumberState(int value,bool isChosen)
        {
            var item = _listItem.FirstOrDefault(p => p.Value == value);
            if (item == null)
                return;
            _listItem[_listItem.IndexOf(item)].IsChosen = isChosen;
            NotifyDataSetChanged();
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            ChosenNumAdapterViewHolder holder = null;

            if (view != null)
                holder = view.Tag as ChosenNumAdapterViewHolder;

            if (holder == null)
            {
                holder = new ChosenNumAdapterViewHolder();
                var inflater = _context.GetSystemService(Context.LayoutInflaterService).JavaCast<LayoutInflater>();
                //replace with your item and your holder items
                //comment back in
                view = inflater.Inflate(Resource.Layout.ChosenItem, parent, false);
                holder.Value = view.FindViewById<TextView>(Resource.Id.value_tv);
                holder.Background = view.FindViewById<RelativeLayout>(Resource.Id.background);

                view.Tag = holder;
            }


            //fill in your items
            //holder.Title.Text = "new text here";

            holder.Value.Text = _listItem[position].Value.ToString();

            if (_listItem[position].IsChosen)
            {
                holder.Background.SetBackgroundResource(Resource.Drawable.number_chosen);
                holder.Value.SetTextColor(Color.Black);
            }
            else
            {
                holder.Background.SetBackgroundResource(Resource.Drawable.number_not_chosen);
                holder.Value.SetTextColor(Color.White);
            }


            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return _listItem.Count;
            }
        }

        public override NumberModel this[int position] => _listItem[position];
    }

    class ChosenNumAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        public TextView Value { get; set; }
        public RelativeLayout Background { get; set; }
    }
}