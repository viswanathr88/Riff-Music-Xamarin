using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    /// <summary>
    /// Represents an interface that every data viewmodel will implement
    /// </summary>
    public interface IDataViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets whether the viewmodel is currently loading
        /// </summary>
        bool IsLoading
        {
            get;
        }
        /// <summary>
        /// Gets whether the viewmodel has loaded data
        /// </summary>
        bool IsLoaded
        {
            get;
        }
        /// <summary>
        /// Load data
        /// </summary>
        /// <param name="parameter">input param</param>
        /// <returns></returns>
        Task LoadAsync(object parameter);
        /// <summary>
        /// Gets the error object
        /// </summary>
        Exception Error
        {
            get;
        }
        /// <summary>
        /// Gets the parameter
        /// </summary>
        object Parameter
        {
            get;
        }
    }
    /// <summary>
    /// DataViewModel interface with a typesafe Param
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public interface IDataViewModel<TParam> : IDataViewModel
    {
        /// <summary>
        /// Load with a type safe parameter
        /// </summary>
        /// <param name="parameter">input param</param>
        /// <returns></returns>
        Task LoadAsync(TParam parameter);
        /// <summary>
        /// Gets the parameter
        /// </summary>
        new TParam Parameter
        {
            get;
        }
    }
}
