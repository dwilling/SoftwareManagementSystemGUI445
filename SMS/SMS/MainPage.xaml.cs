using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SMS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        static sqlHelper.Service1Client DBConnector = new sqlHelper.Service1Client();
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login(txtUserName.Text, txtPassword.Password);
        }

        private async void Login(string UN, string PW)
        {
            //Run the query
            var task = DBConnector.ExecuteQueryAsync("Select Users_Password,Users_ID From Users Where Users_Email = " + Quote(UN));
            List<List<object>> rows = await task;
            string[] tempData = new string[2];
            if (rows.Count != 0)
            {
                foreach (List<object> row in rows)
                {
                    int i = 0;
                    foreach (object item in row)
                    {
                        tempData[i] = item.ToString();
                        i++;
                    }
                }
                if (tempData[0] == PW)
                {
                    if (this.Frame != null)
                        this.Frame.Navigate(typeof(SoftwareSearcher), tempData[1]);
                }
                else
                {
                    txtMsg.Text = "Invalid Password.";
                }
            }
            else
            {
                txtMsg.Text = "Invalid Username.";
            }

        }

        public string Quote(string temp)
        {
            string newString = "'" + temp.Replace("'", "\"") + "'";
            return newString;
        }

        private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount();
        }

        private async void CreateAccount()
        {
            var task = DBConnector.ExecuteQueryAsync("Select Users_Password From Users Where Users_Email = " + Quote(txtUserName.Text));
            List<List<object>> rows = await task;

            if (rows.Count == 0)
            {
                var task3 = DBConnector.GetNewIDAsync("Users");
                string newID = await task3;
                newID = (Convert.ToInt32(newID) + 2).ToString();
                var task2 = DBConnector.ExecuteNonQueryAsync("INSERT INTO Users (Users_ID,Users_Email,Users_Password,Users_SugarCRMID) Values (" + newID + "," + Quote(txtUserName.Text) + "," + Quote(txtPassword.Password.ToString()) + ",'Black')");
                bool worked = await task2;
                if (worked == true)
                    txtMsg.Text = "Your account has been created!";
            }
            else
            {
                txtMsg.Text = "This username already exists. Please select a new username.";
            }

        }
    }
}
