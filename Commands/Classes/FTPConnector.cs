using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bimtab.Commands.Classes
{
    class FTPConnector
    {
        private string ftpServer = "ftp://ftp.sample.com";
        private string username = "username";
        private string password = "password";
        private string filename = string.Empty;
        private string file = string.Empty;

        public FTPConnector(string Ftpurl,string Username,string Password) 
        { 
            ftpServer = Ftpurl;
            username = Username;
            password = Password;
        }
        public void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "Executable Files|*.dll;*.exe;*.msi|All Files (*.*)|*.*"; // Filter for specific file types
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = openFileDialog.FileName;
            }

            filename = openFileDialog.SafeFileName;
        }
        public void UploadFile()
        {
            if (file.Length.Equals(0))
            {
                MessageBox.Show("Please Use SelectFile method firdt !");
            }
            else
            {
                try
                {
                    FileInfo Dosya = new FileInfo(file);
                    string To = ftpServer + @"/inside/" + filename;

                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(username, password);
                        client.UploadFile(To, WebRequestMethods.Ftp.UploadFile, file);
                    }
                }
                catch
                {

                }
            }
        }

        public void GetDirectoryInfos(string ftppath)
        {
            // FTP server details

            try
            {
                // Create a request to list the directory
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftppath));
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(username, password);

                // Get the response from the FTP server
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                // Read and display the directory listing
                string directoryListing = reader.ReadToEnd();
                //txtDirectoryListing.Text = directoryListing;

                // Close the response
                reader.Close();
                response.Close();

                MessageBox.Show(directoryListing);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
