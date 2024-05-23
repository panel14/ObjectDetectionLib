using HelixToolkit.Wpf.SharpDX;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PointsVisualization
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public EffectsManager EffectsManager { get; }

        public Camera Camera { get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new PerspectiveCamera();
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
