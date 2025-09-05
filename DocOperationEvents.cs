using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using interop.ICApiIronCAD.SelectionTool;
using System.Windows.Forms;

namespace ICApiAddin.SelectionTool
{
    static class DocOperationEvents
    {
  
        static internal void RegisterEvents( interop.ICApiIronCAD.SelectionTool.ZOperationEvents OperationEvents)
        {
            if (OperationEvents != null)
            {
                OperationEvents.OnDelete += new _IZOperationEvents_OnDeleteEventHandler(OperationEvents_OnDelete);
                OperationEvents.OnUndoRedo += new _IZOperationEvents_OnUndoRedoEventHandler(OperationEvents_OnUndoRedo);
                OperationEvents.OnPaste += new _IZOperationEvents_OnPasteEventHandler(OperationEvents_OnPaste);

            }
        }

        static internal void UnRegisterEvents(ZOperationEvents OperationEvents)
        {
            if (OperationEvents != null)
            {
                OperationEvents.OnDelete -= OperationEvents_OnDelete;
                OperationEvents.OnUndoRedo -= OperationEvents_OnUndoRedo;
                OperationEvents.OnPaste -= OperationEvents_OnPaste;
            }
        }
        static void OperationEvents_OnUndoRedo(bool vbIsUndo)
        {
           if(vbIsUndo )
            {
                MessageBox.Show("OnUndo");

            }
            MessageBox.Show("OnRedo");

        }

        static void OperationEvents_OnDelete(ZArray piArrayObj, bool vbIsUndo)
        {
            int count = 0;
            piArrayObj.Count(out count);
            for (int i = 0; i < count; i++)
            {
                object obj = null;
                piArrayObj.Get(i, out obj);
                IZElement izElement = obj as IZElement;
                string name;
                if (izElement != null)
                {
                    name = izElement.Name;
                }
            }
        }

        static void OperationEvents_OnPaste(ZArray piArrayObj)
        {
            int count = 0;
            piArrayObj.Count(out count);
            for (int i = 0; i < count; i++)
            {
                object obj = null;
                piArrayObj.Get(i, out obj);
                IZElement izElement = obj as IZElement;
                string name;
                if (izElement != null)
                {
                    name = izElement.Name;
                }
            }
        }

    }
}

