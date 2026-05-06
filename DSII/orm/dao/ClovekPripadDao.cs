using Oracle.ManagedDataAccess.Client;

namespace DSII.orm.dao;

public static class ClovekPripadDao
{
    private static string SqlExists =
        "SELECT COUNT(*) FROM CLOVEK_PRIPAD WHERE CID = :cid AND PID = :pid";

    public static bool Exists(Database pDb, int cid, int pid)
    {
        Database db = Database.Connect(pDb);
        
        OracleCommand command = db.CreateCommand(SqlExists);
        command.Parameters.Add(new OracleParameter("cid", cid));
        command.Parameters.Add(new OracleParameter("pid", pid));
        
        int count = db.ExecuteScalar(command);
        Database.Close(pDb, db);
        
        return count > 0;
    }

    private static string SqlInsert =
        "INSERT INTO CLOVEK_PRIPAD (CID, PID, ROID) VALUES (:cid, :pid, :roid)";

    public static void Insert(Database pDb, DSII.orm.dto.ClovekPripadDto cp)
    {
        Database db = Database.Connect(pDb);
        OracleCommand command = db.CreateCommand(SqlInsert);
        
        command.Parameters.Add(new OracleParameter("cid",
            cp.cid.HasValue ? (object)cp.cid.Value : DBNull.Value));
        
        command.Parameters.Add(new OracleParameter("pid",  cp.pid));
        command.Parameters.Add(new OracleParameter("roid", cp.roid));
        
        db.ExecuteNonQuery(command);
        Database.Close(pDb, db);
    }
}
