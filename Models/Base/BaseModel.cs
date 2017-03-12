using System.ComponentModel;
using System.Runtime.CompilerServices;
using Editor.Annotations;

namespace Editor.Models.Base
{
    public class BaseModel : INotifyPropertyChanged
    {
        #region Attributes
        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors
        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
