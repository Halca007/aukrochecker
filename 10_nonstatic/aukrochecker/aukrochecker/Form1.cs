// To be implemented: zakazani prodejci, odkaz na klik, beh jako deamon vlakno s vyskakovacimi okny na timer, domyslet porovnani(ID aukce)
//  list kontrolovanych se zapominanim



using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using aukrochecker.MiningEngine;
using aukrochecker.AppControl;


namespace aukrochecker
{
    using AppControl;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Xml.Serialization;
    using MiningEngine;




    public partial class Form1 : Form
    {
        // string[] urlAddreses = { Constants.BASEURL + Constants.CATURL + Constants.SECTIONURL, Constants.BASEURL + Constants.CATURL2 + Constants.SECTIONURL };
        Storage u_centralStorage;
        Core u_core;
        PointsQueue u_pointsInProcess;


        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            SemaphorRobotThread.setStatus(false);
            u_centralStorage.saveConfig();
        }






        public void sendSMS()
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.seznam.cz");
            SmtpServer.Port = 25;
            SmtpServer.Credentials =
            new System.Net.NetworkCredential("halcin@seznam.cz", "_z9A47b10");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("halcin@seznam.cz");
            mail.To.Add("halcin@seznam.cz");
            mail.Subject = "Test Mail";
            mail.Body = "This is for testing SMTP mail from GMAIL";
            SmtpServer.Send(mail);
        }

        public void loadWords(string jsonFilePath)
        {
            // override jsou natahovana rovnou na zacatku z XML
            //string[] tmp = { "bez hdd", "bez ram", "!!!", "Chybí: HDD, RAM, nabíječka.", "chybí HDD", "Chybí: HDD", "nemá HDD" };

            // foreach (string word in GlobalOptions.prohibitedPhrases)
            // {
            //   listOfprohibits.Add(word.ToLower());
            //}

        }



        public string json2string(string jsonFilePath)
        {



            return "not implemented yet";
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("file.json"))
            {
                string json = r.ReadToEnd();
                //  List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
        }

        public void createMiningThread()
        {
            PointOfInterest point = new PointOfInterest();
            createMiningThread(point);
        }

        public void createMiningThread(PointOfInterest pointThreadToBeCreated)
        {

            u_core.miningRoutine(pointThreadToBeCreated);
        }




        public class Item
        {
            public int millis;
            public string stamp;
            public DateTime datetime;
            public string light;
            public float temp;
            public float vcc;

        }


        public struct Words
        {
            public string word;
            public Words(string word)
            {
                this.word = word;
            }
        }





        public List<Core.InfoFromFilter> listOfOffers = new List<Core.InfoFromFilter>();


        public Form1()
        {
            InitializeComponent();
            u_core = new Core();
            u_centralStorage = new Storage(u_core);
            
            loadWords("no path implemented");
        }










        private string readHttpPage(string urlAddress)
        {
            //prepsat na funktor

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string data = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();
            }
            return data;

        }




        //public void dEcho(string sValue)
        //{


        //    if (SemaphorRobotThread.getStatus())
        //    {
        //        Invoke(new MethodInvoker(
        //               delegate
        //               {
        //                   listBox1.Items.Add(sValue + "\n");
        //                   listBox1.SelectedIndex = listBox1.Items.Count - 1;
        //               }
        //               ));
        //    }

        //}





        public void dEcho(string sValue)
        {
            if (SemaphorRobotThread.getStatus())
            {
                Invoke(new MethodInvoker(
                       delegate
                       {
                           listBox1.Items.Add(sValue + "\n");
                           listBox1.SelectedIndex = listBox1.Items.Count - 1;
                       }
                       ));
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 myRef = this;
            AppControl.ConsoleCtrl.setUsedForm(ref myRef);

            SemaphorRobotThread.setStatus(true);
            this.button1.Enabled = false;
            this.button2.Enabled = true;

            u_centralStorage.loadConfig();


            // var analystthread = new Thread(new ParameterizedThreadStart(new MiningEngine.Core().miningRoutine));
            Thread analystthread = new Thread(new ParameterizedThreadStart(u_core.miningRoutine));

            analystthread.Name = "Aukro analyst robot";
            analystthread.Start(u_pointsInProcess);

        }


        public void informUser()
        {

        }



        private void button2_Click(object sender, EventArgs e)
        {
            SemaphorRobotThread.setStatus(false);

            this.button1.Enabled = false;
            this.button2.Enabled = true;
        }







        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            sendSMS();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {


        }




        private void updateValue(ref Dictionary<string, string> dic, string paramName, string paramVal)
        {
            string tmp;
            if (paramVal != "" && paramVal != "0")
            {
                if (dic.TryGetValue(paramName, out tmp)) { dic[paramName] = paramVal; }
                else { dic.Add(paramName, paramVal); }
            }
            else { dic.Remove(paramVal); }

        }

        private void linkBackModificator(ref Dictionary<string, string> oldValues)
        {
            updateValue(ref oldValues, "price_from", textBox9.Text);
            updateValue(ref oldValues, "price_to", textBox10.Text);
            updateValue(ref oldValues, "string", textBox11.Text);
            updateValue(ref oldValues, "state", textBox12.Text);
            updateValue(ref oldValues, "postcode", textBox13.Text);
            updateValue(ref oldValues, "distance", textBox14.Text);

            if (this.checkBox1.Checked) { updateValue(ref oldValues, "buyNew", "1"); }
            else { updateValue(ref oldValues, "buyNew", "0"); }

            if (this.checkBox2.Checked) { updateValue(ref oldValues, "buyUsed", "1"); }
            else { updateValue(ref oldValues, "buyUsed", "0"); }

            if (this.checkBox3.Checked) { updateValue(ref oldValues, "buyNow", "1"); }
            else { updateValue(ref oldValues, "buyNow", "0"); }

            if (this.checkBox4.Checked) { updateValue(ref oldValues, "offerTypeAuction", "1"); }
            else { updateValue(ref oldValues, "offerTypeAuction", "0"); }


        }
        private void linkDataAccepter(Dictionary<string, string> currentValues)
        {
            //don't forgot to define default values for dictionary
            string getVal = "";


            if (currentValues.TryGetValue("price_from", out getVal))
            {
                textBox9.Text = getVal;
            }
            if (currentValues.TryGetValue("price_to", out getVal))
            {
                textBox10.Text = getVal;
            }
            if (currentValues.TryGetValue("string", out getVal))
            {
                textBox11.Text = getVal;
            }
            if (currentValues.TryGetValue("state", out getVal)) //region
            {
                textBox12.Text = getVal;
            }
            if (currentValues.TryGetValue("postcode", out getVal))
            {
                textBox13.Text = getVal;
            }
            if (currentValues.TryGetValue("distance", out getVal))
            {
                textBox14.Text = getVal;
            }

            if (currentValues.TryGetValue("buyNew", out getVal))
            {
                if (getVal == "1") { checkBox2.Checked = true; }
            }
            else
            {
                checkBox1.Checked = false;
            }

            if (currentValues.TryGetValue("buyUsed", out getVal))
            {
                if (getVal == "1") { checkBox2.Checked = true; }
            }
            else
            {
                //  checkBox2.Checked = false;
            }

            if (currentValues.TryGetValue("BuyNow", out getVal))
            {
                if (getVal == "1") { checkBox3.Checked = true; }
            }
            else
            {
                //   checkBox3.Checked = false;
            }

            if (currentValues.TryGetValue("offerTypeAuction", out getVal))
            {
                // if (getVal == "1") { checkBox4.Checked = true; }
            }
            else
            {
                checkBox4.Checked = false;
            }









        }

        private void button5_Click(object sender, EventArgs e)
        {

            //    string[] pointOfInterestOptions = Directory.GetFiles(".\\", "*_configSell.xml");
            //    listBox2.DataSource = pointOfInterestOptions;

            Dictionary<string, string> newPointParams = new Dictionary<string, string>();
            LinkImporter tmpImporter = new LinkImporter();

            tmpImporter.convertToData(this.textBox8.Text, ref newPointParams);
            this.linkDataAccepter(newPointParams);





        }

        private void button6_Click(object sender, EventArgs e)
        {

            u_centralStorage.loadConfig(this.textBox6.Text);



        }
       
        private void button8_Click(object sender, EventArgs e)
        {

            if (u_core.refPoints != null) {
                this.listBox3.Items.Clear();
                foreach (PointOfInterest p in u_core.refPoints){
                    this.listBox3.Items.Add(p.fileName);

                }
            }
            else { this.listBox3.Items.Clear(); }

        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            foreach (PointOfInterest p in u_core.refPoints) {
                if (p.fileName == this.listBox3.SelectedItem.ToString()) {
                    this.textBox4.Text = p.BASEURL;
                    this.textBox5.Text = p.CATURL[0];
                    this.textBox3.Text = p.REFRESHDELAY.ToString();
                    this.textBox2.Text = p.MAXHOURSOLD.ToString();
                    this.textBox1.Text = p.MAXPRICE.ToString();
                    this.textBox6.Text = p.fileName;

                    string tmpPP = "";

                     
                    foreach (string s in p?.prohibitedPhrases) {
                        tmpPP = tmpPP + ";" + s;
                    }
                    this.textBox7.Text = tmpPP;
                    

                    break;

                }
                    

            }

        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (u_core.refPoints == null) {u_core.refPoints = new List<PointOfInterest>(); }

            bool find = false;
            foreach (PointOfInterest p in u_core.refPoints)
            {
                if (p.fileName == this.listBox3.SelectedItem.ToString())
                {
                    
                    p.BASEURL = this.textBox4.Text;
                    p.CATURL[0] = this.textBox5.Text;
                    p.REFRESHDELAY = Convert.ToInt32(this.textBox3.Text);
                    p.MAXHOURSOLD = Convert.ToInt32(this.textBox2.Text);
                    p.MAXPRICE = Convert.ToInt32(this.textBox1.Text);
                    p.fileName = this.textBox6.Text;
                    p.prohibitedPhrases = new List<String>(this.textBox7.Text.ToString().Split(';'));
                    p.nameOfPoint = this.textBox6.Text;
                   

                    u_centralStorage.saveConfig(p);
                    find = true;
                    break;
                }

            }
            if (!find) {
                PointOfInterest p = new PointOfInterest();
                p.BASEURL = this.textBox4.Text;
                p.CATURL[0] = this.textBox5.Text;
                p.REFRESHDELAY = Convert.ToInt32(this.textBox3.Text);
                p.MAXHOURSOLD = Convert.ToInt32(this.textBox2.Text);
                p.MAXPRICE = Convert.ToInt32(this.textBox1.Text);
                p.fileName = this.textBox6.Text;
                p.prohibitedPhrases = new List<String>(this.textBox7.Text.ToString().Split(';'));


                u_core.refPoints.Add(p);
                u_centralStorage.saveConfig(p);

            }
        }
       
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            PointOfInterest tmp = new PointOfInterest();
            tmp.nameOfPoint = this.textBox15.Text;
            
            string[] catURLs = { this.textBox8.Text };
            tmp.CATURL = catURLs;
         //   !!! tmp.BASEURL = this.textBox4.Text;

            tmp.MINPRICE = Convert.ToInt16(this.textBox9.Text);
            tmp.MAXPRICE = Convert.ToInt16(this.textBox10.Text);
            tmp.SEARCHEDPHRASE = this.textBox11.Text;
            tmp.REGION = this.textBox12.Text;
            tmp.ZIPCODE = Convert.ToInt16(this.textBox13.Text);
            tmp.DISTANCE = Convert.ToInt16(this.textBox14.Text);
            tmp.SEARCHNEW = this.checkBox1.Checked;
            tmp.SEARCHUSED = this.checkBox2.Checked;
            tmp.BUYNOW = this.checkBox3.Checked;
            tmp.AUCTIONS = this.checkBox4.Checked;
            tmp.MAXHOURSOLD = Convert.ToInt16(this.textBox16.Text);


            try
            {
            tmp.REFRESHDELAY = Convert.ToInt16(this.textBox3.Text);
                //tmp.CloneSellers(GlobalOptions.prohibitedSellers);                
            tmp.CloneWords(new List<string>(this.textBox17.Text.Split(';')));
            }
            catch { }

            if (u_core.refPoints == null) { u_core.refPoints = new List<PointOfInterest>(); }
            u_core.refPoints.Add(tmp);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (u_pointsInProcess == null) u_pointsInProcess = new PointsQueue();

            foreach (string s in listBox4.SelectedItems)
            {
                if (!listBox5.Items.Contains(s))
                {
                    foreach (PointOfInterest p in u_core.refPoints)
                    {
                        if (p.nameOfPoint == s)
                        {
                            u_pointsInProcess.Enqueue(p);
                            listBox5.Items.Add(s);
                        }
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listBox2.Items.AddRange(u_centralStorage.getExistsPointOIfiles(true));
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

            List<string> filesToLoad = new List<string>();
            foreach (string s in listBox2.SelectedItems) {
                filesToLoad.Add(s);
            }

            u_centralStorage.loadConfig(true, filesToLoad);


            foreach (string f in filesToLoad)
            {
                /*string[] tmp = Regex.Split(f, "_configSell.xml");
                tmp = Regex.Split(tmp[tmp.Length - 2], @".\\");
                string realName = tmp[tmp.Length - 1];

                PointOfInterest point = new PointOfInterest();
                point.fileName = realName;
                u_centralStorage.loadConfig(point);
                point.updateURLadresses();
                
                if (u_core.refPoints == null) { u_core.refPoints = new List<PointOfInterest>(); }
                */
                
            }
            listBox4.Items.Clear();
            foreach (PointOfInterest p in u_core.refPoints ?? new List<PointOfInterest>()) {
                listBox4.Items.Add(p.nameOfPoint);
            }



        }

        private void button14_Click(object sender, EventArgs e)
        {
            
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }
    }
    static class Constants
    {
        public const int MAXPRICE = 2000;
        public const int MAXHOURSOLD = 1;
        public const int REFRESHDELAY = 10000;
        public const string BASEURL = "http://aukro.cz";
        public static string SECTIONURL = "&startingTime=" + Convert.ToString(Constants.MAXHOURSOLD) + "&startingTime_enabled=1&price_to=" + Convert.ToString(Constants.MAXPRICE) + "&price_enabled=1";
        public const string CATURL = "/notebooky-prislusenstvi-notebooky-netbooky-100709";
        public const string CATURL2 = "/notebooky-prislusenstvi-poskozene-100720";
        public static string[] separatingChars = { "<article " };
        public static string[] separLnkNameTag = { "<header>\n  <h2><a href=\"" };
        public static string[] separLnkNameTagEnd = { "\"><span>" };
        public static string[] separNameTagEnd = { "</span>" };
        public static string[] separAttribTag = { "Kup teď</span>" };
        public static string[] separAttribTagEnd = { "Kč" };
        public static string[] separSellerID = { "\"s\":\"" };
        public static string[] separSellerIDend = { "\"" };
        public static string[] separItemID = { "id=\"item-" };
        public static string[] separItemIDend = { "\"" };
    }

    static class GlobalOptions
    {
        // public static string[] prohibitedSellers = { "7162736", "21766751", "9264454", "6369676", "9762876", "15231645", "25657300", "1460820", "40083912", "22910066", "9721613", "33450390", "34215566", "3742876", "9721613", "39156433", "25998530" };
        public static List<string> prohibitedSellers;
        public static List<string> prohibitedPhrases; //= new List<string>(Array({ "bez hdd", "bez ram", "!!!", "Chybí: HDD, RAM, nabíječka.", "chybí HDD", "Chybí: HDD", "nemá HDD" }));
    }



    [Serializable()]
    public class PointOfInterest
    {
        public int MAXPRICE = 2000;
        public int MAXHOURSOLD = 1;
        public int REFRESHDELAY = 10000;
        public string BASEURL = "http://aukro.cz";
        public string SECTIONURL; // = "&startingTime=" + Convert.ToString(Constants.MAXHOURSOLD) + "&startingTime_enabled=1&price_to=" + Convert.ToString(Constants.MAXPRICE) + "&price_enabled=1"; //?ref=electronics-layer-popular
        public string[] CATURL; // = { "/notebooky-prislusenstvi-notebooky-netbooky-100709", "/notebooky-prislusenstvi-poskozene-100720" };
        public List<string> prohibitedSellers;
        public List<string> prohibitedPhrases;
        public string[] urlAddresses; //= { Constants.BASEURL + Constants.CATURL + Constants.SECTIONURL, Constants.BASEURL + Constants.CATURL2 + Constants.SECTIONURL };

        public string fileName = "config";
        public long MINPRICE;
        public string SEARCHEDPHRASE;
        public string REGION;
        public int ZIPCODE;
        public int DISTANCE;
        public bool SEARCHNEW;
        public bool SEARCHUSED;
        public bool BUYNOW;
        public bool AUCTIONS;


        public string nameOfPoint { get; set; }


        public void updateURLadresses()
        {
            this.SECTIONURL = "&startingTime=" + Convert.ToString(this.MAXHOURSOLD) + "&startingTime_enabled=1&price_to=" + Convert.ToString(this.MAXPRICE) + "&price_enabled=1";
            this.urlAddresses = new string[] { this.BASEURL + this.CATURL[0] + this.SECTIONURL };
            
        }

        public string composeURLadress(Int16 MINPRICE, Int16 MAXPRICE, string SEARCHEDPHRASE, string REGION, Int16 ZIPCODE, Int16 DISTANCE, string SEARCHNEW,
            string SEARCHUSED, string BUYNOW, string AUCTIONS, Int16 MAXHOURSOLD)
        {

            string url;
            url = "?string="+SEARCHEDPHRASE+"&price_from=";
            url += MINPRICE.ToString();
            return url;
        }

        public PointOfInterest() { }
        public PointOfInterest(List<string> prohibitedSellers, List<string> prohibitedPhrases)
        {
            this.prohibitedSellers = new List<string>(prohibitedSellers);
            this.prohibitedPhrases = new List<string>(prohibitedPhrases);
        }


        public void CloneSellers(List<string> prohibitedSellers)
        {
            this.prohibitedSellers = prohibitedSellers;
        }

        public void CloneWords(List<string> prohibitedPhrases)
        {
            this.prohibitedPhrases = prohibitedPhrases;
        }




    }

}

    

