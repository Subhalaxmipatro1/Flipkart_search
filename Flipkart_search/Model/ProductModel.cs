using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Flipkart_search.Model
{
    public class ProductModel : INotifyPropertyChanged
    {
        
        public string Title { get; set; }
        public string Price { get; set; }
        public string Rating { get; set; }
        public string Image { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public string CurrentImageUrl { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
}
}
