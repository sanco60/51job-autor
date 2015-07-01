using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MyFirstWebTest
{
    class ApplyAction
    {
        public ApplyAction(Form1 f)
        {
            m_Form = f;
            m_FilterAction = new FilterAction(f);
        }

        public bool start()
        {
            if (null == m_Form)
                return false;

            int _webID = WebElementPool.WEB1_ID;
            //string _szRegex = @"class=tr0";
            string _szCoRegex = @"class=coname";
            string _szJobRegex = @"class=jobname";
            string _szCoDetailsPattern = @"class=td1234";
            string _szCheckRegex = @"select";
            string _szCoName = "";
            string _szJobName = "";
            string _szCoDetails = "";
            string _szBtnApply = @"申请选中职位";
            string _szBtnNext = @"/pageron.gif";
            string _szScalePattern = @"\d+(?=人)";

            Attribute _attr, _classPattern, _btnClose;
            _attr.szKey = WebElementPool.HREF;
            _attr.szValue = "";
            _classPattern.szKey = WebElementPool.CLASS;
            _classPattern.szValue = @"^tr[01]";
            _btnClose.szKey = WebElementPool.ID;
            _btnClose.szValue = "window_close_apply";

            Regex _rg = new Regex(_szScalePattern);
            bool bNext = false;
            do {
                m_Form.cacheElements2(_webID, WebElementPool.TR, _classPattern);
                while ( 0 < m_Form.cacheCount() )
                {
                    //获取职位名称
                    m_Form.nextTextInCache(WebElementPool.A, _szJobRegex, ref _szJobName);
                    //获取公司名称
                    m_Form.nextTextInCache(WebElementPool.A, _szCoRegex, ref _szCoName);
                    
                    m_Form.cacheNext();
                    //获取公司规模
                    m_Form.nextTextInCache(WebElementPool.TD, _szCoDetailsPattern, ref _szCoDetails);
                    
                    m_FilterAction.setContent(EFilter.JobName, _szJobName);
                    m_FilterAction.setContent(EFilter.CoName, _szCoName);
                    m_FilterAction.setContent(EFilter.CoSize, _rg.Match(_szCoDetails).ToString());

                    if (m_FilterAction.isPass())
                    {
                        //Console.WriteLine("+++ 公司名：{0} +++", _szCoName);
                        m_Form.nextInvokeInCache(WebElementPool.INPUT, _szCheckRegex, WebElementPool.CLICK);
                    }
                    m_Form.cacheNext();
                }
                //点击申请选中职位
                m_Form.invokeMember2(_webID, WebElementPool.A, _szBtnApply, WebElementPool.CLICK);
                m_Form.wait(_webID, 5000);
                
                //关闭弹出子窗口
                m_Form.invokeMember(_webID, WebElementPool.IMG, _btnClose, WebElementPool.CLICK);

                //点击下一页
                bNext = m_Form.invokeMember2(_webID, WebElementPool.A, _szBtnNext, WebElementPool.CLICK);
                if (bNext)
                    m_Form.wait(_webID, 0);

            } while (bNext);
            
            Console.WriteLine("===ApplyAction over.===");
            return true;
        }

        private Form1 m_Form;

        private FilterAction m_FilterAction;
    }
}
