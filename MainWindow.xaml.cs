using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Editor.ViewModels;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes
        #endregion

        #region Properties

        public EditorViewModel EditorViewModel { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            // Initialization
            InitializeComponent();
            EditorViewModel = new EditorViewModel();
            DataContext = EditorViewModel;

            // Command Bindings
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenCommandExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommandExecuted, SaveCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseCommandExecuted));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, BrowseCommandExecuted, BrowseCommandCanExecute));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteCommandExecuted, DeleteCommandCanExecute));

            // Event handlers
            Closing += MainWindow_Closing;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Command invoked to create a new map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Command invoked to open a previously saved map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the currently opened map can be saved during the save command invoke. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        /// <summary>
        /// Command invoked to save the current map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Command invoked to quit the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Checks if new assets can be added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        /// <summary>
        /// Command invoked to browse for assets to load into the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the assets can be deleted from the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        /// <summary>
        /// Command invoked to delete assets from the editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs checks to avoid closing the editor without saving changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Bye!");
        }

        #endregion
    }
}
