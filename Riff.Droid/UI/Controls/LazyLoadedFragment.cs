

namespace Riff.Droid.UI.Controls
{
    public abstract class LazyLoadedFragment : Android.Support.V4.App.Fragment
    {
        private readonly int layoutId;

        public LazyLoadedFragment(int layoutId)
        {
            this.layoutId = layoutId;
        }

        private bool IsViewCreated { get; set;  }
        private bool IsViewVisible { get; set; }

        private bool IsViewLoaded { get; set; }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(layoutId, container, false);
            IsViewCreated = true;

            // Load the remaining view & data
            LoadBase(view);

            return view;
        }

        private void LoadBase(Android.Views.View view)
        {
            if (IsViewCreated && IsViewVisible && !IsViewLoaded)
            {
                Load(view);
                IsViewLoaded = true;
            }
        }

        public override bool UserVisibleHint 
        { 
            get => base.UserVisibleHint; 
            set
            {
                IsViewVisible = value;
                LoadBase(View);
                base.UserVisibleHint = value;
            }
        }

        protected abstract void Load(Android.Views.View view);
    }
}