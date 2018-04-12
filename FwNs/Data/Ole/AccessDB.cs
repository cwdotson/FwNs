namespace FwNs.Data.Ole
{
    using System;
    using System.Data.OleDb;
    using System.Runtime.InteropServices;

    public class AccessDB
    {
        private string connectionString;
        private OleDbConnection dbReaderConnection;
        private OleDbCommand dbReaderCommand;
        private OleDbDataReader dbReader;

        public AccessDB(string dsn)
        {
            this.connectionString = "PROVIDER=Microsoft.JET.OLEDB.4.0;DATA SOURCE = " + dsn;
        }

        public int DeleteData(string tableName, int primaryKey, out string errMessage)
        {
            int num = 0;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            errMessage = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                string str = ("DELETE FROM " + tableName) + " WHERE ID = " + string.Format("{0}", primaryKey);
                command = new OleDbCommand {
                    Connection = connection,
                    CommandText = str
                };
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                errMessage = "errMessage:" + exception.Message;
                num = -1;
            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return num;
        }

        public int ExecuteNonQuery(string queryString, out string errMessage)
        {
            int num = 0;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            errMessage = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                command = new OleDbCommand {
                    Connection = connection,
                    CommandText = queryString
                };
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                errMessage = "errMessage:" + exception.Message;
                return -1;
            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return num;
        }

        public OleDbDataReader ExecuteReader(string queryString, out string errMessage)
        {
            errMessage = null;
            try
            {
                this.dbReaderConnection = new OleDbConnection();
                this.dbReaderConnection.ConnectionString = this.connectionString;
                this.dbReaderConnection.Open();
                this.dbReaderCommand = new OleDbCommand();
                this.dbReaderCommand.Connection = this.dbReaderConnection;
                this.dbReaderCommand.CommandText = queryString;
                this.dbReader = this.dbReaderCommand.ExecuteReader();
            }
            catch (Exception exception)
            {
                errMessage = "errMessage:" + exception.Message + string.Format("\r\n\r\nMethod Name : SelectData", new object[0]);
                return null;
            }
            return this.dbReader;
        }

        public object ExecuteScalar(string queryString, out string errMessage)
        {
            object obj2 = null;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            errMessage = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                command = new OleDbCommand {
                    Connection = connection,
                    CommandText = queryString
                };
                obj2 = command.ExecuteScalar();
            }
            catch (Exception exception)
            {
                errMessage = "errMessage:" + exception.Message;
                return null;
            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return obj2;
        }

        ~AccessDB()
        {
        }

        public void FinishReader(out string errMessage)
        {
            errMessage = null;
            try
            {
                this.dbReader.Close();
                this.dbReader.Dispose();
                this.dbReaderConnection.Close();
                this.dbReaderConnection.Dispose();
            }
            catch (Exception exception)
            {
                errMessage = exception.Message;
            }
        }

        public int GetRecordCount(string tableName, out string errMessage)
        {
            int num = 0;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            errMessage = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                string str = "SELECT COUNT(*)";
                str = str + " FROM " + tableName;
                command = new OleDbCommand {
                    Connection = connection,
                    CommandText = str
                };
                num = (int) command.ExecuteScalar();
            }
            catch (Exception)
            {
                num = -1;
            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return num;
        }

        public int InsertData(string tableName, string columnName, string strValue, out string errMessage)
        {
            OleDbConnection connection = null;
            OleDbCommand command = null;
            int num;
            errMessage = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                string str = ((("INSERT INTO " + tableName + " ( ") + columnName + ") ") + " VALUES ( '") + strValue + "')";
                command = new OleDbCommand {
                    Connection = connection,
                    CommandText = str
                };
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                errMessage = exception.Message + string.Format("\r\n\r\nMethod Name : InsertData", new object[0]);
                num = -1;
            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return num;
        }

        public int SelectData(out string errMessage)
        {
            int num = 0;
            errMessage = null;
            OleDbConnection connection = null;
            OleDbDataReader reader = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                string str = "SELECT * FROM DBINFO";
                reader = new OleDbCommand { 
                    Connection = connection,
                    CommandText = str
                }.ExecuteReader();
                while (reader.Read())
                {
                    num++;
                }
            }
            catch (Exception exception)
            {
                errMessage = "errMessage:" + exception.Message + string.Format("\r\n\r\nMethod Name : SelectData", new object[0]);
                num = -1;
            }
            finally
            {
                reader.Close();
                reader.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return num;
        }

        public int UpdateData(string tableName, string columnName, string strValue, int primaryKey, out string errMessage)
        {
            int num = 0;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            errMessage = null;
            try
            {
                connection = new OleDbConnection {
                    ConnectionString = this.connectionString
                };
                connection.Open();
                string str2 = "UPDATE " + tableName;
                string str = (str2 + " SET (" + columnName + "= '" + strValue + "')") + " WHERE (ID=" + string.Format("{0}", primaryKey) + ")";
                command = new OleDbCommand {
                    Connection = connection,
                    CommandText = str
                };
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                errMessage = "errMessage:" + exception.Message;
                num = -1;
            }
            finally
            {
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return num;
        }
    }
}

