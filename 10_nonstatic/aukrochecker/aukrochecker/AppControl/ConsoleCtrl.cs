using aukrochecker.MiningEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace aukrochecker.AppControl
{
     class ConsoleCtrl
    {      
        public static Form1 myForm;

        public static void setUsedForm(ref Form1 setForm){
            myForm = setForm;
        }


       

        public void runCMDurl(string url)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "chrome.exe";
            startInfo.Arguments = url;
            process.StartInfo = startInfo;
            process.Start();



        }

        public  void informUser(string price, string topic, string link, string sellerID, PointOfInterest pointInProgress)
        {
            /*
            Producent method creates new MessageBox object with possibility decision Yes/No from user side.
            Accept [string]price, [string]topic and [string]link. No return value. In case user decision for Yes, starts the Google chrome with the link like parameter. 
            */

            System.Media.SystemSounds.Exclamation.Play();
            DialogResult result1 = MessageBox.Show(price + " " + topic, "Open this?", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                runCMDurl(pointInProgress.BASEURL + link);

            }
            DialogResult result2 = MessageBox.Show(price + " " + topic, "To ignore list this?", MessageBoxButtons.YesNo);
            if (result2 == DialogResult.Yes)
            {
                pointInProgress.prohibitedSellers.Add(sellerID);

            }
        }

        public void copyList2Listbox(List<Core.InfoFromFilter> toBePrinted, PointOfInterest pointInProgress, ref List<Core.InfoFromFilter> listOfOffers)
        {
            string outputMs = ""; // to be replaced by StringBuilder

            List<UInt64> toBeStopPopup = new List<UInt64>();

            foreach (Core.InfoFromFilter item in toBePrinted)
            {
                if (item.popup)
                {

                    outputMs += Environment.NewLine + item.price.ToString() + " " + item.topic + " " + pointInProgress.BASEURL + item.link;
                    Thread msgthread = new Thread(() => informUser(item.price.ToString(), item.topic, item.link, item.seller, pointInProgress));
                    msgthread.Start();




                }


                toBeStopPopup.Add(item.id);
            }





            for (int j = 0; j < toBeStopPopup.Count; j++)
            {
                for (int i = 0; i < listOfOffers.Count; i++)
                {
                    if (listOfOffers[i].id == toBeStopPopup[j])
                    {

                        var tmpNod = new Core.InfoFromFilter(listOfOffers[i]);
                        tmpNod.popup = false;



                        listOfOffers[i] = tmpNod;

                        break;
                    }
                }
            }
            //MessageBox.Show(outputMs, "Important Message");
        }
    }
}
