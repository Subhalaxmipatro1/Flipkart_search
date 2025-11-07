using Flipkart_search.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Flipkart_search
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);
    }


    public class StringEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            return string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class ProductModel : INotifyPropertyChanged
        {
            private string _currentImageUrl;
            private int _currentImageIndex = 0;

            public ICommand ShowNextImageCommand { get; private set; }
            public ICommand ShowPreviousImageCommand { get; private set; }

            public string CurrentImageUrl
            {
                get { return _currentImageUrl; }
                set
                {
                    if (_currentImageUrl != value)
                    {
                        _currentImageUrl = value;
                        OnPropertyChanged(nameof(CurrentImageUrl));
                    }
                }
            }

            public ProductModel()
            {
                
                ShowNextImageCommand = new RelayCommand(param => ShowNextImage());
                ShowPreviousImageCommand = new RelayCommand(param => ShowPreviousImage());
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


            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


            public string Title { get; set; }
            public string Price { get; set; }
            public string Rating { get; set; }
            public string Image { get; set; }

            public List<string> ImageUrls { get; set; } = new List<string>();

           
            public string ProductId { get; set; }
            public string Subtitle { get; set; }
            public string Brand { get; set; }
            public string SpecsSnippet { get; set; }
            public string ReviewsCount { get; set; }
            public string OriginalPrice { get; set; }
            public string Specifications { get; set; }

           
            public string BatteryInfo { get; set; }
            public string CameraInfo { get; set; }
            public string ProcessorInfo { get; set; }
            public string WarrantyInfo { get; set; }

            public string RamRomInfo { get; set; }  
            public string DisplayInfo { get; set; }



        }
        private List<ProductModel> allProducts = new List<ProductModel>();
        private int currentPage = 1;
        private const int itemsPerPage = 10;


        //
        private async Task<List<ProductModel>> FetchAllPagesAsync(string query, int maxPages = 10)
        {
            var allResults = new List<ProductModel>();
             var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

            for (int page = 1; page <= maxPages; page++)
            {
                string url = $"https://www.flipkart.com/search?q={Uri.EscapeDataString(query)}&page={page}";
                try
                {
                    string jsonResponse = await FlipkartClient.SearchAsync(query);


                    if (page == 1)
                        System.IO.File.WriteAllText($"page1.html",query);


                    var pageProducts = ParseFlipkartJson(jsonResponse);

                    if (pageProducts.Count == 0)
                        break; 

                    allResults.AddRange(pageProducts);

                    StatusText.Text = $"Loaded Page {page} — Total Items: {allResults.Count}";
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



        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = SearchBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(query))
                {
                    MessageBox.Show("Please enter a search term.");
                    return;
                }


                StatusText.Text = "Searching...";
                ProductList.ItemsSource = null;
                allProducts.Clear();

              
                allProducts = await FetchAllPagesAsync(query, maxPages: 10);

                ProductList.ItemsSource = allProducts;
                StatusText.Text = $"Total Items: {allProducts.Count}";

                //StatusText.Text = "Searching...";
                //ProductList.ItemsSource = null;

                //string json = await FlipkartClient.SearchAsync(query);


                //System.IO.File.WriteAllText("response.json", json);

                //MessageBox.Show($"Response length: {json.Length}");

                //var products = ParseFlipkartJson(json);

                ////ProductList.ItemsSource = products;

                ////StatusText.Text = $"Total Items: {products.Count}";

                //allProducts = products;
                //currentPage = 1;
                //ShowPage(currentPage);
                //StatusText.Text = $"total Items: {allProducts.Count}";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }


        private List<ProductModel> ParseFlipkartJson(string json)
        {
            var list = new List<ProductModel>();

            try
            {
                using (var doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;


            
                    if (!root.TryGetProperty("RESPONSE", out var response)) return list;
                    if (!response.TryGetProperty("slots", out var slots)) return list;

                    foreach (var slot in slots.EnumerateArray())
                    {
                        if (!slot.TryGetProperty("widget", out var widget)) continue;
                        if (!widget.TryGetProperty("data", out var data)) continue;
                        if (!data.TryGetProperty("products", out var value)) continue;

                        foreach (var product in value.EnumerateArray())
                        {
                            if (!product.TryGetProperty("productInfo", out var infoContainer)) continue;
                            if (!infoContainer.TryGetProperty("value", out var info)) continue;

                            string title = "";
                            string price = "";
                            string rating = "";
                            string imageUrl = "";
                            var allimageurls = new List<string>();
                            string productId = "";
                            string subtitle = "";
                            string brand = "";
                            string specs = "";
                            string reviewsCount = "";

                            string batteryInfo = "N/A";
                            string cameraInfo = "N/A";
                            string processorInfo = "N/A";
                            string warrantyInfo = "";
                            string ramRomInfo = "N/A";
                            string displayInfo = "N/A";

                            if (info.TryGetProperty("titles", out var titles) &&
                                titles.TryGetProperty("title", out var titleElement))
                            {
                                title = titleElement.GetString() ?? "";
                            }

                            if (info.TryGetProperty("pricing", out var pricing) &&
                                pricing.TryGetProperty("finalPrice", out var finalPrice) &&
                                finalPrice.TryGetProperty("decimalValue", out var decimalValue))
                            {
                                price = "₹" + decimalValue.GetString();
                            }

                            if (info.TryGetProperty("rating", out var ratingObj) &&
                                ratingObj.TryGetProperty("average", out var avg))
                            {
                                rating = "★" + avg.GetDouble().ToString("0.0");
                            }

                            //if (info.TryGetProperty("media", out var media) &&
                            //    media.TryGetProperty("images", out var images) &&
                            //    images.ValueKind == JsonValueKind.Array &&
                            //    images.GetArrayLength() > 0)
                            //{
                            //    imageUrl = images[0].GetProperty("url").GetString()
                            //        .Replace("{@width}", "300")
                            //        .Replace("{@height}", "300")
                            //        .Replace("{@quality}","70");
                            //    //MessageBox.Show(imageUrl);
                            //}

                            if (info.TryGetProperty("media", out var media) &&
                                media.TryGetProperty("images", out var images) &&
                                 images.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var imageElement in images.EnumerateArray())
                                {
                                    if (imageElement.TryGetProperty("url", out var urlElement))
                                    {
                                       
                                        string currentUrl = urlElement.GetString()
                                                            .Replace("{@width}", "600")
                                                            .Replace("{@height}", "300")
                                                            .Replace("{@quality}", "70");

                                        allimageurls.Add(currentUrl);

                                      
                                        if (string.IsNullOrEmpty(imageUrl))
                                        {
                                            imageUrl = currentUrl;
                                        }
                                    }
                                }
                            }

                            if (info.TryGetProperty("id", out var idElement))
                            {
                                productId = idElement.GetString() ?? "";
                            }
                            if (info.TryGetProperty("titles", out var titles2) &&
                        titles2.TryGetProperty("subtitle", out var subtitleElement))
                            {
                                subtitle = subtitleElement.GetString() ?? "";
                            }
                            if (info.TryGetProperty("brand", out var brandElement))
                            {
                                brand = brandElement.GetString() ?? "";
                            }


                            if (info.TryGetProperty("rating", out var ratingObj2) &&
                        ratingObj2.TryGetProperty("count", out var countElement))
                            {
                              
                                reviewsCount = countElement.GetInt32().ToString() + " Ratings";
                            }

                            if (info.TryGetProperty("warrantySummary", out var warrantyElement) && warrantyElement.ValueKind == JsonValueKind.String)
                            {
                                warrantyInfo = warrantyElement.GetString() ?? "";
                            }

                            JsonElement specsArray;
                            if (info.TryGetProperty("keySpecs", out specsArray) && specsArray.ValueKind == JsonValueKind.Array)
                            {
                                ExtractSpecs(specsArray, ref batteryInfo, ref cameraInfo, ref processorInfo,
                                             ref warrantyInfo, ref ramRomInfo, ref displayInfo, ref specs);
                            }
                            else if (info.TryGetProperty("detailedSpecs", out specsArray) && specsArray.ValueKind == JsonValueKind.Array)
                            {
                                ExtractSpecs(specsArray, ref batteryInfo, ref cameraInfo, ref processorInfo,
                                             ref warrantyInfo, ref ramRomInfo, ref displayInfo, ref specs);
                            }


                            if (!string.IsNullOrEmpty(title))
                            {
                                //list.Add(new ProductModel
                                var newProduct = new ProductModel
                                {
                                    Title = title,
                                    Price = price,
                                    Rating = rating,
                                    Image = imageUrl,
                                    ImageUrls = allimageurls,
                                    ProductId = productId,
                                    Subtitle = subtitle,
                                    Brand = brand,
                                    SpecsSnippet = specs,

                                    ReviewsCount = reviewsCount,
                                    BatteryInfo = batteryInfo,
                                    CameraInfo = cameraInfo,
                                    ProcessorInfo = processorInfo,
                                    WarrantyInfo = warrantyInfo,

                                    
                                    RamRomInfo = ramRomInfo,
                                    DisplayInfo = displayInfo
                                };
                                if (newProduct.ImageUrls.Count > 0)
                                {
                                    newProduct.CurrentImageUrl = newProduct.ImageUrls[0];
                                }

                                list.Add(newProduct);
                            }
                        }
                    }
                }
            }
            

            catch (Exception ex)
            {
                MessageBox.Show("Parsing error: " + ex.Message);
            }

            return list;
        }

      
        private void ExtractSpecs(JsonElement array,
 ref string batteryInfo, ref string cameraInfo, ref string processorInfo,
 ref string warrantyInfo, ref string ramRomInfo, ref string displayInfo, ref string specs)
        {
            foreach (var specElement in array.EnumerateArray())
            {
                string text = " ";
                if (specElement.ValueKind == JsonValueKind.Object &&
           specElement.TryGetProperty("text", out var textElement))
                {
                    text = textElement.GetString() ?? "";
                }
                else if (specElement.ValueKind == JsonValueKind.String)
                {
                    text = specElement.GetString() ?? "";
                }

                if (string.IsNullOrEmpty(text))
                    continue;

                // Capture first spec as summary
                if (string.IsNullOrEmpty(specs))
                    specs = text;

                // Assign extracted values based on keywords
                if ((text.Contains("RAM") || text.Contains("ROM") || text.Contains("Storage")) && ramRomInfo == "N/A")
                    ramRomInfo = text;
                else if ((text.Contains("inch") || text.Contains("Display") || text.Contains("HD")) && displayInfo == "N/A")
                    displayInfo = text;
                else if ((text.Contains("MP") || text.Contains("Camera")) && cameraInfo == "N/A")
                    cameraInfo = text;
                else if ((text.Contains("mAh") || text.Contains("Battery")) && batteryInfo == "N/A")
                    batteryInfo = text;
                else if ((text.Contains("Processor") || text.Contains("Chip") || text.Contains("Snapdragon") || text.Contains("Unisoc")) && processorInfo == "N/A")
                    processorInfo = text;
                else if ((text.Contains("Warranty") || text.Contains("Year")) && warrantyInfo == "N/A")
                    warrantyInfo = text;
            }
            }
        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private void ShowPage(int pageNumbre)
        {
            if(allProducts.Count == 0) return;

            int start = (pageNumbre - 1)* itemsPerPage;
            int count = Math.Min(itemsPerPage,allProducts.Count - start);

            var pageItems = allProducts.GetRange(start, count);
            ProductList.ItemsSource = pageItems;

            PageInfo.Text = $"Page{currentPage} of {Math.Ceiling((double)allProducts.Count/itemsPerPage)}";
        }

        private void Next_click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)allProducts.Count / itemsPerPage);
            if(currentPage < totalPages)
            {
                currentPage++;
                ShowPage(currentPage);
            }
        }

        private void Privious_click(object sender, RoutedEventArgs e)
        {
            if(currentPage > 1)
            {
                currentPage--;
                ShowPage(currentPage);
            }
        }

    }


}

