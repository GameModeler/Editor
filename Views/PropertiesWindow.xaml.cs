using System.Windows;

namespace Editor.Views
{
    /// <summary>
    /// Interaction logic for PropertiesWindow.xaml
    /// </summary>
    public partial class PropertiesWindow : Window
    {
        #region Constructors

        public PropertiesWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void PropertiesBtn_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
