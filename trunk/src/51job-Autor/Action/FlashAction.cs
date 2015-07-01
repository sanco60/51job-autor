using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWebTest
{
    class FlashAction
    {
        public FlashAction(Form1 f)
        {
            m_Form = f;
        }

        public bool start()
        {
            int _webId = WebElementPool.WEB1_ID;
            string _szMyResumePattern = @"我的简历";
            string _szRefreshClose = @"Refresh_success_LastUpdate";
            Attribute _aBtnRefresh, _aBtnSave;
            _aBtnRefresh.szKey = WebElementPool.CLASS;
            _aBtnRefresh.szValue = "iconRefresh";
            _aBtnSave.szKey = WebElementPool.CLASS;
            _aBtnSave.szValue = "panel_btn_s";

            m_Form.invokeMember2(_webId, WebElementPool.A, _szMyResumePattern, WebElementPool.CLICK);
            m_Form.wait(_webId, 0);

            m_Form.cacheElements2(_webId, WebElementPool.A, _aBtnRefresh);
            while(0 < m_Form.cacheCount())
            {
                m_Form.nextInvokeInCache(WebElementPool.CLICK);
                m_Form.wait(_webId, 3000);
                //Console.WriteLine("点击确定");
                m_Form.invokeMember(_webId, WebElementPool.A, _aBtnSave, WebElementPool.CLICK);
                m_Form.wait(_webId, 3000);
                //Console.WriteLine("点击关闭");
                m_Form.invokeMember2(_webId, WebElementPool.A, _szRefreshClose, WebElementPool.CLICK);
                m_Form.cacheNext();
            }

            Console.WriteLine("===FlashAction over.===");
            return true;
        }

        private Form1 m_Form;
    }
}
