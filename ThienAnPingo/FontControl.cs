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
    public static class FontControl
    {
        public static IDictionary<int, Typeface> Cache = new Dictionary<int, Typeface>();
    }
}