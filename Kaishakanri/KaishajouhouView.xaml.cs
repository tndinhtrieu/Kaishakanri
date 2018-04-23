using Kaishakanri.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Kaishakanri
{
    /// <summary>
    /// KaishajouhouView.xaml の相互作用ロジック
    /// </summary>
    public partial class KaishajouhouView : Window
    {
        public KaishajouhouView()
        {
            InitializeComponent();
        }

        private void KaishajouhoViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DialogResult")
            {
                this.DialogResult = (this.FindResource("KaishajouhoViewModel") as KaishajouhoViewModel).DialogResult;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            /////

        }
    }
}
