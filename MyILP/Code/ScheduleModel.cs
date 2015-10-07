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
    class ScheduleModel
    {
        ObservableCollection<ScheduleItem> _scheduleItems;
        public ObservableCollection<ScheduleItem> ScheduleItems { get { return _scheduleItems; } }
        public ScheduleModel()
        {
            _scheduleItems = new ObservableCollection<ScheduleItem>();
        }
    }

    class ScheduleItem : INotifyPropertyChanged
    {
        string _date1, _batch, _slot,
            _course, _faculty, _room, _result;

        public string Date1 { get { return _date1; } set { _date1 = value; OnPropChanged("Date1"); } }
        public string Batch { get { return _batch; } set { _batch = value; OnPropChanged("Batch"); } }
        public string Slot { get { return _slot; } set { _slot = value; OnPropChanged("Slot"); } }
        public string Course { get { return _course; } set { _course = value; OnPropChanged("Course"); } }
        public string Faculty { get { return _faculty; } set { _faculty = value; OnPropChanged("Faculty"); } }
        public string Room { get { return _room; } set { _room = value; OnPropChanged("Room"); } }
        public string Result { get { return _result; } set { _result = value; OnPropChanged("Result"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ScheduleItemConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string temp = (string)value;
            string param = (string)parameter;

            switch (param)
            {
                case "slot":
                    temp = "Slot: " + temp;
                    break;
                case "room":
                    temp = "Room: " + temp;
                    break;
                default:
                    break;
            }

            return temp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
