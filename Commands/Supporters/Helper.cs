using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Autodesk.Revit.DB;

namespace bimtab.Commands.Supporters
{
    class Helper
    {
        public static Autodesk.Revit.DB.FamilySymbol GetSymbol(Autodesk.Revit.DB.Document document, string familyName, string symbolName)
        {
            using (var collector = new Autodesk.Revit.DB.FilteredElementCollector(document))
            {
                using (var families = collector.OfClass(typeof(Autodesk.Revit.DB.Family)))
                {
                    foreach (Autodesk.Revit.DB.Family family in families)
                    {
                        if (family.Name == familyName)
                        {
                            foreach (var symbolId in family.GetFamilySymbolIds())
                            {
                                var symbol = document.GetElement(symbolId) as Autodesk.Revit.DB.FamilySymbol;
                                if (symbol.Name == symbolName)
                                {
                                    return symbol;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
       
        public static string Sorgu(string Host, string post_Datas)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;

            string host = Host;
            string password = @"1";

            X509Certificate2Collection certificates = new X509Certificate2Collection();
            certificates.Import(bimtab.Properties.Resources.cer, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(host);

            req.AllowAutoRedirect = true;
            req.ClientCertificates = certificates;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            string postData = post_Datas;
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            req.ContentLength = postBytes.Length;

            Stream postStream = req.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Flush();
            postStream.Close();

            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            string read = new StreamReader(stream).ReadToEnd();
            return read;
        }
        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
       
        public static void CloseForms(System.Windows.Forms.Form default_form)
        {
            List<System.Windows.Forms.Form> formsToClose = new List<System.Windows.Forms.Form>();
            foreach (System.Windows.Forms.Form form in System.Windows.Forms.Application.OpenForms)
            {
                if (form != default_form)
                {
                    formsToClose.Add(form);
                }
            }

            formsToClose.ForEach(f => f.Close());
        }
        public static ConnectorSet GetConnectors(Element element)
        {

            if (element == null) return null;
            try
            {
                FamilyInstance fi = element as FamilyInstance;
                if (fi != null && fi.MEPModel != null)
                {
                    return fi.MEPModel.ConnectorManager.Connectors;
                }
                MEPSystem system = element as MEPSystem;
                if (system != null)
                {
                    return system.ConnectorManager.Connectors;
                }
                MEPCurve duct = element as MEPCurve;
                if (duct != null)
                {
                    return duct.ConnectorManager.Connectors;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }
        public static List<double> SolidFaces(Autodesk.Revit.DB.Element element)
        {
            List<double> result = new List<double>();

            double wid = 1250.0 / (double)sbyte.MaxValue;

            Autodesk.Revit.DB.Options opt = new Autodesk.Revit.DB.Options();
            Autodesk.Revit.DB.GeometryElement geomElem = element.get_Geometry(opt);

            foreach (Autodesk.Revit.DB.GeometryObject geomObj in geomElem)
            {
                Autodesk.Revit.DB.Solid geomSolid = geomObj as Autodesk.Revit.DB.Solid;

                if (null != geomSolid)
                {
                    foreach (Autodesk.Revit.DB.Face geomFace in geomSolid.Faces)
                    {
                        result.Add(((geomFace.Area * 3000) / wid));
                    }
                }
            }

            return result;
        }
       
        public static System.Drawing.Bitmap InterfaceWebRequest(string url)
        {

            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();
            System.Drawing.Bitmap resourceImage = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(stream);
            stream.Close();

            return resourceImage;
        }
        public static Element NearElement(Element element)
        {
            ConnectorSet con = GetConnectors(element);
            foreach (Connector item in con)
            {
                if (item.IsConnected)
                {
                    ConnectorSet connectorSet = item.AllRefs;
                    ConnectorSetIterator csi = connectorSet.ForwardIterator();
                    while (csi.MoveNext())
                    {
                        Connector connected = csi.Current as Connector;
                        ElementId lastId = connected.Owner.Id;
                        if (lastId == element.Id) continue;
                        else
                        {
                            return connected.Owner;
                        }
                    }
                }
            }
            return null;
        }
       
    }
}

