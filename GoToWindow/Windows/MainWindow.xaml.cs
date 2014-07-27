﻿using GoToWindow.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoToWindow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var windows = WindowsListFactory.Load();
            windowsListView.ItemsSource = windows.Windows;

            if (windowsListView.Items.Count > 1)
                windowsListView.SelectedIndex = 1;
            else if (windowsListView.Items.Count > 0)
                windowsListView.SelectedIndex = 0;
        }

        private void windowsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FocusSelectedWindowItem();
        }

        private void FocusSelectedWindowItem()
        {
            var windowEntry = windowsListView.SelectedItem as IWindowEntry;
            if (windowEntry != null)
            {
                windowEntry.Focus();
                Close();
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var currentSelection = windowsListView.SelectedItem;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(windowsListView.ItemsSource);
            view.Filter = item => SearchFilter((IWindowEntry)item, searchTextBox.Text);

            if (windowsListView.SelectedIndex == -1 && windowsListView.Items.Count > 0)
                windowsListView.SelectedIndex = 0;
        }

        private bool SearchFilter(IWindowEntry window, string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            return StringContains(window.ProcessName + " " + window.Title, text);
        }

        private static bool StringContains(string text, string partial)
        {
            return partial.Split(' ').All(word => CultureInfo.CurrentUICulture.CompareInfo.IndexOf(text, word, CompareOptions.IgnoreCase) > -1);
        }

        private void searchBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Focus();
            searchTextBox.CaretIndex = searchTextBox.Text.Length;
        }

        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                FocusSelectedWindowItem();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down && windowsListView.SelectedIndex < windowsListView.Items.Count - 1)
            {
                windowsListView.SelectedIndex++;
                windowsListView.ScrollIntoView(windowsListView.SelectedItem);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Up && windowsListView.SelectedIndex > 0)
            {
                windowsListView.SelectedIndex--;
                windowsListView.ScrollIntoView(windowsListView.SelectedItem);
                e.Handled = true;
                return;
            }
        }

        private void windowsListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusSelectedWindowItem();
                e.Handled = true;
                return;
            }
        }

        private void clearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
