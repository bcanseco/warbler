using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Warbler.Infrastructure.Chat.Interfaces;
using Warbler.Infrastructure.Chat.Models;

namespace Warbler.Infrastructure.Chat.Repositories
{
    public class SqlUniversityRepository : IUniversityRepository
    {
        private string Database { get; }

        public SqlUniversityRepository(string database)
        {
            Database = database;
        }
        
        public async Task<List<University>> GetUniversities()
        {
            var universities = new List<University>();
            using (var dbConnection = new MySqlConnection(Database))
            {
                await dbConnection.OpenAsync();

                const string sql =
                    @"SELECT university.id,
                             university.name,
                             university.lat,
                             university.lng,
                             server.auth_enabled,
                             server.type
                      FROM server, university
                      WHERE server.id = university.id";

                var request = new MySqlCommand(sql, dbConnection)
                {
                    CommandTimeout = 5
                };

                var response = await request.ExecuteReaderAsync();

                while (await response.ReadAsync())
                {
                    var server = new Server(response.GetInt32(0),
                        response.GetBoolean(4), response.GetInt32(5));

                    universities.Add(new University(
                        server.Id,
                        response.GetString(1),
                        response.GetFloat(2),
                        response.GetFloat(3),
                        server));
                }

                await dbConnection.CloseAsync();
            }
            return universities;
        }
    }
}
