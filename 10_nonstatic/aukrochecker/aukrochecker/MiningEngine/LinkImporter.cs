using System;
using System.Collections.Generic;
using System.Text;

namespace aukrochecker.MiningEngine
{
    

    class LinkImporter
    {
        public LinkImporter() { }

        public void convertToData(string inputLink, ref Dictionary<string, string> paramCollection) {

            
            List<string> tmpList = new List<string>(inputLink.Split('?'));
            tmpList.RemoveAt(0);
            tmpList = new List<string>(tmpList[0].Split('&'));

            foreach (string s in tmpList) {
                string[] tmpArr = s.Split('=');

                if (tmpArr.Length != 2) { throw new ArgumentException("too many values in parameter in " + tmpArr[0]); }
                paramCollection.Add(tmpArr[0],tmpArr[1]);


            }

            

        }
    }
}
