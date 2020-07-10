using System;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    /// <summary>
    /// Base class for all viewmodels that load data from model
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public abstract class DataViewModel<TParam> : ViewModelBase, IDataViewModel<TParam>
    {
        private bool isLoading;
        private bool isLoaded;
        private Exception error;

        /// <summary>
        /// Create an instance of DataViewModel
        /// </summary>
        public DataViewModel()
        {
        }
        /// <summary>
        /// Gets whether the viewmodel is loading data
        /// </summary>
        public bool IsLoading
        {
            get { return this.isLoading; }
            protected set
            {
                SetProperty(ref this.isLoading, value);
            }
        }
        /// <summary>
        /// Gets whether the viewmodel has loaded data
        /// </summary>
        public bool IsLoaded
        {
            get { return this.isLoaded; }
            protected set
            {
                SetProperty(ref this.isLoaded, value);
            }
        }
        /// <summary>
        /// Gets the error
        /// </summary>
        public Exception Error
        {
            get
            {
                return this.error;
            }
            protected set
            {
                SetProperty(ref this.error, value);
            }
        }
        /// <summary>
        /// Load data
        /// </summary>
        /// <param name="parameter">input param</param>
        /// <returns></returns>
        public async Task LoadAsync(object parameter)
        {
            try
            {
                IsLoading = true;
                TParam param = default;

                if (typeof(TParam) == typeof(VoidType))
                {
                    parameter = VoidType.Empty;
                }

                // Perform parameter validation
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }
                else if (!(parameter is TParam))
                {
                    throw new ArgumentOutOfRangeException(nameof(parameter));
                }
                else
                {
                    param = (TParam)parameter;
                    Parameter = param;
                    Reset();
                    await LoadAsync(Parameter);
                    IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                Error = ex;
            }
            finally
            {
                IsLoading = false;
            }
        }
        /// <summary>
        /// Load with typesafe parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract Task LoadAsync(TParam parameter);
        /// <summary>
        /// Reset method for ViewModel
        /// </summary>
        protected virtual void Reset()
        {
            Error = null;
            IsLoaded = false;
            IsLoading = false;
        }
        /// <summary>
        /// Gets the input parameter
        /// </summary>
        public TParam Parameter
        {
            get;
            protected set;
        }
        /// <summary>
        /// Gets the parameter
        /// </summary>
        object IDataViewModel.Parameter
        {
            get
            {
                return Parameter;
            }
        }
    }

    public abstract class DataViewModel : DataViewModel<VoidType>
    {
        public override async Task LoadAsync(VoidType parameter)
        {
            await LoadAsync();
        }

        public abstract Task LoadAsync();
    }
}
