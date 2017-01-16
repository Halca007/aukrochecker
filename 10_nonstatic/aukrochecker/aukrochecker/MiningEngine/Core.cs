using aukrochecker.AppControl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;



//using static aukrochecker.Form1;

namespace aukrochecker.MiningEngine
{
    public class Core
    {
        CoreBasics.Validation c_validation;
        ConsoleCtrl c_consoleCtrl;
        WebCtrl c_webCtrl;

        public Core() {
            c_validation = new CoreBasics.Validation();
            c_consoleCtrl = new ConsoleCtrl();
            c_webCtrl = new WebCtrl();
        }

        public List<PointOfInterest> refPoints;
        



        public struct InfoFromFilter
        {
            public ulong id;
            public string topic;
            public int price;
            public string link;
            public string description;
            public string seller;
            public bool popup;

            public InfoFromFilter(InfoFromFilter oldNod)
            {
                this.id = oldNod.id;
                this.seller = oldNod.seller;
                this.topic = oldNod.topic;
                this.price = oldNod.price;
                this.link = oldNod.link;
                this.description = oldNod.description;
                this.popup = oldNod.popup;

            }

            public InfoFromFilter(ulong id, string topic, int price, string link, string description, string seller)
            {
                this.id = id;
                this.seller = seller;
                this.topic = topic;
                this.price = price;
                this.link = link;
                this.description = description;
                this.popup = true;
            }

            public void setNoPopup()
            {
                this.popup = false;
            }

            public InfoFromFilter(string topic, int price, string link, string description)
            {
                this.topic = topic;
                this.price = price;
                this.link = link;
                this.description = description;
                this.seller = "";
                this.id = 0;
                this.popup = true;
            }

            public InfoFromFilter(string topic, int price, string link)
            {
                this.topic = topic;
                this.price = price;
                this.link = link;
                this.description = null;
                this.seller = "";
                this.id = 0;
                this.popup = true;
            }

        }

        public string composeURLadressAukro(PointOfInterest point)
        {
            string hardURL = "http://aukro.cz/listing/listing.php";
            string volatileURL;

            if (!String.IsNullOrEmpty(point.SEARCHEDPHRASE)) volatileURL = "?string=" + point.SEARCHEDPHRASE;
            else volatileURL = "?string=" + point.nameOfPoint;
            if (point.MINPRICE > 0) volatileURL += "&price_from=" + point.MINPRICE.ToString();
            if (point.MAXPRICE > 0) volatileURL += "&price_to=" + point.MAXPRICE.ToString();
            if (!String.IsNullOrEmpty(point.REGION)) volatileURL += "&state=" + Convert.ToUInt16(point.REGION).ToString();
            if (point.ZIPCODE > 9999 && point.ZIPCODE < 99999) volatileURL += "&postcode=" + point.ZIPCODE.ToString();
            if (point.DISTANCE > 0) volatileURL += "&distance=" + point.DISTANCE.ToString();
            if (point.SEARCHNEW) volatileURL += "&buyNew=" + Convert.ToUInt16(point.SEARCHNEW).ToString();
            if (point.SEARCHUSED) volatileURL += "&buyUsed=" + Convert.ToUInt16(point.SEARCHUSED).ToString();
            if (point.BUYNOW) volatileURL += "&offerTypeBuyNow=" + Convert.ToUInt16(point.BUYNOW).ToString();
            if (point.AUCTIONS) volatileURL += "&offerTypeAuction=" + Convert.ToUInt16(point.AUCTIONS).ToString();
            if (point.MAXHOURSOLD > 0) volatileURL += "&startingTime=" + Convert.ToUInt16(point.MAXHOURSOLD).ToString();

            return hardURL + volatileURL;
        }

        public void miningRoutine(object listRefPoint){
            Form1 form = ConsoleCtrl.myForm;
        List<Core.InfoFromFilter> listOfOffers = new List<Core.InfoFromFilter>();

            PointsQueue pointsInThread = (PointsQueue)listRefPoint;


            while (SemaphorRobotThread.getStatus())
            {


                foreach (PointOfInterest point in pointsInThread)
                {
                    point.urlAddresses = composeURLadressAukro(point);

                    //foreach (string urlAddress in point.urlAddresses)
                    //{

                        form.dEcho("\n new scan at " + string.Format("{0:HH:mm:ss tt}", DateTime.Now) + " with " + point.nameOfPoint);
                       
                        int numAttrb = 0;
                        string[] separatingChars = Constants.separatingChars;
                        string[] separLnkNameTag = Constants.separLnkNameTag;
                        string[] separLnkNameTagEnd = Constants.separLnkNameTagEnd;
                        string[] separNameTagEnd = Constants.separNameTagEnd;
                        string[] separAttribTag = Constants.separAttribTag;
                        string[] separAttribTagEnd = Constants.separAttribTagEnd;
                        string[] separSellerID = Constants.separSellerID;
                        string[] separSellerIDend = Constants.separSellerIDend;
                        string[] separItemID = Constants.separItemID;
                        string[] separItemIDend = Constants.separItemIDend;

                        string sellerID = "";
                        form.dEcho("Used web url:\n"+ point.urlAddresses);
                        string data = c_webCtrl.readHttpPage(point.urlAddresses); //return RAW httppage, return "" if not available
                        
                        string[] parseToTopics = data.Split(separatingChars, StringSplitOptions.None);
                        string[] tempTagParser = null, tempTagParser2 = null, tempTagParser3 = null, tempTagParser4 = null, tempTagParser5 = null;


                        int itemCnt = 0;
                        foreach (string part in parseToTopics)
                        {
                            UInt64 currentID = 0;
                            if (part.Contains("<div class=\"purchase\">"))
                            {
                                itemCnt++;

                                //purchase ID first
                                tempTagParser = part.Split(separLnkNameTag, StringSplitOptions.None);
                                tempTagParser5 = part.Split(separItemID, StringSplitOptions.None);
                                tempTagParser5 = tempTagParser5[1].Split(separItemIDend, StringSplitOptions.None);

                                currentID = (Convert.ToUInt64(tempTagParser5[0]));
                                if (c_validation.isInTheList(currentID, ref listOfOffers)) { continue; }

                                tempTagParser5 = tempTagParser[0].Split(separSellerID, StringSplitOptions.None);
                                tempTagParser5 = tempTagParser5[1].Split(separSellerIDend, StringSplitOptions.None);
                                sellerID = tempTagParser5[0];
                                tempTagParser5 = null;
                                if (tempTagParser.Length > 1)
                                {
                                    tempTagParser2 = tempTagParser[1].Split(separLnkNameTagEnd, StringSplitOptions.None);


                                    // tempTagParser2[0] je link na produkt, tempTagParser5[0] id obchodnika
                                    // tempTagParser2[1] is NAME with sufix
                                }



                                if (tempTagParser2 != null && tempTagParser2?.Length > 0)
                                {
                                    //go to separate NAME
                                    tempTagParser3 = tempTagParser2[1].Split(separNameTagEnd, StringSplitOptions.None);

                                }

                                if (part != null)
                                {
                                    //go to separate ATTRIB
                                    tempTagParser4 = part.Split(separAttribTag, StringSplitOptions.None);
                                    if (tempTagParser4.Length < 2)
                                    {
                                        continue;
                                    } //pokud neni kup ted, rovnou zahod to be parameter
                                    tempTagParser4 = tempTagParser4[1].Split(separAttribTagEnd, StringSplitOptions.None);

                                    //TO DO tempTagParser4[0] .. attribs parsing

                                    string tmpRmWhsp = tempTagParser4[0];
                                    tempTagParser4 = null;

                                    tmpRmWhsp = Regex.Replace(tmpRmWhsp, @"\s+", "");
                                    numAttrb = Int32.Parse(Regex.Match(tmpRmWhsp, @"\d+").Value);


                                }



                                if (tempTagParser2 != null && tempTagParser2.Length > 0)
                                {
                                    //TO BE set like method of container

                                    string name, url, description;
                                    int attrib;
                                    name = tempTagParser3[0];
                                    attrib = numAttrb;
                                    url = tempTagParser2[0];

                                    tempTagParser2 = null;
                                    tempTagParser3 = null;
                                    numAttrb = 0;
                                    form.dEcho(" s:" + sellerID + " ");



                                    if (!c_validation.isValid(attrib, sellerID, point))
                                    {
                                    form.dEcho(" stoped ");
                                        continue;
                                    }

                                    description = c_webCtrl.getDescription(point.BASEURL + url);

                                    listOfOffers.Add(new Core.InfoFromFilter(currentID, name, attrib, url, description, sellerID));
                                    form.dEcho("added ID " + currentID.ToString() + " at " + string.Format("{0:HH:mm:ss tt}", DateTime.Now));
                                }



                            }



                        }

                        form.dEcho("Found " + itemCnt.ToString() + " items");



                        int debugCnt = listOfOffers.Count;
                        List<InfoFromFilter> toBeRemoved = new List<InfoFromFilter>();


                        foreach (InfoFromFilter tmpObj in listOfOffers)
                        {


                            if (c_validation.isProhibitPresent(tmpObj.description.ToLower(), point))
                            {
                                //toBeRemoved.Add(tmpObj);

                                tmpObj.setNoPopup();
                            }
                            if (c_validation.isProhibitPresent(tmpObj.topic.ToLower(), point))
                            {
                                //toBeRemoved.Add(tmpObj);

                                tmpObj.setNoPopup();
                            }



                        }

                        foreach (InfoFromFilter tmpRem in toBeRemoved)
                        {

                            //  listOfOffers.Remove(tmpRem);

                        }

                        int debugCnt2 = listOfOffers.Count;

                        c_consoleCtrl.copyList2Listbox(listOfOffers, point, ref listOfOffers);


                        // listBox1.Items.Add("filtered");

                        Thread.Sleep(Constants.REFRESHDELAY);
                    
                }
            }

        }

       
    }

    



}
