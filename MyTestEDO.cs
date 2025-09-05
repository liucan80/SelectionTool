using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using interop.ICApiIronCAD.SelectionTool;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using stdole;

namespace ICApiAddin.SelectionTool
{
    [Guid("4CEAFE3F-BFC7-4E53-9002-D80C7D009A9D"), ClassInterface(ClassInterfaceType.None), ProgId("SelectionTool.TestEDO")]
    public class TestEDO : IZExtensionDataObject, IZEDOSelectionCallback, IZEDOTriballCallback, IZEDORenderCallback, IZEDOMouseCallback, IPersistStream, IZEDOTreeBrowserCallback
    {
        #region [Private Members]
        private double[] m_points = { 0, 0, 0, 0.1, 0, 0, 0.1, 0.1, 0, 0.1, 0.1, 0.1 };
        private int m_iHitPt = -1;
        int count = 1;
        IZBaseApp m_baseapp;
        bool m_InLoop;
        IZMathUtility iUtility;

        IZSceneElement piSceneEle;
        #endregion
        //Constructor
        public TestEDO()
        {
            m_InLoop = false;
        }

        #region [Public IZExtensionDataObject]
        public string Description
        {
            get { return "SelectionTool.TestEDO Description"; }
        }

        public string Developer
        {
            //  get { throw new NotImplementedException(); }
            get { return "SelectionTool.TestEDO Developer"; }
        }

        public string Name
        {
            // get { throw new NotImplementedException(); }
            get { return "SelectionTool.TestEDO Name"; }
        }

        public void OnInitialized(IZElement piElement)
        {
            //  throw new NotImplementedException();
        }

        public void OnLoadCompleted(IZElement piElement)
        {
            //  throw new NotImplementedException();
        }
        #endregion

        #region [Public IZEDOSelectionCallback]
        public void OnDeselect(IZElement piElement, ref bool pvbOverride)
        {
            // throw new NotImplementedException();
        }

        public void OnSelect(IZElement piElement, ref bool pvbOverride)
        {
            // throw new NotImplementedException();
        }
        #endregion


        #region [Public IZEDOTriballCallback]
        public void OnPostTriballCopy(IZElement piNewElement, ref bool pvbHandled)
        {
            // throw new NotImplementedException();
        }

        public void OnPostTriballMove(IZElement piElement, ref bool pvbHandled)
        {
            // throw new NotImplementedException();
        }

        public void OnPreTriballCopy(IZElement piElementToBeCopied, ref bool pvbHandled)
        {
            //throw new NotImplementedException();
        }

        public void OnPreTriballMove(IZElement piElement, ref bool pvbHandled)
        {
            //throw new NotImplementedException();
        }
        #endregion

        public void OnDrawWithColor(IZElement piElement, CZRender piRender, ref bool pvbOverride, int lColor)
        {
            //throw new NotImplementedException();

            // int lColor = myRgbColor.ToArgb();

            // long lColor = RGB(200, 200, 200);

            int lSize = 1;
            uint hints = 0;
            piRender.DrawPoint3D(m_points, lColor, lSize, hints);
        }
        void TimerTask2()
        {
            ZMathMatrix MatZrot = null;
            double[] mat2 = new double[16];
            mat2[0] = 1; mat2[4] = 0; mat2[8] = 0; mat2[12] = count;
            mat2[1] = 0; mat2[5] = 1; mat2[9] = 0; mat2[13] = 0;
            mat2[2] = 0; mat2[6] = 0; mat2[10] = 1; mat2[14] = 0;
            mat2[3] = 0; mat2[7] = 0; mat2[11] = 0; mat2[15] = 1;
            object res = (object)mat2;
            //  m_baseapp.RegisterICAppClassesForThisThread();

            IZMathUtility utl = m_baseapp as IZMathUtility;
            if (utl != null)
            {
                MatZrot = utl.CreateMathMatrix(res);
            }
            // ZMathMatrix mth = utl.CreateMathMatrix(res);
            MatZrot = iUtility.CreateMathMatrix(res);

            piSceneEle.CreateLinks(1, MatZrot);
            IZElement piEle = piSceneEle as IZElement;
            IZSceneDoc scenedoc = piEle.OwningDoc as IZSceneDoc;
            uint option = (uint)eZDrawOption.Z_DRAW_ALLOW_CHANGE_CAMERA;

            scenedoc.UpdateGraphics(option);

        }

        protected void TimerTask(object state)
        {
            IZMathUtility utl = state as IZMathUtility;
            return;


            ZMathMatrix MatZrot = null;
            double[] mat2 = new double[16];

            mat2[0] = 1; mat2[4] = 0; mat2[8] = 0; mat2[12] = count;
            mat2[1] = 0; mat2[5] = 1; mat2[9] = 0; mat2[13] = 0;
            mat2[2] = 0; mat2[6] = 0; mat2[10] = 1; mat2[14] = 0;
            mat2[3] = 0; mat2[7] = 0; mat2[11] = 0; mat2[15] = 1;

            object res = (object)mat2;

            ZMathMatrix mth = utl.CreateMathMatrix(res);


            MatZrot = iUtility.CreateMathMatrix(res);

            piSceneEle.CreateLinks(1, MatZrot);

            count++;
        }


        #region [Public IZEDORenderCallback]
        public void OnDraw(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
            // Color myRgbColor = Color.Red;
            int lColor = ((int)(((byte)(150) | ((int)((byte)(0)) << 8)) | (((int)(byte)(0)) << 16)));
            OnDrawWithColor(piElement, piRender, ref pvbOverride, lColor);
        }

        public void OnDrawAccessories(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
            // throw new NotImplementedException();
        }

        public void OnHotDraw(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
            // throw new NotImplementedException();
            //Color myRgbColor = Color.Yellow;

            if (m_iHitPt != -1)
            {
                //selected
                int lColorSelected = ((int)(((byte)(255) | ((int)((byte)(255)) << 8)) | (((int)(byte)(0)) << 16)));
                double[] hitPt = new double[3];
                hitPt[0] = m_points[m_iHitPt * 3];
                hitPt[1] = m_points[m_iHitPt * 3 + 1];
                hitPt[2] = m_points[m_iHitPt * 3 + 2];
                int lSize = 2;
                uint hints = 0;
                piRender.DrawPoint3D(hitPt, lColorSelected, lSize, hints);
            }
            else
            {
                int lColor = ((int)(((byte)(0) | ((int)((byte)(255)) << 8)) | ((int)((byte)(255)) << 16)));
                OnDrawWithColor(piElement, piRender, ref pvbOverride, lColor);

            }

        }
        #endregion

        public void OnLButtonDblClick(IZElement piElement, CZRender piRender, long lHWND, int lClientX, int lClientY, ZMathPoint piWorldPoint, ZMathVector piWorldRay, int lKeyState, ref bool pvbOverride)
        {
            // throw new NotImplementedException();
            if (!m_InLoop)
            {
                m_InLoop = true;

                m_baseapp = piElement.Application;
                iUtility = m_baseapp as IZMathUtility;

                IZElement Target = null;

                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Select CAD file";
                dialog.InitialDirectory = ".\\";
                dialog.Filter = "stp files (*.*)|*.ics";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(dialog.FileName);
                    IZSceneDoc iZsceneDoc = piElement.OwningDoc as IZSceneDoc;

                    if (iZsceneDoc != null)
                    {
                        Target = iZsceneDoc.ImportModel(dialog.FileName, true);
                    }
                }

                piSceneEle = Target as IZSceneElement;
                while (m_InLoop)
                    TimerTask2();
            }
            else
            {
                m_InLoop = false;
            }
            /*
                            Thread newThread =
                                  new Thread(new ThreadStart(TimerTask2));
                           newThread.SetApartmentState(ApartmentState.MTA);


                            newThread.Start();






                        TimerCallback callback = new TimerCallback(TimerTask);

                            //建立Timer物件，第一個參數為TimerCallback，第二個參數表示要傳給callback的物件
                            //第三個參數代表要多久之後執行(延遲啟動)，第四個參數代表每隔多久執行一次(延遲時間)
                            //該物件一旦產生就立刻計時了！
                            System.Threading.Timer timer = new System.Threading.Timer(callback, iUtility, 10000, 10000);

                           // timer.SetApartmentState(ApartmentState.MTA);

                            double[] mat2 = new double[16];

                            mat2[0] = 1; mat2[4] = 0; mat2[8] = 0; mat2[12] = 1;
                            mat2[1] = 0; mat2[5] = 1; mat2[9] = 0; mat2[13] = 0;
                            mat2[2] = 0; mat2[6] = 0; mat2[10] = 1; mat2[14] = 0;
                            mat2[3] = 0; mat2[7] = 0; mat2[11] = 0; mat2[15] = 1;

                            object res = (object)mat2;
                            ZMathMatrix MatZrot = iUtility.CreateMathMatrix(res);

                            piSceneEle.CreateLinks(1, MatZrot);

                        }
                        */
        }

        public void OnLButtonDown(IZElement piElement, CZRender piRender, long lHWND, int lClientX, int lClientY, ZMathPoint piWorldPoint, ZMathVector piWorldRay, int lKeyState, ref bool pvbOverride)
        {
            //throw new NotImplementedException();
        }

        public void OnLButtonUp(IZElement piElement, CZRender piRender, long lHWND, int lClientX, int lClientY, ZMathPoint piWorldPoint, ZMathVector piWorldRay, int lKeyState, ref bool pvbOverride)
        {
            // throw new NotImplementedException();
        }

        public void OnMouseMove(IZElement piElement, CZRender piRender, long lHWND, int lClientX, int lClientY, ZMathPoint piWorldPoint, ZMathVector piWorldRay, int lKeyState, ref bool pvbOverride)
        {
            int iOldHitPtr = m_iHitPt;
            m_iHitPt = -1;
            // throw new NotImplementedException();
            //double[] points = { 0, 0, 0, 0.1, 0, 0, 0.1, 0.1, 0, 0.1, 0.1, 0.1 };
            for (int i = 0; i < 4; i++)
            {

                double viewX = 0;
                double viewY = 0;
                double viewZ = 0;
                piRender.XformModelToView3(m_points[i * 3], m_points[i * 3 + 1], m_points[i * 3 + 2], out viewX, out viewY, out viewZ);

                long boxsize = 5;
                if ((viewX <= lClientX + boxsize) && (viewX >= lClientX - boxsize) &&
                      (viewY <= lClientY + boxsize) && (viewY >= lClientY - boxsize))
                {
                    m_iHitPt = i;
                    break;
                }

            }
            if (m_iHitPt != iOldHitPtr)
            {
                IZDoc iZDoc = piElement.OwningDoc;
                IZSceneDoc iZSceneDoc = iZDoc as IZSceneDoc;
                if (iZSceneDoc != null)
                    iZSceneDoc.Redraw();
            }
        }


        public void OnRButtonDown(IZElement piElement, CZRender piRender, long lHWND, int lClientX, int lClientY, ZMathPoint piWorldPoint, ZMathVector piWorldRay, int lKeyState, ref bool pvbOverride)
        {
            //  throw new NotImplementedException();
        }

        public void GetClassID(out Guid pClassID)
        {
            pClassID = new Guid("4CEAFE3F-BFC7-4E53-9002-D80C7D009A9D");
        }


        public void GetSizeMax(out _ULARGE_INTEGER pcbSize)
        {
            throw new NotImplementedException();
        }

        public void IsDirty()
        {
            throw new NotImplementedException();
        }

        void ReadString(IStream pstm, out string mystring)
        {
            uint dummy = 0;
            byte[] arrLen = new byte[4];
            pstm.RemoteRead(out arrLen[0], sizeof(int), out dummy);
            uint size = BitConverter.ToUInt32(arrLen, 0);
            byte[] data = new byte[size];
            pstm.RemoteRead(out data[0], size, out dummy);

            Encoding encode = System.Text.ASCIIEncoding.Unicode;
            //mystring = BitConverter.ToString(data, 0);
            mystring = encode.GetString(data);
        }

        public void Load(IStream pstm)
        {
            // throw new NotImplementedException();
            string data1;
            string data2;
            ReadString(pstm, out data1);
            ReadString(pstm, out data2);
            Marshal.ReleaseComObject(pstm);
            pstm = null;
        }

        void WriteString(IStream pstm, string mystring)
        {
            Encoding encode = System.Text.ASCIIEncoding.Unicode;
            byte[] data = encode.GetBytes(mystring);
            int size = data.Length;
            byte[] arrLen = BitConverter.GetBytes(size);
            uint dummy = 0;
            pstm.RemoteWrite(ref arrLen[0], sizeof(int), out dummy);
            pstm.RemoteWrite(ref data[0], (uint)size, out dummy);
        }


        public void Save(IStream pstm, int fClearDirty)
        {
            string data1 = "My Data Start  中文3公元 fff";

            string data2 = "My Data path: c:/temp";
            WriteString(pstm, data1);
            WriteString(pstm, data2);

            //pstm = null;

            Marshal.ReleaseComObject(pstm);
            pstm = null;

        }


        public void OnDrawCustomIcon(IZElement piElement, ref IPictureDisp spiSmallIcon, ref bool pvbAppend)
        {
            stdole.IPictureDisp oIcon = ConvertImage.ImageToPictureDisp(Properties.Resources.Robot);
            spiSmallIcon = oIcon;
            pvbAppend = false;
        }
    }

}

  