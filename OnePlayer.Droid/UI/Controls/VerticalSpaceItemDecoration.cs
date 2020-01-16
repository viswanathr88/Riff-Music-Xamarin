using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace OnePlayer.Droid.UI.Controls
{
    public class VerticalSpaceItemDecoration : Android.Support.V7.Widget.RecyclerView.ItemDecoration
    {

        private readonly int verticalSpaceHeight;

        public VerticalSpaceItemDecoration(int verticalSpaceHeight)
        {
            this.verticalSpaceHeight = verticalSpaceHeight;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            outRect.Bottom = verticalSpaceHeight;
        }
    }
}