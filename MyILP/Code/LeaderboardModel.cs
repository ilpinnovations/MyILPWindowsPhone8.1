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
    class LeaderboardModel
    {
        public int MyPoints { get; set; }
        ObservableCollection<LeaderboardItem> _leaderboardItems;
        public ObservableCollection<LeaderboardItem> LeaderboardItems { get { return _leaderboardItems; } }
        public LeaderboardModel()
        {
            MyPoints = 0;   
            _leaderboardItems = new ObservableCollection<LeaderboardItem>();
        }
    }

    class LeaderboardItem : INotifyPropertyChanged
    {
        string _id, _name, _batch;
        int _points;

        public string EmployeeId { get { return _id; } set { _id = value; OnPropChanged("EmployeeId"); } }
        public string EmployeeName { get { return _name; } set { _name = value; OnPropChanged("EmployeeName"); } }
        public string Batch { get { return _batch; } set { _batch = value; OnPropChanged("Batch"); } }
        public int Points { get { return _points; } set { _points = value; OnPropChanged("Points"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    class PointsToBadgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int index = AchievementsHelper.GetLevel((int)value);
            string imgPath = string.Format("/Assets/Badges/badge{0}.png", index);

            return imgPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
