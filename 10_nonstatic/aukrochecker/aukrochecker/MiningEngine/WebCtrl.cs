using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using aukrochecker.AppControl;
using System.Diagnostics;

namespace aukrochecker.MiningEngine
{
     class WebCtrl
    {
        public string getDescription(string uri)
        {

            string[] separatingDesc = { "fieldset id=\"user_field\"" };
            string[] separatingDescEnd = { "</fieldset>" };

            string content = readHttpPage(uri);
            string[] tmpArr;
            string tmpStr;

            tmpArr = content.Split(separatingDesc, StringSplitOptions.None);
            if (tmpArr.Length > 1)
            {

                tmpArr = tmpArr[1].Split(separatingDescEnd, StringSplitOptions.None);
                tmpStr = StripHTML(tmpArr[0]);
                return tmpStr;

            }
            return "";

        }

        public string readHttpPage(string urlAddress)
        {
            //prepsat na funktor

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response;

            try
            {

                response = (HttpWebResponse)request.GetResponse();



            }
            catch (WebException e)
            {
                Debug.WriteLine("Exception on the web side");
                return "";
                
            }
            catch (InvalidCastException e)
            {
                Debug.WriteLine("Invalid cast exception");
                return "";
            }


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


        

        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

    }
}
