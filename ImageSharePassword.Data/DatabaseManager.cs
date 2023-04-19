using System.Data.SqlClient;
using System.Xml.Linq;

namespace ImageSharePassword.Data
{
    public class DatabaseManager
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=ImageSharePassword;Integrated Security=true;";
        public int UploadImage(string imageName, string imagePath, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Images VALUES (@imageName, @imagePath, @password, 1) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@imageName", imageName);
            cmd.Parameters.AddWithValue("@imagePath", imagePath);
            cmd.Parameters.AddWithValue("@password", password);
            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }
        public Image GetImageById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM  Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            Image img = new Image();
            while (reader.Read())
            {
                img.Id = (int)reader["Id"];
                img.ImageName = (string)reader["ImageName"];
                img.ImagePath = (string)reader["ImagePath"];
                img.Password = (string)reader["Password"];
                img.Views = (int)reader["Views"];
            }
            return img;
        }
        public void IncrementViewsById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
    }
}