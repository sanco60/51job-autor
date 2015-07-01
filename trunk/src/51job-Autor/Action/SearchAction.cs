using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWebTest
{
    class SearchAction
    {
        public SearchAction(Form1 m)
        {
            m_Form = m;
        }

        public bool start(SSearchFactor sSearchFactor)
        {
            if (null == m_Form)
                return false;

            //DatabaseAdapter sDatabaseAdapter = DatabaseAdapter.instance();
            string _searchUrl = "";

            //string _keywordID = "keyword";
            int _webID = WebElementPool.WEB1_ID;

            string _btnSearch = @"/main_search_btn.gif.+=middle";

            Attribute _btnCareerName, _inputValue, _inputKeyAttr;
            _btnCareerName.szKey = WebElementPool.ID;
            _btnCareerName.szValue = "keyword_type_job";
            _inputValue.szKey = WebElementPool.VALUE;
            _inputValue.szValue = "测试";
            _inputKeyAttr.szKey = WebElementPool.CLASS;
            _inputKeyAttr.szValue = "textbox";

            m_Form.navigate(_webID, _searchUrl, 0);
            //第一次加载的网页会跳转，再等待一次
            m_Form.wait(_webID, 3000);

            //点击职位名
            m_Form.invokeMember(_webID, WebElementPool.LI, _btnCareerName, WebElementPool.CLICK);

            //填入搜索关键字
            m_Form.setAttribute(_webID, WebElementPool.INPUT, _inputKeyAttr, _inputValue);

            //点击搜索按钮            
            m_Form.invokeMember2(_webID, WebElementPool.INPUT, _btnSearch, WebElementPool.CLICK);

            m_Form.wait(_webID, 0);

            Console.WriteLine("===Search over.===");
            return true;
        }

        private Form1 m_Form;

        //private SSearchFactor m_SearchFactor;
    }    

    enum ESearchCata
    {
        EJobName,
        ECoName,
        EContent
    }
}
