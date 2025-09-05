using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using interop.ICApiIronCAD.SelectionTool;
using System.Runtime.InteropServices;

namespace ICApiAddin.SelectionTool
{
   [Guid("06D8B851-0962-4127-8A1F-EAAC25952F4B"), ClassInterface(ClassInterfaceType.None), ProgId("SelectionTool.TestEDODraw")]
    public class TestEDODraw : IZExtensionDataObject, IZEDORenderCallback, IZEDOHittestCallback, IZEDOPropertyChangedCallback
    {

        string IZExtensionDataObject.Description
        {
            get { return "SelectionTool.TestEDODraw Description"; }
        }
        string IZExtensionDataObject.Developer
        {
            get { return "SelectionTool.TestEDODraw Developer XYZ Company"; }
        }

        string IZExtensionDataObject.Name
        {
            get { return "SelectionTool.TestEDODraw Developer XYZ MyName"; }
        }

        void IZExtensionDataObject.OnInitialized(IZElement piElement)
        {
           
        }

        void IZExtensionDataObject.OnLoadCompleted(IZElement piElement)
        {
            
        }

        void IZEDORenderCallback.OnDraw(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
           // throw new NotImplementedException();
            ZCoordinateStyle zStyle =  piRender.CreateCoordinateStyle();
            zStyle.SetAxisLength(eZXYZType.Z_Y, 50);
            int lColorYellow = ((int)(((byte)(250) | ((int)((byte)(250)) << 8)) | (((int)(byte)(0)) << 16)));
            zStyle.SetLineStyle(eZXYZType.Z_Y, lColorYellow, eZLinePattern.Z_SOLID, eZLineHiddenType.Z_NO_HIDDEN_LINES,5);

            piRender.DrawCoordindate(null, zStyle);
        }

        void IZEDORenderCallback.OnDrawAccessories(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
            //throw new NotImplementedException();
        }

        void IZEDORenderCallback.OnHotDraw(IZElement piElement, CZRender piRender, ref bool pvbOverride)
        {
         //   throw new NotImplementedException();
        }

        public void OnBoundingBox(IZElement piElement, CZRender piRender, ZMathPoint piLocalMinPoint, ZMathPoint piLocalMaxPoint, ref bool pvbOverride)
        {
          //  throw new NotImplementedException();
        }

        public void OnHittest(IZElement piElement, CZRender piRender, long lHWND, ZMathPoint piWorldPoint, ZMathVector piWorldRay, ref bool pvbHit, ref double pdRayDistance, ref double pdScreenDistance, ref bool pvbOverride)
        {
         //   throw new NotImplementedException();
        }


        public void OnElementNameChange(IZElement piElement)
        {
            string name = piElement.Name;

        }
    }
}
