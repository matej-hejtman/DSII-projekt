using System;
using Oracle.ManagedDataAccess.Client;
using DSII.orm.dto;

namespace DSII.orm.dao;

public static class TransactionsDao
{
    public static bool PridatOsobuDoPripadu(
        Database pDb,
        int      p_cid,
        int      p_pid,
        int      p_roid,
        string   p_autor)
    {
        Database db  = Database.Connect(pDb);
        bool     ret = true;

        try
        {
            db.BeginTransaction();

            string stav = PripadDao.GetStav(db, p_pid);
            if (stav == null || (stav != "open" && stav != "running"))
                throw new InvalidOperationException(
                    $"Případ {p_pid} neexistuje nebo není aktivní (stav='{stav}').");

            if (ClovekPripadDao.Exists(db, p_cid, p_pid))
                throw new InvalidOperationException(
                    $"Osoba {p_cid} již v případu {p_pid} figuruje.");

            int aktivni = PripadDao.CountAktivniPripady(db, p_cid);
            if (aktivni >= 10)
                throw new InvalidOperationException(
                    $"Osoba {p_cid} má již {aktivni} aktivních případů (max 10).");

            ClovekPripadDto vazba = new ClovekPripadDto
            {
                cid  = p_cid,
                pid  = p_pid,
                roid = p_roid
            };
            ClovekPripadDao.Insert(db, vazba);

            ClovekDao.IncrementPocetPripadu(db, p_cid, p_autor);

            db.EndTransaction();
        }
        catch (OracleException ex)
        {
            Console.Error.WriteLine($"[Oracle chyba] {ex.Message}");
            db.Rollback();
            ret = false;
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"[Podmínka] {ex.Message}");
            db.Rollback();
            ret = false;
        }

        Database.Close(pDb, db);
        return ret;
    }

    public static bool PridatOsobuDoPripadu_sp(
        Database pDb,
        int      p_cid,
        int      p_pid,
        int      p_roid,
        string   p_autor)
    {
        Database db    = Database.Connect(pDb);
        bool     v_ret = false;

        OracleCommand command = db.CreateCommand("PridatOsobuDoPripadu_sp");
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new OracleParameter("p_cid",   p_cid));
        command.Parameters.Add(new OracleParameter("p_pid",   p_pid));
        command.Parameters.Add(new OracleParameter("p_roid",  p_roid));
        command.Parameters.Add(new OracleParameter("p_autor", p_autor));

        OracleParameter pRet = new OracleParameter("p_ret", Oracle.ManagedDataAccess.Client.OracleDbType.Int32);
        pRet.Direction = System.Data.ParameterDirection.Output;
        command.Parameters.Add(pRet);

        db.ExecuteNonQuery(command);

        if (pRet.Value != DBNull.Value)
        {
            var oracleDecimal = (Oracle.ManagedDataAccess.Types.OracleDecimal)pRet.Value;
            int retVal = oracleDecimal.ToInt32();
            v_ret = (retVal == 1);
        }

        Database.Close(pDb, db);
        return v_ret;
    }
}
