using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using interop.ICApiIronCAD.SelectionTool;
using System.Runtime.InteropServices;
using MVOI = Microsoft.VisualStudio.OLE.Interop;
using System.Windows.Forms;

namespace ICApiAddin.SelectionTool
{
    class CatalogEvents
    {
        private enum STGM
        {
            STGM_READ = 0x00000000,
            STGM_WRITE = 0x00000001,
            STGM_READWRITE = 0x00000002,
            STGM_SHARE_DENY_NONE = 0x00000040,
             STGM_SHARE_DENY_READ =  0x00000030,
            STGM_SHARE_DENY_WRITE = 0x00000020,
            STGM_SHARE_EXCLUSIVE = 0x00000010,
            STGM_DIRECT_SWMR = 0x00400000,
            STGM_CREATE = 0x00001000
        }

        private enum STGC
        {
            STGC_DEFAULT = 0,
            STGC_OVERWRITE = 1,
            STGC_ONLYIFCURRENT = 2,
            STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
            STGC_CONSOLIDATE = 8
        };

        string strMyRootName = "My_Delta_Data";
        IZBaseApp m_izBaseApp = null;

        internal void SetApp(IZBaseApp izBaseApp)
        {
            m_izBaseApp = izBaseApp;
        }

        internal void RegisterEvents(ZCatalogEvents iZCatalogEvents)
        {
            if (iZCatalogEvents != null)
            {
                iZCatalogEvents.CatalogOpened += new _IZCatalogEvents_CatalogOpenedEventHandler(OnCatalogOpened);
                iZCatalogEvents.CatalogSavedCoreData += new _IZCatalogEvents_CatalogSavedCoreDataEventHandler(OnCatalogSavedCoreData);
                iZCatalogEvents.OnClickCatalogEntry += new _IZCatalogEvents_OnClickCatalogEntryEventHandler(OnClickCatalogEntry);
                

            }
        }

        internal void UnRegisterEvents(interop.ICApiIronCAD.SelectionTool.ZCatalogEvents iZCatalogEvents)
        {
            if (iZCatalogEvents != null)
            {
                iZCatalogEvents.CatalogOpened -= OnCatalogOpened;
                iZCatalogEvents.CatalogSavedCoreData -= OnCatalogSavedCoreData;
                iZCatalogEvents.OnClickCatalogEntry -= OnClickCatalogEntry;
            }
        }
        internal void OnClickCatalogEntry(ZCatalog piCatalog, ZCatalogEntry piEntry, int clientX, int clientY, bool vbLeftClick, int lKeyState, ref bool pvbOverride)
        {
            if (piCatalog == null || piEntry == null)
                return;

            IStorage spiStorage = piEntry.GetAddinStorage(strMyRootName, false, 0);

            MVOI.IStorage Stg = spiStorage as MVOI.IStorage;
 
            uint grfMode = (uint)(STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE);
            IStream spStream = null;
            MVOI.IStream spStreamMS = null;
            byte reserved = 0;
            if (Stg != null)
            {
                try
                {
                    IntPtr f = (IntPtr)0;
                    Stg.OpenStream("CatalogEntryInfo", f, grfMode, 0, out spStreamMS);
                    spStream = spStreamMS as IStream;
                }
                catch
                {
                }
                if (spStream == null)
                {
                    IZUtility pUty = m_izBaseApp.Utility;
                    grfMode = (uint)(STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE);
                    if (pUty != null)
                        spStream = pUty.OpenStream(spiStorage, "CatalogEntryInfo", grfMode);
                }
                string Readdata1;
                if (spStream != null)
                {
                    uint dummy = 0;
                    ReadString(spStream, out Readdata1);



                    MessageBox.Show(string.Format("Read my CatalogEntryInfo and Get: {0} ", Readdata1));

                    Marshal.ReleaseComObject(spStream);
                }
            }
           else
            {
                spiStorage = piEntry.GetAddinStorage(strMyRootName, true, 0);
                if (spiStorage == null)
                    return;

                grfMode = (uint)(STGM.STGM_WRITE | STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE);
                spiStorage.CreateStream("CatalogEntryInfo", grfMode, 0, 0, out spStream);
                if (spStream != null)
                {
                    string data = "My Robotic Information:XXXX";
                    WriteString(spStream, data);
                    spStream.Commit((uint)STGC.STGC_DEFAULT);
                    Marshal.ReleaseComObject(spStream);
                    spStream = null;

                    spiStorage.Commit((uint)STGC.STGC_DEFAULT);
                    piEntry.CommitAddInStorage(strMyRootName, spiStorage);

                    MessageBox.Show("Add : My Robotic Information:XXXX");

                }

            }
            Marshal.ReleaseComObject(spiStorage);
        }

    
        internal void OnCatalogOpened(ZCatalog piCatalog)
        {
            ReadDataFromCatalogStorage(piCatalog);
        }
        void OnCatalogSavedCoreData(ZCatalog izCatalog, bool vbSaveAs, string bstrFileName, string bstrOrgFileName)
        {
            SaveDataToCatalogStorage(izCatalog, vbSaveAs, bstrFileName, bstrOrgFileName);

        }
        void ReadString(IStream pstm, out string mystring)
        {
            uint dummy = 0;
            byte[] arrLen = new byte[sizeof(UInt32)];
            pstm.RemoteRead(out arrLen[0], sizeof(UInt32), out dummy);
            UInt32 size = BitConverter.ToUInt32(arrLen, 0);
            byte[] data = new byte[size];
            pstm.RemoteRead(out data[0], size, out dummy);

            Encoding encode = System.Text.ASCIIEncoding.Unicode;
            //mystring = BitConverter.ToString(data, 0);
            mystring = encode.GetString(data);
        }

        void WriteString(IStream pstm, string mystring)
        {
            Encoding encode = System.Text.ASCIIEncoding.Unicode;
            byte[] data = encode.GetBytes(mystring);
            UInt32 size = (UInt32)data.Length;
            byte[] arrLen = BitConverter.GetBytes(size);
            uint dummy = 0;
            pstm.RemoteWrite(ref arrLen[0], sizeof(UInt32), out dummy);
            pstm.RemoteWrite(ref data[0], (UInt32)size, out dummy);
        }


        void SaveDataToCatalogStorage(ZCatalog izCatalog, bool vbSaveAs, string bstrFileName, string bstrOrgFileName) 
       {
            IStorage spiStorage = izCatalog.GetAddinStorage(strMyRootName, true, 0) ;
            if(spiStorage!=null)
            {
                uint grfMode = (uint)(STGM.STGM_WRITE | STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE);
                IStream spStream = null;
                spiStorage.CreateStream("Info", grfMode, 0, 0, out spStream);
                if (spStream != null)
                {
                    string data1 = "My Data Start  中文3公元 fff";
                    WriteString(spStream, data1);
                    Int32 myVersionNum = 111;
                    byte[] byteArray = BitConverter.GetBytes(myVersionNum);
                    uint dummy = 0;
                    spStream.RemoteWrite( ref byteArray[0], sizeof(Int32), out dummy);
                    spStream.Commit( (uint)STGC.STGC_DEFAULT);
                     Marshal.ReleaseComObject(spStream);
                    spStream = null;
                }
                spiStorage.Commit((uint)STGC.STGC_DEFAULT);
                izCatalog.CommitAddInStorage(strMyRootName, spiStorage);  
                Marshal.ReleaseComObject(spiStorage);
            }     
        }
        void ReadDataFromCatalogStorage(ZCatalog izCatalog)
        {
           IStorage spiStorage = izCatalog.GetAddinStorage(strMyRootName, false, 0);

            MVOI.IStorage Stg =  spiStorage as MVOI.IStorage;
            if (spiStorage == null)
                return;
            uint grfMode =(uint) (STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE);
            IStream spStream = null;
            MVOI.IStream spStreamMS = null;
            byte reserved = 0;
            try
            {
                IntPtr f = (IntPtr)0;
                Stg.OpenStream("Info", f, grfMode, 0, out spStreamMS);
                spStream = spStreamMS as IStream;
            }
            catch
            {
            }
            if (spStream == null)
            {
                IZUtility pUty = m_izBaseApp.Utility;
                grfMode = (uint)(STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE);
                if (pUty!= null)
                    spStream = pUty.OpenStream(spiStorage, "Info", grfMode);
              
      
            }

            if (spStream!=null)
            {
               
                uint dummy = 0;
                string data1;
                ReadString(spStream, out data1);
                
                byte[] arrLen = new byte[sizeof(UInt32)];
                spStream.RemoteRead(out arrLen[0], sizeof(UInt32), out dummy);
                UInt32 myVersionNum = BitConverter.ToUInt32(arrLen, 0);
               
                Marshal.ReleaseComObject(spStream);
            }
            Marshal.ReleaseComObject(spiStorage);
        }

    }
}



