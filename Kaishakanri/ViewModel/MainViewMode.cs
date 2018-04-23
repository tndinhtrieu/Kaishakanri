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
    public class MainViewMode : BaseViewModel
    {
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
        private string _Path;
        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                RaisePropertyChanged(() => Path);
            }
        }

        public ICommand ImportdataCommand { get { return new DelegateCommand(ImportData); } }
        public ICommand InsertTodatabaseCommand { get { return new DelegateCommand(insertTodatabaseCommand); } }

        private void insertTodatabaseCommand()
        {
            if(ListObj!=null&&ListObj.Count>0)
            { 
                
            foreach(Kaishakanri.Mode.Kaishajouhou obj in ListObj)
            {
                obj.ID=Guid.NewGuid().ToString();
                DatabaseObj.Kaishajouhous.Add(obj);

            }
            DatabaseObj.SaveChanges();
            }
        }
        private void ImportData()
        {
           
            //Kaishajouhou obj = new Kaishajouhou() { ID = Guid.NewGuid().ToString(), Kaishaname = "ゼロ four　株式会社", Daihyoumei = "石田", Denwa = "09011275934", Fax = "09011275934" };
            //listObj.Add(obj);
            //obj = new Kaishajouhou() { ID = Guid.NewGuid().ToString(), Kaishaname = "helloworld　株式会社", Daihyoumei = "Mr Phuong", Denwa = "09011275934", Fax = "09011275934" };
            //listObj.Add(obj);

           
           

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Text files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            KaishajouhouView view = new KaishajouhouView();
            if (openFileDialog.ShowDialog() == true)
            {

                dynamic listObj = Kaishakanri.Commons.CollectionHelper.LoadXml(openFileDialog.FileName, typeof(ObservableCollection<Kaishakanri.Mode.Kaishajouhou>));
                (view.FindResource("KaishajouhoViewModel") as Kaishakanri.ViewModels.KaishajouhoViewModel).ListObj = listObj;
                bool isshowdialog = view.ShowDialog().Value;
                if(isshowdialog)
                {
                    Path = openFileDialog.FileName;
                    ListObj = listObj;
                }

                
            }
            //bool value = view.ShowDialog().Value;
        }
    }
}
