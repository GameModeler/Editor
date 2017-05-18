using System.Windows;
using Editor.ViewModels;

namespace Editor.Views
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        #region Properties

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Setting window initialization.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
