using DataAccess.Model;
using DataAccess.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Utility;
using ViewModel;
using Timer = System.Timers.Timer;

namespace DataAccess
{
    public class DB
    {
        SQLiteConnection sqlite_conn;
        private long UnixTime { get { return ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds(); } }
        public DB(bool checkExistDB = true)
        {
            return;
            //InitDB();
            CopyDB();
            sqlite_conn = CreateConnection();
            qt = new Queue<List<BlenderReceiveDataViewModel>>();
            qtECG = new Queue<ECGReceiveDataViewModel[]>();
        }
        private void CopyDB()
        {
            try
            {
                string basePath = "C:\\ParsaTeb";
                string fileName = "Database.db";
                string fullPath = basePath + "\\" + fileName;
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);
                if (!File.Exists(fullPath))
                    File.Copy(Environment.CurrentDirectory + "\\App_Data\\" + fileName, fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public void InitDB()
        {
            CreateTable(sqlite_conn);
            InitSettings();
            RemoveOldData();
            StartCheckDataTimer();
            InsertToDBFromQueue();
            InsertECGToDBFromQueue();
        }
        public SQLiteConnection CreateConnection()
        {
            return null;
            // Create a new database connection:
            var sqlite_conn = new SQLiteConnection("Data Source=C:\\ParsaTeb\\Database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error1 {0}", ex);
            }
            return sqlite_conn;
        }
        public void CreateTable(SQLiteConnection conn)
        {
            return;
            try
            {
                string tableName = "DataList";

                string Createsql = "CREATE TABLE DataList(Id INTEGER PRIMARY KEY,OximeterValue INTEGER,FlowValue REAL,SPo2 INTEGER,HR INTEGER,ExtraData TEXT,CreateDateTime DATETIME,ElapsedMilliseconds NUMBER,PatientId varchar,Oxygen REAL)";
                CreateTableCommand(conn, tableName, Createsql);


                #region UserActivity
                tableName = "UserActivity";
                Createsql = $"CREATE TABLE {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT,BusinessDate datetime,CreateDateTime datetime,UserId varchar,UserName varchar,ActionId int,ActionName varchar,OtherInfo text)";
                string Createsql2 = $"CREATE INDEX Index_BU ON {tableName}(BusinessDate);";
                CreateTableCommand(conn, tableName, Createsql, Createsql2);
                #endregion

                #region Settings
                tableName = "Settings";
                Createsql = $"CREATE TABLE {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT,BusinessDate varchar,UpdateDateTime datetime,UserId varchar,UserName varchar,ParameterId int,ParameterName varchar,ParameterValue varchar)";
                CreateTableCommand(conn, tableName, Createsql);
                #endregion

                #region Patient
                tableName = "Patient";
                Createsql = $"CREATE TABLE {tableName} (Id INTEGER PRIMARY KEY AUTOINCREMENT,CreateDateTime datetime,UpdateDateTime datetime,UserId varchar,UserName varchar," +
                    $"HospitalName varchar,Section varchar,Bed varchar,PatientName varchar,PatientId varchar,Gender varchar,Age varchar,Weight varchar)";
                Createsql2 = $"CREATE INDEX Index_BU ON {tableName}(PatientId);";
                CreateTableCommand(conn, tableName, Createsql, Createsql2);

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error2 {0}", ex);
            }

        }
        private void CreateTableCommand(SQLiteConnection conn, string tableName, params string[] Createsql)
        {
            return;
            SQLiteCommand sqlite_cmd;
            SQLiteDataReader res;
            bool isExistTable = false;
            try
            {
                string existCommand = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                using (sqlite_cmd = conn.CreateCommand())
                {
                    sqlite_cmd.CommandText = existCommand;
                    res = sqlite_cmd.ExecuteReader();
                    while (res.Read())
                    {
                        isExistTable = true;
                        break;
                    }
                    res.Close();
                    if (!isExistTable)
                    {
                        foreach (var cmd in Createsql)
                        {
                            sqlite_cmd.CommandText = cmd;
                            sqlite_cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error3 {0}", ex);
            }
        }
        public void ExecuteCommand(string command)
        {
            try
            {
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = command;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error4 {0}", ex);
            }

        }
        static Queue<List<BlenderReceiveDataViewModel>> qt;
        static Queue<ECGReceiveDataViewModel[]> qtECG;
        public void InsertData(List<BlenderReceiveDataViewModel> data)
        {
            return;
            if (!Config.Instance.SaveMode) return;
            Console.WriteLine(DateTime.Now + " InsertData");
            qt.Enqueue(data);
            return;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    CheckConnection();
                    //using (var transaction = sqlite_conn.BeginTransaction())
                    {
                        var command = sqlite_conn.CreateCommand();
                        command.CommandText = @"INSERT INTO DataList(CreateDateTime,ElapsedMilliseconds,OximeterValue,FlowValue,SPo2,HR,ExtraData,PatientId,Oxygen) VALUES (@CreateDateTime,@ElapsedMilliseconds,@OximeterValue,@FlowValue,@SPo2,@HR,@ExtraData,@PatientId,@Oxygen)";

                        command.CommandType = System.Data.CommandType.Text;
                        var tmp = qt.Dequeue();
                        foreach (var item in tmp)
                        {
                            try
                            {
                                command.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                                command.Parameters.AddWithValue("@ElapsedMilliseconds", item.ElapsedMilliseconds);
                                command.Parameters.AddWithValue("@ExtraData", item.ExtraData);
                                command.Parameters.AddWithValue("@OximeterValue", item.OximeterValue);
                                command.Parameters.AddWithValue("@FlowValue", item.FlowValue);
                                command.Parameters.AddWithValue("@SPo2", item.SPo2);
                                command.Parameters.AddWithValue("@HR", item.HR);
                                command.Parameters.AddWithValue("@PatientId", Config.Instance.PatientId);
                                command.Parameters.AddWithValue("@Oxygen", item.Oxygen);
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error30 {0}", ex);
                            }
                        }
                        //transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error5 {0}", ex);
                }
            });
        }
        public void InsertECGData(ECGReceiveDataViewModel[] data)
        {
            return;
            if (!Config.Instance.SaveMode) return;
            Console.WriteLine(DateTime.Now + " InsertDataECG");
            qtECG.Enqueue(data);
            return;
        }
        private void InsertECGToDBFromQueue()
        {
            return;
            Task.Factory.StartNew(async () =>
            {
                while (qtECG.Count > 0)
                {
                    try
                    {
                        CheckConnection();
                        using (var transaction = sqlite_conn.BeginTransaction())
                        {
                            lock (IdentityECG)
                            {
                                var command = sqlite_conn.CreateCommand();
                                command.CommandText = @"INSERT INTO ECGData(CreateDateTime,Channel1,Channel2,Channel3,Channel4,Channel5,Channel6,Channel7,Channel8,Channel9,Channel10,Channel11,Channel12,Channel13) VALUES 
                (@CreateDateTime,@Channel1,@Channel2,@Channel3,@Channel4,@Channel5,@Channel6,@Channel7,@Channel8,@Channel9,@Channel10,@Channel11,@Channel12,@Channel13)";

                                command.CommandType = System.Data.CommandType.Text;
                                var tmp = qtECG.Dequeue();
                                foreach (var item in tmp)
                                {
                                    try
                                    {
                                        command.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                                        command.Parameters.AddWithValue("@Channel1", item.Channel[0].value);
                                        command.Parameters.AddWithValue("@Channel2", item.Channel[1].value);
                                        command.Parameters.AddWithValue("@Channel3", item.Channel[2].value);
                                        command.Parameters.AddWithValue("@Channel4", item.Channel[3].value);
                                        command.Parameters.AddWithValue("@Channel5", item.Channel[4].value);
                                        command.Parameters.AddWithValue("@Channel6", item.Channel[5].value);
                                        command.Parameters.AddWithValue("@Channel7", item.Channel[6].value);
                                        command.Parameters.AddWithValue("@Channel8", item.Channel[7].value);
                                        command.Parameters.AddWithValue("@Channel9", item.Channel[8].value);
                                        command.Parameters.AddWithValue("@Channel10", item.Channel[9].value);
                                        command.Parameters.AddWithValue("@Channel11", item.Channel[10].value);
                                        command.Parameters.AddWithValue("@Channel12", item.Channel[11].value);
                                        command.Parameters.AddWithValue("@Channel13", item.Channel[12].value);

                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error40 {0}", ex);
                                    }
                                }
                                transaction.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error6 {0}", ex);
                    }
                }
                await Task.Delay(3000);
                InsertECGToDBFromQueue();
            });
        }
        static readonly object Identity = new object();
        static readonly object IdentityECG = new object();
        private void InsertToDBFromQueue()
        {
            return;
            Task.Factory.StartNew(async () =>
            {
                while (qt.Count > 0)
                {
                    try
                    {
                        CheckConnection();
                        using (var transaction = sqlite_conn.BeginTransaction())
                        {
                            lock (Identity)
                            {
                                var command = sqlite_conn.CreateCommand();
                                command.CommandText = @"INSERT INTO DataList(CreateDateTime,ElapsedMilliseconds,OximeterValue,FlowValue,SPo2,HR,ExtraData,PatientId,Oxygen) VALUES (@CreateDateTime,@ElapsedMilliseconds,@OximeterValue,@FlowValue,@SPo2,@HR,@ExtraData,@PatientId,@Oxygen)";

                                command.CommandType = System.Data.CommandType.Text;
                                var tmp = qt.Dequeue().ToArray();
                                foreach (var item in tmp)
                                {
                                    try
                                    {
                                        if (item.OximeterValue == 0)
                                        { }
                                        command.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                                        command.Parameters.AddWithValue("@ElapsedMilliseconds", item.ElapsedMilliseconds);
                                        command.Parameters.AddWithValue("@ExtraData", item.ExtraData);
                                        command.Parameters.AddWithValue("@OximeterValue", item.OximeterValue);
                                        command.Parameters.AddWithValue("@FlowValue", item.FlowValue);
                                        command.Parameters.AddWithValue("@SPo2", item.SPo2);
                                        command.Parameters.AddWithValue("@HR", item.HR);
                                        command.Parameters.AddWithValue("@PatientId", Config.Instance.PatientId);
                                        command.Parameters.AddWithValue("@Oxygen", item.Oxygen);
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error30 {0}", ex);
                                    }
                                }
                                transaction.Commit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error5 {0}", ex);
                    }
                }
                await Task.Delay(3000);
                InsertToDBFromQueue();
            });
        }

        readonly object _object = new object();
        public async Task<List<BlenderReceiveDataViewModel>> ReadData(int page = 0, int pageSize = 100, int lastId = 0)
        {
            return await Task.Factory.StartNew(() =>
            {
                var result = new List<BlenderReceiveDataViewModel>();
                try
                {
                    if (page < 0)
                        page = 0;
                    using (var conn = CreateConnection())
                    {
                        SQLiteDataReader sqlite_datareader;
                        SQLiteCommand sqlite_cmd;
                        sqlite_cmd = conn.CreateCommand();
                        var where = lastId > 0 ? $"where id<{lastId} " : "";
                        sqlite_cmd.CommandText = $"SELECT * FROM DataList {where} order by ElapsedMilliseconds DESC limit {pageSize} offset {page * pageSize}";

                        sqlite_datareader = sqlite_cmd.ExecuteReader();
                        while (sqlite_datareader.Read())
                        {
                            try
                            {
                                result.Add(new BlenderReceiveDataViewModel
                                {
                                    OximeterValue = sqlite_datareader.GetInt32(1),
                                    FlowValue = sqlite_datareader.GetFloat(2),
                                    SPo2 = sqlite_datareader.GetInt32(3),
                                    HR = sqlite_datareader.GetInt32(4),
                                    ExtraData = sqlite_datareader.GetValue(5)?.ToString(),
                                    CreateDateTime = sqlite_datareader.GetDateTime(6),
                                    ElapsedMilliseconds = sqlite_datareader.GetInt64(7),///Unix Time,
                                    Id = sqlite_datareader.GetInt32(0),
                                    Oxygen = sqlite_datareader.GetFloat(9)
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error8 {0}", ex);
                            }
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error6 {0}", ex);
                }
                return result;
            });
        }
        public List<BlenderReceiveDataViewModel> ReadDataSync(int page = 0, int pageSize = 100, int lastId = 0)
        {
            Console.WriteLine("ReadData: {0}", page);
            var result = new List<BlenderReceiveDataViewModel>();
            try
            {
                if (page < 0)
                    page = 0;
                using (var conn = CreateConnection())
                {
                    SQLiteDataReader sqlite_datareader;
                    SQLiteCommand sqlite_cmd;
                    sqlite_cmd = conn.CreateCommand();
                    var where = lastId > 0 ? $"where id<{lastId} " : "";
                    sqlite_cmd.CommandText = $"SELECT * FROM DataList {where} order by Id DESC limit {pageSize} offset {page * pageSize}";

                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        try
                        {
                            result.Add(new BlenderReceiveDataViewModel
                            {
                                OximeterValue = sqlite_datareader.GetInt32(1),
                                FlowValue = sqlite_datareader.GetFloat(2),
                                SPo2 = sqlite_datareader.GetInt32(3),
                                HR = sqlite_datareader.GetInt32(4),
                                ExtraData = sqlite_datareader.GetValue(5)?.ToString(),
                                CreateDateTime = sqlite_datareader.GetDateTime(6),
                                ElapsedMilliseconds = sqlite_datareader.GetInt64(7),///Unix Time,
                                Id = sqlite_datareader.GetInt32(0),
                                Oxygen = sqlite_datareader.GetFloat(9)
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error7 {0}", ex);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error9 {0}", ex);
            }
            return result;
        }
        public void RemoveOldData()
        {
            if (!Config.Instance.AutoRemoveOldData) return;
            //Task.Factory.StartNew(() =>
            {
                var result = new List<BlenderReceiveDataViewModel>();
                try
                {
                    using (var conn = CreateConnection())
                    {
                        SQLiteCommand sqlite_cmd;
                        sqlite_cmd = conn.CreateCommand();
                        var unixTimeOld = ((DateTimeOffset)DateTime.Now.AddDays(-2)).ToUnixTimeMilliseconds();
                        var now = DateTime.Now.AddDays(-2);
                        sqlite_cmd.CommandText = $"delete from DataList where ElapsedMilliseconds<{unixTimeOld}";

                        var res = sqlite_cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error10 {0}", ex);
                }
                //return result;
            }
            //);
        }

        #region UserActivity
        public void InsertUserActivityDataAsync(UserActivityDataModel item)
        {
            return;
            Task.Factory.StartNew(() =>
            {
                CheckConnection();
                using (var transaction = sqlite_conn.BeginTransaction())
                {
                    try
                    {
                        var command = sqlite_conn.CreateCommand();
                        command.CommandText = @"INSERT INTO UserActivity(BusinessDate,CreateDateTime,UserId,UserName,ActionId,ActionName,OtherInfo) VALUES (@BusinessDate,@CreateDateTime,@UserId,@UserName,@ActionId,@ActionName,@OtherInfo)";

                        command.CommandType = System.Data.CommandType.Text;
                        {
                            command.Parameters.AddWithValue("@BusinessDate", DateTime.Now.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                            command.Parameters.AddWithValue("@UserId", Config.Instance.CurrentUserId);
                            command.Parameters.AddWithValue("@UserName", Config.Instance.CurrentUserName);
                            command.Parameters.AddWithValue("@ActionId", item.ActionId);
                            command.Parameters.AddWithValue("@ActionName", item.ActionId.ToString());
                            command.Parameters.AddWithValue("@OtherInfo", item.OtherInfo);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error11 {0}", ex);
                    }
                }
            });

        }

        public async Task<List<UserActivityDataModel>> GetUserActivities(int page = 0, int pageSize = 50)
        {
            return null;
            return await Task.Factory.StartNew(() =>
            {
                var result = new List<UserActivityDataModel>();
                try
                {
                    using (var conn = CreateConnection())
                    {
                        SQLiteDataReader sqlite_datareader;
                        SQLiteCommand sqlite_cmd;
                        sqlite_cmd = conn.CreateCommand();
                        sqlite_cmd.CommandText = $"SELECT * FROM UserActivity order by id DESC limit {pageSize} offset {page * pageSize}";
                        sqlite_datareader = sqlite_cmd.ExecuteReader();
                        while (sqlite_datareader.Read())
                        {
                            try
                            {
                                result.Add(new UserActivityDataModel
                                {
                                    Id = sqlite_datareader.GetInt32(0),
                                    BusinessDate = sqlite_datareader.GetString(1),
                                    CreateDate = sqlite_datareader.GetDateTime(2),
                                    UserId = sqlite_datareader.GetValue(3)?.ToString(),
                                    UserName = sqlite_datareader.GetValue(4)?.ToString(),
                                    ActionId = (ActionType)sqlite_datareader.GetInt32(5),
                                    ActionName = sqlite_datareader.GetString(6),
                                    OtherInfo = sqlite_datareader.GetValue(7)?.ToString()
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error25 {0}", ex);
                            }
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error24 {0}", ex);
                }
                return result;
            });
        }
        public async Task<List<UserActivityDataModel>> GetErrorLogs(int page = 0, int pageSize = 50)
        {
            return null;
            return await Task.Factory.StartNew(() =>
            {
                var result = new List<UserActivityDataModel>();
                try
                {
                    if (page < 0)
                        page = 0;
                    using (var conn = CreateConnection())
                    {
                        SQLiteDataReader sqlite_datareader;
                        SQLiteCommand sqlite_cmd;
                        sqlite_cmd = conn.CreateCommand();
                        sqlite_cmd.CommandText = $"select * from UserActivity WHERE ActionId>101 order by id DESC limit {pageSize} offset {page * pageSize}";

                        sqlite_datareader = sqlite_cmd.ExecuteReader();
                        while (sqlite_datareader.Read())
                        {
                            try
                            {
                                result.Add(new UserActivityDataModel
                                {
                                    Id = sqlite_datareader.GetInt32(0),
                                    ActionId = (ActionType)sqlite_datareader.GetInt32(5),
                                    ActionName = sqlite_datareader.GetString(6),
                                    OtherInfo = sqlite_datareader.GetValue(7)?.ToString(),
                                    CreateDate = sqlite_datareader.GetDateTime(2),
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error23 {0}", ex);
                            }
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error22 {0}", ex);
                }
                return result;
            });
        }
        #endregion

        #region Settings
        public void InsertSettingsDataAsync(SettingsDataModel item)
        {
            return;
            CheckConnection();
            using (var transaction = sqlite_conn.BeginTransaction())
            {
                try
                {
                    var command = sqlite_conn.CreateCommand();
                    command.CommandText = @"INSERT INTO Settings(BusinessDate,UpdateDateTime,UserId,UserName,ParameterId,ParameterName,ParameterValue) VALUES (@BusinessDate,@UpdateDateTime,@UserId,@UserName,@ParameterId,@ParameterName,@ParameterValue)";

                    command.CommandType = System.Data.CommandType.Text;
                    {
                        command.Parameters.AddWithValue("@BusinessDate", DateTime.Now.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@UpdateDateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@UserId", Config.Instance.CurrentUserId);
                        command.Parameters.AddWithValue("@UserName", Config.Instance.CurrentUserName);
                        command.Parameters.AddWithValue("@ParameterId", item.Parameter);
                        command.Parameters.AddWithValue("@ParameterName", item.Parameter.ToString());
                        command.Parameters.AddWithValue("@ParameterValue", item.ParameterValue);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error21 {0}", ex);
                }
            }
        }
        public async Task UpdateSettingsValueAsync(ParameterName param, string value)
        {
            return;
            //await Task.Factory.StartNew(() =>
            {
                try
                {
                    CheckConnection();
                    var conn = CreateConnection();
                    using (var transaction = conn.BeginTransaction())
                    {
                        var command = conn.CreateCommand();
                        command.CommandText = @"update settings set ParameterValue=@ParameterValue where ParameterId=@ParameterId";

                        command.CommandType = System.Data.CommandType.Text;
                        {
                            command.Parameters.AddWithValue("@BusinessDate", DateTime.Now.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@UpdateDateTime", DateTime.Now);
                            command.Parameters.AddWithValue("@UserId", Config.Instance.CurrentUserId);
                            command.Parameters.AddWithValue("@UserName", Config.Instance.CurrentUserName);
                            command.Parameters.AddWithValue("@ParameterId", param);
                            command.Parameters.AddWithValue("@ParameterName", param.ToString());
                            command.Parameters.AddWithValue("@ParameterValue", value);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error20 {0}", ex);
                }
            }
            //});
        }

        public List<SettingsDataModel> GetSettings()
        {
            var result = new List<SettingsDataModel>();
            try
            {
                using (var conn = CreateConnection())
                {
                    SQLiteDataReader sqlite_datareader;
                    SQLiteCommand sqlite_cmd;
                    sqlite_cmd = conn.CreateCommand();
                    sqlite_cmd.CommandText = "SELECT * FROM Settings order by id DESC limit 1000";
                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        try
                        {
                            result.Add(new SettingsDataModel
                            {
                                Id = sqlite_datareader.GetInt32(0),
                                BusinessDate = sqlite_datareader.GetString(1),
                                CreateDate = sqlite_datareader.GetDateTime(2),
                                UserId = sqlite_datareader.GetValue(3)?.ToString(),
                                UserName = sqlite_datareader.GetValue(4)?.ToString(),
                                Parameter = (ParameterName)sqlite_datareader.GetInt32(5),
                                ParameterName = sqlite_datareader.GetString(6),
                                ParameterValue = sqlite_datareader.GetString(7)
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error9 {0}", ex);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error18 {0}", ex);
            }
            return result;
        }
        private void InitSettings()
        {
            return;
            var list = GetSettings();
            foreach (ParameterName param in (ParameterName[])Enum.GetValues(typeof(ParameterName)))
            {
                try
                {
                    if (list.FirstOrDefault(p => p.Parameter == param) == null)
                    {
                        var value = 0;
                        if (param == ParameterName.SilentTime)
                            value = 120000;
                        InsertSettingsDataAsync(new SettingsDataModel
                        {
                            Parameter = param,
                            ParameterValue = value.ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error17 {0}", ex);
                }
            }

        }
        #endregion

        #region Patient
        public async Task<bool> InsertPatientData(PatientDataModel item)
        {
            return await Task.Factory.StartNew(() =>
              {
                  CheckConnection();
                  using (var transaction = sqlite_conn.BeginTransaction())
                  {
                      try
                      {
                          var command = sqlite_conn.CreateCommand();
                          command.CommandText = @"INSERT INTO Patient(CreateDateTime,UpdateDateTime,UserId,UserName,
                                        HospitalName,Section,Bed,PatientName,PatientId,Gender,Age,Weight) VALUES (@CreateDateTime,@UpdateDateTime,@UserId,@UserName,
                                        @HospitalName,@Section,@Bed,@PatientName,@PatientId,@Gender,@Age,@Weight)";
                          command.CommandType = System.Data.CommandType.Text;
                          {
                              command.Parameters.AddWithValue("@CreateDateTime", DateTime.Now);
                              command.Parameters.AddWithValue("@UpdateDateTime", DateTime.Now);
                              command.Parameters.AddWithValue("@UserId", Config.Instance.CurrentUserId);
                              command.Parameters.AddWithValue("@UserName", Config.Instance.CurrentUserName);
                              command.Parameters.AddWithValue("@HospitalName", item.HospitalName);
                              command.Parameters.AddWithValue("@Section", item.Section);
                              command.Parameters.AddWithValue("@Bed", item.Bed);
                              command.Parameters.AddWithValue("@PatientName", item.PatientName);
                              command.Parameters.AddWithValue("@PatientId", item.PatientId);
                              command.Parameters.AddWithValue("@Gender", item.Gender);
                              command.Parameters.AddWithValue("@Age", item.Age);
                              command.Parameters.AddWithValue("@Weight", item.Weight);
                              command.ExecuteNonQuery();
                          }
                          transaction.Commit();
                          return true;
                      }
                      catch (Exception ex)
                      {
                          Console.WriteLine("Error16 {0}", ex);
                          return false;
                      }
                  }
              });
        }
        public async Task<bool> UpdatePatientData(string patientId, PatientDataModel item)
        {
            return await Task.Factory.StartNew(() =>
            {
                CheckConnection();
                using (var transaction = sqlite_conn.BeginTransaction())
                {
                    try
                    {
                        var command = sqlite_conn.CreateCommand();
                        command.CommandText = @"update Patient 
                                        set UpdateDateTime=@UpdateDateTime,UserId=@UserId,UserName=@UserName,
                                        HospitalName=@HospitalName,Section=@Section,Bed=@Bed,PatientName=@PatientName,
                                        Gender=@Gender,Age=@Age,Weight=@Weight where PatientId=@PatientId";
                        command.CommandType = System.Data.CommandType.Text;
                        {
                            command.Parameters.AddWithValue("@UpdateDateTime", DateTime.Now);
                            command.Parameters.AddWithValue("@UserId", Config.Instance.CurrentUserId);
                            command.Parameters.AddWithValue("@UserName", Config.Instance.CurrentUserName);
                            command.Parameters.AddWithValue("@HospitalName", item.HospitalName);
                            command.Parameters.AddWithValue("@Section", item.Section);
                            command.Parameters.AddWithValue("@Bed", item.Bed);
                            command.Parameters.AddWithValue("@PatientName", item.PatientName);
                            command.Parameters.AddWithValue("@PatientId", item.PatientId);
                            command.Parameters.AddWithValue("@Gender", item.Gender);
                            command.Parameters.AddWithValue("@Age", item.Age);
                            command.Parameters.AddWithValue("@Weight", item.Weight);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error15 {0}", ex);
                        return false;
                    }
                }
            });
        }
        public async Task<PatientDataModel> GetPatient(string patientId)
        {
            return await Task.Factory.StartNew(() =>
           {
               PatientDataModel result = null;
               try
               {
                   using (var conn = CreateConnection())
                   {
                       SQLiteDataReader sqlite_datareader;
                       SQLiteCommand sqlite_cmd;
                       sqlite_cmd = conn.CreateCommand();
                       var where = !string.IsNullOrEmpty(patientId) ? $"where PatientId={patientId}" : "";
                       sqlite_cmd.CommandText = $"SELECT * FROM Patient {where} ORDER by id desc limit 1";
                       sqlite_datareader = sqlite_cmd.ExecuteReader();
                       while (sqlite_datareader.Read())
                       {
                           try
                           {
                               result = new PatientDataModel
                               {
                                   Id = sqlite_datareader.GetInt32(0),
                                   CreateDateTime = sqlite_datareader.GetDateTime(1),
                                   UpdateDateTime = sqlite_datareader.GetDateTime(2),
                                   UserId = sqlite_datareader.GetValue(3)?.ToString(),
                                   UserName = sqlite_datareader.GetValue(4)?.ToString(),
                                   HospitalName = sqlite_datareader.GetValue(5)?.ToString(),
                                   Section = sqlite_datareader.GetValue(6)?.ToString(),
                                   Bed = sqlite_datareader.GetValue(7)?.ToString(),
                                   PatientName = sqlite_datareader.GetValue(8)?.ToString(),
                                   PatientId = sqlite_datareader.GetValue(9)?.ToString(),
                                   Gender = sqlite_datareader.GetValue(10)?.ToString(),
                                   Age = sqlite_datareader.GetValue(11)?.ToString(),
                                   Weight = sqlite_datareader.GetValue(12)?.ToString(),
                               };
                           }
                           catch (Exception ex)
                           {
                               Console.WriteLine("Error14 {0}", ex);
                           }
                       }
                       conn.Close();
                   }
               }
               catch (Exception ex)
               {
                   Console.WriteLine("Error13 {0}", ex);
               }
               return result;
           });
        }
        public async Task<List<PatientDataModel>> GetPatients()
        {
            var res = await Task.Factory.StartNew(() =>
             {
                 var result = new List<PatientDataModel>();
                 try
                 {
                     using (var conn = CreateConnection())
                     {
                         SQLiteDataReader sqlite_datareader;
                         SQLiteCommand sqlite_cmd;
                         sqlite_cmd = conn.CreateCommand();
                         sqlite_cmd.CommandText = "SELECT * FROM Patient order by id DESC limit 1000";
                         sqlite_datareader = sqlite_cmd.ExecuteReader();
                         while (sqlite_datareader.Read())
                         {
                             try
                             {
                                 result.Add(new PatientDataModel
                                 {
                                     Id = sqlite_datareader.GetInt32(0),
                                     CreateDateTime = sqlite_datareader.GetDateTime(1),
                                     UpdateDateTime = sqlite_datareader.GetDateTime(2),
                                     UserId = sqlite_datareader.GetValue(3)?.ToString(),
                                     UserName = sqlite_datareader.GetValue(4)?.ToString(),
                                     HospitalName = sqlite_datareader.GetValue(5)?.ToString(),
                                     Section = sqlite_datareader.GetValue(6)?.ToString(),
                                     Bed = sqlite_datareader.GetValue(7)?.ToString(),
                                     PatientName = sqlite_datareader.GetValue(8)?.ToString(),
                                     PatientId = sqlite_datareader.GetValue(9)?.ToString(),
                                     Gender = sqlite_datareader.GetValue(10)?.ToString(),
                                     Age = sqlite_datareader.GetValue(11)?.ToString(),
                                     Weight = sqlite_datareader.GetValue(12)?.ToString(),
                                 });
                             }
                             catch (Exception ex)
                             {

                             }
                         }
                         conn.Close();
                     }
                 }
                 catch (Exception ex)
                 {

                 }
                 return result;
             });
            return res;
        }
        #endregion

        private void CheckConnection()
        {
            try
            {
                if (sqlite_conn.State != System.Data.ConnectionState.Open)
                    sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error12 {0}", ex);
            }
        }

        #region CheckOldData Timer

        Timer timerCheckForRemoveOldData;
        private void StartCheckDataTimer()
        {
            try
            {
                timerCheckForRemoveOldData = new Timer();
                timerCheckForRemoveOldData.Interval = 10800000;///2 hour
                timerCheckForRemoveOldData.Elapsed += TimerCheckForRemoveOldData_Elapsed;
                timerCheckForRemoveOldData.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private void TimerCheckForRemoveOldData_Elapsed(object sender, ElapsedEventArgs e)
        {
            RemoveOldData();
        }
        #endregion
    }
}
