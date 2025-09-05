using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using interop.ICApiIronCAD.SelectionTool;
using System.Runtime.InteropServices;

namespace ICApiAddin.SelectionTool
{
    [Guid("CEE56433-BD2C-494C-8889-0B8BBFE57774"), ClassInterface(ClassInterfaceType.None), ProgId("SelectionTool.DropHiliteEDO")]
    public class DropHiliteEDO : IZExtensionDataObject, IZEDOHighlightCallback, IZEDORenderCallback
    {
        public string Description
        {
            get { return "SelectionTool.DropHiliteEDO Description"; }
        }

        public string Developer
        {
            get { return "SelectionTool.DropHiliteEDO Dev"; }
        }

        public string Name
        {
            get { return "SelectionTool.DropHiliteEDO Name"; }
        }

        public void OnInitialized(IZElement piElement)
        {
           // throw new NotImplementedException();
        }

        public void OnLoadCompleted(IZElement piElement)
        {
         //   throw new NotImplementedException();
        }

        public void OnDrawHitHighlightTopology(IZRayTestResult piRayTest, CZRender piRender, ref bool pvbOverride)
        {
            IZElement piElement = piRayTest.HitElement;
            if (piElement == null)
            {
                return;
            }
            IZMathPoint spiPoint = piRayTest.HitPoint;
            // Get the point values...
            double dX = 0.0, dY = 0.0, dZ = 0.0;
            string name = piElement.Name;

            // Given the hit point figure out where on the 2D screen it cooresponds to..
            int lScreenX = 0, lScreenY = 0;

            piRender.XformModelToView(spiPoint.X, spiPoint.Y, spiPoint.Z, out lScreenX, out lScreenY);

            // Now specify the font and the text for what we will draw...
            string Font = "Arial";
            double dSizeHeader = 24.0;
            piRender.SetTextStyle(Font, dSizeHeader, false, true);

            int lSX = 0;
            int lSY = 0;
            int lColorSelected = ((int)(((byte)(255) | ((int)((byte)(255)) << 8)) | (((int)(byte)(255)) << 16)));
            piRender.DrawTextString2D(name, lScreenX, lScreenY, lColorSelected, out lSX, out lSY);

        }

        public void OnDraw(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
           // throw new NotImplementedException();
        }

        public void OnDrawAccessories(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
          //  throw new NotImplementedException();
        }

        public void OnHotDraw(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
          //  throw new NotImplementedException();
        }
    }
}
