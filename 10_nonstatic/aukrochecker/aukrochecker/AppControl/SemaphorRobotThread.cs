using System;
using System.Collections.Generic;
using System.Text;

namespace aukrochecker.AppControl
{
    static class SemaphorRobotThread
    {

        static bool actvBckgContxt = false;
        static readonly object _lockstopBckgContxt = new object();

        static public void setStatus(bool setVal)
        {

            lock (_lockstopBckgContxt)
            {
                actvBckgContxt = setVal;
            }

        }

        static public bool getStatus()
        {
            return actvBckgContxt;
        }
    }
}
