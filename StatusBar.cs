/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;

namespace UtkarshShigihalli.VisualStudioStatusBarDemo
{
    /// <summary>
    /// Class to interact with the VS status bar
    /// </summary>
    public class StatusBar
    {
        #region Fields
        IServiceProvider serviceProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public StatusBar(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        #endregion

        #region Properties
        private IVsStatusbar bar;

        /// <summary>
        /// Gets the status bar.
        /// </summary>
        /// <value>The status bar.</value>
        protected IVsStatusbar Bar
        {
            get
            {
                if(bar == null)
                {
                    bar = serviceProvider.GetService(typeof(SVsStatusbar)) as IVsStatusbar;
                }

                return bar;
            }
        }
        #endregion

        #region Public Implementation
        /// <summary>
        /// Displays the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void DisplayMessage(string message)
        {
            int frozen;

            Bar.IsFrozen(out frozen);

            if(frozen == 0)
            {
                Bar.SetText(message);
            }
        }

        public void DisplayAndShowIcon(string message)
        {
            object icon = Constants.SBAI_General;
            Bar.Animation(1, ref icon);
            Bar.SetText(message);
            Thread.Sleep(5000);

            Bar.Animation(0, ref icon);
            Bar.Clear();
        }

        public void DisplayAndShowProgress(string message)
        {
            object icon = Constants.SBAI_General;
            Bar.Animation(1, ref icon);
            Bar.SetText(message);
            Thread.Sleep(5000);

            Bar.Animation(0, ref icon);
            Bar.Clear();
        }
        #endregion
    }
}