using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using bimtab.Commands.Supporters;

namespace bimtab.Commands.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DisconnectElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction transaction = new Transaction(doc, "Disconnect Element");
            transaction.Start();

            try
            {
                Element ele = doc.GetElement(uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element));

                List<Element> connected_elements = Support.ConnectedElements(ele);//Seçili bağlı olduğu elemanlar

                List<Connector> current_cons = Support.ElementConnectors(ele);//Seçili elemanın konnectörleri

                List<Connector> connecteds_cons = new List<Connector>();//Bağlı oldupu elemanların konnectörleri

                for (int i = 0; i < connected_elements.Count(); i++)
                {
                    List<Connector> cons = Support.ElementConnectors(connected_elements[i]);

                    foreach (Connector con in cons)
                    {
                        connecteds_cons.Add(con);
                    }
                }

                for (int i = 0; i < current_cons.Count(); i++)
                {
                    for (int j = 0; j < connecteds_cons.Count(); j++)
                    {
                        try
                        {
                            if (Support.ModifyPoint(current_cons[i].Origin).ToString().Equals(Support.ModifyPoint(connecteds_cons[j].Origin).ToString()))//Konnectör noktalarının orijinleri aynı nokta ise
                            {
                                current_cons[i].DisconnectFrom(connecteds_cons[j]);
                            }
                        }
                        catch (Exception e)
                        {
                            //System.Windows.MessageBox.Show(e.ToString());
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
           
            transaction.Commit();

            return Result.Succeeded;
        }
    }
}
