using System;
using System.Collections.Generic;
using System.Text;

using interop.ICApiIronCAD.SelectionTool;
//using interop.ICApiInovate;
using System.Windows.Forms;
using System.Collections;



namespace ICApiAddin.SelectionTool
{
    class SelectionToolAppEvents
    {
  
        private ZIronCADApp m_zIronCADApp;
        private interop.ICApiInovate.SelectionTool.ZInovateApp m_zInovateApp;

        Dictionary<IZDoc, SelectionToolDocEvents> m_mapDocToEvent;

        internal SelectionToolAppEvents()
        {
            m_mapDocToEvent = new Dictionary<IZDoc, SelectionToolDocEvents>();
        }
  

        internal void RegisterForAppEvents(IZBaseApp izBaseApp)
        {
            m_zIronCADApp = izBaseApp as ZIronCADApp;
            if (m_zIronCADApp != null)
            {
                m_zIronCADApp.ActiveDocChanged += new _IZApplicationEvents_ActiveDocChangedEventHandler(zIronCADApp_ActiveDocChanged);
                m_zIronCADApp.DocumentOpened += new _IZApplicationEvents_DocumentOpenedEventHandler(m_zIronCADApp_DocumentOpened);
                m_zIronCADApp.DocumentCreated += new _IZApplicationEvents_DocumentCreatedEventHandler(m_zIronCADApp_DocumentCreated);
                m_zIronCADApp.DocumentPreClose += new _IZApplicationEvents_DocumentPreCloseEventHandler(m_zIronCADApp_DocumentPreClose);
            }
            else
            {
                m_zInovateApp = izBaseApp as interop.ICApiInovate.SelectionTool.ZInovateApp;
                if (m_zInovateApp != null)
                {
                    m_zInovateApp.ActiveDocChanged +=new interop.ICApiInovate.SelectionTool._IZApplicationEvents_ActiveDocChangedEventHandler(m_zInovateApp_ActiveDocChanged);
                    m_zInovateApp.DocumentOpened += new interop.ICApiInovate.SelectionTool._IZApplicationEvents_DocumentOpenedEventHandler(m_zInovateApp_DocumentOpened);
                    m_zInovateApp.DocumentCreated += new interop.ICApiInovate.SelectionTool._IZApplicationEvents_DocumentCreatedEventHandler(m_zInovateApp_DocumentCreated);
                    m_zInovateApp.DocumentPreClose += new interop.ICApiInovate.SelectionTool._IZApplicationEvents_DocumentPreCloseEventHandler(m_zInovateApp_DocumentPreClose);
 
                }
            }

            ZCatalogEvents SceneCatalog = izBaseApp.SceneCatalogEvents;

            if(SceneCatalog != null)
            {
                CatalogEvents aEvent = new CatalogEvents();
                aEvent.SetApp(izBaseApp);
                aEvent.RegisterEvents(SceneCatalog);

            }
            List<IZDoc> iZDocList = ICAddInUtils.GetAllOpenDocs(izBaseApp);
            foreach (IZDoc iZDoc in iZDocList)
            {
                if (iZDoc != null)
                {
                    RegisterForDocEvents(iZDoc);
                }
            }
        }

        internal void UnRegisterForAppEvents()
        {
            if (m_zIronCADApp != null)
            {
                m_zIronCADApp.ActiveDocChanged -= zIronCADApp_ActiveDocChanged;
                m_zIronCADApp.DocumentCreated -= m_zIronCADApp_DocumentCreated;
                m_zIronCADApp.DocumentOpened -= m_zIronCADApp_DocumentOpened;
                m_zIronCADApp.DocumentPreClose -= m_zIronCADApp_DocumentPreClose;

                IZBaseApp izBaseApp = m_zIronCADApp as IZBaseApp;
                if (izBaseApp != null)
                {
                    List<IZDoc> iZDocList = ICAddInUtils.GetAllOpenDocs(izBaseApp);
                    foreach (IZDoc iZDoc in iZDocList)
                    {
                        if (iZDoc != null)
                        {
                            UnRegisterForDocEvents(iZDoc);
                        }
                    }
                 }
            }
        }

        void m_zIronCADApp_DocumentPreClose(IZDoc pNewDoc) 
        {
          //  MessageBox.Show("DocumentPreClose event fired.");
            if (pNewDoc != null)
            {
                UnRegisterForDocEvents(pNewDoc);
            }
        }

        void m_zIronCADApp_DocumentCreated(IZDoc piNewDoc)
        {
            if (piNewDoc != null)
            {
                RegisterForDocEvents(piNewDoc);
            }
            /*
            IStream spStream = null;
           // MessageBox.Show("DocumentCreated event fired.");
            if (piNewDoc != null)
            {
                RegisterForDocEvents(piNewDoc);
                IStorage piStg = piNewDoc.Create3rdPartyAddinStorage2("Mytestddd", 0);
                uint flag = (0x00001000 | 0x00000002 | 0x00000010);
                if (piStg != null)
                {
                    piStg.CreateStream("Info", flag, 0, 0, out spStream);
                    if(spStream != null)
                    {
                      
                    }
                }
            }
            */
        }


        void m_zIronCADApp_DocumentOpened(IZDoc piNewDoc)
        {
            //MessageBox.Show("DocumentOpened event fired.");
            if (piNewDoc != null)
            {
                RegisterForDocEvents(piNewDoc);
            }
        }

        void zIronCADApp_ActiveDocChanged(IZDoc piNewDoc)
        {
           // MessageBox.Show("ActiveDocChanged event fired.");
        }


        void m_zInovateApp_DocumentPreClose(interop.ICApiInovate.SelectionTool.IZDoc pNewDoc)
        {
            IZDoc piDoc = pNewDoc as IZDoc;

            m_zIronCADApp_DocumentPreClose(piDoc);
            
        }

        void m_zInovateApp_DocumentCreated(interop.ICApiInovate.SelectionTool.IZDoc piNewDoc)
        {
            IZDoc piDoc = piNewDoc as IZDoc;
            m_zIronCADApp_DocumentCreated(piDoc);
   
        }

        void m_zInovateApp_DocumentOpened(interop.ICApiInovate.SelectionTool.IZDoc piNewDoc)
        {
            IZDoc piDoc = piNewDoc as IZDoc;
            m_zIronCADApp_DocumentOpened(piDoc);
        }

        void m_zInovateApp_ActiveDocChanged(interop.ICApiInovate.SelectionTool.IZDoc piNewDoc)
        {
            IZDoc piDoc = piNewDoc as IZDoc;
            zIronCADApp_ActiveDocChanged(piDoc);
   
        }

        internal void RegisterForDocEvents(IZDoc iZDoc)
        {
            if (iZDoc != null)
            {
                //Doc events
                /// int code= iZDoc.GetHashCode();
                if (!m_mapDocToEvent.ContainsKey(iZDoc))
                {

                    SelectionToolDocEvents docEvents = new SelectionToolDocEvents();
                    docEvents.RegisterEvents(iZDoc);
                    m_mapDocToEvent.Add(iZDoc, docEvents);

                    SelectionToolDocEvents docEventsTest= null;
                    m_mapDocToEvent.TryGetValue(iZDoc, out docEventsTest);


                    //Handle Events
                    {
                        ZHandleEvents zHandleEvents = (ZHandleEvents)iZDoc.HandleEvents;
                        SelectionToolDocHandleEvents docHandleEvents = new SelectionToolDocHandleEvents();
                        docHandleEvents.RegisterEvents(zHandleEvents);
                    }


                    {
                        ZOperationEvents zOptEvents = iZDoc.OperationEvents;
                        DocOperationEvents.RegisterEvents(zOptEvents);

                    }
                }
            }
        }

        internal void UnRegisterForDocEvents(IZDoc iZDoc)
        {
            if (iZDoc != null)
            {
                int code = iZDoc.GetHashCode();
                SelectionToolDocEvents docEvents=null;
                if (!m_mapDocToEvent.TryGetValue(iZDoc, out docEvents))
                {
                    foreach (var pair in m_mapDocToEvent)
                    {
                                 
                        docEvents = pair.Value;
                        docEvents.UnRegisterEvents(iZDoc);
                        break;
                       
                    }
                }
                    
               if(docEvents!=null)
                {
                    // SelectionToolDocEvents docEvents = new SelectionToolDocEvents();      
                    docEvents.UnRegisterEvents(iZDoc);

                    SelectionToolDocHandleEvents docHandleEvents = new SelectionToolDocHandleEvents();
                    ZHandleEvents zHandleEvents = (ZHandleEvents)iZDoc.HandleEvents;
                    docHandleEvents.UnRegisterEvents(zHandleEvents);


                    //DocOperationEvents docOptEvents = new DocOperationEvents();
                    ZOperationEvents zOptEvents = iZDoc.OperationEvents;
                    DocOperationEvents.UnRegisterEvents(zOptEvents);
                }
            }
        }
    }
}
