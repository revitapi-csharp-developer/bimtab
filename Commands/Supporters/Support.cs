using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bimtab.Commands.Supporters
{
    class Support
    {
        public static void TaskDialog(TaskDialogIcon taskDialogIcon, string title, string maininstraction, string footertext)
        {
            TaskDialog td = new TaskDialog("TaskDialog");
            td.Id = "id";
            td.MainIcon = taskDialogIcon;
            td.Title = title;
            //td.TitleAutoPrefix = true;
            td.MainInstruction = maininstraction;
            td.FooterText = footertext;

            td.Show();

        }
        private static bool ListContains(List<string> currentlist, string value)
        {
            return currentlist.Contains(value);
        }
        public static Autodesk.Revit.DB.Plumbing.Pipe CreatePipe(Autodesk.Revit.DB.Document doc, Autodesk.Revit.DB.XYZ start, Autodesk.Revit.DB.XYZ end)
        {
            //System Type (DomesticHotWater, DomesticColdWater, Sanitary, etc)
            Autodesk.Revit.DB.MEPSystemType mepSystemType = new Autodesk.Revit.DB.FilteredElementCollector(doc)
                 .OfClass(typeof(Autodesk.Revit.DB.MEPSystemType))
                 .Cast<Autodesk.Revit.DB.MEPSystemType>()
                 .FirstOrDefault(sysType => sysType.SystemClassification == Autodesk.Revit.DB.MEPSystemClassification.DomesticColdWater);

            //Pipe Type (Standard, ChilledWater)
            Autodesk.Revit.DB.Plumbing.PipeType pipeType = new Autodesk.Revit.DB.FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Plumbing.PipeType))
                .Cast<Autodesk.Revit.DB.Plumbing.PipeType>()
                .FirstOrDefault();

            //Level
            Autodesk.Revit.DB.Level level = new Autodesk.Revit.DB.FilteredElementCollector(doc)
                .OfClass(typeof(Autodesk.Revit.DB.Level))
                .Cast<Autodesk.Revit.DB.Level>()
                .FirstOrDefault();

            Autodesk.Revit.DB.Plumbing.Pipe newPipe = Autodesk.Revit.DB.Plumbing.Pipe.Create(doc, mepSystemType.Id, pipeType.Id, level.Id, start, end);

            try
            {
                Autodesk.Revit.DB.Parameter par = newPipe.LookupParameter("Dimension");
                par.Set("3 mm");
            }
            catch (System.Exception)
            {
            }

            return newPipe;
        }
        public static void MoveForwardAndBack(Document doc, Element first)
        {
            try
            {
                XYZ Direction = Support.MoveDirection(first);

                if (Math.Round(Direction.X, 2) > 0 && Math.Round(Direction.Y, 2).Equals(0))
                {
                    ElementTransformUtils.MoveElement(doc, first.Id, new XYZ(0.01, 0, 0));
                    ElementTransformUtils.MoveElement(doc, first.Id, new XYZ(-0.01, 0, 0));
                }
                else if (Math.Round(Direction.X, 2) > 0 && Math.Round(Direction.Y, 2) > 0)
                {
                    ElementTransformUtils.MoveElement(doc, first.Id, new XYZ(0, 0.01, 0));
                    ElementTransformUtils.MoveElement(doc, first.Id, new XYZ(0, -0.01, 0));
                }
                else
                {
                    ElementTransformUtils.MoveElement(doc, first.Id, new XYZ(0, 0, 0.01));
                    ElementTransformUtils.MoveElement(doc, first.Id, new XYZ(0, 0, -0.01));
                }
            }
            catch (Exception)
            {

            }

        }
        public static XYZ MoveDirection(Element element)
        {
            ConnectorSet conSetTransationFirst = Helper.GetConnectors(element);
            List<Connector> list1 = new List<Connector>();

            foreach (Connector item in conSetTransationFirst)
            {
                list1.Add(item);
            }

            return Line.CreateBound(list1.First().Origin, list1.Last().Origin).Direction;
        }
        public static List<XYZ> ElementConnectorPoints(Element element)
        {
            List<XYZ> result = new List<XYZ>();
            ConnectorSet connectorSet = Helper.GetConnectors(element);
            foreach (Connector item in connectorSet)
            {
                result.Add(item.Origin);
            }
            return result;
        }
        public static List<Connector> ElementConnectors(Element element)
        {
            List<Connector> result = new List<Connector>();

            try
            {
                ConnectorSet connectorSet = Helper.GetConnectors(element);
                foreach (Connector item in connectorSet)
                {
                    result.Add(item);
                }

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public static List<Connector> ElementsEmptyConnectors(Element element)
        {
            List<Connector> result = new List<Connector>();

            try
            {
                ConnectorSet connectorSet = Helper.GetConnectors(element);

                foreach (Connector item in connectorSet)
                {
                    if (item.IsConnected.Equals(false))
                    {
                        result.Add(item);
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public static List<Element> ConnectedElements(Element element)
        {
            List<Element> conn_elements = new List<Element>();

            ConnectorSet con = Helper.GetConnectors(element);
            foreach (Connector cc in con)
            {
                if (cc.IsConnected)
                {
                    ConnectorSet connectorSet = cc.AllRefs;
                    ConnectorSetIterator csi = connectorSet.ForwardIterator();

                    while (csi.MoveNext())
                    {
                        Connector connected = csi.Current as Connector;
                        //kendine bağlı değil ise 
                        if (element.Id != connected.Owner.Id)
                        {
                            conn_elements.Add(connected.Owner as Element);
                        }
                    }
                }
            }

            return conn_elements;
        }
        public static bool IsGoingUp(XYZ P1, XYZ P2)
        {
            double difz = Math.Round(Math.Abs(P1.Z - P2.Z), 2);

            if (difz > 0)
            {
                return true;
            }

            return false;
        }
        public static XYZ ModifyPoint(XYZ point)
        {
            double _X = point.X;
            double _Y = point.Y;
            double _Z = point.Z;

            if (_X.ToString().Contains("E"))
            {
                _X = 0;
            }
            if (_Y.ToString().Contains("E"))
            {
                _Y = 0;
            }
            if (_Z.ToString().Contains("E"))
            {
                _Z = 0;
            }

            return new XYZ(Math.Round(_X, 2), Math.Round(_Y, 2), Math.Round(_Z, 2));
        }
        public static bool DoesPointsEqual(XYZ P1, XYZ P2)
        {
            string s1 = P1.X.ToString("0.00000000") + P1.Y.ToString("0.00000000") + P1.Z.ToString("0.00000000");
            string s2 = P2.X.ToString("0.00000000") + P2.Y.ToString("0.00000000") + P2.Z.ToString("0.00000000");

            if (s1.Equals(s2))
            {
                return true;
            }

            return false;
        }
        public static XYZ Abs_of_Point(XYZ Input)
        {
            return new XYZ(Math.Round(Math.Abs(Input.X), 2), Math.Round(Math.Abs(Input.Y), 2), Math.Round(Math.Abs(Input.Z), 2));
        }
        public static bool CheckLineId(bool secondvers, Element element)
        {
            bool result = false;

            try
            {
                foreach (Parameter par in element.Parameters)
                {
                    if (par.Definition.Name.Equals("LineId"))
                    {
                        if (par.AsString().Length.Equals(0))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            try
            {
                foreach (Parameter par in element.Parameters)
                {
                    if (par.Definition.Name.Equals("EL_NominalCurrentCode"))
                    {
                        if (par.AsInteger().ToString().Length.Equals(0))
                        {
                            if (par.AsInteger().Equals(0))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            try
            {
                foreach (Parameter par in element.Parameters)
                {
                    if (par.Definition.Name.Equals("EL_NominalCurrent"))
                    {
                        if (par.AsDouble().ToString().Length.Equals(0))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return result;
        }
        public static void RotateVertically(Autodesk.Revit.DB.Document document, Autodesk.Revit.DB.Element instance, XYZ Referance_Direction)
        {
            try
            {
                ConnectorSet connectorSet = Helper.GetConnectors(instance);
                var connectors = new Connector[2];

                foreach (Connector connector in connectorSet)
                {
                    if (connector.Id == 1)
                        connectors[0] = connector;
                    else
                        connectors[1] = connector;
                }

                Transform transform = ((Instance)instance).GetTransform();
                XYZ basisX = transform.BasisX;
                XYZ xyz1 = basisX.CrossProduct(Referance_Direction);
                XYZ origin = transform.Origin;

                Line bound = Line.CreateBound(origin, origin.Add(xyz1));
                double num6 = Referance_Direction.AngleTo(basisX);
                ElementTransformUtils.RotateElement(document, ((Element)instance).Id, bound, num6);
            }
            catch (Exception e)
            {
                //System.Windows.MessageBox.Show(e.ToString());
            }
        }
        public static void RotationByOwnCenterLine(Autodesk.Revit.DB.Document document, Autodesk.Revit.DB.Element element)
        {
            // 90 Derece Döndürme yapar // 
            ConnectorSet connectorSet = Helper.GetConnectors(element);
            var connectors = new Connector[2];

            foreach (Connector connector in connectorSet)
            {
                if (connector.Id == 1)
                    connectors[0] = connector;
                else
                    connectors[1] = connector;
            }

            Line axis = Line.CreateBound(connectors[0].Origin, connectors[1].Origin);
            ElementTransformUtils.RotateElement(document, element.Id, axis, Math.PI / 2.0);
        }
        public static void RotationByOwnCenterLine(Autodesk.Revit.DB.Document document, Autodesk.Revit.DB.Element element, double angle)
        {
            // 90 Derece Döndürme yapar // 
            ConnectorSet connectorSet = Helper.GetConnectors(element);
            var connectors = new Connector[2];

            foreach (Connector connector in connectorSet)
            {
                if (connector.Id == 1)
                    connectors[0] = connector;
                else
                    connectors[1] = connector;
            }

            Line axis = Line.CreateBound(connectors[0].Origin, connectors[1].Origin);
            ElementTransformUtils.RotateElement(document, element.Id, axis, angle);
        }
        public static double AngleBetweenElement(XYZ NoutralRefDirection, XYZ element2)
        {
            double angle = NoutralRefDirection.AngleTo(element2);

            return angle;
        }
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        public static XYZ MidPoint(Element element)
        {
            List<XYZ> points = Support.ElementConnectorPoints(element);

            XYZ result = new XYZ(0, 0, 0);

            for (int i = 0; i < points.Count(); i++)
            {
                result += points[i];
            }

            return result / points.Count();
        }
        public static double Distance(XYZ first, XYZ second)
        {
            return first.DistanceTo(second);
        }
        public static double Distance(Connector first, XYZ second)
        {
            return first.Origin.DistanceTo(second);
        }
        public static double Distance(Connector first, Connector second)
        {
            return first.Origin.DistanceTo(second.Origin);
        }
        /// <summary>
        /// Elemanın boş konnektöründen dolusuna doğru bir doğru çizer ve onun doğrultusunu geri döndürür.
        /// Eğer elemenın konnektör sayısı 2 ise 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static XYZ Direction(Element element)
        {
            List<Connector> connectors = Support.ElementConnectors(element);

            if (connectors.Count().Equals(2))
            {
                Connector empyt = Support.ElementsEmptyConnectors(element).FirstOrDefault();
                Connector connected = null;

                for (int j = 0; j < connectors.Count(); j++)
                {
                    if (connectors[j].IsConnected.Equals(true))
                    {
                        connected = connectors[j];
                        break;
                    }
                }

                return Line.CreateBound(connected.Origin, empyt.Origin).Direction;
            }

            return null;
        }

    }
}
