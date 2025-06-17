using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SqlAgentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SalesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("monthly-summary")]
        public IActionResult GetMonthlySalesSummary(string month, string year, string mailId)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[db_datareader].[PROC_AI_Sales_Specificmonth]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MonthName", month);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@MailId", mailId);

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        var result = new List<Dictionary<string, object>>();

                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            result.Add(row);
                        }

                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }
    }
}
