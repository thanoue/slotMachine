using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Collections.Generic;
using Com.Airbnb.Lottie;
using Android.Media;
using System.Threading;
using System.Threading.Tasks;
using Android.Views;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Content;

namespace ThienAnPingo
{
    [Activity(Label = "@string/app_name", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : AppCompatActivity, Android.Content.IDialogInterfaceOnClickListener
    {

        private ImageView _rollBtn;
        private SlideImage _left, _center, _right;
        IList<int> _currChosenNums; int _currIndex = 0;
        IList<int> _rolledNumbers;
        int _currNum = 0;
        private bool _isRolling = false;
        private bool _leftFinished, _centerFinished, _rightFinished;
        private LottieAnimationView _animationView;
        MediaPlayer _playingSound, _finishSound;
        private MediaPlayer _startSound;
        private GridView _numberList;
        private ChosenNumAdapter _numberListAdapter;
        private RelativeLayout _rollArea;
        private AlertDialog.Builder _dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppTheme);
            base.OnCreate(savedInstanceState);
            SetFullScreen();
            if (savedInstanceState != null)
                return;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            _rollBtn = FindViewById<ImageView>(Resource.Id.roll_img);

            _left = FindViewById<SlideImage>(Resource.Id.slide_left);
            _center = FindViewById<SlideImage>(Resource.Id.slide_center);
            _right = FindViewById<SlideImage>(Resource.Id.slide_right);

            _rollBtn.Click += _rollBtn_Click;

            _right.OnFinishRoll += RollFinishing;
            _center.OnFinishRoll += RollFinishing;
            _left.OnFinishRoll += RollFinishing;

            _currChosenNums = new List<int>();
            _rolledNumbers = new List<int>();

            _animationView = FindViewById<LottieAnimationView>(Resource.Id.animation_view);
            _animationView.Visibility = Android.Views.ViewStates.Invisible;
            _animationView.SetAnimation("blow_anim.json");

            _finishSound = BuildPlayer(Resource.Raw.finish_rolling);
            _startSound = BuildPlayer(Resource.Raw.start_rolling);

            FindViewById<Button>(Resource.Id.choose_number_btn).Click += delegate
            {
                if (_isRolling)
                    return;

                if (_numberList.Visibility == ViewStates.Visible)
                    return;

                if (_currChosenNums.Count >= ModelHelper.Instance.ChosenCounting)
                {
                    _numberList.Visibility = ViewStates.Visible;
                    _rollArea.Visibility = ViewStates.Gone;
                    return;
                }

                if (_currChosenNums.Contains(_currNum))
                    return;

                _currChosenNums.Add(_currNum);

                _numberListAdapter.UpdateNumberState(_currNum, true);



            };

            _numberList = FindViewById<GridView>(Resource.Id.chosen_list);
            _numberListAdapter = new ChosenNumAdapter(this);
            _numberList.Adapter = _numberListAdapter;

            _rollArea = FindViewById<RelativeLayout>(Resource.Id.roll_area);

            _dialog = new AlertDialog.Builder(this)
               .SetTitle("Restart the Game")
               .SetMessage("Do you really want to restart the current game ? Your all data will be lost!")
               .SetIcon(Android.Resource.Drawable.IcDialogInfo)
               .SetPositiveButton(Android.Resource.String.Yes, this)
               .SetNegativeButton(Android.Resource.String.No, this);

            FindViewById<Button>(Resource.Id.get_list_btn).Click += delegate
            {
                if (_numberList.Visibility == ViewStates.Visible)
                {
                    _numberList.Visibility = ViewStates.Gone;
                    _rollArea.Visibility = ViewStates.Visible;
                    return;
                }              
               
                _numberList.Visibility = ViewStates.Visible;
                _rollArea.Visibility = ViewStates.Gone;
            };

            FindViewById<Button>(Resource.Id.restart_btn).Click += delegate
            {
                _dialog.Show();
            };

            _numberList.ItemClick += _numberList_ItemClick;
        }

        private void _numberList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = _numberListAdapter[e.Position];

            _numberListAdapter.UpdateNumberState(item.Value, !item.IsChosen);

            if (_currChosenNums.Contains(item.Value))
                _currChosenNums.Remove(item.Value);
            else
                _currChosenNums.Add(item.Value);
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

        private MediaPlayer BuildPlayer(int raw)
        {
            MediaPlayer mediaPlayer = MediaPlayer.Create(this, raw);
            mediaPlayer.Looping = false;
            mediaPlayer.SetAudioStreamType(Stream.Music);
            return mediaPlayer;
        }

        void RollFinishing(int id)
        {
            if (id == _left.Id)
                _leftFinished = true;
            if (id == _center.Id)
                _centerFinished = true;
            if (id == _right.Id)
                _rightFinished = true;

            if (!_leftFinished || !_centerFinished || !_rightFinished)
                return;

            _playingSound.Stop();
            _finishSound.Start();
            _animationView.Visibility = Android.Views.ViewStates.Visible;
            _animationView.PlayAnimation();

            _isRolling = false;

            _leftFinished = _rightFinished = _centerFinished = false;

            _currIndex++;
        }

        void LoadNumbers()
        {
            _currNum = ModelHelper.Instance.ChosenNumbers[_currIndex];
            _rolledNumbers.Add(_currNum);

            _numberListAdapter.AddItem(_currNum, false);

            _left.LoadValues(ModelHelper.Instance.LeftSlideNums[_currIndex]);
            _center.LoadValues(ModelHelper.Instance.CenterSlideNums[_currIndex]);
            _right.LoadValues(ModelHelper.Instance.RightSlideNums[_currIndex]);

            _left.LoadDuration(ModelHelper.Instance.LeftDuration[_currIndex]);
            _center.LoadDuration(ModelHelper.Instance.CenterDuration[_currIndex]);
            _right.LoadDuration(ModelHelper.Instance.RightDuration[_currIndex]);
        }

        private void _rollBtn_Click(object sender, System.EventArgs e)
        {
            if (_isRolling)
                return;

            if (_currChosenNums.Count >= ModelHelper.Instance.ChosenCounting)
            {
                _numberList.Visibility = ViewStates.Visible;
                _rollArea.Visibility = ViewStates.Gone;
                return;
            }

            if (_currIndex >= ModelHelper.Instance.ChosenNumbers.Count)
            {
                _isRolling = true;
                return;
            }

            LoadNumbers();


            _startSound.Start();

            Handler han = new Handler();
            han.PostDelayed(() =>
            {
                _playingSound = BuildPlayer(Resource.Raw.rolling);
                _playingSound.Start();
            }, 500);



            _left.GetReady(); _center.GetReady(); _right.GetReady();
            _isRolling = true;
            _left.StartRoll(); _center.StartRoll(); _right.StartRoll();
        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();
            if (which == -1)
            {

                ModelHelper.Instance.Restart();

                StartActivity(new Intent(this, typeof(SplashActivity)));

                Handler handler = new Handler();
                handler.PostDelayed(() =>
                {
                    this.Finish();
                }, 2000);
            }
        }
    }
}