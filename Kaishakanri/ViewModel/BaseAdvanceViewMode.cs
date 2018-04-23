using Kaishakanri.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace kaishakanri.ViewModels
{
    public class BaseAdvanceViewMode<T> : NotificationObject
    {
        private T m_ObjMode;

        public T ObjMode
        {
            get { return m_ObjMode; }
            set { m_ObjMode = value; }
        }

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

        public Kaishakanri.Mode.KaishakanriDatabaseEntities GetDatabase()
        {
            return new Kaishakanri.Mode.KaishakanriDatabaseEntities(Common.ConnectString);
			
        }
    }
}
