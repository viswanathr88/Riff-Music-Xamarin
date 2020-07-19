using Mirage.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public class ItemWithOverlayViewModel<T> : ItemViewModel<T>
    {
        private bool isPointerOver = false;

        public ItemWithOverlayViewModel(T item) : base(item)
        {
        }

        public bool ShowOverlayControls
        {
            get => IsPointerOver || IsSelected;
        }

        public bool IsPointerOver
        {
            get => isPointerOver;
            set
            {
                if (SetProperty(ref this.isPointerOver, value))
                {
                    RaisePropertyChanged(nameof(ShowOverlayControls));
                }
            }
        }

        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();

            RaisePropertyChanged(nameof(ShowOverlayControls));
        }
    }
}
