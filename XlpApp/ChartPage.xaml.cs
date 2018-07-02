using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

//using JEXEServerLib;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XlpApp.UserControls;

namespace XlpApp
{
    /// <summary>
    /// Interaction logic for ChartPage.xaml
    /// </summary>
    public partial class ChartPage : Page
    {
        #region Private Fields

        private static int uniqeTabName = 0;

        private List<TabItem> tabItems;

        #endregion Private Fields

        #region Public Constructors

        public ChartPage()
        {
            InitializeComponent();

            DataContext = this;
            tabItems = new List<TabItem>();

            InitializeTabInterface();
        }

        #endregion Public Constructors

        #region Eventhandlers

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabDynamic
                .Items.Cast<TabItem>()
                .Where(i => i.Name.Equals(tabName))
                .SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                if (tabItems.Count < 3)
                {
                    MessageBox.Show("Cannot remove last tab.");
                }
                else
                {
                    var result = MessageBox.Show(
                        string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
                        "Remove Tab", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        TabItem selectedTab = tabDynamic.SelectedItem as TabItem;
                        tabDynamic.DataContext = null;
                        tabItems.Remove(tab);
                        tabDynamic.DataContext = tabItems;

                        if (selectedTab == null || selectedTab.Equals(tab))
                        {
                            selectedTab = tabItems[0];
                        }
                        tabDynamic.SelectedItem = selectedTab;
                    }
                }
            }
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            TabItem tab = tabDynamic.SelectedItem as TabItem;
            if (e.Source is TabControl && tab != null && tab.Header != null)
            {
                if (tab.Header.Equals("+"))
                {
                    tabDynamic.DataContext = null;
                    TabItem newTab = this.AddNewTab();

                    tabDynamic.DataContext = tabItems;
                    tabDynamic.SelectedItem = newTab;
                }
                else
                {
                    // your code here...
                }
            }
        }

        #endregion Eventhandlers

        #region Helper methods

        #region Tab logic

        private void InitializeTabInterface()
        {
            TabItem plusTab = new TabItem();
            // plusTab.Header = "+";

            plusTab.Header = "+";
            plusTab.HeaderTemplate = tabDynamic.FindResource("AddNewTabTemplate") as DataTemplate;
            plusTab.Background = new SolidColorBrush(Colors.Black);
            tabItems.Add(plusTab);
            this.AddNewTab();
            tabDynamic.DataContext = tabItems;
            tabDynamic.SelectedIndex = 0;
        }

        private TabItem AddNewTab()
        {
            uniqeTabName++;
            int count = tabItems.Count;

            TabItem tab = new TabItem();
            tab.Background = new SolidColorBrush(Colors.Black);
            tab.Header = string.Format("Tab {0}", uniqeTabName);
            tab.Name = string.Format("tabItem{0}", uniqeTabName);
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            tab.Content = new ChartUserControl();
            tabItems.Insert(count - 1, tab);
            return tab;
        }

        #endregion Tab logic

        #endregion Helper methods

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged members

        private void addNewTab_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

internal class Stock
{
    #region Public Properties

    public string Name { get; set; }

    #endregion Public Properties
}