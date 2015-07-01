using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MyFirstWebTest
{
    enum EFilter
    {
        JobName,
        CoName,
        CoSize,
        EFilterMax
    }
    class FilterAction
    {
        public FilterAction(Form1 f)
        {
            m_Form = f;
            m_szContents = new string[(int)EFilter.EFilterMax];
        }

        public bool isPass()
        {
            //int _webId = WebElementPool.WEB2_ID;
            string _szCoPattern = @"^((武汉)|(湖北))";
            string _szJobPattern1 = @"测试";
            string _szJobPattern2 = @"((应届)|(实习))";

            Regex r = new Regex(_szCoPattern);
            //如果是 武汉|湖北 开头的公司，规模小的屏蔽
            if (r.IsMatch(m_szContents[(int)EFilter.CoName]))
            {
                int _iSize = int.Parse(m_szContents[(int)EFilter.CoSize]);
                if ( 1000 > _iSize )
                    return false;
            }

            r = new Regex(_szJobPattern1);
            if (!r.IsMatch(m_szContents[(int)EFilter.JobName]))
                return false;

            r = new Regex(_szJobPattern2);
            if (r.IsMatch(m_szContents[(int)EFilter.JobName]))
                return false;

            //屏蔽指定公司
            foreach (string _szNo in SayNoCoName.names)
            {
                if (-1 != m_szContents[(int)EFilter.CoName].IndexOf(_szNo))
                {
                    return false;
                }
            }

            //m_Form.navigate(_webId, szUrl, 0);
            return true;
        }

        public void setContent(EFilter iPos, string szContent)
        {
            if (iPos >= EFilter.EFilterMax)
                return;

            m_szContents[(int)iPos] = szContent;
        }

        private Form1 m_Form;

        private string[] m_szContents;

    }
}
