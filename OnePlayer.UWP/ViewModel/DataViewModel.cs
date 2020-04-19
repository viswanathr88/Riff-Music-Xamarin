using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
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
        public async Task LoadAsync(object parameter, bool fReload)
        {
            try
            {
                TParam param = default(TParam);

                // Perform parameter validation
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                // Check parameter type
                if (parameter is string && !string.IsNullOrEmpty(parameter as string))
                {
                    await LoadFromStringAsync(parameter as string);
                    return;
                }
                else if (!(parameter is TParam))
                {
                    throw new ArgumentOutOfRangeException(nameof(parameter));
                }

                param = (TParam)parameter;

                // Check if the parameters match. If they do, this ViewModel
                // has already been loaded, so skip loading
                if (!fReload && IsLoaded && object.Equals(Parameter, param))
                {
                    return;
                }

                // Set the parameter on the VM
                Parameter = param;

                Reset();

                await LoadAsync(Parameter);
            }
            catch (Exception ex)
            {
                Error = ex;
            }
        }
        /// <summary>
        /// Load with typesafe parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract Task LoadAsync(TParam parameter);
        /// <summary>
        /// Method that is called when the parameter passed is a uri string
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected virtual Task LoadFromStringAsync(string param)
        {
            // To be overridden by derived classes
            return Task.FromResult(0);
        }
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
}
