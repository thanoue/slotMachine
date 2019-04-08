using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;

namespace ThienAnPingo
{
    [Activity(MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private LottieAnimationView animationView;
        private LinearLayout setupLayout;
        private LinearLayout loadingLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppTheme);
            base.OnCreate(savedInstanceState);
            SetFullScreen();

            SetContentView(Resource.Layout.splash_layout);

            animationView = FindViewById<LottieAnimationView>(Resource.Id.animation_view);
            animationView.SetAnimation("loadng_anim.json");
            animationView.Loop(true);

            FindViewById<ImageView>(Resource.Id.logo_img).SetScaleType(ImageView.ScaleType.FitCenter);

            setupLayout = FindViewById<LinearLayout>(Resource.Id.setupLayout);
            loadingLayout = FindViewById<LinearLayout>(Resource.Id.loading_loans);

            if (!ModelHelper.Instance.IsSetted)
            {
                loadingLayout.Visibility = ViewStates.Gone;
                setupLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                StartApplication();
                return;
            }

            // Create your application here
            var min_tv = FindViewById<EditText>(Resource.Id.min_num);
            var max_tv = FindViewById<EditText>(Resource.Id.max_num);
            var chosen_count = FindViewById<EditText>(Resource.Id.chosen_counting);

            FindViewById<Button>(Resource.Id.save_change_btn).Click += delegate
            {
                if (string.IsNullOrEmpty(min_tv.Text) || string.IsNullOrEmpty(max_tv.Text) || string.IsNullOrEmpty(chosen_count.Text))
                    return;

                var min = int.Parse(min_tv.Text);
                var max = int.Parse(max_tv.Text);
                var chosenCount = int.Parse(chosen_count.Text);

                ModelHelper.Instance.ChosenCounting = chosenCount;

                if ((max - min) <= (ModelHelper.Instance.Turns + 10))
                    return;

                ModelHelper.Instance.MinNumber = min;
                ModelHelper.Instance.MaxNumber = max;
                
                StartApplication();
            };
        }

        private void StartApplication()
        {
            RunOnUiThread(() =>
            {
                loadingLayout.Visibility = ViewStates.Visible;
                setupLayout.Visibility = ViewStates.Gone;

                animationView.PlayAnimation();
            });           

            Task.Run(() =>
            {
                var rand = new Random();
                for (int i = 0; i < ModelHelper.Instance.Turns; i++)
                {
                    var chose = false;
                    var currNum = 0;
                    while (!chose)
                    {
                        var number = rand.Next(ModelHelper.Instance.MinNumber, ModelHelper.Instance.MaxNumber);
                        if (ModelHelper.Instance.ChosenNumbers.Contains(number))
                            continue;
                        chose = true;
                        currNum = number;
                        ModelHelper.Instance.ChosenNumbers.Add(number);
                    }

                    var left = 0;
                    var center = 0;
                    var right = 0;

                    right = currNum % 10;
                    currNum = currNum / 10;
                    center = currNum % 10;
                    left = currNum / 10;

                    var leftCount = rand.Next(10, 40);
                    ModelHelper.Instance.LeftSlideNums.Add(i, new List<int>());
                    for (int j = 0; j < leftCount; j++)
                    {
                        var num = rand.Next(0, 9);
                        ModelHelper.Instance.LeftSlideNums[i].Add(num);
                    }

                    var centerCount = rand.Next(10, 40);
                    ModelHelper.Instance.CenterSlideNums.Add(i, new List<int>());
                    for (int l = 0; l < centerCount; l++)
                    {
                        var num = rand.Next(0, 9);
                        ModelHelper.Instance.CenterSlideNums[i].Add(num);
                    }

                    var rightCount = rand.Next(10, 40);
                    ModelHelper.Instance.RightSlideNums.Add(i, new List<int>());
                    for (int k = 0; k < rightCount; k++)
                    {
                        var num = rand.Next(0, 9);
                        ModelHelper.Instance.RightSlideNums[i].Add(num);
                    }

                    ModelHelper.Instance.LeftSlideNums[i].Add(left);
                    ModelHelper.Instance.CenterSlideNums[i].Add(center);
                    ModelHelper.Instance.RightSlideNums[i].Add(right);

                    var leftDuration = rand.Next(3000, 6000);
                    ModelHelper.Instance.LeftDuration.Add(leftDuration);

                    var centerDuration = rand.Next(3000,6700);
                    ModelHelper.Instance.CenterDuration.Add(centerDuration);

                    var rightDuration = rand.Next(3000, 6900);
                    ModelHelper.Instance.RightDuration.Add(rightDuration);                  
                }
              
            }).ContinueWith(async =>
            {
                RunOnUiThread(() =>
                {
                    StartActivity(new Intent(this, typeof(MainActivity)));
                });
            });
        }

        private void SetFullScreen()
        {
            View decorView = Window.DecorView;
            var uiOptions = (int)decorView.SystemUiVisibility;
            var newUiOptions = (int)uiOptions;
            newUiOptions |= (int)SystemUiFlags.Fullscreen;
            newUiOptions |= (int)SystemUiFlags.HideNavigation;
            newUiOptions |= (int)SystemUiFlags.ImmersiveSticky;
            decorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
        }
    }
}