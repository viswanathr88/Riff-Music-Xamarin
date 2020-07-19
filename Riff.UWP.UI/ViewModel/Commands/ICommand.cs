using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public interface ICommand<TParam> : ICommand
    {
        bool CanExecute(TParam param);

        void Execute(TParam param);

        Exception Error { get; }
    }

    public interface IAsyncCommand<TParam> : ICommand<TParam>
    {
        Task ExecuteAsync(TParam param);
    }
}
