using System;
using System.Collections.Generic;
using System.Text;

namespace aukrochecker.MiningEngine.CoreBasics
{
     class Validation
    {
        public bool isValid(int price, string seller, string description)
        {
            //switch context by pointOfinterest
            return isValid(price, seller, description, null);


        }

        public bool isValid(int price, string seller, string description, PointOfInterest point)
        {

            if (!isPriceValid(price, point)) { return false; }
            if (!isSellerValid(seller, point)) { return false; }
            if (isProhibitPresent(description, point)) { return false; }

            return true;
        }


        public bool isValid(int price, string seller, PointOfInterest point)
        {

            return isValid(price, seller, "", point);
        }

        public bool isValid(int price, string seller)
        {

            return isValid(price, seller, "");
        }

        public bool isPriceValid(int price, PointOfInterest point)
        {
            if (price > point.MAXPRICE) { return false; }


            return true;
        }

        public bool isSellerValid(string seller, PointOfInterest point)
        {
            //string[] prohibitedSellers = { "7162736", "21766751", "9264454", "6369676", "9762876", "15231645", "25657300", "1460820", "40083912", "22910066", "9721613", "33450390", "34215566", "3742876", "9721613", "39156433", "25998530" };


            foreach (string curseller in point?.prohibitedSellers)
            {
                if (seller == curseller) { return false; }


            }



            return true;
        }

        public bool isInTheList(UInt64 id, ref List<Core.InfoFromFilter> listOfOffers)
        {

            foreach (Core.InfoFromFilter item in listOfOffers)
            {
                if (item.id == id) return true;
            }

            return false;
        }



        public bool isProhibitPresent(string description, PointOfInterest point)
        {
            if (description == "") { return false; }


            foreach (string tmp in point?.prohibitedPhrases)
            {
                if (description.Contains(tmp)) { return true; }
            }


            return false;
        }

    }
}
