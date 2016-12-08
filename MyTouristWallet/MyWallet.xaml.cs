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
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyTouristWallet
{
	public partial class MyWallet : ContentPage
	{
		static Database database;
		static List<CurrencyCall> calls = new List<CurrencyCall>();
		ObservableCollection<Amount> amountList;
		AbsoluteLayout layout;

		bool added;

		Dictionary<string, string> currencies = new Dictionary<string, string>
		{
			{ "EUR", "Euro" },
			{ "USD", "Dollars" },
			{ "GBP", "Pounds" },
			{ "HKD", "Hong Kong Dollars" }
		};

		Dictionary<string, Color> colors = new Dictionary<string, Color>
		{
			{ "Aqua", Color.Aqua }, { "Black", Color.Black },
			{ "Blue", Color.Blue }, { "Fuchsia", Color.Fuchsia },
			{ "Gray", Color.Gray }, { "Green", Color.Green },
			{ "Lime", Color.Lime }, { "Maroon", Color.Maroon },
			{ "Navy", Color.Navy }, { "Olive", Color.Olive },
			{ "Purple", Color.Purple }, { "Red", Color.Red },
			{ "Silver", Color.Silver }, { "Teal", Color.Teal },
			{ "White", Color.White }, { "Yellow", Color.Yellow }
		};

		public MyWallet()
		{
			InitializeComponent();

			amountList = new ObservableCollection<Amount>(Database.GetAmounts());
			walletView.ItemsSource = amountList;

			added = false;

			foreach (string currencyName in currencies.Keys)
			{
				curr.Items.Add(currencyName);
			}

			foreach (string currencyName in currencies.Keys)
			{
				newCurr.Items.Add(currencyName);
			}

			foreach (string c in colors.Keys)
			{
				colorPicker.Items.Add(c);
			}
			colorPicker.SelectedIndexChanged += changeBoxColor;

			layout = new AbsoluteLayout();
			layout.Padding = new Thickness(5, 5, 5, 5);

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

		async void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return;
				//ItemSelected is called on deselection, 
				//which results in SelectedItem being set to null
			}
			var answer = await DisplayAlert("Question?", "Delete amount?", "Yes", "No");
			if (answer)
			{
				amountList.Remove((Amount)e.SelectedItem);
			}
			//var vSelUser = (Employee)e.SelectedItem;
			//Navigation.PushAsync(new ShowEmplyee(vSelUser));
		}

		void AddAmount(object sender, EventArgs e)
		{
			if (added == false)
			{
				added = true;
				newEntry.IsVisible = true;
			}
			else {
				added = false;
				newEntry.IsVisible = false;
				decimal nValue = decimal.Parse(amountEntry.Text); //Usar TryParse com msg de erro!!
				string nCurrency = newCurr.Items[newCurr.SelectedIndex];
				string nDescription;
				if (!currencies.TryGetValue(nCurrency, out nDescription))
				{
					// the key isn't in the dictionary.
					return; // or whatever you want to do
				}
				string nColor = colorPicker.Items[colorPicker.SelectedIndex];
				Amount a = new Amount(nCurrency, nDescription, nValue, nColor);
				amountList.Add(a);
				Database.SaveAmount(a);
			}
		}

		void changeBoxColor(object sender, System.EventArgs e)
		{
			Color cPreview;
			if (!colors.TryGetValue(colorPicker.Items[colorPicker.SelectedIndex], out cPreview))
			{
				// the key isn't in the dictionary.
				return; // or whatever you want to do
			}
			colorPreview.Color = cPreview;
		}

		private async void SumAmounts(object sender, EventArgs e)
		{
			if (curr.SelectedIndex == -1)
				return;
			string targetCurrency = curr.Items[curr.SelectedIndex];
			decimal sum = decimal.Zero;
			decimal convertedValue;
			double pos = 0;

			foreach (Amount a in amountList)
			{
				Color colorGraph;
				if (!colors.TryGetValue(a.color, out colorGraph))
				{
					// the key isn't in the dictionary.
					return; // or whatever you want to do
				}

				convertedValue = await ConvertValue(a.value, a.currency, targetCurrency);

				var box = new BoxView 
				{ 
					Color = colorGraph, 
					HeightRequest = (double)convertedValue 
				};
				AbsoluteLayout.SetLayoutBounds(box, new Rectangle(pos, 0, 25, (double) convertedValue));
				AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.PositionProportional);

				layout.Children.Add(box);

				pos = pos + 0.1;
				sum = decimal.Add(sum, convertedValue);
			}

			walletView.Footer = new ContentView
			{
				Content = layout
			};

			string targetCurrencyDescription;
			if (!currencies.TryGetValue(targetCurrency, out targetCurrencyDescription))
			{
				// the key isn't in the dictionary.
				return; // or whatever you want to do
			}
			inText.IsVisible = false;
			totalAmount.Text = "You have " + sum + " " + targetCurrencyDescription + " total";
		}

		public static async Task<decimal> ConvertValue(decimal value, string firstCurrency, string secondCurrency)
		{
			string url = "http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s=" +
				firstCurrency + secondCurrency + "=X";
			string result = "";

			try
			{
				var httpClient = new HttpClient();
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
				var response = await httpClient.SendAsync(request);
				result = await response.Content.ReadAsStringAsync();
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine("CAUGHT EXCEPTION:");
				System.Diagnostics.Debug.WriteLine(exception);
			}
			string[] answer = result.Split(',');

			//save time by saving currency rates
			CurrencyCall conversion = new CurrencyCall(answer[0], decimal.Parse(answer[1]), answer[2], answer[3]);
			//calls.Add(conversion);
			Database.SaveCurrencyCall(conversion);
			//------

			return decimal.Multiply(conversion.rate, value);
		}

	}


}

