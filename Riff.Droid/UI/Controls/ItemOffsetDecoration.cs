using Android.Content;
using Android.Views;

namespace Riff.Droid.UI.Controls
{
    public class ItemOffsetDecoration : Android.Support.V7.Widget.RecyclerView.ItemDecoration
    {

        private readonly int mItemOffset;

        public ItemOffsetDecoration(int itemOffset)
        {
            mItemOffset = itemOffset;
        }

        public ItemOffsetDecoration(Context context, int itemOffsetId) :
                this(context.Resources.GetDimensionPixelSize(itemOffsetId))
        {
        }

        public override void GetItemOffsets(Android.Graphics.Rect outRect, View view, Android.Support.V7.Widget.RecyclerView parent, Android.Support.V7.Widget.RecyclerView.State state)
        {
            base.GetItemOffsets(outRect, view, parent, state);
            outRect.Set(mItemOffset, mItemOffset, mItemOffset, mItemOffset);
        }
    }
}