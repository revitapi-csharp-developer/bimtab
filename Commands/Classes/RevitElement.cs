using Autodesk.Revit.DB;
using bimtab.Commands.Supporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace bimtab.Commands.Classes
{
    class RevitElement
    {
        public Autodesk.Revit.DB.ElementId ElementId { get; set; }
        public Autodesk.Revit.DB.Element Element { get; set; }
        public string ElementName { get; set; }
        public List<Connector> ElementConnectors 
        { 
            get{ return Support.ElementConnectors(Element); }
            set { }
        }
        public Color Color { get; set; }
        public XYZ ElementDirection()
        {
            OrganiseConnectors();
            return Line.CreateBound(ElementConnectors.First().Origin, ElementConnectors.Last().Origin).Direction;
        }
        private void OrganiseConnectors()
        {
            if (ElementConnectors == null)
            {
                ElementConnectors = Support.ElementConnectors(Element);
                List<Connector> newcons = new List<Connector>();

                if (ElementConnectors.Count().Equals(2))
                {
                    for (int i = 0; i < ElementConnectors.Count(); i++)
                    {
                        if (ElementConnectors[i].IsConnected.Equals(false))
                        {
                            newcons.Add(ElementConnectors[i]);
                            break;
                        }
                    }

                    ElementConnectors.Remove(newcons.First());
                    newcons.Add(ElementConnectors.FirstOrDefault());

                    ElementConnectors.Clear();
                    ElementConnectors = newcons;
                }
                else
                {
                    Support.TaskDialog(Autodesk.Revit.UI.TaskDialogIcon.TaskDialogIconInformation, Language.Query("Information"), "The Direction is make sense if selected element has 2 connector.", Language.Query("Copyrights © 2023 All Rights Reserved by bimtab Inc."));
                }
            }
        }
    }
  
    class bimtabElement:RevitElement
    {
        public void ElementInfo()
        {
            Support.TaskDialog(Autodesk.Revit.UI.TaskDialogIcon.TaskDialogIconInformation, 
                Language.Query("Information"), 
                ElementId.ToString() + Environment.NewLine + 
                ElementName.ToString() + Environment.NewLine + 
                "Connector Count : " + ElementConnectors.Count().ToString(), 
                Language.Query("Copyrights © 2023 All Rights Reserved by bimtab Inc."));
        }
    }
}
