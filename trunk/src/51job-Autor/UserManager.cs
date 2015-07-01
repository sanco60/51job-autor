using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWebTest
{
    class UserManager
    {
        private UserManager()
        {
            m_uaList = new List<SUserAccount>();
            m_DatabaseAdapter = new DatabaseAdapter();
        }

        public static bool init()
        {
            if (null != s_UserManager)
                return false;

            s_UserManager = new UserManager();

            if (!s_UserManager.restore())
                return false;

            return true;
        }

        public static UserManager instance()
        {
            return s_UserManager;
        }

        public bool getUserAccount(ref SUserAccount ua)
        {
            if (0 == m_uaList.Count)
                return false;

            ua = m_uaList[0];
            return true;
        }

        public bool restore()
        {
            SUserAccount _ua = new SUserAccount();
            int userLength = 1;
            DatabaseObj[] _uaaList = new DatabaseObj[userLength];
            _uaaList[0] = new UserAccountAdapter();

            m_DatabaseAdapter.getDatas(ref _uaaList);

            Object _oUA = _ua;
            _uaaList[0].put(ref _oUA);
            
            return true;
        }

        private static UserManager s_UserManager;

        private List<SUserAccount> m_uaList;

        private DatabaseAdapter m_DatabaseAdapter;
    }
}
