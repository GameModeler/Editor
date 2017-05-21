using System.Windows;

namespace Editor.Views
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Setting window initialization.
        /// </summary>
        public SettingsWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            DataContext = Application.Current.MainWindow.DataContext;
        }

        #endregion

        #region Methods

        private void BottomButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
