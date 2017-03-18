using System.ComponentModel;
using System.Windows;
using Editor.ViewModels;

namespace Editor.Views
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Constructors

        public PropertiesWindow()
        {
            InitializeComponent();

            Closing += PropertiesWindow_Closing;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Makes the window disappear without closing it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertiesBtn_Clicked(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Prevents the window from closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertiesWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        #endregion
    }
}
