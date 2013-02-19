//
//  Settings.cs
//
//  Author:
//       KryDos <furyinbox@gmail.com>
//
//  Copyright (c) 2013 KryDos
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace orgmodereminder
{
    public class Settings : Form
    {
        public static string configFile = "config.xml";
        private Label status;

        public Settings()
        {
            this.Icon = new Icon("app.ico", 40, 40);

            status = new Label();
            status.Location = new Point(75, 0);
            status.Size = new Size(200, 40);
            status.Font = new Font(status.Font.FontFamily.Name, 20);
            status.ForeColor = Color.Green;
            this.Controls.Add(status);

            Button selectFilesButton = new Button();
            selectFilesButton.Location = new Point(100, 230);
            selectFilesButton.Click += new EventHandler(selectFilesButton_Click);
            selectFilesButton.Text = "Set Org Files";
            this.Controls.Add(selectFilesButton);
        }

        private void selectFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = "All Files|*.*";
            
            if (fileDialog.ShowDialog() == DialogResult.OK) {
                CreateXMLDocument(configFile);
                foreach (string fileName in fileDialog.FileNames) {
                    AddOrgFileToConfig(fileName);
                }
                status.Text = "Complete!";
            }
        }

        private void CreateXMLDocument(string filePath)
        {
            //XmlTextWriter xtw = new XmlTextWriter(filePath, System.Text.Encoding.UTF8);
            System.IO.FileStream fs = new System.IO.FileStream(configFile, System.IO.FileMode.Create);
            XmlTextWriter xtw = new XmlTextWriter(fs, System.Text.Encoding.UTF8);
            xtw.WriteStartDocument();
            xtw.WriteStartElement("files");
            xtw.WriteEndDocument();
            xtw.Close();
            fs.Close();
            fs.Dispose();
        }

        private void AddOrgFileToConfig(string pathToOrgFile)
        {
            XmlDocument xd = new XmlDocument();
            System.IO.FileStream fs = new System.IO.FileStream(configFile, System.IO.FileMode.Open);
            xd.Load(fs);

            XmlElement orgFile = xd.CreateElement("file");
            XmlText path = xd.CreateTextNode(pathToOrgFile);
            orgFile.AppendChild(path);

            xd.DocumentElement.AppendChild(orgFile);

            fs.Close();
            xd.Save(configFile);
        }

        public List<string> getOrgFilesPath()
        {
            List<string> OrgFilePaths = new List<string>();

            XmlDocument xd = new XmlDocument();
            System.IO.FileStream fs = new System.IO.FileStream(configFile, System.IO.FileMode.Open);
            xd.Load(fs);

            XmlNodeList list = xd.GetElementsByTagName("file");

            for (int i = 0; i < list.Count; i++) {
                OrgFilePaths.Add(list.Item(i).InnerText);
            }

            return OrgFilePaths;
        }
    }
}

