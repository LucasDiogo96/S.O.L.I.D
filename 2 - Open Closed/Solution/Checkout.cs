using System;

namespace OCP.Solution
{
    public class Checkout : CheckService
    {
        public override void RealizarCheck(Check check)
        {
            //Verify if it has't a checkin without checkout and other business logics

            //Verify if hasen't conflicted with the previous checkin timestamp

            //Populate checkout data into visit

            //save
        }
    }
}
