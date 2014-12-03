using SMS.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SMS
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SoftwareDetail : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        static sqlHelper.Service1Client DBConnector = new sqlHelper.Service1Client();
        public string userID;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        string ID = string.Empty;

        public SoftwareDetail()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        string[] data = new string[12];
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] temp = (e.Parameter.ToString()).Split(',');
            ID = temp[0];
            userID = temp[1];
            navigationHelper.OnNavigatedTo(e);
            LoadScreen();
            SetColor();
        }
        string currentColor = "Black";
        private async void SetColor()
        {
            var task3 = DBConnector.ExecuteQueryAsync("SELECT USERS_SUGARCRMID FROM USERS WHERE USERS_ID=" + userID);
            List<List<object>> rows3 = await task3;
            string[] tempArray = new string[15];
            //Add items to list
            foreach (List<object> row in rows3)
            {
                foreach (object item in row)
                {
                    currentColor = item.ToString();
                }
            }
            if (currentColor == string.Empty)
            {
                currentColor = "Black";
            }
            switch (currentColor)
            {
                case "Black":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Black);
                    break;
                case "Blue":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
                    break;
                case "Green":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case "Indigo":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Indigo);
                    break;
                case "Orange":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Orange);
                    break;
                case "Red":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
                case "Violet":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Violet);
                    break;
                case "White":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.White);
                    break;
                case "Yellow":
                    mainGrid.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
            }

        }
        private async void LoadScreen()
        {
            if (ID != "0")
            {
                var task = DBConnector.ExecuteQueryAsync("SELECT * FROM SWEntries Inner Join SWCategories On(SWC_ID = SW_CATEGORY_ID) Where SW_ID=" + ID);
                List<List<object>> rows = await task;

                //Add items to list
                foreach (List<object> row in rows)
                {
                    int counter = 0;
                    foreach (object item in row)
                    {
                        data[counter] = item.ToString();
                        counter++;
                    }
                }

                txtName.Text = data[2]; //2
                txtDLLink.Text = data[3]; //3
                txtResourceLink.Text = data[4]; //4
                txtCDKeys.Text = data[5]; //5
                txtNotes.Text = data[6];//6 


                LoadCombo(cmbCategories, data[10]);
            }
            else
            {
                LoadCombo(cmbCategories);
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ScreenToData();
        }

        public async void LoadCombo(ComboBox cmbBox, string defaultSelection = "")
        {
            var task = DBConnector.ExecuteQueryAsync("SELECT SWC_NAME,SWC_ID FROM SWCATEGORIES ORDER BY SWC_NAME ASC");
            List<List<object>> rows = await task;

            //Add items to list
            foreach (List<object> row in rows)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                int i = 0;
                foreach (object item in row)
                {
                    if (i == 0)
                    {
                        cbi.Content = item.ToString();
                        i++;
                    }
                    else
                    {
                        cbi.Tag = item.ToString();
                        i = 0;
                    }
                }
                cmbBox.Items.Add(cbi);
            }
            foreach (ComboBoxItem comboItem in cmbBox.Items)
            {
                if (comboItem.Content != null)
                    if (comboItem.Content.ToString() == defaultSelection)
                        cmbBox.SelectedValue = comboItem;
            }
        }
        private async void ScreenToData()
        {
            string category = string.Empty;
            string catID = "4";
            if (cmbCategories.SelectedItem != null)
            {
                if (((ComboBoxItem)cmbCategories.SelectedItem).Content != null)
                {
                    category = ((ComboBoxItem)cmbCategories.SelectedItem).Content.ToString();
                    catID = ((ComboBoxItem)cmbCategories.SelectedItem).Tag.ToString();
                }
            }
            if (ID == "0")
            {
                var task = DBConnector.ExecuteNonQueryAsync("INSERT INTO SWENTRIES Values (" + catID + "," + Quote(txtName.Text) + "," + Quote(txtDLLink.Text) + "," + Quote(txtResourceLink.Text) + "," + Quote(txtCDKeys.Text) + "," + Quote(txtNotes.Text) + ",NULL," + userID + ")");
                bool completed = await task;
            }
            else
            {
                var task = DBConnector.ExecuteNonQueryAsync("Update SWENTRIES Set SW_CATEGORY_ID=" + catID + ",SW_NAME=" + Quote(txtName.Text) + ",SW_DL_LINK=" + Quote(txtDLLink.Text) + ",SW_RESOURCE_LINK=" + Quote(txtResourceLink.Text) + ",SW_CD_KEYS=" + Quote(txtCDKeys.Text) + ",SW_NOTES=" + Quote(txtNotes.Text) + ",SW_DATE_PURCHASED=NULL,SW_OWNER=" + Quote(userID) + " Where SW_ID =" + ID);
                bool completed = await task;
            }
        }
        public string Quote(string temp)
        {
            string newString = "'" + temp.Replace("'", "\"") + "'";
            return newString;
        }

        private void btnMount_Click(object sender, RoutedEventArgs e)
        {
            if(!txtDLLink.Text.Contains(".iso"))
            {
                MessageDialog msg = new MessageDialog("Please type the path of a valid ISO file into the Source Field.","Error");
                msg.ShowAsync();
            }
            else
                MountMe(txtDLLink.Text);              
        }

        private async void MountMe(string loc)
        {
            var task = DBConnector.MountIsoAsync(loc);
            bool completed = await task;
           
        }
    }
}
