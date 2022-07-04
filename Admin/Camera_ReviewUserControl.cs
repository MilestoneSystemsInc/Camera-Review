using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;
using System.ComponentModel;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using VideoOS.Platform.Login;
using VideoOS.Platform.Messaging;
using VideoOS.Platform.Resources;
using VideoOS.Platform.Util;
using VideoOS.Platform.ConfigurationItems;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using CsvHelper;
using System.Globalization;
using static Camera_Review.PasswordAdvisor;


namespace Camera_Review.Admin
{
    
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class Camera_ReviewUserControl : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;
        DataTable dt = new DataTable();
        DataTable dtfirmwares= new DataTable();
        DataTable dtpasswords= new DataTable();

        public Camera_ReviewUserControl()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(dataGridView1_CellFormatting);
            this.Controls.Add(dataGridView1);
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            

        }

        private void filter()
        {
            if (dt.Rows.Count>0)
            {
                string Sfilter = "";
                bool previous = false;

                foreach (object myitems in checkedListBox1.CheckedItems)

                    {
                    if (myitems.ToString()== "Firmware Issue Detected")
                    {
                        previous= true;
                        Sfilter = "FirmwareTest Like '%Warning%'";



                    }
                    if (myitems.ToString() == "Password Complexity Issue")
                    {
                        


                        switch (comboBox1.SelectedItem.ToString())
                        {
                            case "VeryStrong":
                                if (previous) { Sfilter = Sfilter + " OR "; }
                                previous = true;
                                Sfilter = Sfilter + "PasswordComplexityRating = 'Strong'";
                                
                                goto case "Strong";
                            case "Strong":
                                if (previous) { Sfilter = Sfilter + " OR "; }
                                previous = true;
                                Sfilter = Sfilter + "PasswordComplexityRating = 'Medium'";
                                goto case "Medium";
                            case "Medium":
                                if (previous) { Sfilter = Sfilter + " OR "; }
                                previous = true;
                                Sfilter = Sfilter + "PasswordComplexityRating = 'Weak'";
                                goto case "Weak";
                            case "Weak":
                                if (previous) { Sfilter = Sfilter + " OR "; }
                                previous = true;
                                Sfilter = Sfilter + "PasswordComplexityRating = 'VeryWeak'";
                                goto case "VeryWeak";
                            case "VeryWeak":
                                if (previous) { Sfilter = Sfilter + " OR "; }
                                previous = true;
                                Sfilter = Sfilter + "PasswordComplexityRating = 'Blank'";
                                break;
                        }
                        



                    }
                    if (myitems.ToString() == "Password in List")
                    {
                        if (previous) { Sfilter = Sfilter + " OR "; }
                        previous = true;
                        Sfilter = "PaswordMatch Like '%Warning%'";
                    }

                }

                dt.DefaultView.RowFilter = Sfilter;
            }


        }
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataRowView row = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView;
           if (row != null) {
               string mytemp = row.Row.ItemArray[0].ToString();
                
            
                if (row.Row.ItemArray[0].Equals("0"))
                {
                    e.CellStyle.ForeColor = Color.Green;
                }
                else if (int.Parse(mytemp) >0)
                {
                    e.CellStyle.ForeColor = Color.Red;
                }
        }
    }

        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;

            if (e.ProgressPercentage == 100)
            {
               


            }
        }
        internal String DisplayName
        {
            get { return "Camera Review"; }
        }

        /// <summary>
        /// Ensure that all user entries will call this method to enable the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        internal void FillContent(Item item)
        {
            
        }
        public static void SafeInvoke(System.Windows.Forms.Control control, System.Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(new System.Windows.Forms.MethodInvoker(() => { action(); }));
            else
                action();
        }

    

        internal void ClearContent()
        {
            
        }

        private void Camera_ReviewUserControl_Load(object sender, EventArgs e)
        {

        }
        private void Log_Message(string mymessage, Boolean error)
        {
            EnvironmentManager.Instance.Log(false, "4Js - Camera Review", mymessage); ;
        }

        private void loadfirmware()
        {
            string filename = "";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open supported hardware csv download File";
            dialog.Filter = "CSV Files (*.csv)|*.csv";
            dtfirmwares.Clear();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.FileName;
                try {
                    using (var reader = new StreamReader(filename)) 
                using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
                {
                    // Do any configuration to `CsvReader` before creating CsvDataReader.
                    using (var dr = new CsvDataReader(csv))
                    {

                        dtfirmwares.Load(dr);
                            label1.Text = "Firmware File loaded";
                            label1.ForeColor = Color.Black;
                    }
                    }
                }
                catch (System.IO.IOException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        private void loaddefaultpasswords()
        {
            string filename = "";
            button3.Enabled = false;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open Pasword File";
            dialog.Filter = "CSV/text Files (*.csv)|*.csv|(*.txt)|*.txt";
            dtpasswords.Clear();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.FileName;
                try
                {
                    using (var reader = new StreamReader(filename))
                    using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
                    {
                        // Do any configuration to `CsvReader` before creating CsvDataReader.
                        using (var dr = new CsvDataReader(csv))
                        {

                            dtpasswords.Load(dr);
                            label2.Text = "Password File loaded";
                            label2.ForeColor = Color.Black;
                            button3.Enabled = true;
                        }
                    }
                }
                catch (FileLoadException e)
                {

                }
            }
        }

        private string checkpassword(string mylivepassworde)
        {
         
            String returnresult = "Not in list";



            for (int i = 0; i < dtpasswords.Rows.Count; i++)
            {
                ;
                if (dtpasswords.Rows[i]["passwordlist"].ToString().Equals(mylivepassworde))
                {

                    returnresult = "Warning:Password in list";

                    break;
                }
            }

            return returnresult;
        }
        private string checkfirmware(string mymodel, string myfirmware)
        {
            String searchValue = mymodel;
            String returnresult = "Model not found";



            for (int i=0;i< dtfirmwares.Rows.Count;i++)
            {
                ;
                if (dtfirmwares.Rows[i]["DeviceName"].ToString().ToLower().Equals(searchValue.ToLower()))
                {
                    
                    returnresult = firmwarecomparison(myfirmware.ToLower(),dtfirmwares.Rows[i]["FirmwareName"].ToString().ToLower());

                    break;
                }
            }

            return returnresult;
        }

     private string firmwarecomparison(string mylive, string mylatest)
        {
            string myresult = "Could not determine version";
            string pattern = "[\\d]+(?:\\.\\d+)+";
            Match match1 = Regex.Match(mylive, pattern);
            Match match2 = Regex.Match(mylatest, pattern);
            if (match1.Success && match2.Success)
            {
                Version myliveV = new Version(mylive);
                Version mylatestV = new Version(mylatest);
                if (myliveV < mylatestV)
                {
                    myresult = "Warning: Firmware is below the latest supported version"; 
                    
                }
                else
                {
                    myresult = "Firmware is the latest or better then the Milestone supported version";
                }
            }
            if (mylive.ToLower().Trim()==mylatest.ToLower().Trim())
            {
                myresult = "Firmware Matches";

            }
            return myresult;

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            BackgroundWorker helperBW = sender as BackgroundWorker;
            e.Result = BackgroundProcessLogicMethod(helperBW);
            if (helperBW.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                Log_Message("Assigning Values to UI", false);
                //SafeInvoke(button1, () => { button1.Enabled = true; });
                SafeInvoke(dataGridView1, () => { dataGridView1.DataSource = dt; });
                SafeInvoke(dataGridView1, () => { dataGridView1.Columns["rating"].Visible = false; });
                SafeInvoke(dataGridView1, () => { dataGridView1.RowHeadersVisible = false; });
                SafeInvoke(dataGridView1, () => { dataGridView1.ResumeLayout(); });
                //SafeInvoke(button2, () => { button2.Enabled = true; });
                //SafeInvoke(textBoxSearch, () => { textBoxSearch.Enabled = true; });
                //SafeInvoke(progressBar1, () => { progressBar1.Visible = false; });
                //SafeInvoke(hidelabel, () => { hidelabel.Visible = false; });
                //SafeInvoke(hidelabel, () => { hidelabel.Visible = false; });
                //SafeInvoke(listBox1, () => {
                //    for (int intCount = 0; intCount < dt.Rows.Count; intCount++)
                //    {
                //        var val = dt.Rows[intCount]["DeviceType"].ToString();

                //        //check if it already exists
                //        if (!listBox1.Items.Contains(val))
                //        {
                //            listBox1.Sorted = false;
                //            listBox1.Items.Add(val);
                //            listBox1.SetSelected(listBox1.Items.Count - 1, true);
                //            listBox1.Sorted = true;
                //        }
                //    }

                //});
                Log_Message("Assigned Values to UI", false);
            }
        }

        private int BackgroundProcessLogicMethod(BackgroundWorker bw)
        {
            int counter = 0;
            int Total = 0;
            List<string> passme = new List<string>();
            
            Log_Message("Checking System - pulling config", false);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
           

            bw.ReportProgress(0);
            Configuration.Instance.RefreshConfiguration(Kind.Server);
            List<Item> sites = Configuration.Instance.GetItemsByKind(Kind.Server, ItemHierarchy.SystemDefined);
            Log_Message("Initial Config pulled", false);
            bw.ReportProgress(50);
            dt.Clear();
            dt.Columns.Clear();


            if (dt.Columns.Count == 0)
            {
                dt.Columns.Add("Rating");
                dt.Columns.Add("Address");
                dt.Columns.Add("SiteName");
                dt.Columns.Add("RecordingServer");
                dt.Columns.Add("HardwareName");
                dt.Columns.Add("MAC");
                dt.Columns.Add("Model");
                dt.Columns.Add("Enabled");
                dt.Columns.Add("PasswordComplexityRating");
                dt.Columns.Add("PaswordMatch");
                dt.Columns.Add("Current Firmware");
                dt.Columns.Add("FirmwareTest");

            }

            Log_Message("Columns Rendered", false);
            foreach (Item site in sites)
            {
                ManagementServer managementServer = new ManagementServer(site.FQID);

                ICollection<RecordingServer> servers = managementServer.RecordingServerFolder.RecordingServers;
                Log_Message("recording servers Pulled:" + servers.Count, false);
                foreach (RecordingServer server in servers)
                {
                    Total = sites.Count * servers.Count;
                    counter = +1;

                    bw.ReportProgress(50 + (50 * (counter / Total)));
                    ICollection<Hardware> hardwares = server.HardwareFolder.Hardwares;

                    foreach (Hardware hardware in hardwares)
                    {
                        passme.Clear();
                        string strMACAddress = hardware.HardwareDriverSettingsFolder.HardwareDriverSettings.FirstOrDefault().HardwareDriverSettingsChildItems.FirstOrDefault().Properties.GetValue("MacAddress");
                        string strFirmware = hardware.HardwareDriverSettingsFolder.HardwareDriverSettings.FirstOrDefault().HardwareDriverSettingsChildItems.FirstOrDefault().Properties.GetValue("FirmwareVersion");
                        var passwordread = hardware.ReadPasswordHardware();
                        string strPassword = passwordread.GetProperty("Password");
                        
                        DataRow _ravi = dt.NewRow();
                            var test=CheckStrength(strPassword);
                            _ravi["Enabled"] = hardware.Enabled;
                            PasswordScore passmeup = CheckStrength(strPassword);
                            _ravi["PasswordComplexityRating"] = CheckStrength(strPassword);
                        
                        _ravi["RecordingServer"] = server.Name;
                            _ravi["SiteName"] = site.Name;
                            _ravi["HardwareName"] = hardware.Name;
                            _ravi["Model"] = hardware.Model;
                            _ravi["Address"] = hardware.Address;
                            _ravi["FirmwareTest"] = checkfirmware(hardware.Model, strFirmware);
                            _ravi["MAC"] = strMACAddress;
                            _ravi["Current Firmware"] = strFirmware;
                            _ravi["PaswordMatch"] = checkpassword(strPassword);
                        
                        passme.Add(_ravi["FirmwareTest"].ToString());
                        passme.Add(_ravi["PaswordMatch"].ToString());

                        if (((int)passmeup)<3)
                        {
                            passme.Add("Warning");
                        }

                        int irating=howbadisit(passme.ToArray());
                        if (irating == 0)
                        {
                            _ravi["Rating"] = "0";
                        }
                        else if (irating==1)
                        {
                            _ravi["Rating"] = "1";
                        }
                        else
                        {
                            _ravi["Rating"] = "2";
                        }
                        
                        dt.Rows.Add(_ravi);

                    }

                }
            }
            Log_Message("Config Pulled and stored : Total devices : " + dt.Rows.Count + ": took :" + stopwatch.ElapsedMilliseconds, false);
            stopwatch.Stop();
            bw.ReportProgress(100);
            return 0;
        }

        private int  howbadisit(string[] mystrings)
        {
            
            int rating = 0;
            foreach(string s in mystrings)
            {
                if (s.ToLower().Contains("warning"))
                {rating++; }
            }

            return rating;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync(2000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadfirmware();
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loaddefaultpasswords();
            
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                filter();
            }));
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "deviceassessment.csv";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            int columnCount = dataGridView1.Columns.Count;
                            string columnNames = "";
                            string[] outputCsv = new string[dataGridView1.Rows.Count + 1];
                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames += dataGridView1.Columns[i].HeaderText.ToString() + ",";
                            }
                            outputCsv[0] += columnNames;

                            for (int i = 1; (i - 1) < dataGridView1.Rows.Count; i++)
                            {
                                for (int j = 0; j < columnCount; j++)
                                {
                                    outputCsv[i] += dataGridView1.Rows[i - 1].Cells[j].Value.ToString() + ",";
                                }
                            }

                            File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }
    }
}
