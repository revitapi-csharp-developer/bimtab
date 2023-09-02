using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using bimtab.Commands.Supporters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bimtab.Commands.ExternalCommands
{
    /// <seealso cref="Autodesk.Revit.UI.IExternalCommand"/>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    class ConnectElements : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction transaction = new Transaction(doc, "Manual Connection");
            transaction.Start();

            Support.TaskDialog(TaskDialogIcon.TaskDialogIconInformation, Language.Query("Information"), Language.Query("Select First Element"), Language.Query("Copyrights © 2023 All Rights Reserved by bimtab Inc."));
            Element first = doc.GetElement(uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element));
            Support.TaskDialog(TaskDialogIcon.TaskDialogIconInformation, Language.Query("Information"), Language.Query("Select Second Element"), Language.Query("Copyrights © 2023 All Rights Reserved by bimtab Inc."));
            Element second = doc.GetElement(uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element));

            try
            {

                ConnectorSet conSetTransationFirst = Helper.GetConnectors(first);
                ConnectorSet conSetTransationSec = Helper.GetConnectors(second);

                List<Connector> list1 = new List<Connector>();
                List<Connector> list2 = new List<Connector>();

                foreach (Connector item in conSetTransationFirst)
                {
                    if (item.IsConnected.Equals(false))
                    {
                        list1.Add(item);
                    }
                }

                foreach (Connector item in conSetTransationSec)
                {
                    if (item.IsConnected.Equals(false))
                    {
                        list2.Add(item);
                    }
                }

                List<CoonectElementValues> BoundElements = new List<CoonectElementValues>();

                for (int i = 0; i < list1.Count(); i++)
                {
                    for (int j = 0; j < list2.Count(); j++)
                    {
                        try
                        {
                            double leng = Line.CreateBound(list1[i].Origin, list2[j].Origin).Length;

                            CoonectElementValues coonectElement = new CoonectElementValues
                            {
                                Bound_Lenght = leng,
                                connector1 = list1[i],
                                connector2 = list2[j],
                                con1_index = i,
                                con2_index = j
                            };

                            BoundElements.Add(coonectElement);
                        }
                        catch (Exception)
                        {
                            CoonectElementValues coonectElement = new CoonectElementValues
                            {
                                Bound_Lenght = 0,
                                connector1 = list1[i],
                                connector2 = list2[j],
                                con1_index = i,
                                con2_index = j
                            };

                            BoundElements.Add(coonectElement);
                        }
                    }
                }

                List<double> lenghts = new List<double>();

                for (int i = 0; i < BoundElements.Count(); i++)
                {
                    lenghts.Add(BoundElements[i].Bound_Lenght);
                }

                int index = lenghts.IndexOf(lenghts.Min());

                Connector c1 = BoundElements[index].connector1;
                Connector c2 = BoundElements[index].connector2;

                try
                {
                    c1.ConnectTo(c2);
                }
                catch (Exception ex)
                {

                }

                doc.Regenerate();

                Support.MoveForwardAndBack(doc, first);

            }
            catch (Exception e)
            {

            }


            transaction.Commit();

            return Result.Succeeded;
        }
    }
    class CoonectElementValues
    {
        public Connector connector1 { get; set; }
        public Connector connector2 { get; set; }
        public int con1_index { get; set; }
        public int con2_index { get; set; }
        public double Bound_Lenght { get; set; }
    }

}
