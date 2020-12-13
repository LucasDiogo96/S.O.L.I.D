namespace DIP.Problem
{
    public class Business
    {
        public void Save(Person person)
        {
            // Here we are using the sql server repository but if we need to change, we need to 
            // modify it and do it in each class that depends of SqlServerRepository

            //MongoDBRepository persistence = new MongoDBRepository();

            SqlServerRepository persistence = new SqlServerRepository();

            persistence.Save(person);
        }
    }

}
