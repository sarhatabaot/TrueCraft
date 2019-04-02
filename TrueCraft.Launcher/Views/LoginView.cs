﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using TrueCraft.Core;
using Xwt;
using Xwt.Drawing;

namespace TrueCraft.Launcher.Views
{
	public class LoginView : VBox
	{
		public LoginView(LauncherWindow window)
		{
			Window = window;
			MinWidth = 250;

			ErrorLabel = new Label("Username or password incorrect")
			{
				TextColor = Color.FromBytes(255, 0, 0),
				TextAlignment = Alignment.Center,
				Visible = false
			};
			UsernameText = new TextEntry();
			PasswordText = new PasswordEntry();
			LogInButton = new Button("Log In");
			RegisterButton = new Button("Register");
			OfflineButton = new Button("Play Offline");
			RememberCheckBox = new CheckBox("Remember Me");
			UsernameText.Text = UserSettings.Local.Username;
			if (UserSettings.Local.AutoLogin)
			{
				PasswordText.Password = UserSettings.Local.Password;
				RememberCheckBox.Active = true;
			}

			using (var stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("TrueCraft.Launcher.Content.truecraft_logo.png"))
				TrueCraftLogoImage = new ImageView(Image.FromStream(stream).WithBoxSize(350, 75));

			UsernameText.PlaceholderText = "Username";
			PasswordText.PlaceholderText = "Password";
			PasswordText.KeyReleased += (sender, e) =>
			{
				if (e.Key == Key.Return || e.Key == Key.NumPadEnter)
					LogInButton_Clicked(sender, e);
			};
			UsernameText.KeyReleased += (sender, e) =>
			{
				if (e.Key == Key.Return || e.Key == Key.NumPadEnter)
					LogInButton_Clicked(sender, e);
			};
			RegisterButton.Clicked += (sender, e) => { Window.WebView.Url = "https://truecraft.io/register"; };
			OfflineButton.Clicked += (sender, e) =>
			{
				Window.User.Username = UsernameText.Text;
				Window.User.SessionId = "-";
				Window.InteractionBox.Remove(this);
				Window.InteractionBox.PackEnd(Window.MainMenuView = new MainMenuView(Window));
			};
			var regoffbox = new HBox();
			RegisterButton.WidthRequest = OfflineButton.WidthRequest = 0.5;
			regoffbox.PackStart(RegisterButton, true);
			regoffbox.PackStart(OfflineButton, true);
			LogInButton.Clicked += LogInButton_Clicked;

			PackEnd(regoffbox);
			PackEnd(LogInButton);
			PackEnd(RememberCheckBox);
			PackEnd(PasswordText);
			PackEnd(UsernameText);
			PackEnd(ErrorLabel);
		}

		public LauncherWindow Window { get; set; }

		public TextEntry UsernameText { get; set; }
		public PasswordEntry PasswordText { get; set; }
		public Button LogInButton { get; set; }
		public Button RegisterButton { get; set; }
		public Button OfflineButton { get; set; }
		public ImageView TrueCraftLogoImage { get; set; }
		public Label ErrorLabel { get; set; }
		public CheckBox RememberCheckBox { get; set; }

		private void DisableForm()
		{
			UsernameText.Sensitive = PasswordText.Sensitive = LogInButton.Sensitive =
				RegisterButton.Sensitive = OfflineButton.Sensitive = false;
		}

		private void EnableForm()
		{
			UsernameText.Sensitive = PasswordText.Sensitive = LogInButton.Sensitive =
				RegisterButton.Sensitive = OfflineButton.Sensitive = true;
		}

		private void LogInButton_Clicked(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(UsernameText.Text) || string.IsNullOrEmpty(PasswordText.Password))
			{
				ErrorLabel.Text = "Username and password are required";
				ErrorLabel.Visible = true;
				return;
			}

			ErrorLabel.Visible = false;
			DisableForm();

			Window.User.Username = UsernameText.Text;
			var request = WebRequest.CreateHttp(TrueCraftUser.AuthServer + "/api/login");
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.AllowAutoRedirect = false;
			request.BeginGetRequestStream(HandleLoginRequestReady, new LogInAsyncState
			{
				Request = request,
				Username = Window.User.Username,
				Password = PasswordText.Password
			});
		}

		private void HandleLoginRequestReady(IAsyncResult asyncResult)
		{
			try
			{
				var state = (LogInAsyncState) asyncResult.AsyncState;
				var request = state.Request;
				var requestStream = request.EndGetRequestStream(asyncResult);
				using (var writer = new StreamWriter(requestStream))
					writer.Write("user={0}&password={1}&version=12", state.Username, state.Password);
				request.BeginGetResponse(HandleLoginResponse, request);
			}
			catch
			{
				Application.Invoke(() =>
				{
					EnableForm();
					ErrorLabel.Text = "Unable to log in";
					ErrorLabel.Visible = true;
					RegisterButton.Label = "Offline Mode";
				});
			}
		}

		private void HandleLoginResponse(IAsyncResult asyncResult)
		{
			try
			{
				var request = (HttpWebRequest) asyncResult.AsyncState;
				var response = request.EndGetResponse(asyncResult);
				string session;
				using (var reader = new StreamReader(response.GetResponseStream()))
					session = reader.ReadToEnd();
				if (session.Contains(":"))
				{
					var parts = session.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					Application.Invoke(() =>
					{
						Window.User.Username = parts[2];
						Window.User.SessionId = parts[3];
						EnableForm();
						Window.InteractionBox.Remove(this);
						Window.InteractionBox.PackEnd(Window.MainMenuView = new MainMenuView(Window));
						UserSettings.Local.AutoLogin = RememberCheckBox.Active;
						UserSettings.Local.Username = Window.User.Username;
						if (UserSettings.Local.AutoLogin)
							UserSettings.Local.Password = PasswordText.Password;
						else
							UserSettings.Local.Password = string.Empty;
						UserSettings.Local.Save();
					});
				}
				else
					Application.Invoke(() =>
					{
						EnableForm();
						ErrorLabel.Text = session;
						ErrorLabel.Visible = true;
						RegisterButton.Label = "Offline Mode";
					});
			}
			catch
			{
				Application.Invoke(() =>
				{
					EnableForm();
					ErrorLabel.Text = "Unable to log in.";
					ErrorLabel.Visible = true;
					RegisterButton.Label = "Offline Mode";
				});
			}
		}

		private class LogInAsyncState
		{
			public HttpWebRequest Request { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
		}
	}
}