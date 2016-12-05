using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyTouristWallet
{
	public partial class MyWallet : ContentPage
	{
		static Database database;
		static List<CurrencyCall> calls = new List<CurrencyCall>();
		ObservableCollection<Amount> amountList;

		Dictionary<string, string> currencies = new Dictionary<string, string>
		{
			{ "EUR", "Euro" },
			{ "USD", "Dollars" },
			{ "GBP", "Pounds" },
			{ "HKD", "Hong Kong Dollars" }
		};

		public MyWallet()
		{
			InitializeComponent();

			amountList = new ObservableCollection<Amount>(Database.GetAmounts());
			walletView.ItemsSource = amountList;

			foreach (string currencyName in currencies.Keys)
			{
				curr.Items.Add(currencyName);
			}

			/*foreach (string currencyName in currencies.Keys)
			{
				newCurrency.Items.Add(currencyName);
			}*/

			calculate.Clicked += SumAmounts;
		}

		public static Database Database
		{
			get
			{
				if (database == null)
				{
					database = new Database();
				}
				return database;
			}
		}

		void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return;
				//ItemSelected is called on deselection, 
				//which results in SelectedItem being set to null
			}
			//var vSelUser = (Employee)e.SelectedItem;
			//Navigation.PushAsync(new ShowEmplyee(vSelUser));
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
		//	amountEntry.IsVisible = true;
		}

		//Add new amount
		void OnNewClicked(object sender, EventArgs args)
		{
			//newCurrency.IsVisible = true;
			//amountEntry.IsVisible = true;
		}

		private async void SumAmounts(object sender, EventArgs e)
		{
			if (curr.SelectedIndex == -1)
				return;
			string targetCurrency = curr.Items[curr.SelectedIndex];
			decimal sum = Decimal.Zero;
			decimal convertedValue;

			foreach (Amount a in amountList)
			{
				convertedValue = await ConvertValue(a.value, a.currency, targetCurrency);
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

			string[] answer = result.Split(',');

			//save time by saving currency rates
			CurrencyCall conversion = new CurrencyCall(answer[0], decimal.Parse(answer[1]), answer[2], answer[3]);
			calls.Add(conversion);
			//------

			return conversion.rate;
		}

	}


}

