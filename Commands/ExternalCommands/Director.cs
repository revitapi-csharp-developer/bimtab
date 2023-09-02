using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace bimtab.Commands.ExternalCommands
{
    /// <seealso cref="Autodesk.Revit.UI.IExternalCommand"/>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class Director : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            System.Diagnostics.Process.Start(bimtab.App.webpage);
            return Result.Succeeded;
        }
    }
}
