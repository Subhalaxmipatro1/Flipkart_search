using Flipkart_search.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace Flipkart_search.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _statusText;
        private string _searchQuery;
        private int _currentPage = 1;
        private const int ItemsPerPage = 10;

        private string _currentImageUrl;
        private int _currentImageIndex;

        public List<string> ImageUrls { get; set; } = new List<string>();

        public ObservableCollection<ProductModel> DisplayedProducts { get; set; } = new ObservableCollection<ProductModel>();
        private List<ProductModel> _allProducts = new List<ProductModel>();

        public ICommand SearchCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }

        public ICommand ShowNextImageCommand { get; }

        public ICommand ShowPreviousImageCommand { get; }

        public RelayCommand OpenLoginCommand { get; }

        public RelayCommand OpencartCommand { get; }

        public RelayCommand FlightCommand { get; }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(nameof(StatusText)); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(nameof(SearchQuery)); }
        }

        public MainViewModel()
        {
            SearchCommand = new RelayCommand(async _ => await SearchAsync());
            NextPageCommand = new RelayCommand(_ => NextPage());
            PrevPageCommand = new RelayCommand(_ => PrevPage());

            ShowNextImageCommand = new RelayCommand(_ => ShowNextImage());
            ShowPreviousImageCommand = new RelayCommand(_ => ShowPreviousImage());

            OpenLoginCommand = new RelayCommand(_ => OpenLoginWindow()); 

            OpencartCommand = new RelayCommand(_ => OpencartWindow());

            FlightCommand = new RelayCommand(_ => OpenFlightwindow());

        }

        private void OpenLoginWindow()
        {
            var loginWindow = new Flipkart_search.View.Login();
            loginWindow.ShowDialog();
        }

        private void OpencartWindow()
        { 
            var cartWindow = new Flipkart_search.View.Cart();
            cartWindow.ShowDialog();
        }

        private void OpenFlightwindow()
        {
            var Flightwindow = new Flipkart_search.View.Flights();
            Flightwindow.ShowDialog();
        }
        public string CurrentImageUrl
        {
            get => _currentImageUrl;
            set
            {
                if (_currentImageUrl != value)
                {
                    _currentImageUrl = value;
                    OnPropertyChanged(nameof(CurrentImageUrl));
                }
            }
        }

        public void ShowNextImage()
        {
            if (ImageUrls == null || ImageUrls.Count <= 1) return;
            _currentImageIndex = (_currentImageIndex + 1) % ImageUrls.Count;
            CurrentImageUrl = ImageUrls[_currentImageIndex];
        }

        public void ShowPreviousImage()
        {
            if (ImageUrls == null || ImageUrls.Count <= 1) return;
            _currentImageIndex = (_currentImageIndex - 1 + ImageUrls.Count) % ImageUrls.Count;
            CurrentImageUrl = ImageUrls[_currentImageIndex];
        }
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                //MessageBox.Show("Enter a product name.");
                MessageBox.Show($"You searched for: {SearchQuery}");

                return;
            }

            StatusText = "Searching...";
            _allProducts.Clear();
            DisplayedProducts.Clear();

            _allProducts = await FetchAllPagesAsync(SearchQuery, 10);
            ShowPage(1);
            StatusText = $"Total Items: {_allProducts.Count}";
        }

        private async Task<List<ProductModel>> FetchAllPagesAsync(string query, int maxPages = 10)
        {
            var allResults = new List<ProductModel>();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            var Scapper = new Scrapping();
            for (int page = 1; page <= maxPages; page++)
            {
                try
                {
                    string jsonResponse = await FlipkartClient.SearchAsync(query);
                    var pageProducts = Scapper.ParseFlipkartJson(jsonResponse);
                    if (pageProducts.Count == 0) break;

                    allResults.AddRange(pageProducts);
                    StatusText = $"Loaded Page {page} — Total Items: {allResults.Count}";
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error on page {page}: {ex.Message}");
                    break;
                }
            }

            return allResults;
        }

       


        private string _pageInfo;
        public string PageInfo
        {
            get => _pageInfo;
            set
            {
                _pageInfo = value;
                OnPropertyChanged(nameof(PageInfo));
            }
        }


        private void ShowPage(int page)
        {
            if (_allProducts.Count == 0) return;

            _currentPage = page;
            DisplayedProducts.Clear();

            int start = (page - 1) * ItemsPerPage;
            int count = Math.Min(ItemsPerPage, _allProducts.Count - start);

            for (int i = start; i < start + count; i++)
                DisplayedProducts.Add(_allProducts[i]);

            PageInfo  = $"Page{_currentPage} of {Math.Ceiling((double)_allProducts.Count / ItemsPerPage)}";
        }




        private void NextPage()
        {
            int totalPages = (int)Math.Ceiling((double)_allProducts.Count / ItemsPerPage);
            if (_currentPage < totalPages)
                ShowPage(_currentPage + 1);
        }

        private void PrevPage()
        {
            if (_currentPage > 1)
                ShowPage(_currentPage - 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
            
           

    
