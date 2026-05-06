using Oracle.ManagedDataAccess.Client;

namespace DSII.orm.dao;

public static class PripadDao
{
    private static string SqlGetStav =
        "SELECT STAV FROM PRIPAD WHERE PID = :pid";

    public static string GetStav(Database pDb, int pid)
    {
        Database db = Database.Connect(pDb);
        OracleCommand command = db.CreateCommand(SqlGetStav);
        command.Parameters.Add(new OracleParameter("pid", pid));

        string stav = null;
        OracleDataReader reader = db.Select(command);
        
        if (reader.Read())
        {
            if (reader.IsDBNull(0))
                stav = null;
            else
            {
                stav = reader.GetString(0);
                if (string.IsNullOrWhiteSpace(stav))
                    stav = null;
            }
        }
        
        reader.Close();
        Database.Close(pDb, db);
        return stav;
    }

    private static string SqlCountAktivni =
        @"SELECT COUNT(*)
          FROM CLOVEK_PRIPAD cp
          JOIN PRIPAD p ON p.PID = cp.PID
          WHERE cp.CID = :cid
            AND p.STAV IN ('open', 'running')";

    public static int CountAktivniPripady(Database pDb, int cid)
    {
        Database db = Database.Connect(pDb);
        OracleCommand command = db.CreateCommand(SqlCountAktivni);
        command.Parameters.Add(new OracleParameter("cid", cid));
        
        int count = db.ExecuteScalar(command);
        Database.Close(pDb, db);
        
        return count;
    }
}
