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

namespace ThienAnPingo.Service
{
    public interface ISecureStorageService 
    {
        Boolean Store(string identifier, string content);
        String Fetch(string identifier);
        Boolean Exist(string identifier);
        Boolean Remove(string identifier);     
    }
}