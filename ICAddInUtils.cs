using System;
using System.Collections.Generic;
using System.Text;

using interop.ICApiIronCAD.SelectionTool;
using System.Collections;

namespace ICApiAddin.SelectionTool
{
    /// <summary>
    /// Utility class conatins static utility functions
    /// </summary>
    static class ICAddInUtils
    {
        static SelectionToolDocSelectionEvents selectionEvent;
        internal static bool StartSelectionTool(IZDoc iZDoc)
        {
            
            if (iZDoc != null)
            {
                IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
                if (iZsceneDoc != null)
                {
                    try
                    {
                        IZSelectionMgr iZselectionMgr = iZsceneDoc.SelectionMgr;
                        CZInteractor cZinteractor = iZselectionMgr.CreateInteractor();
                        //Selection Events
                        CZSelectorEvents cZselectionEvents = cZinteractor.SelectEvents;
                        selectionEvent = new SelectionToolDocSelectionEvents(cZselectionEvents, iZDoc);

                        //Mouse Events
                        ZMouseEvents zMouseEvents = cZinteractor.MouseEvents;
                        SelectionToolMouseEvents mouseEvents = new SelectionToolMouseEvents(zMouseEvents);

                        //Start
                        cZinteractor.Start();
                        return true;
                    }
                    catch
                    {
                        //Do nothing
                    }
                }

            }

            return false;
        }

        internal static bool StopSelectionTool(IZDoc iZDoc)
        {
            if (iZDoc != null)
            {
                IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
                if (iZsceneDoc != null)
                {
                    try
                    {
                        selectionEvent.UnRegister();
                        IZSelectionMgr iZselectionMgr = iZsceneDoc.SelectionMgr;
                        CZInteractor cZinteractor = iZselectionMgr.CreateInteractor();
                        cZinteractor.Stop();
                        return true;
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }
            return false;
        }

        internal static List<IZDoc> GetAllOpenDocs(IZBaseApp izBaseApp)
        {
            List<IZDoc> list = new List<IZDoc>();
            if (izBaseApp != null)
            {
                object vDocs = izBaseApp.GetAllOpenDocs();
                if (vDocs != null)
                {
                    //Process VARIANT of oDocs to IZoc array/list
                    IEnumerable iIZDocs = vDocs as IEnumerable;
                    if (iIZDocs != null)
                    {
                        foreach (object oDoc in iIZDocs)
                        {
                            IZDoc iZDoc = oDoc as IZDoc;
                            if (iZDoc != null)
                            {
                                list.Add(iZDoc);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}
