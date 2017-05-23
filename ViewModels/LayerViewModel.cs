using System;
using System.Windows;
using Editor.ViewModels.Base;
using Map.Models;

namespace Editor.ViewModels
{
    public class LayerViewModel : BaseViewModel
    {
        #region Attributes

        private Layer _layer;

        #endregion

        #region Properties

        public Layer Layer
        {
            get => _layer;
            set
            {
                _layer = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public LayerViewModel()
        {
            Layer = new Layer("My Layer");
        }

        public LayerViewModel(Layer layer)
        {
            Layer = layer;
        }

        #endregion

        #region Methods

        private void ApplyButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
