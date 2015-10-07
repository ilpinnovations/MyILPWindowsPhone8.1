using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Data;

namespace MyILP.Code
{
    class EContactModel
    {
        ObservableCollection<EContactItem> _eContactItems;
        public ObservableCollection<EContactItem> EContactItems { get { return _eContactItems; } }
        public EContactModel()
        {
            _eContactItems = new ObservableCollection<EContactItem>();
        }
    }

    class EContactItem : INotifyPropertyChanged
    {
        string _name, _contact;

        public string Name { get { return _name; } set { _name = value; OnPropChanged("Name"); } }
        public string Contact { get { return _contact; } set { _contact = value; OnPropChanged("Contact"); } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
