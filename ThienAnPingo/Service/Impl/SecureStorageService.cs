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
using Plugin.CurrentActivity;
using Xamarin.Auth;

namespace ThienAnPingo.Service.Impl
{
    public class SecureStorageService : ISecureStorageService
    {
        private AccountStore storage;
        public SecureStorageService()
        {
            storage = AccountStore.Create(CrossCurrentActivity.Current.Activity, "ThienAnPingo.2018@#!$");

        }
        public  bool Exist(string identifier)
        {
            var account = storage.FindAccountsForService(Constants.APP_NAME).FirstOrDefault();
            if (account == null || !account.Properties.ContainsKey(identifier))
                return false;
            return true;

           // return (account == null ? false : account.Properties[identifier] != null);
        }

        public  string Fetch(string identifier)
        {
            string value = null;
            var account = storage.FindAccountsForService(Constants.APP_NAME).FirstOrDefault();
            if (account != null)
            {
                account.Properties.TryGetValue(identifier, out value);
            }

            return value;
        }

        public  bool Store(string identifier, string content)
        {
            var account = storage.FindAccountsForService(Constants.APP_NAME).FirstOrDefault();

            if (account == null)
            {
                account = new Account();
            }

            if (account.Properties.ContainsKey(identifier))
            {
                account.Properties.Remove(identifier);
            }

            account.Properties.Add(identifier, content);

            storage.Save(account, Constants.APP_NAME);

            return true;
        }

        public  bool Remove(string identifier)
        {
            var account = storage.FindAccountsForService(Constants.APP_NAME).FirstOrDefault();

            if (account != null)
            {
                account.Properties.Remove(identifier);

                if (account.Properties.Any())
                {
                    storage.Save(account, Constants.APP_NAME);
                }
                else
                {
                    storage.Delete(account, Constants.APP_NAME);
                }
            }

            return true;
        }
    }
}