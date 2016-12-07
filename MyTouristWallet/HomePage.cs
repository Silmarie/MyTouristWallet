using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyTouristWallet
{
	public class HomePage : ContentPage
	{
		static Database database;
		ListView wallet;
		Label lab1, lab2, labValue;
		Entry entry1, entry2;
		Button button;
		Picker curr1, curr2;
		Image arrow;

		Dictionary<string, string> currencies = new Dictionary<string, string>
		{
			{ "EUR", "Euro" },
			{ "USD", "Dollars" }
		};

		public HomePage()
		{
			wallet = new ListView()
			{
				ItemsSource = Database.GetAmounts()
			};

			var dataTemplate = new DataTemplate(typeof(TextCell));
			dataTemplate.SetBinding(TextCell.TextProperty, "Name");
			/*lab1 = new Label()
			{
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Text = "Currency 1",
				TextColor = Color.FromRgb(1.0, 0.9, 0.9),
				WidthRequest = 100
			};
			lab2 = new Label()
			{
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Text = "Currency 2",
				TextColor = Color.FromRgb(1.0, 0.9, 0.9),
				WidthRequest = 100
			};
			entry1 = new Entry()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Text = "",
				Placeholder = "Value",
			};
			entry2 = new Entry()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Text = "",
				Placeholder = "Value",
			};

			//Currency Type Pickers
			curr1 = new Picker()
			{
				Title = "Currency",
				HorizontalOptions = LayoutOptions.Start
			};

			foreach (string currencyName in currencies.Keys)
			{
				curr1.Items.Add(currencyName);
			}

			curr2 = new Picker()
			{
				Title = "Currency",
				HorizontalOptions = LayoutOptions.Start

			};

			foreach (string currencyName in currencies.Keys)
			{
				curr2.Items.Add(currencyName);
			}


			var stack1 = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Children = { lab1, entry1, curr1 }
			};

			arrow = new Image()
			{
				Aspect = Aspect.AspectFit,
				Source = ImageSource.FromFile("arrow.png")
			};

			var stack2 = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Children = { lab2, entry2, curr2 }
			};

			var stack = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Children = { stack1, arrow, stack2 }
			};

			button = new Button()
			{
				HorizontalOptions = LayoutOptions.Center,
				Text = "Convert",
				TextColor = Color.Blue
			};
			button.Clicked += OnButton_Clicked;
			labValue = new Label()
			{
				HorizontalOptions = LayoutOptions.Center,
				Text = "",
				TextColor = Color.FromRgb(1.0, 0.9, 0.9)
			};*/

			Content = new StackLayout()
			{
				Children = { wallet/*, stack, button, labValue*/ }
			};
			Padding = new Thickness(5, Device.OnPlatform(20, 0, 0));
			BackgroundColor = Color.Gray;
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

		private async void OnButton_Clicked(object sender, EventArgs e)
		{
			var firstCurrency = curr1.Items[curr1.SelectedIndex];
			var secondCurrency = curr2.Items[curr2.SelectedIndex];
			string url = "http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s=" +
				firstCurrency + secondCurrency + "=X";
			string json = await GetCurrencyValue(url);
			string[] value = json.Split(',');
			decimal convertedValue = decimal.Parse(entry1.Text) * decimal.Parse(value[1]); 
			entry2.Text = convertedValue.ToString();
			labValue.Text = json;//String.Format("Result: \nCurrency 1: {0}, Currency 2: {1}", entry1.Text, entry2.Text);

		}

		public static async Task<string> GetCurrencyValue(string url)
		{
			var httpClient = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
			var response = await httpClient.SendAsync(request);
			string result = await response.Content.ReadAsStringAsync();
			return result;
		}
	}
}

