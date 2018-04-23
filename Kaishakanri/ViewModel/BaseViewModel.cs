using Kaishakanri.Helpers;

namespace Kaishakanri.ViewModels
{
    public class BaseViewModel : NotificationObject
    {
        private Kaishakanri.Mode.KaishakanriDatabaseEntities m_databaseObj;

        public Kaishakanri.Mode.KaishakanriDatabaseEntities DatabaseObj
        {
            get { return m_databaseObj; }
            set
            {
                m_databaseObj = value;
                RaisePropertyChanged(() => DatabaseObj);
            }
        }

        public Kaishakanri.Mode.KaishakanriDatabaseEntities GetVBHS()
        {
            return new Mode.KaishakanriDatabaseEntities(Helpers.Common.ConnectString);
        }
    }
}