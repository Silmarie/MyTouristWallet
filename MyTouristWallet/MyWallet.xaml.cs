using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyTouristWallet
{

	public partial class MyWallet : ContentPage
	{
		static Database database;
		static List<CurrencyCall> calls = new List<CurrencyCall>();
		ObservableCollection<Amount> amountList;
		List<string> currenciesInWallet;
		AbsoluteLayout layout;

		bool added;

		Dictionary<string, string> currencies = new Dictionary<string, string>
		{
			{ "EUR", "euro" },
			{ "USD", "dollar" },
			{ "GBP", "pound" },
			{ "HKD", "hong kong dollar"},
			{ "INR", "indian rupee"},
			{ "AUD", "australian dollar" },
			{ "CAD", "canadian dollar" },
			{ "SGD", "singapore dollar" },
			{ "CHF", "suiss franc" },
			{ "MYR", "malaysian ringgit" },
			{ "JPY", "japanese yen" },
			{ "CNY", "chinese yuan renminbi" }
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
			currenciesInWallet = new List<string>();
			walletView.ItemsSource = amountList;

			foreach (Amount a in amountList)
			{
				currenciesInWallet.Add(a.currency);
			}
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
			colorPicker.SelectedIndexChanged += changeButtonColor;
			colorPreview.Clicked += (object s, EventArgs el) =>
				{
					colorPicker.Focus();
				};
			layout = new AbsoluteLayout();
			layout.Padding = new Thickness(15, 5, 15, 5);

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
			}

			editAmount.IsVisible = true;

			int index = amountList.IndexOf((Amount)e.SelectedItem);
			editValue.Clicked += (object s, EventArgs el) =>
			{
				amountList.ElementAt(index).AmountValue = decimal.Parse(newValue.Text);
				Database.SaveAmount(amountList.ElementAt(index));
				editAmount.IsVisible = false;
			};
		}

		public void OnEdit(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
			var a = mi.CommandParameter as Amount;

			editAmount.IsVisible = true;

			int index = amountList.IndexOf(a);
			editValue.Clicked += (object s, EventArgs el) =>
			{
				amountList.ElementAt(index).AmountValue = decimal.Parse(newValue.Text);
				Database.SaveAmount(amountList.ElementAt(index));
				editAmount.IsVisible = false;
			};
		}

		public void OnDelete(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
			var a = mi.CommandParameter as Amount;
			database.DeleteAmount(a.ID);
			amountList.Remove(a);
		}

		void AddAmount(object sender, EventArgs e)
		{
			colorPreview.BackgroundColor = Color.Transparent;
			if (added == false)
			{
				added = true;
				newEntry.IsVisible = true;
			}
			else
			{
				added = false;
				decimal nValue;

				msgNewAmount.Text = "Add Amount to wallet";

				if (!decimal.TryParse(amountEntry.Text, out nValue))
				{
					msgNewAmount.Text = "No value specified";
					msgNewAmount.TextColor = Color.Red;
					return;
				}
				string nCurrency = newCurr.Items[newCurr.SelectedIndex];
				string nDescription;
				if (!currencies.TryGetValue(nCurrency, out nDescription))
				{
					msgNewAmount.Text += "\nNo Currency";
					msgNewAmount.TextColor = Color.Red;
					return;
				}

				string nColor = colorPicker.Items[colorPicker.SelectedIndex];
				if (nValue != 1)
					nDescription += "s";

				if (msgNewAmount.Text == "Add Amount to wallet")
				{
					var a = new Amount(nCurrency, nDescription, nValue, nColor);
					if (currenciesInWallet.IndexOf(a.currency) != -1)
					{
						msgNewAmount.Text = "Currency already added";
						msgNewAmount.TextColor = Color.Red;
						return;
					}
					newEntry.IsVisible = false;
					currenciesInWallet.Add(a.currency);
					amountList.Add(a);
					Database.SaveAmount(a);
				}
			}
		}

		void changeButtonColor(object sender, EventArgs e)
		{
			Color cPreview = Color.Transparent;
			if (!colors.TryGetValue(colorPicker.Items[colorPicker.SelectedIndex], out cPreview))
			{
				return;
			}
			colorPreview.BackgroundColor = cPreview;
		}

		async void SumAmounts(object sender, EventArgs e)
		{
			if (curr.SelectedIndex == -1)
				return;
			string targetCurrency = curr.Items[curr.SelectedIndex];
			decimal sum = decimal.Zero;
			decimal convertedValue;
			double pos = 0;
			layout.Children.Clear();
			foreach (Amount a in amountList)
			{
				Color colorGraph;
				if (!colors.TryGetValue(a.color, out colorGraph))
				{
					return;
				}

				convertedValue = await ConvertValue(a.AmountValue, a.currency, targetCurrency);

				var box = new BoxView
				{
					Color = colorGraph,
					HeightRequest = (double)convertedValue
				};
				AbsoluteLayout.SetLayoutBounds(box, new Rectangle(pos, 0, 25, (double)convertedValue));
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
			totalAmount.Text = "You have " + Math.Round(sum, 2) + " " + targetCurrencyDescription + " total";
		}

		public static async Task<decimal> ConvertValue(decimal value, string firstCurrency, string secondCurrency)
		{
			string url = "http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s=" +
				firstCurrency + secondCurrency + "=X";
			string result = "";
			var conversion = new CurrencyCall("", 0, "", "");

			try
			{

				var httpClient = new HttpClient();
				httpClient.Timeout = TimeSpan.FromSeconds(5);
				var request = new HttpRequestMessage(HttpMethod.Get, url);
				var response = await httpClient.SendAsync(request);
				result = await response.Content.ReadAsStringAsync();
				string[] answer = result.Split(',');
				conversion = new CurrencyCall(answer[0].Replace("\"", ""), decimal.Parse(answer[1]), answer[2].Replace("\"", ""), answer[3].Replace("\"", "").Replace("\n", ""));
				//save time by saving currency rates
				calls.Add(conversion);
				Database.SaveCurrencyCall(conversion);
			}
			catch (Exception exception)
			{
				string currenciesToConvert = firstCurrency + secondCurrency + "=X";
				Debug.WriteLine("OFFLINE CONVERSION");
				Debug.WriteLine(exception);
				conversion = Database.GetCurrencyCallRate(currenciesToConvert).ElementAt(0);
			}

			return decimal.Multiply(conversion.rate, value);
		}
	}


}

