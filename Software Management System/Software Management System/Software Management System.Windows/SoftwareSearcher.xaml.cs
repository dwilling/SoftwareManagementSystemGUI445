﻿using Software_Management_System.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace Software_Management_System
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SoftwareSearcher : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
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

        static sqlHelper.Service1Client DBConnector = new sqlHelper.Service1Client();
        public SoftwareSearcher()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            
        }

        private async void PopulateListView(string searchCondition = "")
        {
            SoftwareList.Items.Clear();

            //Run the query
            string sqltext = "SELECT swe.SW_ID As ID,swe.SW_NAME As Software, swc.SWC_NAME As Category From SWEntries As swe Left Join SWCATEGORIES As swc On(swc.SWC_ID = swe.SW_CATEGORY_ID) Where swe.SW_Owner =" + Convert.ToInt32(userID);
            if(searchCondition != "")
            {
                sqltext += " And (swe.SW_NAME Like " + Quote("%" + searchCondition + "%") + " OR swc.SWC_NAME Like " + Quote("%" + searchCondition + "%") + ")";
            }
            var task = DBConnector.ExecuteQueryAsync(sqltext);
            List<List<object>> rows = await task;

            //Add items to list
            foreach (List<object> row in rows)
            {
                ListBoxItem lbItem = new ListBoxItem();
                lbItem.DoubleTapped += new DoubleTappedEventHandler(OpenWindow);
                lbItem.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                lbItem.BorderThickness = new Thickness(2);
                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };

                bool firstItem = true;
                foreach (object item in row)
                {
                    if (firstItem == false)
                    {
                        TextBlock tBlock = new TextBlock() { Text = item.ToString(),Width=400 };
                        panel.Children.Add(tBlock);
                    }
                    else 
                    {
                        firstItem = false;
                        lbItem.Tag = item.ToString();
                    }
                }                
                lbItem.Content = panel;
                SoftwareList.Items.Add(lbItem);
            }
        }

        private void OpenWindow(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBoxItem lbi = (ListBoxItem)sender;
            if(this.Frame != null)
                this.Frame.Navigate(typeof(SoftwareDetail), (lbi.Tag.ToString() + "," + userID));
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            userID = e.Parameter.ToString();
            navigationHelper.OnNavigatedTo(e);
            PopulateListView();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
                this.Frame.Navigate(typeof(SoftwareDetail), ("0," + userID));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PopulateListView(txtSearch.Text);
        }
        public string Quote(string temp)
        {
            string newString = "'" + temp.Replace("'", "\"") + "'";
            return newString;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (SoftwareList.SelectedItem != null)
            {
                DeleteEntry(((ListBoxItem)SoftwareList.SelectedItem).Tag.ToString());
                PopulateListView(txtSearch.Text);
            }
            else 
            {
                MessageDialog msgDialog = new MessageDialog("Please select an item to delete.", "Error");

                msgDialog.ShowAsync();
            }
        }

        private async void DeleteEntry(string deleteID)
        {
            var task = DBConnector.ExecuteNonQueryAsync("DELETE FROM SWENTRIES WHERE SW_ID=" + deleteID);
            bool completed = await task;
        }
    }
}
