﻿namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        public ErrorWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}