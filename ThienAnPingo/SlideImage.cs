using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content.Res;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace ThienAnPingo
{
    public class SlideImage : RelativeLayout
    {
        private string TAG = "SlideImage";
        private Context _context;
        private LinearLayout _imageBackground;
        private int _imageListHeight = 0;
        private int _imageSize;
        private bool _hasImage;
        private int _durationAnim = 4000;
        private int _imageCount = 0;
        public Action<int> OnFinishRoll;
        private ImageView _logo;

        public SlideImage(Context context) : base(context)
        {
            _context = context;
            Init();
        }

        public SlideImage(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _context = context; Init(attrs);
        }

        public SlideImage(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            _context = context;
            Init(attrs);
        }

        public SlideImage(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            _context = context;
            Init(attrs);
        }

        protected SlideImage(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        private void Init(IAttributeSet attrs = null)
        {
            if (attrs == null)
                return;

            var a = _context.ObtainStyledAttributes(attrs, Resource.Styleable.SlideImage);
            _imageSize = (int)a.GetDimension(Resource.Styleable.SlideImage_ImageSize, 170);
            var view = Inflate(_context, Resource.Layout.SlideImageLayout, this) as RelativeLayout;

            _imageBackground = view.FindViewById<LinearLayout>(Resource.Id.image_list);


            var frame = new RelativeLayout(_context);
            frame.SetBackgroundColor(Color.White);
            LayoutParams frameParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            frameParams.TopMargin = _imageSize;

            var frameBackground = view.FindViewById<RelativeLayout>(Resource.Id.frame);

            frameBackground.AddView(frame, frameParams);

            var cover = new View(_context);
            cover.SetBackgroundResource(Resource.Drawable.stroke_background);
            
            _logo = new ImageView(_context);
            _logo.SetImageResource(Resource.Drawable.logo_small);
            _logo.SetScaleType(ImageView.ScaleType.CenterInside);

            _logo.SetPadding(40, 40, 40, 40);

            var logoParams = new LayoutParams(_imageSize, _imageSize);           
           

            LayoutParams coverParams = new LayoutParams(_imageSize, _imageSize);
            frameBackground.AddView(cover, coverParams);

            frameBackground.AddView(_logo, logoParams);
        }

        public void LoadDuration(int duration)
        {
            _durationAnim = duration;
        }

        public void GetReady()
        {
            _logo.Visibility = ViewStates.Gone;
        }

        public void Restart()
        {
            _logo.Visibility = ViewStates.Gone;
        }

        public void LoadValues(List<int> values)
        {
            var count = values.Count - _imageCount;
            _imageListHeight = 0;
            for (var i = 0; i < count; i++)
            {
               
                LayoutParams layoutParamsTV = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                layoutParamsTV.AddRule(LayoutRules.CenterInParent);

                var textView = new TextView(_context);
                textView.Gravity = GravityFlags.Center;
                textView.SetTextSize(ComplexUnitType.Dip,152);
                SetFont(textView, Resource.Font.ratio);
                
                textView.SetTextColor(Color.White);
                textView.SetTypeface(textView.Typeface, TypefaceStyle.Bold);

                var imageLayout = new RelativeLayout(_context);
                imageLayout.AddView(textView, layoutParamsTV);

                LayoutParams layoutParamBackground = new LayoutParams(_imageSize, _imageSize);
                _imageBackground.AddView(imageLayout, layoutParamBackground);
                
                _imageCount += 1;
            }

            var groupView = (ViewGroup)_imageBackground;
            var childCount = groupView.ChildCount;
            for (int i = 0; i < values.Count; i++)
            {
                var view = (TextView)(((ViewGroup)(groupView.GetChildAt(i))).GetChildAt(0));                

                view.Text= values[i].ToString();
                _imageListHeight += _imageSize;

                _hasImage = true;
            }
        }

        public void StartRoll()
        {
            if (!_hasImage)
                return;

            var _positionAnimator = ValueAnimator.OfFloat(0, (-1) * (_imageListHeight - _imageSize));
            _positionAnimator.SetInterpolator(new AccelerateDecelerateInterpolator());
            _positionAnimator.SetDuration(_durationAnim);
            _positionAnimator.Update += (sender, e) =>
             {
                 float x = (float)e.Animation.GetAnimatedValue("");
                 _imageBackground.SetY(x);
             };

            _positionAnimator.AnimationEnd += (sender, e) => OnFinishRoll?.Invoke(this.Id);


            _positionAnimator.Start();
        }

        public void SetFont(TextView textView,int font)
        {
            if (font == 0) return;

            var tfStyle = TypefaceStyle.Normal;
            if (null != textView.Typeface && textView.Typeface.Style != TypefaceStyle.Bold)
                tfStyle = textView.Typeface.Style;

            if (FontControl.Cache.ContainsKey(font))
            {
                textView.SetTypeface(FontControl.Cache[font], tfStyle);
                return;
            }
            try
            {
                var tf = ResourcesCompat.GetFont(this.Context, font);
                if (null == tf)
                    return;

                FontControl.Cache[font] = tf;
                textView.SetTypeface(tf, tfStyle);
            }
            catch (Exception e)
            {
#if DEBUG
                throw e;
#else
                Log.Error(TAG, e.ToString());
#endif
            }

        }
    }
}