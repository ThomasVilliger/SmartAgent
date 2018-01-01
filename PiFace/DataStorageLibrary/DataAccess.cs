using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace DataStorageLibrary
{
    public static class DataAccess
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS  Machine ( Id INTEGER PRIMARY KEY AUTOINCREMENT, MachineId INTEGER NOT NULL, MachineName TEXT, CycleInputPin INTEGER NOT NULL, MachineStateTimeout INTEGER NOT NULL, PublishingIntervall INTEGER NOT NULL)";
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
"EXISTS  InputMonitoring ( Id INTEGER PRIMARY KEY AUTOINCREMENT, InputPin INTEGER NOT NULL, OutputPin INTEGER NOT NULL)";
                createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                "EXISTS  SmartAgentIdentification (SmartAgentId INTEGER NOT NULL)";
                 createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                "EXISTS  GatewayHub ( Id INTEGER PRIMARY KEY AUTOINCREMENT, Priority INTEGER NOT NULL, IpAddress TEXT NOT NULL)";
                createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();


                tableCommand = "CREATE TABLE IF NOT " +
                   "EXISTS   MachineStateHistory ( Id INTEGER PRIMARY KEY AUTOINCREMENT, MachineId INTEGER NOT NULL, MachineState INT NOT NULL, StartDateTime TEXT NOT NULL, EndDateTime TEXT NOT NULL, Duration TEXT NOT NULL, DailyCycleCounter INTEGER NOT NULL, CyclesInThisPeriod INTEGER NOT NULL )";

                createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                db.Close();
            }

        }



        public static void AddDataMachineStateHistory(MachineStateHistory m)
        {
           


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
                insertCommand.Parameters.AddWithValue("@StartDateTime", m.StartDateTime.ToString());
                insertCommand.Parameters.AddWithValue("@EndDateTime", m.EndDateTime.ToString());
                insertCommand.Parameters.AddWithValue("@Duration", m.Duration.ToString());
                insertCommand.Parameters.AddWithValue("@DailyCycleCounter", m.DailyCycleCounter);
                insertCommand.Parameters.AddWithValue("@CyclesInThisPeriod", m.CyclesInThisPeriod);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }





        public static void SignSmartAgent(int smartAgentId)
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
                insertCommand.CommandText = "INSERT INTO SmartAgentIdentification VALUES (@SmartAgentId);";

                insertCommand.Parameters.AddWithValue("@SmartAgentId", smartAgentId);


                insertCommand.ExecuteReader();

                db.Close();
            }

        }


        public static List<MachineConfiguration> GetMachineConfigurations ()
        {

            var configs = new List<MachineConfiguration>();


            using (SqliteConnection db =
    new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();


                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "select *from Machine";
              SqliteDataReader reader =  command.ExecuteReader();


                while (reader.Read())
                {
                    configs.Add(new MachineConfiguration
                    {                  
                       MachineId= Convert.ToInt32(reader["MachineId"]),
                       MachineName = reader["MachineName"].ToString(),
                       CycleInputPin=Convert.ToInt32(reader["CycleInputPin"]),
                       MachineStateTimeout= Convert.ToInt32(reader["MachineStateTimeout"]),
                       PublishingIntervall= Convert.ToInt32(reader["PublishingIntervall"])

                });

                }
                db.Close();
                return configs;

                
            }
        }



        public static List<MachineStateHistory> GetMachineStateHistoryData(int lastHistoryNumber)
        {

            var historyData = new List<MachineStateHistory>();


            using (SqliteConnection db =
    new SqliteConnection("Filename=SmartAgent.db"))
            {

                try
                {
                    db.Open();


                    SqliteCommand command = new SqliteCommand();
                    command.Connection = db;

                    command.CommandText = "select *from MachineStateHistory where id>" + lastHistoryNumber;
                    SqliteDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        historyData.Add(new MachineStateHistory
                        {
                            SmartAgentHistoryId = Convert.ToInt32(reader["Id"]),
                            MachineId = Convert.ToInt32(reader["MachineId"]),
                            MachineState = (MachineStateHistory.State)  Convert.ToInt32(reader["MachineState"]),
                            StartDateTime = Convert.ToDateTime(reader["StartDateTime"].ToString()),
                            EndDateTime = Convert.ToDateTime(reader["EndDateTime"].ToString()),
                            CyclesInThisPeriod = Convert.ToInt32(reader["CyclesInThisPeriod"]),
                            DailyCycleCounter = Convert.ToInt32(reader["DailyCycleCounter"])
                        });


                        //string startDateTime = (reader["StartDateTime"]);
                    }

                }

                catch(Exception ex)
                {
                    throw ex;
                }


                finally
                {
                    db.Close();
                }

                return historyData;
            }
        }












        public static List<InputMonitoringConfiguration> GetInputMonitoringConfigurations()
        {

            var configs = new List<InputMonitoringConfiguration>();


            using (SqliteConnection db =
    new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();


                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "select *from InputMonitoring";
                SqliteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    configs.Add(new InputMonitoringConfiguration
                    {
                        InputPin = Convert.ToInt32(reader["InputPin"]),
                        OutputPin = Convert.ToInt32(reader["OutputPin"])

                    });

                }
                db.Close();
                return configs;


            }
        }


        public static int GetSmartAgentId()
        {


            int smartAgentId = 0;

            using (SqliteConnection db =
    new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();


                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "select *from SmartAgentIdentification";
                SqliteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    smartAgentId = Convert.ToInt32(reader["SmartAgentId"]);
                }
                db.Close();
                return smartAgentId;
            }
        }


















        public static void MachinesConfigurations(List<MachineConfiguration> machinesConfigurations)
        {

         using (SqliteConnection db =
         new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                SqliteCommand deleteAllCommand = new SqliteCommand();
                deleteAllCommand.Connection = db;

                deleteAllCommand.CommandText = "delete from Machine";
                deleteAllCommand.ExecuteReader();

               


                    foreach (MachineConfiguration config in machinesConfigurations)
                    {

                        SqliteCommand insertCommand = new SqliteCommand();
                        insertCommand.Connection = db;
                        insertCommand.CommandText = "INSERT INTO Machine VALUES (NULL, @MachineId, @MachineName, @CycleInputPin, @MachineStateTimeout, @PublishingIntervall);";

                            insertCommand.Parameters.AddWithValue("@MachineId", config.MachineId);
                            insertCommand.Parameters.AddWithValue("@MachineName", config.MachineName);
                            insertCommand.Parameters.AddWithValue("@CycleInputPin", config.CycleInputPin);
                            insertCommand.Parameters.AddWithValue("@MachineStateTimeout", config.MachineStateTimeout);
                            insertCommand.Parameters.AddWithValue("@PublishingIntervall", config.PublishingIntervall);

                            insertCommand.ExecuteReader();
                        
                    }


                db.Close();
            }

        }



        public static void StoreInputMonitoringConfigurations(List<InputMonitoringConfiguration> inputMonitoringConfigurations)
        {
            using (SqliteConnection db =
         new SqliteConnection("Filename=SmartAgent.db"))
            {
                db.Open();

                SqliteCommand deleteAllCommand = new SqliteCommand();
                deleteAllCommand.Connection = db;

                deleteAllCommand.CommandText = "delete from InputMonitoring";
                deleteAllCommand.ExecuteReader();

               


                foreach (InputMonitoringConfiguration config in inputMonitoringConfigurations)
                {
                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    insertCommand.CommandText = "INSERT INTO InputMonitoring VALUES (NULL, @InputPin, @OutputPin);";


                    insertCommand.Parameters.AddWithValue("@InputPin", config.InputPin);
                    insertCommand.Parameters.AddWithValue("@OutputPin", config.OutputPin);
 
                    insertCommand.ExecuteReader();
                }

                db.Close();
            }

        }



















    }
}

