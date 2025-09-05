using System;
using System.Collections.Generic;
using System.Text;

using interop.ICApiIronCAD.SelectionTool;
using System.Windows.Forms;

namespace ICApiAddin.SelectionTool
{
    class SelectionToolDocHandleEvents
    {
        internal SelectionToolDocHandleEvents()
        {
        }

        internal void RegisterEvents(ZHandleEvents zHandleEvents)
        {
            if (zHandleEvents != null)
            {
                zHandleEvents.OnBeginDrag += new _IZHandleEvents_OnBeginDragEventHandler(zHandleEvents_OnBeginDrag);
            }
        }

        internal void UnRegisterEvents(ZHandleEvents zHandleEvents)
        {
            if (zHandleEvents != null)
            {
                zHandleEvents.OnBeginDrag -= zHandleEvents_OnBeginDrag;
            }
        }

        void zHandleEvents_OnBeginDrag(ZHandle piHandle, int lClientX, int lClientY)
        {
           // MessageBox.Show("OnBeginDrag event fired.");
        }
    }
}
