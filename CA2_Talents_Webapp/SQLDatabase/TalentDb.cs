using CA2_Talents_Webapp.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Talents_Webapp.SQLDatabase
{
    public class TalentDb
    {
        private IConfiguration configuration;

        public TalentDb(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // GET ALL
        public List<Talent> getAllTalents()
        {
            try
            {
                List<Talent> talents = new List<Talent>();

                // Step #1 - Connect to the DB
                string connStr = configuration.GetConnectionString("MyConnStr");
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Step #2 - Create a command
                    string query = "SELECT * FROM dbo.Talent;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Step #3 - query the DB
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Talent talent = new Talent();
                                talent.TalentId = Convert.ToInt32(reader["talentId"].ToString());
                                talent.TalentName = reader["talentName"].ToString();
                                talent.TalentTitle = reader["talentTitle"].ToString();
                                talent.TalentDesc = reader["talentDesc"].ToString();
                                talent.ImageURL = reader["imageURL"].ToString();
                                talent.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                                talent.CreatedBy = reader["createdBy"].ToString();
                                talent.UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString());
                                talent.UpdatedBy = reader["updatedBy"].ToString();
                                
                                talents.Add(talent);
                            }
                        }
                    }

                    // Step #4 - close the connection
                    conn.Close();
                }

                return talents;
            }
            catch (Exception ex)
            {
                Console.WriteLine("--INTERNAL ERROR GETTING TALENT BY ID!!--, " + ex);
                List<Talent> talents = new List<Talent>(); // will be null
                return talents;
            }
        }

        // GET
        public Talent getTalentById(int talentId)
        {
            try
            {
                Talent talent = new Talent();

                // Step #1 - Connect to the DB
                string connStr = configuration.GetConnectionString("MyConnStr");
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                // Step #2 - Create a command
                string query = "SELECT * FROM dbo.Talent WHERE TalentId = @talentId;";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@talentId", talentId);

                // Step #3 - query the DB
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();

                talent.TalentId = talentId;
                talent.TalentName = reader["talentName"].ToString();
                talent.TalentTitle = reader["talentTitle"].ToString();
                talent.TalentDesc = reader["talentDesc"].ToString();
                talent.ImageURL = reader["imageURL"].ToString();
                talent.CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString());
                talent.CreatedBy = reader["createdBy"].ToString();
                talent.UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString());
                talent.UpdatedBy = reader["updatedBy"].ToString();

                // Step #4 - close the connection
                conn.Close();
                return talent;
            }
            catch (Exception ex)
            {
                Console.WriteLine("--INTERNAL ERROR GETTING TALENT BY ID!!--, " + ex);
                Talent talent = new Talent(); // will be null
                return talent;
            }
        }

        // INSERT
        public string addTalent(Talent talent)
        {
            try
            {
                // Step #1 - Connect to the DB
                string connStr = configuration.GetConnectionString("MyConnStr");
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                // Step #2 - Create a command
                string query = "INSERT INTO [dbo].[Talent]([TalentName],[TalentTitle],[TalentDesc],[ImageURL],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) " +
                    "VALUES " +
                    "(@talentName, @talentTitle, @talentDesc, @imageURL, GETDATE(), @createdBy, GETDATE(), @updatedBy);";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@talentName", talent.TalentName);
                cmd.Parameters.AddWithValue("@talentTitle", talent.TalentTitle);
                cmd.Parameters.AddWithValue("@talentDesc", talent.TalentDesc);
                cmd.Parameters.AddWithValue("@imageURL", talent.ImageURL);
                cmd.Parameters.AddWithValue("@createdBy", talent.CreatedBy);
                cmd.Parameters.AddWithValue("@updatedBy", talent.UpdatedBy);

                // Step #3 - query the DB
                cmd.ExecuteReader();

                // Step #4 - close the connection
                conn.Close();
                return "success";
            } catch (Exception ex)
            {
                Console.WriteLine("--INTERNAL ERROR ADDING TALENT!!--, " + ex);
                return "failed";
            }
        }

        // UPDATE
        public string editTalent(Talent talent)
        {
            try
            {
                // Step #1 - Connect to the DB
                string connStr = configuration.GetConnectionString("MyConnStr");
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                // Step #2 - Create a command
                string query = "UPDATE [dbo].[Talent] " +
                "SET [TalentName] = @talentName, [TalentTitle] = @talentTitle, [TalentDesc] = @talentDesc, [ImageURL] = @imageURL, [UpdatedDate] = GETDATE(), [UpdatedBy] = @updatedBy " +
                "WHERE [TalentId] = @talentId;";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@talentName", talent.TalentName);
                cmd.Parameters.AddWithValue("@talentTitle", talent.TalentTitle);
                cmd.Parameters.AddWithValue("@talentDesc", talent.TalentDesc);
                cmd.Parameters.AddWithValue("@imageURL", talent.ImageURL);
                cmd.Parameters.AddWithValue("@updatedBy", talent.UpdatedBy);
                cmd.Parameters.AddWithValue("@talentId", talent.TalentId);

                // Step #3 - query the DB
                // ExecuteNonQuery : ExecuteNonQuery used for executing queries that does not return any data. 
                // It is used to execute the sql statements like update, insert, delete etc. 
                // ExecuteNonQuery executes the command and returns the number of rows affected.
                cmd.ExecuteNonQuery();

                // Step #4 - close the connection
                conn.Close();
                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine("--INTERNAL ERROR ADDING TALENT!!--, " + ex);
                return "failed";
            }
        }

        // DELETE
        public string deleteTalent(int talentId)
        {
            try
            {
                // Step #1 - Connect to the DB
                string connStr = configuration.GetConnectionString("MyConnStr");
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                // Step #2 - Create a command
                string query = "DELETE FROM [dbo].[Talent] WHERE [TalentId] = @talentId;";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@talentId", talentId);

                // Step #3 - query the DB
                // ExecuteNonQuery : ExecuteNonQuery used for executing queries that does not return any data. 
                // It is used to execute the sql statements like update, insert, delete etc. 
                // ExecuteNonQuery executes the command and returns the number of rows affected.
                cmd.ExecuteNonQuery();

                // Step #4 - close the connection
                conn.Close();
                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine("--INTERNAL ERROR ADDING TALENT!!--, " + ex);
                return "failed";
            }
        }


    }
}
