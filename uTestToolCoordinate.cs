using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using interop.ICApiIronCAD.SelectionTool;
//using uPLibrary.Networking.M2Mqtt;

namespace ICApiAddin.SelectionTool
{
   // public partial class uTestToolCoordinate : UserControl
     public partial class uTestToolCoordinate
    {
        IZDoc iZDoc;
        CZInteractor cZinteractor;
        SelectionTool m_OpenTool;
        // MqttClient client;
        public uTestToolCoordinate(IZDoc izDoc, SelectionTool opentool )
        {
            m_OpenTool = opentool;
            //InitializeComponent();
            iZDoc = izDoc;
            if (iZDoc != null)
            {
                IZSceneDoc iZSceneDoc = iZDoc as IZSceneDoc;
                if (iZSceneDoc != null)
                {
                    IZSelectionMgr iZselectionMgr = iZSceneDoc.SelectionMgr;
                    if (iZselectionMgr != null)
                    {
                        int types = (int)(eZSelectionType.Z_SEL_ASSEMBLY);
                        cZinteractor = iZselectionMgr.CreateInteractor();
                        cZinteractor.Multiselect = false;
                        CZSelectorEvents m_SelectionEvents = cZinteractor.SelectEvents;
                        m_SelectionEvents.SetSelectionFilterChoices(types, eZSelectionType.Z_SEL_ASSEMBLY);
                        m_SelectionEvents.Selected += M_SelectionEvents_Selected;

                        cZinteractor.Start();
                        iZSceneDoc.UpdateGraphics(0);
                    }
                }
            }
        }

        private void M_SelectionEvents_Selected(IZElement piElement, ZMathPoint piModelCoord, int lXWindowPixel, int lYWindowPixel, int lEFlags, [System.Runtime.InteropServices.ComAliasName("interop.GetToolCoordinate.eZEntityType")] eZEntityType eEntType, object varEntIds)
        {
            CZSelectorEvents m_SelectionEvents = cZinteractor.SelectEvents;
            m_SelectionEvents.Selected -= M_SelectionEvents_Selected;
            cZinteractor.Stop();

            MessageBox.Show("OK");
            if (m_OpenTool != null)
                m_OpenTool.DoneSelection();

        }
    }
}
