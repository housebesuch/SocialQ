using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;

namespace SocialQ.Forms
{
    public partial class StoreSearch
    {
        public StoreSearch()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel.InitializeData)
                .SelectMany(x => x.Execute())
                .Subscribe()
                .DisposeWith(PageDisposables);

            this.WhenAnyValue(x => x.ViewModel.Stores)
                .Where(x => x != null)
                .BindTo(this, x => x.StoreList.ItemsSource)
                .DisposeWith(PageDisposables);

            this.WhenAnyValue(x => x.ViewModel.IsLoading)
                .Subscribe(isLoading =>
                {
                    LoadingIndicator.IsVisible = isLoading;
                    LoadingIndicator.IsRunning = isLoading;
                });
            
            SearchBar
                .Events()
                .TextChanged
                .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
                .Where(x => x?.OldTextValue?.Length > 0 && x.NewTextValue?.Length == 0)
                .Select(x => Unit.Default)
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(this, x => x.ViewModel.Search)
                .DisposeWith(PageDisposables);

            Search
                .Events()
                .Pressed
                .ToSignal()
                .InvokeCommand(this, x => x.ViewModel.Search)
                .DisposeWith(PageDisposables);

            StoreList
                .Events()
                .ItemSelected
                .Subscribe(item =>
                {
                    StoreList.SelectedItem = null;
                })
                .DisposeWith(PageDisposables);
        }
    }
}