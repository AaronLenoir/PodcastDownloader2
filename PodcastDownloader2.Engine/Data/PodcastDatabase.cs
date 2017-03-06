using System.Data.SQLite;

namespace PodcastDownloader2.Engine.Data
{
    public class PodcastDatabase
    {
        private string _path;

        public static PodcastDatabase CreateOrOpen(string path)
        {
            return new PodcastDatabase(path);
        }

        private PodcastDatabase(string path)
        {
            _path = path;

            CreateDbIfRequired();
        }

        public bool ContainsId(string id)
        {
            using (var conn = OpenConnection())
            {
                var query = "SELECT 1 FROM EpisodesDownloaded WHERE id = @id";
                var idParameter = new SQLiteParameter("id", id);
                var cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.Parameters.Add(idParameter);
                var result = cmd.ExecuteScalar();

                return !(result == null);
            }
        }

        public void AddId(string id)
        {
            using (var conn = OpenConnection())
            {
                var query = "INSERT INTO EpisodesDownloaded (id) VALUES (@id)";
                var idParameter = new SQLiteParameter("id", id);
                var cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.Parameters.Add(idParameter);
                var result = cmd.ExecuteNonQuery();
            }
        }

        public void RemoveId(string id)
        {
            using (var conn = OpenConnection())
            {
                var query = "DELETE FROM EpisodesDownloaded WHERE id = @id";
                var idParameter = new SQLiteParameter("id", id);
                var cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.Parameters.Add(idParameter);
                var result = cmd.ExecuteNonQuery();
            }
        }

        private void CreateDbIfRequired()
        {
            using (var conn = OpenConnection())
            {
                var query = "CREATE TABLE IF NOT EXISTS EpisodesDownloaded (id)";
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private SQLiteConnection OpenConnection()
        {
            var conn = new SQLiteConnection($"Data Source={_path};Version=3;");
            conn.Open();
            return conn;
        }
    }
}
