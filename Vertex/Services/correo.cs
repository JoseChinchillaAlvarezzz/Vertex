using Microsoft.Data.SqlClient;

namespace Vertex.Services
{
    public class correo
    {
        private IConfiguration _configuration;

        public correo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void enviar(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                string connectionString = _configuration.GetSection("ConnectionStrings").GetSection("ticketsDbConnection").Value;

                string Query = @"EXEC msdb.dbo.sp_send_dbmail 
                            @profile_name = 'Soporte de Vertex', 
                            @recipients = @par_destinatarios, 
                            @subject = @par_asunto, 
                            @body = @par_mensaje,
                            @body_format = 'HTML'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@par_destinatarios", destinatario);
                        command.Parameters.AddWithValue("@par_asunto", asunto);
                        command.Parameters.AddWithValue("@par_mensaje", cuerpo);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.Message);
            }
        }

    }
}
