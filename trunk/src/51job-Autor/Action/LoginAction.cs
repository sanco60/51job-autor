using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MyFirstWebTest
{
    class LoginAction
    {
        public LoginAction(Form1 m)
        {
            m_Form = m;
        }

        public bool start()
        {
            if (null == m_Form)
                return false;

            UserManager _ua = UserManager.instance();
            SUserAccount _account = _ua.getUserAccount();
            if (null == _account)
                return false;

            string _loginUrl = "http://www.51job.com/";

            string _userNameId = "username";
            string _passWordId = "userpwd";
            int _webID = WebElementPool.WEB1_ID;

            Attribute _autoKey, _loginKey;
            _autoKey.szKey = WebElementPool.NAME;
            _autoKey.szValue = "autologin";
            _loginKey.szKey = WebElementPool.SRC;
            _loginKey.szValue = @"main_login_btn.gif";

            m_Form.navigate(_webID, _loginUrl, 15000);
            m_Form.setAttribute(_webID, _userNameId, _account.szID);
            m_Form.setAttribute(_webID, _passWordId, _account.szPassword);
            
            m_Form.invokeMember(_webID, _autoKey, WebElementPool.CLICK);
            m_Form.invokeMember(_webID, _loginKey, WebElementPool.CLICK);
            m_Form.wait(_webID, 15000);

            Console.WriteLine("===Login over.===");
            return true;
        }

        private Form1 m_Form;
    }
}
