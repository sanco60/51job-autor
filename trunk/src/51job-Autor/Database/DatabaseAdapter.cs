using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWebTest
{
    class DatabaseAdapter
    {
        public DatabaseAdapter()
        {
            m_DatabaseAccesor = DatabaseAccesor.instance();
        }

        public void getDatas(ref DatabaseObj[] list, out int iRest)
        {
            m_DatabaseAccesor.getDatas(ref list, out iRest);
            return;
        }

        //静态成员
        //private static DatabaseAdapter s_DatabaseAdapter;

        private DatabaseAccesor m_DatabaseAccesor;

    }    
}

