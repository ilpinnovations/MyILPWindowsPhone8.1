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
    class NotificationModel
    {
        ObservableCollection<NotificationItem> _notificationItems;
        public ObservableCollection<NotificationItem> NotificationItems { get { return _notificationItems; } }
        public NotificationModel()
        {
            _notificationItems = new ObservableCollection<NotificationItem>();
        }
    }

    class NotificationItem : INotifyPropertyChanged
    {
        int _serialNo;
        string _msg;
        DateTime _date;

        public int SerialNo { get { return _serialNo; } set { _serialNo = value; OnPropChanged("SerialNo"); } }
        public string Message { get { return _msg; } set { _msg = value; OnPropChanged("Message"); } }
        public DateTime Date { get { return _date; } set { _date = value; OnPropChanged("Date"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
