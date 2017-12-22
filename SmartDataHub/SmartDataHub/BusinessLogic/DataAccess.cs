using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDataHub
{
    public static class DataAccess
    {

        private static string _connectionString = Properties.Resources.DbConnectionString;


        public static List<Dictionary<string, string>> GetCycleMachineConfigurations(int smartAgentId)
        {

            var cycleMachineConfigurations = new List<Dictionary<string, string>>();

          

            using
            (SqlConnection db = new SqlConnection(_connectionString))
            {

                db.Open();


                SqlCommand givAllCycleMachineConfigs = new SqlCommand();
                givAllCycleMachineConfigs.Connection = db;

                givAllCycleMachineConfigs.CommandText = String.Format("select *from CycleMachineConfiguration where SmartAgentId={0} and Active='true'", smartAgentId);
                SqlDataReader reader = givAllCycleMachineConfigs.ExecuteReader();

                try
                {

                    while (reader.Read())
                    {

                        cycleMachineConfigurations.Add(new Dictionary<string, string>
                    {
                        {   "MachineId", reader["CycleMachineConfigurationId"].ToString() },
                        {   "MachineName", reader["MachineName"].ToString()},
                        {   "CycleInputPin", reader["CycleInputPin"].ToString() },
                        {   "MachineStateTimeout", reader["MachineStateTimeout"].ToString() },
                        {   "PublishingIntervall", reader["PublishingIntervall"].ToString() }

                });


                    }


                }


                catch (Exception ex)
                {

                }
                db.Close();
                return cycleMachineConfigurations;
            }

            
        }



        public static List<Dictionary<string, string>> GetInputMonitoringConfigurations(int smartAgentId)
        {

            var monitoringConfigurations = new List<Dictionary<string, string>>();
            

            using
           (SqlConnection db = new SqlConnection(_connectionString))
            {

                db.Open();


                SqlCommand givAllMonitoringConfigs = new SqlCommand();
                givAllMonitoringConfigs.Connection = db;

                givAllMonitoringConfigs.CommandText = String.Format("select *from InputMonitoringConfiguration where SmartAgentId={0} and Active='true'", smartAgentId);
                SqlDataReader reader = givAllMonitoringConfigs.ExecuteReader();


                while (reader.Read())
                {

                        monitoringConfigurations.Add(new Dictionary<string, string>
                    {
                        {   "InputPin", reader["InputPin"].ToString() },
                        {   "OutputPin", reader["OutputPin"].ToString()}

                });

                }
                db.Close();
                return monitoringConfigurations;

            }
        }
    }
}
