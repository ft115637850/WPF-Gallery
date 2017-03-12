using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            this.MyImgsSrc = new List<BitmapSource>()
                    {
                        new BitmapImage(new Uri("pack://application:,,,/Pic/Chrysanthemum.jpg", UriKind.Absolute)),
                        new BitmapImage(new Uri("pack://application:,,,/Pic/Desert.jpg", UriKind.Absolute)),
                        new BitmapImage(new Uri("pack://application:,,,/Pic/Hydrangeas.jpg", UriKind.Absolute))
                    };
            InitializeComponent();
            this.DataContext = this;
        }

        public List<BitmapSource> MyImgsSrc
        {
            get;
            set;
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
