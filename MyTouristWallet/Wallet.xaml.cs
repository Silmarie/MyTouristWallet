using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTouristWallet
{
	public partial class Wallet : ContentPage
	{
		ObservableCollection<Amount> amountList = new ObservableCollection<Amount>();

		Dictionary<string, string> currencies = new Dictionary<string, string>
		{
			{ "EUR", "Euro" },
			{ "USD", "Dollars" },
			{ "GBP", "Pounds" },
			{ "HKD", "Hong Kong Dollars" }
		};

		public Wallet()
		{
			InitializeComponent();

			Amount amount1 = new Amount("EUR", "Euros", 10, "Blue");
			Amount amount2 = new Amount("USD", "Dollars", 20, "Red");
			Amount amount3 = new Amount("GBP", "Pounds", 30, "Green");
			Amount amount4 = new Amount("HKD", "Hong Kong Dollars", 40, "Yellow");

			amountList.Add(amount1);
			amountList.Add(amount2);
			amountList.Add(amount3);
			amountList.Add(amount4);

			walletView.ItemsSource = amountList;


			foreach (string currencyName in currencies.Keys)
			{
				curr.Items.Add(currencyName);
			}

			calculate.Clicked += SumAmounts;

		}

		private async void SumAmounts(object sender, EventArgs e)
		{
			if (curr.SelectedIndex == -1)
				return;
			string targetCurrency = curr.Items[curr.SelectedIndex];
			decimal sum = Decimal.Zero;
			decimal convertedValue;

			foreach (Amount amount in amountList)
			{
				convertedValue = await ConvertValue(amount.value, amount.currency, targetCurrency);
				sum = Decimal.Add(sum, convertedValue);
			}

			string targetCurrencyDescription;
			if (!currencies.TryGetValue(targetCurrency, out targetCurrencyDescription))
			{
				// the key isn't in the dictionary.
				return; // or whatever you want to do
			}
			inText.Text = "";
			totalAmount.Text = "Total amount in your wallet is " + sum + " " + targetCurrencyDescription;
		}

		public static async Task<decimal> ConvertValue(decimal value, string firstCurrency, string secondCurrency)
		{
			string url = "http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s=" +
				firstCurrency + secondCurrency + "=X";

			var httpClient = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
			var response = await httpClient.SendAsync(request);
			string result = await response.Content.ReadAsStringAsync();

			string[] rate = result.Split(',');
			decimal convertedValue = value * decimal.Parse(rate[1]);

			return convertedValue;
		}
	}


}

