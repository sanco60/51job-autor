using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWebTest
{
    class DatabaseAccesor
    {
        private DatabaseAccesor()
        { }

        public static DatabaseAccesor instance()
        {
            if (null == s_DatabaseAccesor)
                s_DatabaseAccesor = new DatabaseAccesor();
            return s_DatabaseAccesor;
        }

        public bool getDatas(ref DatabaseObj[] list, out int iRest)
        {
            return getDatas(ref list, 0, out iRest);
        }

        public bool getDatas(ref DatabaseObj[] list, int iStart, out int iRest)
        {
            iRest = 0;
            if (null == list || 0 == list.Length)
            {
                return false;
            }

            string _szFileName = list[0].getTitle() + AutorConstPool.FILE_SUFFIX;
            ConfigFileReader _cfr = new ConfigFileReader(_szFileName);
            _cfr.read(iStart, iStart + list.Length)
            return true;
        }

        private static DatabaseAccesor s_DatabaseAccesor;
    }
}
