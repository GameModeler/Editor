using System.Windows;
using Editor.ViewModels;
using Map.Models;

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
        public SettingsWindow()
        {
            Owner = Application.Current.MainWindow;
            DataContext = new SettingsViewModel();
            InitializeComponent();
        }

        public SettingsWindow(World world) : this()
        {
            DataContext = new SettingsViewModel(world);
        }

        #endregion

        #region Methods

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}
