namespace OCP.Problem
{
    public class CheckService
    {
        public void Save(Check check)
        {
            if (check.Type == CheckTypeEnum.IN)
            {         
                //DO SOMETHING
            }
            else if (check.Type == CheckTypeEnum.OUT)
            {
                //DO SOMETHING ELSE
            }
        }
    }
}
