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
            _cfr.read();

            List<List<string>> _valueRows = _cfr.getValueRows();
            List<string> _attrNames = _cfr.getAttrNames();
                        
            for (int _i = 0, _indexList = 0; _i < _valueRows.Count; _i++, _indexList++)
            {
                if (_i < iStart || _valueRows[_i].Count != _attrNames.Count)
                    continue;
                if (_indexList == list.Length)
                {
                    iRest = _valueRows.Count - _i;
                    break;
                }
                for (int _j = 0; _j < _attrNames.Count; _j++)
                {
                    list[_indexList].setValue(_attrNames[_j], _valueRows[_i][_j]);
                }
            }
            return true;
        }

        private static DatabaseAccesor s_DatabaseAccesor;
    }
}
