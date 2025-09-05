using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using interop.ICApiIronCAD.SelectionTool;
using System.Windows.Forms;

namespace ICApiAddin.SelectionTool
{
    static class DockBarEvents
    {
        static bool testB = false;

        static internal void RegisterEvents( interop.ICApiIronCAD.SelectionTool.ZWindowEvents WindowEvents)
        {
            if (WindowEvents != null)
            {
                WindowEvents.OnSize += new _IZWindowEvents_OnSizeEventHandler(WindowEvents_OnSize);
                WindowEvents.OnShowWindow += new _IZWindowEvents_OnShowWindowEventHandler(WindowEvents_OnShowWindow);
            }
        }

        static internal void UnRegisterEvents(ZWindowEvents WindowEvents)
        {
            if (WindowEvents != null)
            {
                WindowEvents.OnSize -= WindowEvents_OnSize;
                WindowEvents.OnShowWindow -= WindowEvents_OnShowWindow;
            }
        }
        
        static void WindowEvents_OnShowWindow(bool bShow, uint nStatus)
        {
            
            
        }

        static void WindowEvents_OnSize(uint nType, int cx,int cy)
        {
         
        }

  

    }
}

