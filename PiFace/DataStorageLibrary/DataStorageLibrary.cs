using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace DataStorageLibrary
{
    public static class DataStorageLibrary
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS  CycleMachineConfiguration ( Id INTEGER PRIMARY KEY AUTOINCREMENT, MachineId INTEGER NOT NULL, MachineName TEXT, CycleInputPin INTEGER NOT NULL, MachineStateTimeout INTEGER NOT NULL, PublishingIntervall INTEGER NOT NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                   "EXISTS   MachineStateHistory ( Id INTEGER PRIMARY KEY AUTOINCREMENT, MachineId INTEGER NOT NULL, MachineState TEXT NOT NULL, StartDateTime TEXT NOT NULL, EndDateTime TEXT NOT NULL, Duration TEXT NOT NULL, DailyCycleCounter INTEGER NOT NULL, CyclesInThisPeriod INTEGER NOT NULL )";

                createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();


            }

        }



        public static void AddDataMachineStateHistory(Dictionary<string, string> stateEntity)
        {
            MachineStateHistoryEntity m = new MachineStateHistoryEntity {

                DailyCycleCoutner = Convert.ToInt32(stateEntity.FirstOrDefault(s => s.Key == "DailyCycleCounter").Value),
                MachineId = Convert.ToInt32(stateEntity.FirstOrDefault(s => s.Key == "MachineId").Value),
                CyclesInThisPeriod = Convert.ToInt32(stateEntity.FirstOrDefault(s => s.Key == "CyclesInThisPeriod").Value),
                Duration = stateEntity.FirstOrDefault(s => s.Key == "Duration").Value,
                StartDateTime = stateEntity.FirstOrDefault(s => s.Key == "StartDateTime").Value,
                EndDateTime = stateEntity.FirstOrDefault(s => s.Key == "EndDateTime").Value,
                MachineState = stateEntity.FirstOrDefault(s => s.Key == "MachineState").Value
            };




            using (SqliteConnection db =
                new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MachineStateHistory VALUES (NULL, @MachineId, @MachineState, @StartDateTime, @EndDateTime, @Duration, @DailyCycleCounter, @CyclesInThisPeriod);";

                insertCommand.Parameters.AddWithValue("@MachineId", m.MachineId);
                insertCommand.Parameters.AddWithValue("@MachineState", m.MachineState);
                insertCommand.Parameters.AddWithValue("@StartDateTime", m.StartDateTime);
                insertCommand.Parameters.AddWithValue("@EndDateTime", m.EndDateTime);
                insertCommand.Parameters.AddWithValue("@Duration", m.Duration);
                insertCommand.Parameters.AddWithValue("@DailyCycleCounter", m.DailyCycleCoutner);
                insertCommand.Parameters.AddWithValue("@CyclesInThisPeriod", m.CyclesInThisPeriod);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }





        public static void SignSmartAgent(int smartAgentIdentificationId)
        {
  


            using (SqliteConnection db =
                new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();


                SqliteCommand deleteAllCommand = new SqliteCommand();
                deleteAllCommand.Connection = db;

                deleteAllCommand.CommandText = "delete from SmartAgentIdentification";
                deleteAllCommand.ExecuteReader();


                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO SmartAgentIdentification VALUES (@SmartAgentIdentificationId);";

                insertCommand.Parameters.AddWithValue("@SmartAgentIdentificationId", smartAgentIdentificationId);


                insertCommand.ExecuteReader();

                db.Close();
            }

        }


        public static List<Dictionary <string, string>> GetCycleMachineConfigurations ()
        {

            var cycleMachineConfigurations = new List<Dictionary<string, string>>();


            using (SqliteConnection db =
    new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();


                SqliteCommand givAllCycleMachineConfigs = new SqliteCommand();
                givAllCycleMachineConfigs.Connection = db;

                givAllCycleMachineConfigs.CommandText = "select *from CycleMachineConfiguration";
              SqliteDataReader reader =  givAllCycleMachineConfigs.ExecuteReader();


                while (reader.Read())
                {
                    cycleMachineConfigurations.Add(new Dictionary<string, string>
                    {
                        {   "MachineId", reader["MachineId"].ToString() },
                        {   "MachineName", reader["MachineName"].ToString()},
                        {   "CycleInputPin", reader["CycleInputPin"].ToString() },
                        {   "MachineStateTimeout", reader["MachineStateTimeout"].ToString() },
                        {   "PublishingIntervall", reader["PublishingIntervall"].ToString() }

                });

                }
                db.Close();
                return cycleMachineConfigurations;

                
            }



        }












        public static List<Dictionary<string, string>> GetInputMonitoringConfigurations()
        {

            var monitoringConfigurations = new List<Dictionary<string, string>>();


    //        using (SqliteConnection db =
    //new SqliteConnection("Filename=SmartAgent.db"))
    //        {
    //            db.Open();


    //            SqliteCommand givAllMonitoringConfigs = new SqliteCommand();
    //            givAllMonitoringConfigs.Connection = db;

    //            givAllMonitoringConfigs.CommandText = "select *from InputMonitoringConfiguration";
    //            SqliteDataReader reader = givAllMonitoringConfigs.ExecuteReader();


    //            while (reader.Read())
    //            {
    //                monitoringConfigurations.Add(new Dictionary<string, string>
    //                {
    //                    {   "InputPin", reader["InputPin"].ToString() },
    //                    {   "OutputPin", reader["OutputPin"].ToString()}

    //            });

    //            }
    //            db.Close();
                return monitoringConfigurations;
            //}
        }



















        public static void StoreCycleMachineConfigurations(List<CycleMachineConfiguration> cycleMachinesConfigurations)
        {

         using (SqliteConnection db =
         new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                SqliteCommand deleteAllCommand = new SqliteCommand();
                deleteAllCommand.Connection = db;

                deleteAllCommand.CommandText = "delete from CycleMachineConfiguration";
                deleteAllCommand.ExecuteReader();

               


                    foreach (CycleMachineConfiguration config in cycleMachinesConfigurations)
                    {

                        SqliteCommand insertCommand = new SqliteCommand();
                        insertCommand.Connection = db;
                        insertCommand.CommandText = "INSERT INTO CycleMachineConfiguration VALUES (NULL, @MachineId, @MachineName, @CycleInputPin, @MachineStateTimeout, @PublishingIntervall);";

                            insertCommand.Parameters.AddWithValue("@MachineId", config.MachineId);
                            insertCommand.Parameters.AddWithValue("@MachineName", config.MachineName);
                            insertCommand.Parameters.AddWithValue("@CycleInputPin", config.CycleInputPin);
                            insertCommand.Parameters.AddWithValue("@MachineStateTimeout", config.MachineStateTimeOut);
                            insertCommand.Parameters.AddWithValue("@PublishingIntervall", config.PublishingIntervall);

                            insertCommand.ExecuteReader();
                        
                    }


                db.Close();
            }

        }



        public static void StoreInputMonitoringConfigurations(List<Dictionary<string, string>> inputMonitoringConfigurations)
        {
            using (SqliteConnection db =
         new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                SqliteCommand deleteAllCommand = new SqliteCommand();
                deleteAllCommand.Connection = db;

                deleteAllCommand.CommandText = "delete from InputMonitoringConfiguration";
                deleteAllCommand.ExecuteReader();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO InputMonitoringConfiguration VALUES (NULL, @InputPin, @OutputPin);";


                foreach (Dictionary<string, string> monitoringConfig in inputMonitoringConfigurations)
                {

                    var config = new InputMonitoringConfiguration
                    {
                        InputPin = Convert.ToInt32(monitoringConfig.FirstOrDefault(c => c.Key == "InputPin").Value),
                        OutputPin = Convert.ToInt32(monitoringConfig.FirstOrDefault(c => c.Key == "OutputPin").Value),
                    };

                    insertCommand.Parameters.AddWithValue("@InputPin", config.InputPin);
                    insertCommand.Parameters.AddWithValue("@OutputPin", config.OutputPin);
 
                    insertCommand.ExecuteReader();
                }

                db.Close();
            }

        }













    }
}

