using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using interop.ICApiIronCAD.SelectionTool;

namespace ICApiAddin.SelectionTool
{
    [Guid("8B3BC00C-E8CA-407F-8CD0-D8FE7C9714EB"), ClassInterface(ClassInterfaceType.None), ProgId("SelectionTool.SelectionTool")]
    public class SelectionTool : IZAddinServer
    {
        #region [Private Members]
        private ZAddinSite m_izAddinSite;
        private ZCommandHandler cButton1;
        private SelectionToolAppEvents m_appEvents;
        private IZAssembly m_izTempAssembly;
        private ZRibbonComboBox m_cComboBox;
        private int m_ComboBoxCurSel = 0;

        private uTestToolCoordinate m_uTestool = null;

        const int nButtons = 5;
        private ZCommandHandler[] m_buttons = new ZCommandHandler[nButtons] { null, null, null , null, null };
        private IZDockingBar m_izDockingBar;
        bool m_bDockBarShown;
        #endregion

        //Constructor
        public SelectionTool()
        {

        }

        #region [Properties]
        /// <summary>
        /// Get IRONCAD application object
        /// </summary>
        public IZBaseApp IronCADApp
        {
            get
            {
                if (m_izAddinSite != null)
                    return m_izAddinSite.Application;
                return null;
            }

        }

        internal SelectionToolAppEvents AppEvents
        {
            get { return m_appEvents; }
            set { m_appEvents = value; }
        }
        #endregion

        #region [IZAddinServer Members]
        /// <summary>
        /// This function is called by IRONCAD when the Addin is loaded
        /// </summary>
        /// <param name="piAddinSite"></param>
        public void InitSelf(ZAddinSite piAddinSite)
        {
            //if (piAddinSite != null)
            //{
            //    m_izAddinSite = piAddinSite;
            //    try
            //    {
            //        //Create button handler
            //        stdole.IPictureDisp oImageSmall = ConvertImage.ImageToPictureDisp(Properties.Resources.c1_small);
            //        stdole.IPictureDisp oImageLarge = ConvertImage.ImageToPictureDisp(Properties.Resources.c1_large);
            //        cButton1 = piAddinSite.CreateCommandHandler("SelectionTool", "Selecter", "Selection Tool", "Seletion Tool", oImageSmall, oImageLarge);
            //        cButton1.Enabled = true;

            //        //Control bar
            //        ZControlBar cControlBar;
            //        ZEnvironmentMgr cEnvMgr = this.IronCADApp.EnvironmentMgr;
            //        ZControls cControls;
            //        IZControl cControl;
            //        ZRibbonBar cRibbonBar;

            //        //Setup toolbar for scene
            //        IZEnvironment cEnv = cEnvMgr.get_Environment(eZEnvType.Z_ENV_SCENE);
            //        cRibbonBar = cEnv.GetRibbonBar(eZRibbonBarType.Z_RIBBONBAR);
            //        cControlBar = cEnv.AddControlBar(piAddinSite, "Selection Tool");
            //        cControls = cControlBar.Controls;
            //        cControl = cControls.Add(ezControlType.Z_CONTROL_BUTTON, cButton1.ControlDescriptor, null);

            //        //Add button to RibbonBar
            //        cRibbonBar.AddButton(cButton1.ControlDescriptor);

            //        //Event handlers
            //        cButton1.OnClick += new _IZCommandEvents_OnClickEventHandler(cButton1_OnClick);
            //        cButton1.OnUpdate += new _IZCommandEvents_OnUpdateEventHandler(cButton1_OnUpdate);

            //        //Register App Events
            //        m_appEvents = new SelectionToolAppEvents();
            //        RegisterForAppEvents();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("Error: " + ex.Message);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Addin Server is null.");
            //}

            if (piAddinSite != null)
            {
                m_izAddinSite = piAddinSite;


                string[] strID = new string[nButtons] { "EDOCSharpTest_Create", "EDOCSharpTest_Path", "EDOCSharpTest_Import", "CheckBox", "ComboBox" }; ;
                string[] strName = new string[nButtons] { "Create", "Path", "Import", "CheckBox", "ComboBox" }; ;
                string[] strDesc = new string[nButtons] { "Create Spin Shape", "Create Path", "Import Model", "CheckBox", "ComboBox" }; ;
                string[] strTip = new string[nButtons] { "Create Spin Shape", "Create Path", "Import Model", "CheckBox", "ComboBox" };

                //Control bar
                ZControlBar cControlBar;
                ZEnvironmentMgr cEnvMgr = this.IronCADApp.EnvironmentMgr;
                ZControls cControls;
                IZControl cControl;
                ZRibbonBar cRibbonBar;

                //Setup toolbar for scene
                IZEnvironment cEnv = cEnvMgr.get_Environment(eZEnvType.Z_ENV_SCENE);
                cRibbonBar = cEnv.GetRibbonBar(eZRibbonBarType.Z_RIBBONBAR);
                cControlBar = cEnv.AddControlBar(piAddinSite, "First Addin");
                cControls = cControlBar.Controls;


                //Loop through, adding the buttons...
                for (int i = 0; i < nButtons; i++)
                {
                    stdole.IPictureDisp oImageSmall = ConvertImage.ImageToPictureDisp(Properties.Resources.c1_small);
                    stdole.IPictureDisp oImageLarge = ConvertImage.ImageToPictureDisp(Properties.Resources.c1_large);

                    m_buttons[i] = piAddinSite.CreateCommandHandler(strID[i], strName[i], strDesc[i], strTip[i], oImageSmall, oImageLarge);
                    m_buttons[i].Enabled = true;

                    cControl = cControls.Add(ezControlType.Z_CONTROL_BUTTON, m_buttons[i].ControlDescriptor, null);

                    //Add button to RibbonBar


                    //Event handlers
                    if (i == 0)
                    {
                        cRibbonBar.AddButton(m_buttons[i].ControlDescriptor);
                        m_buttons[i].OnClick += new _IZCommandEvents_OnClickEventHandler(button_Create_OnClick);
                        m_buttons[i].OnUpdate += new _IZCommandEvents_OnUpdateEventHandler(button_Create_OnUpdate);
                    }
                    else if (i == 1)
                    {
                        cRibbonBar.AddButton(m_buttons[i].ControlDescriptor);
                        m_buttons[i].OnClick += new _IZCommandEvents_OnClickEventHandler(button_Path_OnClick);
                        m_buttons[i].OnUpdate += new _IZCommandEvents_OnUpdateEventHandler(button_Path_OnUpdate);
                    }
                    else if (i == 2)
                    {
                        cRibbonBar.AddButton(m_buttons[i].ControlDescriptor);
                        m_buttons[i].OnClick += new _IZCommandEvents_OnClickEventHandler(button_Import_OnClick);
                        m_buttons[i].OnUpdate += new _IZCommandEvents_OnUpdateEventHandler(button_Import_OnUpdate);
                    }
                    else if (i == 3)
                    {
                        cRibbonBar.AddCheckBox(m_buttons[i].ControlDescriptor);
                        m_buttons[i].OnClick += new _IZCommandEvents_OnClickEventHandler(CheckBox_OnClick);
                        m_buttons[i].OnUpdate += new _IZCommandEvents_OnUpdateEventHandler(CheckBox_OnUpdate);
                    }
                    else if (i == 4)
                    {
                        m_cComboBox = cRibbonBar.AddComboBox(m_buttons[i].ControlDescriptor, 180);
                        m_buttons[i].OnComboBoxSelectItem += new _IZCommandEvents_OnComboBoxSelectItemEventHandler(ComboBox_OnComboBoxSelectItem);

                        m_buttons[i].OnClick += new _IZCommandEvents_OnClickEventHandler(ComboBox_OnClick);
                        m_buttons[i].OnUpdate += new _IZCommandEvents_OnUpdateEventHandler(ComboBox_OnUpdate);
                        // m_buttons[i].OnComboBoxSelectItem += SelectionTool_OnComboBoxSelectItem;
                        //m_buttons[i].OnComboBoxSelectItem += ComboBox_OnComboBoxSelectItem;
                        //m_buttons[i].OnComboBoxSelectItem += new SelectionTool_OnComboBoxSelectItem;
               

                    }
                }
                // Register App Events

                m_appEvents = new SelectionToolAppEvents();
                RegisterForAppEvents();

            }
            else
            {
                MessageBox.Show("Addin Server is null.");
            }


        }

        private void SelectionTool_OnTest()
        {
            throw new NotImplementedException();
        }

    
 

        /// <summary>
        /// This function is called by IRONCAD when the Addin is unloaded
        /// </summary>
        public void DeInitSelf()
        {
            //cButton1  -= m_zIronCADApp_ActiveDocChanged;
            m_buttons[0].OnClick -= button_Create_OnClick;
            m_buttons[0].OnUpdate -= button_Create_OnUpdate;

            m_buttons[1].OnClick -= button_Path_OnClick;
            m_buttons[1].OnUpdate -= button_Path_OnUpdate;

            m_buttons[2].OnClick -= button_Import_OnClick;
            m_buttons[2].OnUpdate -= button_Import_OnUpdate;

            m_buttons[3].OnClick -= CheckBox_OnClick;
            m_buttons[3].OnUpdate -= CheckBox_OnUpdate;


            m_buttons[4].OnUpdate -= ComboBox_OnUpdate;
            m_buttons[4].OnComboBoxSelectItem -= ComboBox_OnComboBoxSelectItem;



            for (int i = 0; i < nButtons; i++)
            {
                m_buttons[i] = null;
            }

            UnRegisterForAppEvents();
        }

        #endregion

        #region [Public Methods]
        public bool RegisterForAppEvents()
        {
            try
            {
                if (m_appEvents != null && this.IronCADApp != null)
                {
                    m_appEvents.RegisterForAppEvents(this.IronCADApp);
                    return true;
                }
            }
            catch
            {
                //Do nothing
            }
            return false;
        }

        public bool UnRegisterForAppEvents()
        {
            try
            {
                if (m_appEvents != null && this.IronCADApp != null)
                {
                    m_appEvents.UnRegisterForAppEvents();
                    return true;
                }
            }
            catch
            {
                //Do nothing
            }
            return false;
        }

        public bool StartSelectionTool(IZDoc iZDoc)
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
                        SelectionToolDocSelectionEvents selectionEvent = new SelectionToolDocSelectionEvents(cZselectionEvents, iZDoc);

                        //Mouse Events
                        ZMouseEvents zMouseEvents = cZinteractor.MouseEvents;
                        SelectionToolMouseEvents mouseEvents = new SelectionToolMouseEvents(zMouseEvents);

                        //Start
                        cZinteractor.Start();
                        MessageBox.Show("cZinteractor.Start();");
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


        #endregion

        #region [Private Methods]
        /// <summary>
        /// This function is used to determine whether the button 1 on the ribbonbar or toolbar is enabled or not
        /// </summary>
        private void cButton1_OnUpdate()
        {
            cButton1.Enabled = true;  //Change to cButton1.Enabled = false; to disable the button  
        }

        /// <summary>
        /// This function is called when the button 1 on the ribbonbar or toolbar is clicked
        /// </summary>
        private void cButton1_OnClick()
        {
            //ZMathPoint temp = new ZMathPoint();
            //temp.X = 0.01;
            //temp.Y = 0.02;
            //temp.Z = 0.03;
            //TestModelCreation_Sphere(temp,0.01);
            TestModelCreation_Spin();
            TestModelCreation_Block();

            TestSpinCreation();

            IZDoc iZDoc = GetActiveDoc();
            if (iZDoc != null)
            {
                DocSelectionForm frm = new DocSelectionForm();
                frm.IronCADDocument = iZDoc;
                frm.Show();
            }

            //int TestCreate = 1;
            //if (TestCreate == 1)
            //{
            //    TestModelCreation_Spin();
            //    TestModelCreation_Block();

            //}

            //IZDoc iZDoc = GetActiveDoc();
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "Step Files(*.stp)|*.stp|All Files(*.*)|*.*";
            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            //    string importFileName = dlg.FileName;
            //    if (string.IsNullOrEmpty(importFileName))
            //        return;

            //    IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
            //    if (iZsceneDoc == null)
            //        return;
            //    iZsceneDoc.ImportModel(importFileName, false);
            //}



        }

        private void TestModelCreation_Spin()
        {
            IZDoc iZDoc = GetActiveDoc();
            IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
            if (iZsceneDoc == null)
                return;


            IZProfile iNewProfile = iZsceneDoc.CreateProfile();
            if (iNewProfile == null)
                return;

            //Create a
            double[] dUL = { 0.01, 0.0 };
            double[] dUR = { 0.00, 0.02 };
            int id1 = iNewProfile.CreateLine(dUL, dUR, 1);

            //create a new part

            IZPart iNewPart = iZsceneDoc.CreatePart();
            if (iNewPart == null)
                return;


            IZPartFeatureMgr iPartFeatMgr = iNewPart as IZPartFeatureMgr;
            if (iPartFeatMgr == null)
                return;

            double angle = 2 * Math.PI;
            //Create the cone feature...
            IZSpinFeature cone = iPartFeatMgr.CreateSpinFeature(eZOperationType.Z_UNITE, false, iNewProfile, eZFeatureProfileRelType.Z_FEATURE_PROFILE_ABSORB, angle, false);


            IZProfile iNewSplineProfile = iZsceneDoc.CreateProfile();
            if (iNewSplineProfile == null)
                return;

            //create a 4 points natural spline;
            double[] dPoints = { 0.01, 0.0, 0.005, 0.01, 0.015, 0.02, 0.008, 0.03 };
            double[] startDir = { 0, 0 };
            double[] endDir = { 0, 0 };
            iNewSplineProfile.CreateInterpolationSpline(dPoints, startDir, endDir, 1);

            //Move the profile to {0,0,0.03} position;
            IZBaseApp iZBaseApp = this.IronCADApp;
            IZMathUtility iZMathUtl = iZBaseApp as IZMathUtility;
            double[] dVec = { 0, 0, 0.03 };
            ZMathVector iZVec = iZMathUtl.CreateMathVector(dVec);
            ZMathMatrix iMtx = iZMathUtl.CreateTranslationMathMatrix(iZVec);

            IZSceneElement iSplineSceneEle = iNewSplineProfile as IZSceneElement;

            iSplineSceneEle.SetPositionTransform(iMtx);

            IZSpinFeature splineSpinFeat = iPartFeatMgr.CreateSpinFeature(eZOperationType.Z_UNITE, false, iNewSplineProfile, eZFeatureProfileRelType.Z_FEATURE_PROFILE_ABSORB, angle, false);

            iNewPart.Update();
            return;
        }

        private void TestModelCreation_Block()
        {
            IZDoc iZDoc = GetActiveDoc();
            IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
            if (iZsceneDoc == null)
                return;


            IZProfile iNewProfile = iZsceneDoc.CreateProfile();
            if (iNewProfile == null)
                return;
            double[] dUL = { 0.01, -0.01 };
            double[] dUR = { 0.01, 0.01 };
            double[] dLL = { -0.01, -0.01 };
            double[] dLR = { -0.01, 0.01 };
            int id1 = iNewProfile.CreateLine(dUL, dUR, 1);
            int id2 = iNewProfile.CreateLine(dUR, dLR, 2);
            int id3 = iNewProfile.CreateLine(dLR, dLL, 3);
            int id4 = iNewProfile.CreateLine(dLL, dUL, 4);

            //create a new part
            IZPart iNewPart = iZsceneDoc.CreatePart();
            if (iNewPart == null)
                return;

            IZPartFeatureMgr iPartFeatMgr = iNewPart as IZPartFeatureMgr;
            if (iPartFeatMgr == null)
                return;

            //Create the block...
            IZExtrudeFeature spiBlock = iPartFeatMgr.CreateExtrudeFeature(eZOperationType.Z_UNITE, false, 0.01, 0.00, 0, iNewProfile, eZFeatureProfileRelType.Z_FEATURE_PROFILE_ABSORB);

            iNewPart.Update();
            return;
        }

        private void TestSpinCreation()
        {
            IZDoc iZDoc = GetActiveDoc();
            IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
            if (iZsceneDoc == null)
                return;

            if (m_izTempAssembly == null)
            {

                m_izTempAssembly = iZsceneDoc.CreateAssembly();

                IZWire izWire = m_izTempAssembly.CreateWire();

                double[] points = { 0, 0, 0, 0.1, 0, 0, 0.1, 0.1, 0, 0.1, 0.1, 0.1 };
                double[] startVec = { 0, 0, 0, 0 };
                double[] startEnd = { 0, 0, 0, 0 };
                int id = 0;
                int returnID = izWire.CreateInterpolationSpline(points, startVec, startEnd, id);
            }
            else
            {
                IZElement izElem = m_izTempAssembly as IZElement;
                izElem.RemoveEx(false);
                m_izTempAssembly = null;
            }
            iZsceneDoc.Update();
        }

        public void TestModelCreation_Sphere(ZMathPoint CenterPt, double rad)
        {
            IZDoc iZDoc = GetActiveDoc();
            IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
            if (iZsceneDoc == null)
                return;

            IZPart iNewPart = iZsceneDoc.CreatePart();
            if (iNewPart == null)
                return;

            IZPartFeatureMgr iPartFeatMgr = iNewPart as IZPartFeatureMgr;
            if (iPartFeatMgr == null)
                return;

            object OCenterPt = new double[3] { CenterPt.X, CenterPt.Y, CenterPt.Z };
            IZSpinFeature Sphere = iPartFeatMgr.CreateSphere(rad, OCenterPt);

            iNewPart.Update();
            return;
        }

        private IZDoc GetActiveDoc()
        {
            if (this.IronCADApp != null)
            {
                return this.IronCADApp.ActiveDoc;
            }
            return null;
        }

        private void button_Create_OnUpdate()
        {
            m_buttons[0].Enabled = true;  //Change to m_button.Enabled = false; to disable the button  
        }
        private void button_Path_OnUpdate()
        {
            m_buttons[1].Enabled = true;  //Change to m_button.Enabled = false; to disable the button  
            ZControlDescriptor des = m_buttons[1].ControlDescriptor;
            string name = des.DisplayName;
            name = name + "+";
            des.DisplayName = name;
        }
        private void button_Import_OnUpdate()
        {
            m_buttons[2].Enabled = true;  //Change to m_button.Enabled = false; to disable the button  
        }

        private void button_Create_OnClick()
        {
            IZDoc iZDoc = GetActiveDoc();
            /*
            if (iZDoc != null)
            {
                ICToolsForm frm = new ICToolsForm();
                frm.BaseApp = this.IronCADApp;
                frm.IronCADDocument = iZDoc;
                frm.Show();
            }
            */
            IZPart part1 = null;
            IZSceneDoc iZsceneDoc = iZDoc as IZSceneDoc;
            if (iZsceneDoc != null)
            {
                bool bactive = iZsceneDoc.IsTriballActive();
                IZSelectionMgr iZselectionMgr = iZsceneDoc.SelectionMgr;
                if (iZselectionMgr != null)
                {
                    object varElements = iZselectionMgr.GetSelectedElements();
                    object[] oElements = varElements as object[];

                    bool bcreated = false;
                    if (oElements != null)
                    {
                        foreach (object oEle in oElements)
                        {
                            IZElement izEle = oEle as IZElement;
                            if (izEle != null && (izEle.Type == eZElementType.Z_ELEMENT_ASSEMBLY ||
                                                    izEle.Type == eZElementType.Z_ELEMENT_PART ||
                                                    izEle.Type == eZElementType.Z_ELEMENT_SHEETMETAL_PART ||
                                                    izEle.Type == eZElementType.Z_ELEMENT_WIRE
                                                    )
                                            )
                            {
                                IZSceneElement izSceneEle = izEle as IZSceneElement;
                                izSceneEle.Ghost = true;
                                izSceneEle.Ghost = false;

                                bool vbInterfere = false;
                                bool vbInterfere2 = false;
                                if (part1 == null)
                                {
                                    part1 = oEle as IZPart;
                                }
                                else
                                {


                                    vbInterfere = part1.HasInterference(izEle);

                                    ZArray dummy = null;
                                    //  part1.CheckInterference(izEle, out vbInterfere2, out dummy);
                                    part1.CheckInterference(izEle, out vbInterfere2, out dummy);
                                    if (vbInterfere != vbInterfere2)
                                    {
                                        MessageBox.Show("CheckInterference error");
                                    }


                                    IZPart part2 = izEle as IZPart;
                                    if (part2 != null)
                                    {
                                        IZBody b1 = part1.GetBody(true);

                                        IZBody b2 = part2.GetBody(true);
                                        if (b1 != null && b2 != null)
                                        {
                                            object Box1 = b1.GetBoundingBox();
                                            object Box2 = b2.GetBoundingBox();

                                            IZSceneElement zSE1 = part1 as IZSceneElement;
                                            IZSceneElement zSE2 = part2 as IZSceneElement;
                                            ZMathMatrix mToG1 = zSE1.GetTransformToGlobal();
                                            ZMathMatrix mToG2 = zSE2.GetTransformToGlobal();
                                            IZBaseApp iZBaseApp = this.IronCADApp;
                                            IZMathUtility iZMathUtl = iZBaseApp as IZMathUtility;
                                            bool bYes = iZMathUtl.CheckBoxIntersect(Box1, mToG1, Box2, mToG2);

                                            if (vbInterfere2 != bYes)
                                                MessageBox.Show("BoundingBox check result is different from CheckInterference");

                                        }
                                    }

                                }




                                //bool b = true;
                                IZExtensibility iZExtensibility = izEle as IZExtensibility;
                                if (iZExtensibility != null)
                                {
                                    IZExtensionDataObject obj = null;
                                    try
                                    {
                                        obj = ((IZExtensibility)izEle).GetExtensionObject("SelectionTool.TestEDO", false);

                                        if (obj == null)
                                        {
                                            MessageBox.Show("Does not GetExtensionObject:SelectionTool.TestEDO in SelectionTool.dll");
                                            obj = iZExtensibility.AddExtensionObject("SelectionTool.TestEDO", false);
                                        }
                                        else
                                            MessageBox.Show("Get GetExtensionObject: SelectionTool.TestEDO in SelectionTool.dll");


                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (obj == null)
                                    {
                                        try
                                        {
                                            //iZExtensibility.AddExtensionObject("EDOCSharpTest.TestEDO", false);
                                            // iZExtensibility.AddExtensionObject("{4CEAFE3F-BFC7-4E53-9002-D80C7D009A9D}", false);
                                            //  iZExtensibility.AddExtensionObject("SelectionTool.TestEDODraw", false);

                                            obj = iZExtensibility.AddExtensionObject("SelectionTool.DropHiliteEDO", false);
                                            //   iZExtensibility.AddExtensionObject("4143083C-3952-45C3-97C3-6F934282C50E", true);
                                            bcreated = true;
                                            if (obj != null)
                                                MessageBox.Show("AddExtensionObject: SelectionTool.TestEDO in SelectionTool.dll");
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Error: " + ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        bcreated = true;
                                        string name = obj.Name;
                                        string desc = obj.Description;
                                     


                                    }
                                }
                            }

                        }

                    }
                    if (bcreated == false)
                    {
                        ZEDOSiteElement siteElement;
                        iZDoc.InsertNewEDOSiteElement(out siteElement);
                        IZExtensibility iZExtensibility = siteElement as IZExtensibility;
                        if (iZExtensibility != null)
                        {
                            //iZExtensibility.AddExtensionObject("EDOCSharpTest.TestEDO", false);
                            try
                            {
                                iZExtensibility.AddExtensionObject("SelectionTool.TestEDO", false);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.Message);
                            }
                            iZsceneDoc.UpdateGraphics(0);
                            iZsceneDoc.UpdateBrowser(0);
                            //iZExtensibility.AddExtensionObject("{4CEAFE3F-BFC7-4E53-9002-D80C7D009A9C}", false);                           
                        }

                        /*
                        IZElement izEle =  iZsceneDoc.GetTopElement();
                        IZExtensibility iZExtensibility = izEle as IZExtensibility;
                        if (iZExtensibility != null  )
                        {
                            IZEDORenderCallback piCallback = null;
                            try
                            {
                                piCallback = iZExtensibility.GetExtensionObject("{4CEAFE3F-BFC7-4E53-9002-D80C7D009A9C}", false);
                            }
                           catch (Exception ex)
                           {
                         
                           }
                            if (piCallback == null)
                            {
                                //iZExtensibility.AddExtensionObject("EDOCSharpTest.TestEDO", false);
                                iZExtensibility.AddExtensionObject("{4CEAFE3F-BFC7-4E53-9002-D80C7D009A9C}", false);
                            }
                        
                        }
                         * */
                    }
                }
            }
        }
        private void button_Path_OnClick()
        {
            cButton1_OnClick();
            IZDoc iZDoc = GetActiveDoc();
            ToggleDockBarWindow();
            /*
            if (iZDoc != null)
            {
                ICToolsForm frm = new ICToolsForm();
                frm.BaseApp = this.IronCADApp;
                frm.IronCADDocument = iZDoc;
                frm.Show();
            }
            */
        }
        private void button_Import_OnClick()
        {
            IZDoc iZDoc = GetActiveDoc();
            if (m_uTestool == null)
                m_uTestool = new uTestToolCoordinate(iZDoc, this);
            //  IZDoc iZDoc = GetActiveDoc();
            ////  StartSelectionTool(iZDoc);

            //  if (iZDoc != null)
            //  {
            //      DocSelectionForm frm = new DocSelectionForm();
            //      frm.IronCADDocument = iZDoc;
            //      frm.Show();
            //      /*
            //      ICToolsForm frm = new ICToolsForm();
            //      frm.BaseApp = this.IronCADApp;
            //      frm.IronCADDocument = iZDoc;
            //      frm.Show();
            //       * */
            //  }

        }

        public void DoneSelection()
        {
            if (m_uTestool != null)
            {
                m_uTestool = null;
                // GC.Collect();
            }
        }

        private void ToggleDockBarWindow()
        {
            if (m_izDockingBar == null)
            {

                IZEnvironmentMgr izEnvMgr = this.IronCADApp.EnvironmentMgr;
                IZEnvironment izEnv = izEnvMgr.get_Environment(eZEnvType.Z_ENV_SCENE);
                uint alignment = 0xE81D; // AFX_IDW_DOCKBAR_RIGHT;
                m_izDockingBar = izEnv.AddDockingBar(m_izAddinSite, 1, "EDOCShapeTestDockingBar", alignment);
                m_izDockingBar.ShowControlBar(1, 0, 1);
                m_bDockBarShown = true;
                //long handle = m_izDockingBar.CreateSubWindow("EDOSubWindow");

                // STDMETHODIMP CZDockingBar::CreateSubWindow(/*[in]*/ BSTR bstrClassName, /*[out, retval]*/ LONG_PTR *plHWND);

                ulong handle = m_izDockingBar.GetCWnd();

                NativeWindow nw = new NativeWindow();
                nw.AssignHandle((IntPtr)handle);
                // NativeWindow(handle);
                IntPtr h = nw.Handle;

                Control ctrl = Control.FromHandle((IntPtr)h);

                //HwndSource hwndSource = HwndSource.FromHwnd(handle);

                //Window = hwndSource.RootVisual as Window;

                if (ctrl != null)
                {
                    Type type = ctrl.GetType();

                    TreeView tv = new TreeView();
                    tv.Nodes.Add("First Node");
                    tv.Nodes.Add("Second Node");
                    TreeNode treeNode = new TreeNode("Windows");

                    ctrl.Controls.Add(tv);

                }


                ZWindowEvents zEvents = m_izDockingBar as ZWindowEvents;

                DockBarEvents.RegisterEvents(zEvents);



            }
            else
            {

                m_bDockBarShown = !m_bDockBarShown;
                int show = Convert.ToInt32(m_bDockBarShown);
                m_izDockingBar.ShowControlBar(show, 0, 0);

                // m_izDockingBar.ShowControlBar(0, 0, 0);
            }
        }


        private void CheckBox_OnUpdate()
        {
            m_buttons[3].Enabled = true;  //Change to m_button.Enabled = false; to disable the button  
        }

        private void CheckBox_OnClick()
        {

            bool bchecked = m_buttons[3].CheckState;
            if (bchecked == true)
                m_buttons[3].CheckState = false;
            else
                m_buttons[3].CheckState = true;
        }
        private void ComboBox_OnClick()
        {

   
        }
        private void ComboBox_OnUpdate()
        {
            m_buttons[4].Enabled = true;  //Change to m_button.Enabled = false; to disable the button  


            if (m_cComboBox != null && m_cComboBox.Count == 0)
            {
                string bstr = "MyItem1";

                m_cComboBox.InsertItem(0, bstr);

                bstr = "MyItem2";
                m_cComboBox.InsertItem(1, bstr);

                bstr = "MyItem3";
       
                m_cComboBox.InsertItem(2, bstr);

                string bstrGet = m_cComboBox.GetItem(1);
                m_cComboBox.CurSel = m_ComboBoxCurSel;
            }
        }

        void ComboBox_OnComboBoxSelectItem(int pos, string bstrItem, out bool pvbOverride)
        {
            MessageBox.Show(bstrItem);
            if ( pos == 1)
            {
            
            }
            pvbOverride = false;
        }
        #endregion
    }
}
