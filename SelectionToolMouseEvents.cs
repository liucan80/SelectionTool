using System;
using System.Collections.Generic;
using System.Text;

using interop.ICApiIronCAD.SelectionTool;
using System.Windows.Forms;

namespace ICApiAddin.SelectionTool
{
    class SelectionToolMouseEvents
    {
        internal SelectionToolMouseEvents(ZMouseEvents zMouseEvent)
        {
            if (zMouseEvent != null)
            {
                zMouseEvent.OnLButtonDown += new _IZMouseEvents_OnLButtonDownEventHandler(m_zMouseEvents_OnLButtonDown);
                zMouseEvent.OnLButtonDblClick += new _IZMouseEvents_OnLButtonDblClickEventHandler(m_zMouseEvents_OnLButtonDblClick);
                zMouseEvent.OnMouseMove += new _IZMouseEvents_OnMouseMoveEventHandler(m_zMouseEvents_OnMouseMove);
            }
        }

        void m_zMouseEvents_OnLButtonDblClick(int lClientX, int lClientY, int lKeyState)
        {
            MessageBox.Show("Left mouse button double clicked.");
        }

        void m_zMouseEvents_OnLButtonDown(int lClientX, int lClientY, int lKeyState)
        {
            //MessageBox.Show("Left mouse button down.");
        }
        void m_zMouseEvents_OnMouseMove(int lClientX, int lClientY, int lKeyState)
        {
            //MessageBox.Show("Left mouse Move");
        }
    }
}
