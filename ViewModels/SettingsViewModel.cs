using Editor.ViewModels.Base;
using Map.Models;

namespace Editor.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Attributes

        private World _world;

        #endregion

        #region Properties

        public World World
        {
            get => _world;
            set
            {
                _world = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public SettingsViewModel()
        {
            World = new World();
        }

        public SettingsViewModel(World world)
        {
            World = world;
        }

        #endregion
    }
}
