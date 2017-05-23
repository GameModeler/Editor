using System.Windows;
using Editor.ViewModels;
using Map.Models;

namespace Editor.Views
{
    /// <summary>
    /// Interaction logic for LayerWindow.xaml
    /// </summary>
    public partial class LayerWindow : Window
    {
        public LayerWindow()
        {
            Owner = Application.Current.MainWindow;
            DataContext = new LayerViewModel();
            InitializeComponent();
        }

        public LayerWindow(Layer layer) : this()
        {
            DataContext = new LayerViewModel(layer);
        }

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
