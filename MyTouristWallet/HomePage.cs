using System;

using Xamarin.Forms;

namespace MyTouristWallet
{
	public class HomePage : ContentPage
	{
		Label lab1, lab2, labValue;
		Entry entry1, entry2;
		Button button;

		public HomePage()
		{
			lab1 = new Label()
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

			var stack1 = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Children = { lab1, entry1 }
			};

			var stack2 = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Children = { lab2, entry2 }
			};

			var stack = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				Children = { stack1, stack2 }
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
			};

			Content = new StackLayout()
			{
				Children = { stack, button, labValue }
			};
			Padding = new Thickness(5, Device.OnPlatform(20, 0, 0));
			BackgroundColor = Color.Gray;
		}

		private void OnButton_Clicked(object sender, EventArgs e)
		{
			labValue.Text = String.Format("Result: \nCurrency 1: {0}, Currency 2: {1}", entry1.Text, entry2.Text);
		}
	}
}

