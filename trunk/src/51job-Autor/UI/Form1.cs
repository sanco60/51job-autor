﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using SHDocVw;
using System.Runtime.InteropServices;

namespace MyFirstWebTest
{    
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, IntPtr wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public Form1()
        {
            InitializeComponent();

            m_Thread = new Thread(start);
            m_CloseThread = new Thread(closeMessageBox);
            m_bThreadStarted = false;
            m_bCloseThreadStop = false;
            m_Web1Event = new AutoResetEvent(false);
            m_Web2Event = new AutoResetEvent(false);
            m_bWeb1EventHandled = false;
            m_elementsQueue = new Queue<HtmlElement>();            

            mNavigate = new NavigateDelegate(navigate);
            mSetAttribute = new SetAttributeDelegate(setAttribute);
            mSetAttribute2 = new SetAttribute2Delegate(setAttribute2);
            mInvokeMember = new InvokeMemberDelegate(invokeMember);
            mInvokeMember2 = new InvokeMember2Delegate(invokeMember2);
            mCacheElements = new CacheElementsDelegate(cacheElements);
            mNextValueInCache = new NextValueInCacheDelegate(nextValueInCache);
            mNextInvokeInCache = new NextInvokeInCacheDelegate(nextInvokeInCache);
            mNextTextInCache = new NextTextInCacheDelegate(nextTextInCache);
            mCacheElements2 = new CacheElements2Delegate(cacheElements2);
            mNextInvokeInCache2 = new NextInvokeInCache2Delegate(nextInvokeInCache);

            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        public static void start(object _obj)
        {            
            Form1 _f = (Form1)_obj;
            if ( null == _f )
            {
                Console.WriteLine("Error: Thread didn't start.");
                return;
            }
            if (null == s_ScenarioProcessor)
            {
                s_ScenarioProcessor = new ScenarioProcessor(_f);
                s_ScenarioProcessor.setForm(_f);
            }
            s_ScenarioProcessor.start();
            return;
        }

        private void start_btn_Click(object sender, EventArgs e)
        {
            if (true == m_bThreadStarted)
                return;

            m_Thread.Start(this);
            m_CloseThread.Start();
            m_bThreadStarted = true;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (WebBrowserReadyState.Complete == webBrowser1.ReadyState
                && e.Url == webBrowser1.Document.Url)
            {
                if ( false == m_bWeb1EventHandled )
                {
                    m_bWeb1EventHandled = true;
                    SHDocVw.WebBrowser shWeb = (SHDocVw.WebBrowser)this.webBrowser1.ActiveXInstance;
                    shWeb.NewWindow3 += new DWebBrowserEvents2_NewWindow3EventHandler(this.webBrowser1_OpeningNew);
                }
                m_Web1Event.Set();
            }
        }

        private void webBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (WebBrowserReadyState.Complete == webBrowser2.ReadyState
                && e.Url == webBrowser2.Document.Url)
            {
                m_Web2Event.Set();
            }
        }

        private void webBrowser1_OpeningNew(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            Cancel = true;
            this.webBrowser1.Navigate(bstrUrl);
            //this.navigate(WebElementPool.WEB1_ID, , 0);
        }

        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Form1_FormClosing");
            if (!m_bThreadStarted)
                return;

            m_Thread.Join();
            m_bCloseThreadStop = true;
            m_CloseThread.Join();
        }

        public void wait(int iWebID, int msTime)
        {
            AutoResetEvent _e = (WebElementPool.WEB1_ID == iWebID) ? m_Web1Event : m_Web2Event;
            if (0 == msTime)
            {
                _e.WaitOne();
            } else
            {
                _e.WaitOne(msTime);
            }
        }

        public void navigate(int id, string url, int msTime)
        {
//             if (this.InvokeRequired)
//             {
//                 this.Invoke(mNavigate, new object[] { id, url, msTime });
//             }
//             else
            {
                System.Windows.Forms.WebBrowser _webBrowser = (WebElementPool.WEB1_ID == id) ? webBrowser1 : webBrowser2;
                AutoResetEvent _e = (WebElementPool.WEB1_ID == id) ? m_Web1Event : m_Web2Event;
                _e.Reset();
                                
                _webBrowser.Navigate(url);
                //this.wait(_webBrowser, msTime);
                this.wait(id, msTime);
            }
        }

        private HtmlElement findElement(int id, string szTag, Attribute sKey)
        {
            HtmlElement _elem = null;
            System.Windows.Forms.WebBrowser _webBrowser = (WebElementPool.WEB1_ID == id) ? webBrowser1 : webBrowser2;
            Regex r = new Regex(sKey.szValue);
            string szInput = "";

            HtmlElementCollection hc = _webBrowser.Document.GetElementsByTagName(szTag);
            foreach (HtmlElement _e in hc)
            {
                szInput = _e.GetAttribute(sKey.szKey);
                if (null != szInput && 0 < szInput.Length)
                {
                    if ( r.IsMatch(szInput) )
                    {
                        _elem = _e;
                        break;
                    }
                }
            }
            return _elem;
        }

        private HtmlElement findElement(int id, string szTag, string szRegex)
        {
            HtmlElement _elem = null;
            System.Windows.Forms.WebBrowser _webBrowser = (WebElementPool.WEB1_ID == id) ? webBrowser1 : webBrowser2;
            Regex r = new Regex(szRegex);
            string szInput = null;

            HtmlElementCollection hc = _webBrowser.Document.GetElementsByTagName(szTag);
            foreach (HtmlElement _e in hc)
            {
                szInput = _e.OuterHtml;
                if (null != szInput && 0 < szInput.Length)
                {
                    if (r.IsMatch(szInput))
                    {
                        _elem = _e;
                        break;
                    }
                }
            }
            return _elem;
        }

        public bool setAttribute(int id, string szID, string szValue)
        {
            Attribute sKey, sInput;
            sKey.szKey = WebElementPool.ID;
            sKey.szValue = szID;
            sInput.szKey = WebElementPool.VALUE;
            sInput.szValue = szValue;
            return setAttribute(id, WebElementPool.INPUT, sKey, sInput);
        }

        public bool setAttribute(int id, string szTag, Attribute sKey, Attribute sInput)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mSetAttribute, new object[] { id, szTag, sKey, sInput });
            }
            else
            {
                HtmlElement _elem = this.findElement(id, szTag, sKey);
                if (null == _elem)
                {
                    Console.WriteLine("Error: No element had been found.");
                    return false;
                }
                _elem.SetAttribute(sInput.szKey, sInput.szValue);
            }

            return true;
        }

        public bool setAttribute2(int id, string szTag, string szRegex, Attribute sInput)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mSetAttribute2, new object[] { id, szTag, szRegex, sInput });
            }
            else
            {
                HtmlElement _elem = this.findElement(id, szTag, szRegex);
                if (null == _elem)
                {
                    Console.WriteLine("Error: No element had been found.");
                    return false;
                }
                string _szValue = _elem.GetAttribute(sInput.szKey);
                if (null != _szValue && 0 < _szValue.Length)
                {
                    _elem.SetAttribute(sInput.szKey, sInput.szValue);
                } else
                {
                    Console.WriteLine("Error: there isn't attribute: {0}", sInput.szKey);
                }
            }

            return true;
        }

        public bool invokeMember(int id, Attribute sKey, string szAction)
        {
            return invokeMember(id, WebElementPool.INPUT, sKey, szAction);
        }

        public bool invokeMember(int id, string szTag, Attribute sKey, string szAction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mInvokeMember, new object[] { id, szTag, sKey, szAction });
            } else
            {
                AutoResetEvent _event = (WebElementPool.WEB1_ID == id) ? m_Web1Event : m_Web2Event;
                _event.Reset();

                HtmlElement _elem = this.findElement(id, szTag, sKey);
                if (null == _elem)
                {
                    Console.WriteLine("Error: No element had been found.");
                    return false;
                }
                _elem.InvokeMember(szAction);
            }
            return true;
        }

        public bool invokeMember2(int id, string szTag, string szRegex, string szAction)
        {
            bool bResult = false;
            if (this.InvokeRequired)
            {
                bResult = (bool)this.Invoke(mInvokeMember2, new object[] { id, szTag, szRegex, szAction });
            }
            else
            {
                AutoResetEvent _event = (WebElementPool.WEB1_ID == id) ? m_Web1Event : m_Web2Event;
                _event.Reset();

                HtmlElement _elem = this.findElement(id, szTag, szRegex);
                if (null == _elem)
                {
                    Console.WriteLine("Error: No element had been found.");
                    return false;
                }
                _elem.InvokeMember(szAction);
                bResult = true;
            }
            return bResult;
        }

        public void cacheElements(int id, string szTag, string szRegex)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mCacheElements, new object[] { id, szTag, szRegex });
            } else
            {
                System.Windows.Forms.WebBrowser _webBrowser = (WebElementPool.WEB1_ID == id) ? webBrowser1 : webBrowser2;
                System.Windows.Forms.HtmlElementCollection hc = null;
                m_elementsQueue.Clear();

                hc = _webBrowser.Document.GetElementsByTagName(szTag);
                if (null == hc)
                    return;
                
                Regex r = new Regex(szRegex);
                foreach(HtmlElement e in hc)
                {
                    if (r.IsMatch(e.OuterHtml))
                    {
                        m_elementsQueue.Enqueue(e);
                    }
                }
            }
            return;
        }

        public void cacheElements2(int id, string szTag, Attribute sAttr)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mCacheElements2, new object[] { id, szTag, sAttr });
            }
            else
            {
                System.Windows.Forms.WebBrowser _webBrowser = (WebElementPool.WEB1_ID == id) ? webBrowser1 : webBrowser2;
                System.Windows.Forms.HtmlElementCollection hc = null;
                m_elementsQueue.Clear();

                hc = _webBrowser.Document.GetElementsByTagName(szTag);
                if (null == hc)
                    return;

                Regex r = new Regex(sAttr.szValue);
                string _tmpValue = "";
                foreach (HtmlElement e in hc)
                {
                    _tmpValue = e.GetAttribute(sAttr.szKey);
                    if (null == _tmpValue || 0 == _tmpValue.Length)
                        continue;

                    if (r.IsMatch(_tmpValue))
                    {
                        m_elementsQueue.Enqueue(e);
                    }
                }
            }
            return;
        }

        public bool nextValueInCache(string szTag, string szRegex, ref Attribute sAttribute)
        {
            if (this.InvokeRequired)
            {
                object[] objArgs = new object[] { szTag, szRegex, sAttribute };
                this.Invoke(mNextValueInCache, objArgs);
                sAttribute = (Attribute)objArgs[2];
            } else
            {
                if (0 == m_elementsQueue.Count)
                    return false;

                HtmlElement elem = m_elementsQueue.Peek();
                Regex r = new Regex(szRegex);

                foreach (HtmlElement e in elem.GetElementsByTagName(szTag))
                {
                    if (r.IsMatch(e.OuterHtml))
                    {
                        sAttribute.szValue = e.GetAttribute(sAttribute.szKey);
                        break;
                    }
                }
            }
            return true;
        }

        public bool nextTextInCache(string szTag, string szRegex, ref string szText)
        {
            if (this.InvokeRequired)
            {
                object[] objArgs = new object[] { szTag, szRegex, szText };
                this.Invoke(mNextTextInCache, objArgs);
                szText = (string)objArgs[2];
            }
            else
            {
                if (0 == m_elementsQueue.Count)
                    return false;

                HtmlElement elem = m_elementsQueue.Peek();
                Regex r = new Regex(szRegex);

                foreach (HtmlElement e in elem.GetElementsByTagName(szTag))
                {
                    if (r.IsMatch(e.OuterHtml))
                    {
                        szText = e.OuterText;
                        break;
                    }
                }
            }
            return true;
        }

        public void nextInvokeInCache(string szTag, string szRegex, string szAction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mNextInvokeInCache, new object[] { szTag, szRegex, szAction });
            } else
            {
                if (0 == m_elementsQueue.Count)
                    return;

                HtmlElement elem = m_elementsQueue.Peek();
                Regex r = new Regex(szRegex);

                foreach (HtmlElement e in elem.GetElementsByTagName(szTag))
                {
                    if (r.IsMatch(e.OuterHtml))
                    {
                        e.InvokeMember(szAction);
                        break;
                    }
                }
            }
            return;
        }

        public void nextInvokeInCache(string szAction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(mNextInvokeInCache2, new object[] { szAction });
            } else
            {
                HtmlElement elem = m_elementsQueue.Peek();
                elem.InvokeMember(szAction);
            }
            return;
        }

        public void cacheNext()
        {
            if (0 == m_elementsQueue.Count)
                return;

            m_elementsQueue.Dequeue();
            return;
        }

        public int cacheCount()
        {
            return m_elementsQueue.Count;
        }

        public void closeMessageBox()
        {
            while (!m_bCloseThreadStop)
            {
                Thread.Sleep(1000);
                //System.Windows.Forms.WebBrowser _webBrowser = (WebElementPool.WEB1_ID == id) ? webBrowser1 : webBrowser2;
                IntPtr _msgBoxHandle = FindWindow("#32770", "来自网页的消息");
                if (IntPtr.Zero == _msgBoxHandle)
                    continue;

                IntPtr _btnHandle = FindWindowEx(_msgBoxHandle, 0, null, null);
                uint _message = 0xF5;
                SendMessage(_btnHandle, _message, IntPtr.Zero, 0);
            }
        }

        private Thread m_Thread;

        private bool m_bThreadStarted;

        private Thread m_CloseThread;

        private bool m_bCloseThreadStop;

        private static ScenarioProcessor s_ScenarioProcessor;

        private AutoResetEvent m_Web1Event;

        private AutoResetEvent m_Web2Event;

        private bool m_bWeb1EventHandled;

        private Queue<HtmlElement> m_elementsQueue;

        /*******  代理 *******/
        public delegate void NavigateDelegate(int id, string url, int msTime);
        public delegate bool SetAttributeDelegate(int id, string szTag, Attribute sKey, Attribute sInput);
        public delegate bool SetAttribute2Delegate(int id, string szTag, string szRegex, Attribute sInput);
        public delegate bool InvokeMemberDelegate(int id, string szTag, Attribute sKey, string szAction);
        public delegate bool InvokeMember2Delegate(int id, string szTag, string szRegex, string szAction);
        public delegate void CacheElementsDelegate(int id, string szTag, string szRegex);
        public delegate bool NextValueInCacheDelegate(string szTag, string szRegex, ref Attribute sAttribute);
        public delegate void NextInvokeInCacheDelegate(string szTag, string szRegex, string szAction);
        public delegate bool NextTextInCacheDelegate(string szTag, string szRegex, ref string szText);
        public delegate void CacheElements2Delegate(int id, string szTag, Attribute sAttr);
        public delegate void NextInvokeInCache2Delegate(string szAction);

        private NavigateDelegate mNavigate;

        private SetAttributeDelegate mSetAttribute;

        private SetAttribute2Delegate mSetAttribute2;

        private InvokeMemberDelegate mInvokeMember;

        private InvokeMember2Delegate mInvokeMember2;

        private CacheElementsDelegate mCacheElements;

        private NextValueInCacheDelegate mNextValueInCache;

        private NextInvokeInCacheDelegate mNextInvokeInCache;

        private NextTextInCacheDelegate mNextTextInCache;

        private CacheElements2Delegate mCacheElements2;

        private NextInvokeInCache2Delegate mNextInvokeInCache2;
    }
}
