using System;
using System.Collections.Generic;
using System.Text;

using interop.ICApiIronCAD.SelectionTool;
using System.Windows.Forms;


namespace ICApiAddin.SelectionTool
{
    class SelectionToolDocSelectionEvents
    {
        long iHighWaterMark;
        IZDoc local_iZDoc;
        private CZSelectorEvents m_selectionEvents;

        List<IZHighlight> listHighlight = new List<IZHighlight>();

        ~SelectionToolDocSelectionEvents()
        {

        }
 
        internal SelectionToolDocSelectionEvents(CZSelectorEvents selectionEvents, IZDoc iZDoc)
        {
            local_iZDoc = iZDoc;
            if (selectionEvents != null)
            {
                m_selectionEvents = selectionEvents;
                //int types = (int)(eZSelectionType.Z_SEL_ANY | eZSelectionType.Z_SEL_POINT | eZSelectionType.Z_SEL_FACE | eZSelectionType.Z_SEL_EDGE | eZSelectionType.Z_SEL_FEV);
                // m_selectionEvents.SetSelectionFilterChoices(types, eZSelectionType.Z_SEL_FEV);
                  int types = (int)(eZSelectionType.Z_SEL_ASSEMBLY );
                m_selectionEvents.SetSelectionFilterChoices(types, eZSelectionType.Z_SEL_ASSEMBLY);
                m_selectionEvents.Selected += new _IZSelectEvents_SelectedEventHandler(m_selectionEvents_Selected);
                m_selectionEvents.DeSelected += new _IZSelectEvents_DeSelectedEventHandler(m_selectionEvents_DeSelected);
                m_selectionEvents.DeSelectedAll += new _IZSelectEvents_DeSelectedAllEventHandler(m_selectionEvents_DeSelectedAll);
              //  m_selectionEvents.Call_Selected2 += new _IZSelectEvents_Call_Selected2EventHandler(m_selectionEvents_Call_Selected2);
            }
            iHighWaterMark = 0;
        }

        internal void UnRegister()
        {
     
            m_selectionEvents.Selected -= m_selectionEvents_Selected;
            m_selectionEvents.DeSelected -= m_selectionEvents_DeSelected;
            m_selectionEvents.DeSelectedAll -= m_selectionEvents_DeSelectedAll;
            IZSceneDoc iZSceneDoc = local_iZDoc as IZSceneDoc;
            if (iZSceneDoc != null)
            {
                foreach (IZHighlight Highlight in listHighlight)
                {
                    iZSceneDoc.RemoveHighlight(Highlight);
                }
            }
            listHighlight.Clear();
        }

        void m_selectionEvents_DeSelectedAll()
        {
          //  MessageBox.Show("DeSelectedAll event fired.");
            IZSceneDoc iZSceneDoc = local_iZDoc as IZSceneDoc;
            if (iZSceneDoc != null)
            {
                foreach (IZHighlight Highlight in listHighlight)
                {
                    int lColor = ((int)(((byte)(250) | ((int)((byte)(250)) << 8)) | (((int)(byte)(250)) << 16)));
                    Highlight.Color = lColor;
                    iZSceneDoc.AddHighlight(Highlight);
                }
            }

        }

        void m_selectionEvents_DeSelected(IZElement piElement, ZMathPoint piModelCoord, int lXWindowPixel, int lYWindowPixel, int lEFlags, eZEntityType eEntType, object varFaceIds)
        {
        //    MessageBox.Show("DeSelected event fired.");
        }

        void GetPathManage(IZSceneDoc iZsceneDoc, out IZElement zManager)
        {
            zManager = null;
            string MyManagerName = "PathManager";
            object varElements = iZsceneDoc.GetChildElements();
            object[] oElements = varElements as object[];
            if (oElements != null)
            {
                foreach (object oEle in oElements)
                {
                    IZElement izEle = oEle as IZElement;
                    eZElementType type = izEle.Type;
                    string name = izEle.Name;
                    if (name.Equals(MyManagerName))
                    {
                        IZEDOSiteElement siteElement = izEle as IZEDOSiteElement;
                        if (siteElement != null)
                        {
                            zManager = izEle;
                            break;
                        }
                    }

                }
            }
            IZDoc iZDoc = iZsceneDoc as IZDoc;
            if (zManager == null)
            {

                ZEDOSiteElement siteElement = null;
                iZDoc.InsertNewEDOSiteElement(out siteElement);
                zManager = siteElement as IZElement;
                zManager.Name = MyManagerName;
            }
        }

        void CreateCoordindateEDO(IZElement zManager, ZMathMatrix coordMtx)
        {
            IZEDOSiteElement siteElementMgr = zManager as IZEDOSiteElement;

            ZEDOSiteElement siteElement = null;
            siteElementMgr.InsertNewEDOSiteElement(out siteElement);
          

            IZExtensibility iZExtensibility = siteElement as IZExtensibility;
            if (iZExtensibility != null)
            {
                //IZSceneElement zElement = siteElement as IZSceneElement;
                siteElement.SetPositionTransform(coordMtx);
                IZElement zEle = siteElement as IZElement;
                string CoordName = "Coord_" + Convert.ToString(iHighWaterMark);
                iHighWaterMark++;
                zEle.Name = CoordName;

                IZElement zParent = zEle.GetParent();
                IZElement piFirst = zParent.GetFirstChild();
                bool bAfter = true;
                zEle.MoveToReferenceLocation(piFirst, bAfter);
                {
                    try
                    {
                         iZExtensibility.AddExtensionObject("{06D8B851-0962-4127-8A1F-EAAC25952F4B}", false);
                       
                       // iZExtensibility.AddExtensionObject("SelectionTool.TestEDODraw", false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }                   
                }
            }
        }
  
        void m_selectionEvents_Selected(IZElement piElement, ZMathPoint piModelCoord, int lXWindowPixel, int lYWindowPixel, int lEFlags, eZEntityType eEntType, object varEntIds)
        {
            //MessageBox.Show("Selected event fired.");
            if( piElement != null )
            {
                ZMathMatrix piSelectToGlobal = null;
                IZSceneElement piSceneEle = piElement as IZSceneElement;
                if (piSceneEle != null)
                    piSelectToGlobal = piSceneEle.GetTransformToGlobal();

                string sName = piElement.Name;
                string sType=string.Empty;
                if (eEntType == eZEntityType.Z_ENTITY_FACE)
                {
                    sType ="Face:";
                }
                else if (eEntType == eZEntityType.Z_ENTITY_EDGE)
                {
                    sType ="Edge:";
                }
                else if (eEntType == eZEntityType.Z_ENTITY_VERTEX)
                {
                    sType ="Vertex:";
                }
                Int32[] oEntIds = varEntIds as Int32[];
                if (oEntIds != null)
                {
                    IZBodyEdge piBodyEdge = null;
                    
                    if (eEntType == eZEntityType.Z_ENTITY_EDGE)
                    {
                        IZPart piPart = piElement as IZPart;
                        if( piPart != null )
                        {
                            IZBody piBody =  piPart.GetBody(true );
                            if( piBody != null )
                            {
                                piBodyEdge = piBody as IZBodyEdge;
                            }
                        }
                    }
                    IZMathUtility iUtility = m_selectionEvents.Application as IZMathUtility;
                    IZElement zManager = null;

                  

                    IZSceneDoc iZsceneDoc = piElement.OwningDoc as IZSceneDoc;

                    GetPathManage( iZsceneDoc, out zManager);

                    ZFace zFace = null;
                    foreach (Int32 iId in oEntIds)
                    {
                       // MessageBox.Show(string.Format("Shape name: {0}, Type: {1},  Id: {2} ", sName, sType, iId));   
                        if( piBodyEdge != null )
                        {
                            ZMathPoint pFaceMidPt = null;
                            ZMathVector pFaceMidNorm = null;

                            IZHighlight highlight = iZsceneDoc.CreateEntityHighlight(iId, piElement, eZEntityType.Z_ENTITY_EDGE);
                            if (highlight != null)
                                listHighlight.Add(highlight);

                            IZEdge piEdge = piBodyEdge.GetEdge(iId);
                            if (piEdge != null)
                            {
                                int cond = (int)eZEdgeSearchCondition.Z_EDGE_SEARCH_SMOOTH;
                                ZArray edges = piEdge.SearchConnectedEdges(cond);
                                if( edges != null )
                                {
                                    int size = 0;
                                   
                                    object piNext = null;
                                    edges.Count( out size);
                                    for (int i = 0; i < size; i++)
                                    {
                                        edges.Get(i, out piNext);
                                        if (piNext != null)
                                        {
                                            IZEntity piSmooth = piNext as IZEntity;
                                            if (piSmooth != null)
                                            {
                                                int id = piSmooth.Id;

                                                System.Diagnostics.Debug.WriteLine(string.Format("Id: {0} \n ", piSmooth.Id));
                                            }
                                        }
                                    }
                                }

                                 object varFaceIds = null;
                                piBodyEdge.GetEdgeFaceIds(iId, out varFaceIds);
                                {
                                   
                                   Int32[] oFaceIds = varFaceIds as Int32[];
                                   if (oFaceIds != null)
                                   {
                                       IZBodyFace piBodyFace = piBodyEdge as IZBodyFace;
                                       if (piBodyFace != null)
                                       {
                                           foreach (Int32 iIndex in oFaceIds)
                                           {
                                               piBodyFace.GetFaceMidInfo(iIndex, out pFaceMidPt, out pFaceMidNorm);

                                               if( zFace == null )
                                                 zFace = piBodyFace.GetFace(iIndex);
         
                                 

                                               break;
                                           }
                                       }
                                   }
                                }




                                IZEdgeCurve piEdgeCurve = piEdge as IZEdgeCurve;

                                double dStartPara = 0;
                                double dEndPara = 0;
                                piEdgeCurve.GetParameterRange(out dStartPara, out dEndPara);

                                double dLen = 0;
                                piEdgeCurve.GetLength(dStartPara, dEndPara, out dLen);


                                ZMathPoint piMidPt;
                                ZMathVector piMidDir;
                                piBodyEdge.GetEdgeMidInfo(iId, out piMidPt, out piMidDir);

                                double midPara = piEdgeCurve.GetParameterAt(piMidPt);
                              
                                double dPeriodRange = 0;
                                if (piEdgeCurve.IsPeriodic())
                                {
                                    piEdgeCurve.GetPeriodicParameterRange(out dPeriodRange);
                                    if (dPeriodRange != 0)
                                    {
                                        if (midPara < dStartPara)
                                        {
                                            midPara += dPeriodRange;
                                        }
                                        else if (midPara > dEndPara)
                                        {
                                            midPara -= dPeriodRange;
                                        }
                                    }
                                }

                                ZMathPoint spiPrevPt;
                                ZMathPoint spiNextPt;
                                double dPrevPara = 0;
                                double dNextPara = 0;

                                double count = 200;
                                double Dist = 0.003;
                                double TotalDist = 0;

                                while (true)
                                {
                                    if (TotalDist > dLen)
                                        break;

                                    piEdgeCurve.GetNextPointAtCurveLength(TotalDist, dStartPara, true, out spiNextPt, out dNextPara);

                                    IZEntity ent = piEdgeCurve as IZEntity;
                                    eZPointEntityRel rel2 = ent.GetPointEntityRelation(spiNextPt);

                                    if (zManager != null)
                                    {
                                        ZMathVector zNorm = null;
                                        double curvature = 0;
                                        curvature = piEdgeCurve.EvaluateCurvature(dNextPara, out zNorm);


                                        ZMathVector zTangent = null;
                                        ZMathPoint pt;
                                        ZMathVector zSecond;
                                        piEdgeCurve.Evaluate(dNextPara, out pt, out zTangent, out zSecond);
                                        zTangent.NormalizeSelf();
                                        ZMathVector YDir = null;

                                        ZMathVector zCopy = zTangent.Copy();
                                        zCopy.TransformBy(piSelectToGlobal);
                                        YDir = zNorm.Cross(zCopy);

                                         //the curve has no curvature, use face normal
                                      //  if( Math.Abs(curvature) < 1e-6 )
                                        {
                                            if (zFace != null)
                                            {
                                                zNorm = zFace.GetNormal(spiNextPt);
                                                YDir = zNorm.Cross(zTangent);
                                                zNorm = zTangent.Cross(YDir);
   
                                            }
                                            else{
                                                if (pFaceMidNorm != null)
                                                {
                                                    //makes sure they are pendicular.
                                                    zNorm = pFaceMidNorm.Copy();
                                                    YDir = zNorm.Cross(zTangent);
                                                    zNorm = zTangent.Cross(YDir);
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Error: Need to handle the wire case.");
                                                    return;
                                                }
                                            }
                               
                                        }
                                        if( YDir == null )
                                            YDir = zNorm.Cross(zTangent);

  

                                        for (int n = 0; n < 2; n++)
                                        {
                                            if (n == 1)
                                            {
                                                double tol = 0.001;
                                                spiNextPt.X = spiNextPt.X + (zNorm.X * tol);
                                                spiNextPt.Y = spiNextPt.Y + (zNorm.Y * tol);
                                                spiNextPt.Z = spiNextPt.Z + (zNorm.Z * tol);

                                                IZEntity zBodyEntity = piBodyEdge as IZEntity;
                                                IZEntity piHitEnt = null;
                                                ZMathVector piHitNorm = null;
                                                double dTol = 0.000001;
                                                try
                                                {
                                                    ZMathPoint piHitPt = zBodyEntity.GetProjectedPoint(spiNextPt, YDir, dTol, eZEntityType.Z_ENTITY_FACE, out piHitEnt, out piHitNorm);

                                                    spiNextPt.X = piHitPt.X + (piHitNorm.X * tol);
                                                    spiNextPt.Y = piHitPt.Y + (piHitNorm.Y * tol); ;
                                                    spiNextPt.Z = piHitPt.Z + (piHitNorm.Z * tol); ;


                                                    eZPointEntityRel rel = zBodyEntity.GetPointEntityRelation(piHitPt);


                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                            //X: norm, tangent is Z
                                            ZMathMatrix coordMtx = iUtility.CreateMathMatrixWithOriginXYAxis(spiNextPt, zNorm, YDir);


                                            coordMtx.MultiplyBy(piSelectToGlobal);
                                            CreateCoordindateEDO(zManager, coordMtx);

                                        }
                                    }
                                      /*
                                    if (piSelectToGlobal != null)
                                         spiNextPt.TransformBy(piSelectToGlobal);
                                    MakePoint(null, spiNextPt, Dist / 4);
                                    */
                                //    MakePoint(piSelectToGlobal, spiNextPt, Dist / 4);
                                    //做完後設定下次條件並檢查，此算法為算頭不算尾
                                    TotalDist = TotalDist + Dist;
                                }
                                for (int i = 0; i < 20; i++)
                                {
                                    dNextPara = dStartPara + i * (dEndPara - dStartPara) / 20;
                                    ZMathVector pVector;
                                    piEdgeCurve.EvaluateCurvature(dNextPara, out pVector);

                                }
                                //for (double i = 1; i < count; i++)
                                //{
                                //    piEdgeCurve.GetNextPointAtCurveLength(Dist*i, dStartPara, true, out spiNextPt, out dNextPara);
                                //    MakePoint(spiNextPt, 0.001);
                                //}
                            }
			
                        }
                    }

                    iZsceneDoc.UpdateGraphics(0);
                    iZsceneDoc.UpdateBrowser(0);
 			

                    /*
                    foreach (object oId in oEntIds)
                    {
                        if (oId is Int32)
                        {
                            Int32 iId = (Int32)oId;

                            MessageBox.Show(string.Format("Shape name: {0}, Type: {1},  Id: {2} ", sName, sType, iId));
                        }
                    }
                     */
                }
                else
                {
                    if (varEntIds is Int32)
                    {
                        Int32 iId = (Int32)varEntIds;

                        MessageBox.Show(string.Format("Shape name: {0}, Type: {1},  Id: {2} ", sName, sType, iId));
                    }
                }
            }

        }

        public void MakePoint(ZMathMatrix mtx, ZMathPoint CenterPt, double rad)
        {
            //IZDoc iZDoc = GetActiveDoc();
            IZSceneDoc iZsceneDoc = local_iZDoc as IZSceneDoc;
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

            if (mtx != null)
            {
                IZSceneElement piSceneEle = iNewPart as IZSceneElement;
                SetTransformToGlobal(piSceneEle, mtx, CenterPt);
            }
 
            iNewPart.Update();
            return;
        }

        public void SetTransformToGlobal(IZSceneElement izChild, IZMathMatrix izGlobalTransform, ZMathPoint CenterPt)
        {
            IZElement izEle = izChild as IZElement;
            IZElement izParent = izEle.GetParent();
            if (izParent != null)
            {
                IZSceneElement izParentSceneEle = izParent as IZSceneElement;
                ZMathMatrix parentToGlobal = izParentSceneEle.GetTransformToGlobal();
                parentToGlobal.Invert(); //Global to Parent
                ZMathMatrix ChildToParent = izGlobalTransform.Copy();

  

                //ChildToGlobalTransformation = ChildToParent * ParentToGolbal.  So ChildToParent = ChildToGlobalTransformation * Inverse of ParentToGolbal
                ChildToParent.MultiplyBy(parentToGlobal);  //This will be child to parent transformation

                IZMathUtility iUtility = m_selectionEvents.Application as IZMathUtility;
                //izChild.SetTransformToParent(ChildToParent);

                //transform the picked point to parent space.
                object OCenterPt = CenterPt.Data;

                ZMathPoint PtInParent = iUtility.CreateMathPoint(OCenterPt);
  
                 PtInParent.TransformBy(ChildToParent);

                 OCenterPt = PtInParent.Data;
                 // object OCenterPt = new double[3] { TransformBy.X, pt.Y, pt.Z }; 


                ZMathVector vec = iUtility.CreateMathVector(OCenterPt);
                ZMathMatrix piChildPos = iUtility.CreateTranslationMathMatrix(vec);

                ZMathMatrix piNewPos = piChildPos.Copy();
     
       
                    piChildPos.Invert();
                    ChildToParent.MultiplyBy(piChildPos);
                    ChildToParent.Invert();
                    izChild.SetAnchorTransform(ChildToParent);
                    izChild.SetPositionTransform(piNewPos);
                    return;
                
                
                
            }
        }
    }
}
