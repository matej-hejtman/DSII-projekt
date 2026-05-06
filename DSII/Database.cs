using System;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DSII;

public class Database
{
    private OracleConnection  _connection;
    private OracleTransaction _transaction;
    private bool              _ownsConnection;

    private Database() { }

    public static Database Connect(Database pDb = null)
    {
        if (pDb != null) 
        { 
            pDb._ownsConnection = false; 
            return pDb; 
        }

        Database db = new Database();
        string cs = ConfigurationManager
                        .ConnectionStrings["ConnectionStringOracle"]
                        .ConnectionString;
        db._connection = new OracleConnection(cs);
        db._connection.Open();
        db._ownsConnection = true;
        return db;
    }

    public static void Close(Database pDb, Database db)
    {
        if (db._ownsConnection) 
            db._connection.Close();
    }

    public void BeginTransaction()
    {
        _transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
    }

    public void EndTransaction()
    {
        _transaction.Commit();
        _transaction = null;
    }

    public void Rollback()
    {
        if (_transaction != null) 
        { 
            _transaction.Rollback(); 
            _transaction = null; 
        }
    }

    public OracleCommand CreateCommand(string sql)
    {
        OracleCommand cmd = new OracleCommand(sql, _connection);
        cmd.BindByName = true;
        
        if (_transaction != null) 
            cmd.Transaction = _transaction;
        
        return cmd;
    }

    public int ExecuteScalar(OracleCommand command)
    {
        object result = command.ExecuteScalar();
        return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
    }

    public int ExecuteNonQuery(OracleCommand command)
    {
        return command.ExecuteNonQuery();
    }

    public OracleDataReader Select(OracleCommand command)
    {
        return command.ExecuteReader();
    }
}
