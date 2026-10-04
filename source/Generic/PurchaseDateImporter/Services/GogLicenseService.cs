using Playnite.SDK;
using Playnite.SDK.Data;
using PlayniteUtilitiesCommon;
using PurchaseDateImporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchaseDateImporter.Services
{
    public static class GogLicenseService
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        public static Guid PluginId = Guid.Parse("aebe8b7c-6dc3-4a66-af31-e7375c6b5e9e");
        public const string LibraryName = "GOG";
        public const string LoginUrl = @"https://www.gog.com/";

        public static Dictionary<string, LicenseData> GetLicensesDict()
        {
            var licensesDictionary = new Dictionary<string, LicenseData>();
            var licenses = GetLicenses();
            foreach (var license in licenses)
            {
                licensesDictionary[license.Id] = license;
            }

            return licensesDictionary;
        }

        public static List<LicenseData> GetLicenses()
        {
            var licensesList = new List<LicenseData>();
            var apiTemplate = "https://www.gog.com/account/settings/orders/data?canceled=0&completed=1&in_progress=1&not_redeemed=1&page={0}&pending=1&redeemed=1";

            using (var webView = Playnite.SDK.API.Instance.WebViews.CreateOffscreenView())
            {
                for (int i = 0; true; i++)
                {
                    var apiUrl = string.Format(apiTemplate, i);
                    webView.NavigateAndWait(apiUrl);
                    var pageText = webView.GetPageText();
                    if (pageText.IsNullOrEmpty())
                    {
                        break;
                    }

                    if (!Serialization.TryFromJson<GogOrderResponse>(pageText, out var response))
                    {
                        // The endpoint redirects to the login page when there's no logged in
                        // GOG web session on the embedded browser, in which case the response
                        // is not JSON, for example the text content of the login page
                        logger.Debug($"Failed to obtain GOG orders data on page {i}. Page text: {(pageText.Length > 300 ? pageText.Substring(0, 300) : pageText)}");
                        break;
                    }

                    if (!response.Orders.HasItems())
                    {
                        break;
                    }

                    foreach (var order in response.Orders)
                    {
                        var utcDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(order.Date);
                        var localDateTime = utcDateTimeOffset.LocalDateTime; 
                        foreach (var product in order.Products)
                        {
                            licensesList.Add(new LicenseData(product.Title, localDateTime, product.Id));
                        }
                    }
                }
            }

            return licensesList;
        }
    }
}