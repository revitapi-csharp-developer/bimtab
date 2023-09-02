using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using bimtab.Commands.Interface;
using bimtab.Properties;
using adWin = Autodesk.Windows;

namespace bimtab
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class App : IExternalApplication
    {
        #region Variables
        public static LinearGradientBrush brush { get; set; }
        public static RibbonControl _ribbon_control = null;
        public static UIControlledApplication controlledapp = null;
        private static string tabname = "bimtab";
        private static string panelname = "Quick Solutions";
        public static string webpage = "https://bimtab.com/";
        #endregion

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
            // CREATION OF TAB //
            controlledapp = application;
            adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;
            _ribbon_control = ribbon;

            var ribbonTab = ribbon.FindTab(tabname);

            if (ribbonTab == null)
            {
                application.CreateRibbonTab(tabname);
            }

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            var ribbonPanel = application.CreateRibbonPanel(tabname, panelname);

            PushButtonData buttonData1 = new PushButtonData("btn1", tabname, thisAssemblyPath, "bimtab.Commands.ExternalCommands.Director");
            PushButton pushButton1 = ribbonPanel.AddItem(buttonData1) as PushButton;
            pushButton1.LargeImage = BitmapSourceConverter.ConvertFromImage(Resources.logo1);

            PushButtonData buttonData2 = new PushButtonData("btn2", "Connect\r\nElements", thisAssemblyPath, "bimtab.Commands.ExternalCommands.ConnectElements");
            PushButton pushButton2 = ribbonPanel.AddItem(buttonData2) as PushButton;
            pushButton2.LargeImage = BitmapSourceConverter.ConvertFromImage(Resources.logo1);

            ColoriseTab(tabname, panelname, Color.FromRgb(255, 220, 70));

            return Result.Succeeded;
        }
        private static void ColoriseTab(string tabname, string panelname, System.Windows.Media.Color color)
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new System.Windows.Point(0, 0);
            gradientBrush.EndPoint = new System.Windows.Point(1, 0);
            gradientBrush.GradientStops.Add(new GradientStop(color, 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(color, 1));

            foreach (adWin.RibbonTab tab in App._ribbon_control.Tabs)
            {
                if (tab.Name == tabname)
                {
                    foreach (adWin.RibbonPanel panel in tab.Panels)
                    {
                        if (panel.Source.Name.ToString() == panelname)
                        {
                            panel.CustomPanelTitleBarBackground = gradientBrush;
                            panel.CanToggleOrientation = true;
                            panel.AnimationMode = Autodesk.Internal.Windows.RibbonPanelAnimationMode.None;
                        }
                    }
                }
            }
        }
    }
}
