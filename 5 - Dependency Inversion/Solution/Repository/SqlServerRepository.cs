using DIP.Solution.Interfaces;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DIP.Solution
{
    public class SqlServerRepository : IRepository
    {
        public void Save(Person person)
        {
            string connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";

            string sql = "insert into Person(name) values (@name)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
