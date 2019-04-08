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
using ThienAnPingo.DI;
using ThienAnPingo.Service;
using ThienAnPingo.Service.Impl;

namespace ThienAnPingo
{
    public class ModelHelper
    {
        public  ISecureStorageService StorageService => ServiceLocator.Instance.Get<ISecureStorageService>();

        static Lazy<ModelHelper> _instance = new Lazy<ModelHelper>();
        public static ModelHelper Instance => _instance.Value;

        public List<int> ChosenNumbers { get; set; } = new List<int>();
        public int MaxNumber
        {
            get
            {
                return int.Parse( StorageService.Fetch(Constants.MAX_NUMBER));
            }
            set
            {
                StorageService.Store(Constants.MAX_NUMBER, value.ToString());
            }
        }

        public int Turns { get => ChosenCounting * 4; }

        public int MinNumber
        {
            get
            {
                return int.Parse(StorageService.Fetch(Constants.MIN_NUMBER));
            }
            set
            {
                StorageService.Store(Constants.MIN_NUMBER, value.ToString());
            }
        }

        public int ChosenCounting
        {
            get
            {
                return int.Parse(StorageService.Fetch(Constants.CHOSEN_COUNTING));
            }
            set
            {
                StorageService.Store(Constants.CHOSEN_COUNTING, value.ToString());
            }
        }

        public bool IsSetted
        {
            get
            {
                if (!StorageService.Exist(Constants.MIN_NUMBER) || !StorageService.Exist(Constants.MAX_NUMBER) || !StorageService.Exist(Constants.CHOSEN_COUNTING))
                    return false;
                else
                    return true;
            }
           
        }

        public Dictionary<int, List<int>> LeftSlideNums { get; set; } = new Dictionary<int, List<int>>();
        public Dictionary<int, List<int>> CenterSlideNums { get; set; } = new Dictionary<int, List<int>>();
        public Dictionary<int, List<int>> RightSlideNums { get; set; } = new Dictionary<int, List<int>>();

        public List<int> LeftDuration { get; set; } = new List<int>();
        public List<int> CenterDuration { get; set; } = new List<int>();
        public List<int> RightDuration { get; set; } = new List<int>();

        public void Restart()
        {
            StorageService.Remove(Constants.MAX_NUMBER);
            StorageService.Remove(Constants.MIN_NUMBER);
            StorageService.Remove(Constants.CHOSEN_COUNTING);
            LeftDuration.Clear();
            CenterDuration.Clear();
            RightDuration.Clear();
            RightSlideNums.Clear();
            CenterSlideNums.Clear();
            LeftSlideNums.Clear();
            ChosenNumbers.Clear();
        }

    }


}