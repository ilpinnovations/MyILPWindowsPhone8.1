using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyILP.Design
{
    public sealed partial class StarRateControl : UserControl
    {
        public DependencyProperty valueProperty;
        public int Value { get { return (int)GetValue(valueProperty); } set { SetValue(valueProperty, value); UpdateOffsetByValue(value); } }
        public double Offset { get { return offset1.Offset; } 
            set 
            { 
                offset1.Offset = offset2.Offset = value;
            } 
        }
        private void UpdateOffsetByValue(int value)
        {
            Offset = value * 0.2;
        }
        public StarRateControl()
        {
            this.InitializeComponent();
            
            valueProperty = DependencyProperty.Register("Value", typeof(int), typeof(StarRateControl), new PropertyMetadata(3));
            
        }

        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Point point = e.GetPosition(this);
            double normalized = point.X / (sender as UserControl).ActualWidth;
            double rawRate = normalized * 5;
            int rateValue = (int)rawRate + 1;

            double newOffsetValue = 0.2 * rateValue;
            Offset = newOffsetValue;
            Value = rateValue;
#if DEBUG
            string dbgString = string.Format("NO:{0},Raw:{1},Rate:{2}", newOffsetValue, rawRate, rateValue);
            System.Diagnostics.Debug.WriteLine(dbgString);
#endif
        }
    }
}
