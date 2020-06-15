using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Riff.UWP.Test.Infra
{
    public class UITree
    {
        public async Task<Size> GetWindowSize()
        {
            return await CoreDispatcher.RunToCompletionAsync(() =>
            {
                return Task.FromResult(new Size(RootFrame.ActualWidth, RootFrame.Height));
            });
        }

        public async Task<bool> ElementExists<T>(string name) where T : DependencyObject
        {
            return await CoreDispatcher.RunToCompletionAsync(() =>
            {
                var element = VisualTreeHelperExtensions.FindVisualChild<T>(RootFrame, name);
                return Task.FromResult(element != null);
            });
        }

        public async Task<bool> ConditionPasses(Func<bool> fn)
        {
            return await CoreDispatcher.RunToCompletionAsync(() =>
            {
                return Task.FromResult(fn());
            });
        }

        public async Task<bool> ConditionPasses<TElement>(string name, Func<TElement, bool> fn) where TElement : DependencyObject
        {
            return await CoreDispatcher.RunToCompletionAsync(() =>
            {
                bool result = false;
                var element = VisualTreeHelperExtensions.FindVisualChild<TElement>(RootFrame, name);
                if (element != null)
                {
                    result = fn(element);
                }

                return Task.FromResult(result);
            });
        }

        public async Task WaitForElement<T>(string name, int pingFreqencyMs = 100, int totalPings = 20) where T : DependencyObject
        {
            bool found = false;
            for (int i = 0; i < totalPings; i++)
            {
                bool exists = await ElementExists<T>(name);
                if (!exists)
                {
                    await Task.Delay(pingFreqencyMs);
                }
                else
                {
                    found = true;
                    break;
                }
            }

            Xunit.Assert.True(found, $"Failed to find element with name {name}");
        }

        public async Task WaitForElementAndExecute<TElement>(string name, Action<TElement> fn, int pingFrequencyMs = 100, int totalPings = 20) where TElement : DependencyObject
        {
            await WaitForElement<TElement>(name, pingFrequencyMs, totalPings);
            await CoreDispatcher.RunToCompletionAsync(() =>
            {
                var element = VisualTreeHelperExtensions.FindVisualChild<TElement>(RootFrame, name);
                Xunit.Assert.NotNull(element);
                fn(element);
                return Task.CompletedTask;
            });
        }
        public async Task WaitForElementAndCondition<TElement>(string name, Func<TElement, bool> condition, int pingFreqencyMs = 100, int totalPings = 20) where TElement : DependencyObject
        {
            bool success = false;
            for (int i = 0; i < totalPings; i++)
            {
                bool result = await ConditionPasses(name, condition);
                if (!result)
                {
                    await Task.Delay(pingFreqencyMs);
                }
                else
                {
                    success = true;
                    break;
                }
            }

            Xunit.Assert.True(success);
        }

        public async Task<TReturn> WaitForElementAndExecute<TElement, TReturn>(string name, Func<TElement, TReturn> fn, int pingFrequencyMs = 100, int totalPings = 20) where TElement : DependencyObject
        {
            await WaitForElement<TElement>(name, pingFrequencyMs, totalPings);
            return await CoreDispatcher.RunToCompletionAsync(() =>
            {
                var element = VisualTreeHelperExtensions.FindVisualChild<TElement>(RootFrame, name);
                if (element != null)
                {
                    return Task.FromResult(fn(element));
                }

                throw new Exception("Element not found to execute function. Tree has probably changed unexpectedly");
            });
        }

        public async Task WaitForCondition(Func<bool> condition, int pingFreqencyMs = 100, int totalPings = 20)
        {
            bool success = false;
            for (int i = 0; i < totalPings; i++)
            {
                bool result = await ConditionPasses(condition);
                if (!result)
                {
                    await Task.Delay(pingFreqencyMs);
                }
                else
                {
                    success = true;
                    break;
                }
            }

            Xunit.Assert.True(success);
        }

        public async Task<bool> LoadPage<T>(object parameter) where T : Page
        {
            return await CoreDispatcher.RunToCompletionAsync(() =>
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();
                RootFrame.Navigated += navigatedCallback;
                
                void navigatedCallback(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs args)
                {
                    Xunit.Assert.Equal(typeof(T), args.SourcePageType);
                    RootFrame.Navigated -= navigatedCallback;
                    tcs.SetResult(true);
                }

                try
                {
                    RootFrame.Navigate(typeof(T), parameter);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

                return tcs.Task;
            });
        }

        public async Task UnloadAllPages()
        {
            await CoreDispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
            {
                RootFrame.BackStack.Clear();
                RootFrame.Content = null;
                RootFrame.CacheSize = 0;
            });

            await WaitForCondition(() => RootFrame.Content == null);
        }

        private CoreDispatcher CoreDispatcher => Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

        private Frame RootFrame => App.RootFrame;
    }
}
