using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Standalone
{
    abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            field = value;
            OnPropertyChanged(propertyName);
        }

        protected void SetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            SetIfChanged(ref field, value, null, propertyName);
        }

        protected void SetIfChanged<T>(ref T field, T value, IEqualityComparer<T> comparer, [CallerMemberName] string propertyName = "")
        {
            IEqualityComparer<T> c = comparer ?? EqualityComparer<T>.Default;

            if (!c.Equals(field, value))
            {
                Set(ref field, value, propertyName);
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
