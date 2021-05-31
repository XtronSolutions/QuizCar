using System;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;

namespace RacingGameKit.Helpers
{
    //Internal Class for Comparing Racers based their Distance
    internal   class RacerComparer : IComparer<Racer_Detail>
    {
        //public int Compare(Racer_Detail Racer1, Racer_Detail Racer2)
        //{
        //    if (Racer1 == null)
        //    {
        //        if (Racer2 == null)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            return -1;
        //        }
        //    }
        //    else
        //    {
        //        if (Racer2 == null)
        //        {
        //            return 1;
        //        }
        //        else
        //        {
        //            int retval = Racer1.RacerDistance.CompareTo(Racer2.RacerDistance);

        //            if (retval != 0)
        //            {
        //                return retval;
        //            }
        //            else
        //            {
        //                return Racer1.CompareTo(Racer2);
        //            }
        //        }
        //    }
        //}
        public   int Compare(Racer_Detail Racer1, Racer_Detail Racer2)
        {
            return ((IComparable)Racer1.RacerDistance).CompareTo(Racer2.RacerDistance);
        }
    }

    internal class RacerComparerByHighSpeed : IComparer<Racer_Detail>
    {
        public int Compare(Racer_Detail Racer1, Racer_Detail Racer2)
        {
            return -((IComparable)Racer1.RacerHighestSpeed).CompareTo(Racer2.RacerHighestSpeed);
        }
    }
    internal class RacerComparerByTotalHighSpeed : IComparer<Racer_Detail>
    {
        public int Compare(Racer_Detail Racer1, Racer_Detail Racer2)
        {
            return -((IComparable)Racer1.RacerSumOfSpeeds).CompareTo(Racer2.RacerSumOfSpeeds);
        }
    }
}
