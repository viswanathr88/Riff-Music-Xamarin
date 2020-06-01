using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class PlayerViewModel : DataViewModel
    {
        private bool isPlayerVisible;

        public bool IsPlayerVisible
        {
            get => isPlayerVisible;
            private set => SetProperty(ref this.isPlayerVisible, value);
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }



        public override Task LoadAsync()
        {
            throw new NotImplementedException();
        }
    }
}
