using Flipkart_search.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Flipkart_search.ViewModel
{
    public  class Scrapping 
    {
        public List<ProductModel> ParseFlipkartJson(string json)
        {
            var list = new List<ProductModel>();

            try
            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (!root.TryGetProperty("RESPONSE", out var response)) return list;
                if (!response.TryGetProperty("slots", out var slots)) return list;

                foreach (var slot in slots.EnumerateArray())
                {
                    if (!slot.TryGetProperty("widget", out var widget)) continue;
                    if (!widget.TryGetProperty("data", out var data)) continue;
                    if (!data.TryGetProperty("products", out var products)) continue;

                    foreach (var product in products.EnumerateArray())
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

                        if (info.TryGetProperty("keySpecs", out JsonElement specsArray) && specsArray.ValueKind == JsonValueKind.Array)
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


            catch (Exception ex)
            {
                MessageBox.Show($"Parse error: {ex.Message}");
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
    }
}
