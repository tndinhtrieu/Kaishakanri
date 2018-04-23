using Kaishakanri.Helpers;
using Kaishakanri.Mode;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kaishakanri.ViewModels
{
    public class KaishajouhoViewModel : BaseViewModel
    {

        public string _Path;
        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        private ObservableCollection<Kaishakanri.Mode.Kaishajouhou> _listObj;

        public ObservableCollection<Kaishakanri.Mode.Kaishajouhou> ListObj
        {
            get { return _listObj; }
            set
            {
                _listObj = value;
                RaisePropertyChanged(() => ListObj);
            }
        }

        private bool? m_DialogResult;

        public bool? DialogResult
        {
            get { return m_DialogResult; }
            set
            {
                m_DialogResult = value;
                RaisePropertyChanged(() => DialogResult);
            }
        }

        public KaishajouhoViewModel()
        {
            ListObj= Kaishakanri.Commons.CollectionHelper.LoadXml(@"C:\data.xml", typeof(ObservableCollection<Kaishakanri.Mode.Kaishajouhou>));

        }
        public ICommand OKCommand { get { return new DelegateCommand(oKCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(cancelCommand); } }

        private void cancelCommand()
        {
            DialogResult = false;
        }

        private void oKCommand()
        {
            DialogResult = true;
        }

        
    }
}
