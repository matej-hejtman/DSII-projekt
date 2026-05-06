using Oracle.ManagedDataAccess.Client;

namespace DSII.orm.dao;

public static class ClovekDao
{
    private static string SqlIncrementPocetPripadu =
        @"UPDATE CLOVEK
          SET POCET_PRIPADU              = POCET_PRIPADU + 1,
              POSLEDNI_AKTUALIZACE       = SYSDATE,
              AUTOR_POSLEDNI_AKTUALIZACE = :autor
          WHERE CID = :cid";

    public static void IncrementPocetPripadu(Database pDb, int cid, string autor)
    {
        Database db = Database.Connect(pDb);
        OracleCommand command = db.CreateCommand(SqlIncrementPocetPripadu);
        command.Parameters.Add(new OracleParameter("autor", autor));
        command.Parameters.Add(new OracleParameter("cid",   cid));
        db.ExecuteNonQuery(command);
        Database.Close(pDb, db);
    }
}
