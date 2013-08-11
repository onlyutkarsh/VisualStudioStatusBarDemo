using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Constants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace UtkarshShigihalli.VisualStudioStatusBarDemo
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVisualStudioStatusBarDemoPkgString)]
    public sealed class VisualStudioStatusBarDemoPackage : Package
    {
        private IVsStatusbar bar;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VisualStudioStatusBarDemoPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVisualStudioStatusBarDemoCmdSet, (int)PkgCmdIDList.cmdidShowProgressBar);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            DisplayMessage();

            DisplayAndShowProgress();

            StatusBar.SetText("You can also use the default icons...");
            Thread.Sleep(2000);

            var iconDemo = new Dictionary<string, object>
            {
                {"Build Icon", (short) Constants.SBAI_Build},
                {"Deploy Icon", (short) Constants.SBAI_Deploy},
                {"Find Icon", (short) Constants.SBAI_Find},
                {"General Icon", (short) Constants.SBAI_General},
                {"Print Icon", (short) Constants.SBAI_Print},
                {"Save Icon", (short) Constants.SBAI_Save}
            };


            foreach (KeyValuePair<string, object> keyValuePair in iconDemo)
            {
                DisplayAndShowIcon(keyValuePair.Key, keyValuePair.Value);
            }

            SetAndGetStatusBarText("Read this message from status bar");

            StatusBar.FreezeOutput(0);
            StatusBar.Clear();
            StatusBar.SetText(string.Empty);
        }

        /// <summary>
        /// Gets the status bar.
        /// </summary>
        /// <value>The status bar.</value>
        private IVsStatusbar StatusBar
        {
            get
            {
                if (bar == null)
                {
                    bar = GetService(typeof(SVsStatusbar)) as IVsStatusbar;
                }

                return bar;
            }
        }

        #region Public Implementation

        /// <summary>
        /// Displays the message.
        /// </summary>
        public void DisplayMessage()
        {
            int frozen;

            StatusBar.IsFrozen(out frozen);

            for (int i = 5; i > 0; i--)
            {
                if (frozen == 0)
                {
                    StatusBar.SetText(string.Format("This message is being displayed in status bar for {0} seconds", i));
                    Thread.Sleep(1000);
                }
            }
            StatusBar.FreezeOutput(0);
            StatusBar.Clear();
        }

        public void DisplayAndShowIcon(string message, object iconToShow)
        {
            object icon = (short)iconToShow;

            StatusBar.Animation(1, ref icon);
            StatusBar.SetText(message);
            Thread.Sleep(3000);

            StatusBar.Animation(0, ref icon);
            StatusBar.FreezeOutput(0);
            StatusBar.Clear();
        }

        public void DisplayAndShowProgress()
        {
            var messages = new string[]
                {
                    "Demo Long running task...Step 1...",
                    "Step 2...",
                    "Step 3...",
                    "Step 4...",
                    "Completing long running task."
                };
            uint cookie = 0;

            // Initialize the progress bar.
            StatusBar.Progress(ref cookie, 1, "", 0, 0);

            for (uint j = 0; j < 5; j++)
            {
                uint count = j + 1;
                StatusBar.Progress(ref cookie, 1, "", count, 5);
                StatusBar.SetText(messages[j]);
                // Display incremental progress.
                Thread.Sleep(1500);
            }

            // Clear the progress bar.
            StatusBar.Progress(ref cookie, 0, "", 0, 0);
            StatusBar.FreezeOutput(0);
            StatusBar.Clear();

        }

        public void SetAndGetStatusBarText(string message)
        {
            int frozen;

            StatusBar.IsFrozen(out frozen);

            if (frozen == 0)
            {
                // Set the status bar text and make its display static.
                StatusBar.SetText(message);
                StatusBar.FreezeOutput(1);

                // Retrieve the status bar text. 
                string text;
                StatusBar.GetText(out text);
                MessageBox.Show(text);
            }
            // Clear the status bar text.
            StatusBar.FreezeOutput(0);
            StatusBar.Clear();
        }
        #endregion

    }
}
