﻿using System;
using System.Windows.Forms;

namespace Leaf.Forms
{
    public partial class FormWebBrowser : Form
    {
        // TODO: 
        //public CookieContainer Cookies { get; private set; }
        //public SteamSession Session { get; private set; }
        
        public FormWebBrowser()
        {
            InitializeComponent();
            wb.Modernize();
        }

        /// <summary>
        /// Clean up browser allocated memory and close the form.
        /// </summary>
        private void CleanAndClose()
        {
            wb.Visible = false;
            wb.Dispose();
            wb = null;
            // Collect 2 generations of trash
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Close();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            wb.Navigated += Wb_Navigated;
            wb.Navigate("https://partner.steamgames.com/apps/");
        }

        private void Wb_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // Inject css
            //IHTMLDocument2 doc = (wb.Document.DomDocument) as IHTMLDocument2;
            //IHTMLStyleSheet ss = doc.createStyleSheet("", 0);
            //ss.cssText = @".navbar,.navbar-header,.form-group,.header-content,footer { display: none !important; } header.full-header,.reg { background-image: none !important; background-color: white !important; } .form-group:nth-child(3) {display: block !important}";
            
            if (tmrWatcher.Enabled)
                return;

            tmrWatcher_Tick(null, null);
            tmrWatcher.Start();
        }

        //private const int InternetCookieHttponly = 0x2000;
        /*
        private static CookieContainer GetCookieContainer(string baseUrl)
        {
            var uri = new Uri(baseUrl);

            // Determine the size of the cookie
            int datasize = 8192 * 16;
            var cookieData = new StringBuilder(datasize);
            if (!NativeMethods.InternetGetCookieEx(baseUrl, null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!NativeMethods.InternetGetCookieEx(baseUrl, null, cookieData, ref datasize, InternetCookieHttponly,
                    IntPtr.Zero)) 
                    return null;
            }

            if (cookieData.Length <= 0)
                return null;

            var cookies = new CookieContainer();
            cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            return cookies;
        }
        */
        private void ReportError(string message, bool panic = true)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK,
                panic ? MessageBoxIcon.Error : MessageBoxIcon.Exclamation);

            if (panic)
                CleanAndClose();
        }

        private void tmrWatcher_Tick(object sender, EventArgs e)
        {
            // TODO: universal solution
            if (wb == null || wb.Document == null || wb.Document.Body == null ||
                !wb.Document.Body.InnerHtml.Contains("https://partner.steamgames.com/login/logout"))
            {
                return;
            }
            
            tmrWatcher.Stop();

            //var cookieContainer = GetCookieContainer("https://partner.steamgames.com/");
            var owner = wb.Document.GetElementById("landingHeader")?.GetElementsByTagName("h1");

            //string userName = null;
            if (owner == null || owner.Count == 0 || owner[0] == null)
            {
                ReportError("Не могу определить имя аккаунта, ничего страшного." +
                            " Просто не будет отображаться имя аккаунта в программе", 
                            false);
            }
            //else
              //  userName = owner[0].InnerText;

            //Session = new SteamSession(cookieContainer, userName);

            // Session.Save();
            // BinarySerializer.Serialize(Session, "session.dat");
            //Session.Serialize();

            CleanAndClose();
        }
    }
}
