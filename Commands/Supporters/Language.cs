using Autodesk.Revit.ApplicationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace bimtab.Commands.Supporters
{
    class Language
    {
        #region Variables
        private static LanguageType RevitLanguage = bimtab.App.controlledapp.ControlledApplication.Language;
        private static int LanguageIndex = (int)RevitLanguage;
        private static JObject jsonvalues = null;
        private static List<LanguageType> languages = new List<LanguageType>();
        #endregion
        public static string Query(string value)
        {
            return value;
        }
        private static List<LanguageType> GetLanguageTypes()
        {
            List<LanguageType> languageTypes = new List<LanguageType>();

            foreach (LanguageType languageType in Enum.GetValues(typeof(LanguageType)))
            {
                languageTypes.Add(languageType);
            }

            return languageTypes;
        }
        public static void Translater(System.Windows.Forms.Control control)
        {
            foreach (System.Windows.Forms.Control controll in control.Controls)
            {
                controll.Text = Language.Query(controll.Text);
            }
        }
    }
}
