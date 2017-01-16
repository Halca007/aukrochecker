using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using aukrochecker.MiningEngine;
using System.Text.RegularExpressions;
//using static aukrochecker.Form1;

namespace aukrochecker.MiningEngine
{
     class Storage
    {
        Core st_core;
        public Storage(Core core) {
            st_core = core;
        }

        private string[] loadNamesOfPoints()
        {
            string defSetupFile = "setup.xml";
            return loadNameOfPointsFromFile(defSetupFile);
        }


        //accept XML file with serialized String 1D array which includes available points
        private string[] loadNameOfPointsFromFile(string inputFileName)
        {
            string[] tmp = { };

                if (File.Exists(inputFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(tmp.GetType());
                    using (StreamReader sr = new StreamReader(inputFileName))
                    {
                        tmp = (string[])serializer.Deserialize(sr);
                    }

                }
            
            
            return tmp;
        }

        public string[] getExistsPointOIfiles()
        {
           return getExistsPointOIfiles(true);
        }

        public string[] getExistsPointOIfiles(bool binary)
        {
            return getExistsPointOIfiles(".\\", binary);
        }

        public string[] getExistsPointOIfiles(string path, bool binary)
        {
            string[] pointOfInterestOptions;
            if (binary) { pointOfInterestOptions = Directory.GetFiles(path, "*.bin"); }
            else { pointOfInterestOptions = Directory.GetFiles(path, "*_configSell.xml"); }
            
            
            return pointOfInterestOptions;
        }


        public void loadConfig(bool binary) {
            loadConfig(binary, null);
        }

        public void loadConfig(bool binary, List<String> filesToBeLoad) {
           
            string[] tmp;
            string realName;
            if (filesToBeLoad == null) {
                filesToBeLoad = new List<String>(getExistsPointOIfiles(binary));
            }
            if (binary) {
                foreach (string f in filesToBeLoad) {
                    realName = f;
                    PointOfInterest point = new PointOfInterest();
                    point.fileName = realName;
                    loadConfig(point,true);
                    //point.updateURLadresses();

                    if (st_core.refPoints == null) { st_core.refPoints = new List<PointOfInterest>(); }
                    st_core.refPoints.Add(point);
                }
            }
            else {
                
                foreach (string f in filesToBeLoad)
                {
                    tmp = Regex.Split(f, "_configSell.xml");
                    tmp = Regex.Split(tmp[tmp.Length - 2], @".\\");
                    realName = tmp[tmp.Length - 1];

                    PointOfInterest point = new PointOfInterest();
                    point.fileName = realName;
                    loadConfig(point);
                    point.updateURLadresses();

                    if (st_core.refPoints == null) { st_core.refPoints = new List<PointOfInterest>(); }
                    //spatne, prepsat na IComparable
                    if (!st_core.refPoints.Contains(point)) { st_core.refPoints.Add(point); }
                }
            }


        }
        public void loadConfig()
        {
            loadConfig(false);
            // loadConfig("config");
        }


        public void loadConfig(string nameOffile)
        {
            loadConfig(nameOffile, false);
        }

        public void loadConfig(string nameOffile, bool binary)
        {
            PointOfInterest tmp = new PointOfInterest();
            tmp.fileName = nameOffile;
            loadConfig(tmp, binary);

            if (st_core.refPoints == null) { st_core.refPoints = new List<PointOfInterest>(); } //to be replaced by singleton
            st_core.refPoints.Add(tmp);
        }

        public void loadPoint(PointOfInterest point) {
            point = ReadFromBinaryFile<PointOfInterest>(point.fileName);
        }

        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        public void loadConfig(PointOfInterest point, bool binary)
        {
            if (binary)
            {
                loadPoint(point);
                return;
            }
            else {
                List<string> data = new List<string>();

                string tmps = ".\\" + point.fileName + "_config.xml";

                if (File.Exists(point.fileName + "_config.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(data.GetType());
                    using (StreamReader sr = new StreamReader(point.fileName + "_config.xml"))
                    {
                        point.prohibitedPhrases = GlobalOptions.prohibitedPhrases = (List<string>)serializer.Deserialize(sr);

                    }

                    using (StreamReader sr = new StreamReader(point.fileName + "_configSell.xml"))
                    {
                        point.prohibitedSellers = GlobalOptions.prohibitedSellers = (List<string>)serializer.Deserialize(sr);

                    }

                    using (StreamReader sr = new StreamReader(point.fileName + "_options.xml"))
                    {
                        List<String> optionToBeLoad = new List<String>();

                        optionToBeLoad = (List<String>)serializer.Deserialize(sr);
                        point.MAXHOURSOLD = Convert.ToInt32(optionToBeLoad[0]);
                        point.MAXPRICE = Convert.ToInt32(optionToBeLoad[1]);
                        point.nameOfPoint = optionToBeLoad[2];
                        point.REFRESHDELAY = Convert.ToInt32(optionToBeLoad[3]);
                        point.BASEURL = optionToBeLoad[4];
                        point.CATURL = new string[1] { optionToBeLoad[5] };
                    }



                }
            }





        }

        public void loadConfig(PointOfInterest point)
        {

            loadConfig(point, false);
           
            return;


        }


       public void saveConfig()
        {
            if (st_core.refPoints != null) { 
                foreach (PointOfInterest p in st_core.refPoints){
                saveConfig(p);
                saveConfig(p, true);
                }

            }

        }

        public void saveConfig(PointOfInterest pointToBeSaved, bool binary) {
            if (binary) {
                savePoint(pointToBeSaved);
            }
            else
            {   
                List<String> optionToBeSaved = new List<String>();

                if (pointToBeSaved.nameOfPoint == null) { pointToBeSaved.nameOfPoint = "not defined"; }
                if (pointToBeSaved.urlAddresses == null) { pointToBeSaved.urlAddresses = "not defined"; }

                optionToBeSaved.Add(pointToBeSaved.MAXHOURSOLD.ToString());
                optionToBeSaved.Add(pointToBeSaved.MAXPRICE.ToString());
                optionToBeSaved.Add(pointToBeSaved.nameOfPoint.ToString());
                optionToBeSaved.Add(pointToBeSaved.REFRESHDELAY.ToString());
                optionToBeSaved.Add(pointToBeSaved.BASEURL);
                optionToBeSaved.Add(pointToBeSaved.CATURL[0]);

                //optionToBeSaved.Add(pointToBeSaved.MINPRICE.ToString());



                saveConfig(pointToBeSaved.prohibitedPhrases, pointToBeSaved.prohibitedSellers, optionToBeSaved, pointToBeSaved.fileName);

            }
        }

        public void saveConfig(PointOfInterest pointToBeSaved)
        {
            saveConfig(pointToBeSaved, false);
        }

        public void saveConfig(List<string> prohibitedPhrases, List<string> prohibitedSellers)
        {

            saveConfig(prohibitedPhrases, prohibitedSellers, null, "default");

        }

        public void savePoint(PointOfInterest point) {
            WriteToBinaryFile<PointOfInterest>(point.nameOfPoint+".bin", point);
        }

        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public void saveConfig(List<string> prohibitedPhrases, List<string> prohibitedSellers, List<string> optionToBeSaved, string fileName) {

            XmlSerializer serializer = new XmlSerializer(prohibitedPhrases.GetType());

            using (StreamWriter sw = new StreamWriter(fileName + "_config.xml"))
            {
                serializer.Serialize(sw, prohibitedPhrases);
            }

            using (StreamWriter sw = new StreamWriter(fileName + "_configSell.xml"))
            {
                serializer.Serialize(sw, prohibitedSellers);
            }

            using (StreamWriter sw = new StreamWriter(fileName + "_options.xml"))
            {
                serializer.Serialize(sw, optionToBeSaved);
                                

            }
        }
               
    }
}
