using DSII;
using DSII.orm.dao;
using DSII.orm.dto;
using Oracle.ManagedDataAccess.Client;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== DIAGNOSTIKA PŘIPOJENÍ ===\n");
        try
        {
            Console.WriteLine("1. Pokusím se připojit k Oracle...");
            var db = Database.Connect(null);
            Console.WriteLine("   ✅ Připojení OK\n");
            
            Console.WriteLine("2. Kontroluji tabulky...");
            
            // Počet řádků v PRIPAD
            var cmd = db.CreateCommand("SELECT COUNT(*) FROM PRIPAD");
            int countPripad = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - PRIPAD: {countPripad} řádků");
            
            // Počet řádků v CLOVEK
            cmd = db.CreateCommand("SELECT COUNT(*) FROM CLOVEK");
            int countClovek = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - CLOVEK: {countClovek} řádků");
            
            // Počet řádků v ROLE
            cmd = db.CreateCommand("SELECT COUNT(*) FROM ROLE");
            int countRole = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - ROLE: {countRole} řádků");
            
            // Počet řádků v CLOVEK_PRIPAD
            cmd = db.CreateCommand("SELECT COUNT(*) FROM CLOVEK_PRIPAD");
            int countClovekPripad = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - CLOVEK_PRIPAD: {countClovekPripad} řádků\n");
            
            if (countPripad == 0 || countClovek == 0 || countRole == 0)
            {
                Console.WriteLine("❌ CHYBA: Tabulky jsou prázdné!");
                Console.WriteLine("   Musíte nejdřív naplnit databázi testovacími daty.\n");
                Database.Close(null, db);
                return;
            }
            
            Console.WriteLine("3. Zjišťuji stavy v PRIPAD...");
            cmd = db.CreateCommand("SELECT DISTINCT STAV FROM PRIPAD");
            var reader = db.Select(cmd);
            Console.WriteLine("   Existující stavy:");
            while (reader.Read())
            {
                var stav = reader.IsDBNull(0) ? "[NULL]" : $"'{reader.GetString(0)}'";
                Console.WriteLine($"     - {stav}");
            }
            reader.Close();
            
            Console.WriteLine("\n4. Hledám aktivní případ (STAV IN 'open', 'running')...");
            cmd = db.CreateCommand("SELECT PID, STAV FROM PRIPAD WHERE STAV IN ('open', 'running') AND ROWNUM <= 3");
            reader = db.Select(cmd);
            int pid_aktivni = 0;
            if (reader.Read())
            {
                pid_aktivni = reader.GetInt32(0);
                Console.WriteLine($"   ✅ Našel jsem: PID={pid_aktivni}, STAV='{reader.GetString(1)}'");
            }
            else
            {
                Console.WriteLine("   ❌ Žádný aktivní případ! Buď jsou všechny uzavřené, nebo mají jiný stav.");
                Console.WriteLine("   Zadej ručně: jaký stav mají otevřené případy?\n");
                reader.Close();
                Database.Close(null, db);
                return;
            }
            reader.Close();
            
            Console.WriteLine("\n5. Zjišťuji osoby a role...");
            cmd = db.CreateCommand("SELECT CID FROM CLOVEK WHERE ROWNUM <= 2");
            reader = db.Select(cmd);
            int cid1 = 0, cid2 = 0;
            if (reader.Read()) cid1 = reader.GetInt32(0);
            if (reader.Read()) cid2 = reader.GetInt32(0);
            reader.Close();
            
            if (cid2 == 0) cid2 = cid1 + 1; // Fallback
            
            cmd = db.CreateCommand("SELECT ROID FROM ROLE WHERE ROWNUM <= 1");
            reader = db.Select(cmd);
            int roid = 0;
            if (reader.Read()) roid = reader.GetInt32(0);
            reader.Close();
            
            Console.WriteLine($"   CID1={cid1}, CID2={cid2}, ROID={roid}");
            
            Database.Close(null, db);
            
            Console.WriteLine("\n✅ DIAGNOSTIKA OK - můžeme spustit testy!\n");
            Console.WriteLine("=== SPOUŠTÍM TESTY ===\n");
            
            int    pid_closed  = 9999;
            string autor       = "HEJ0094";
            bool ret;

            ret = TransactionsDao.PridatOsobuDoPripadu(null, cid1, pid_aktivni, roid, autor);
            Console.WriteLine($"Volání 1 - PridatOsobuDoPripadu:    ret: {ret},  cid: {cid1}, pid: {pid_aktivni}");

            ret = TransactionsDao.PridatOsobuDoPripadu_sp(null, cid2, pid_aktivni, 2, autor);
            Console.WriteLine($"Volání 2 - PridatOsobuDoPripadu_sp: ret: {ret},  cid: {cid2}, pid: {pid_aktivni}");

            ret = TransactionsDao.PridatOsobuDoPripadu(null, cid1, pid_aktivni, roid, autor);
            Console.WriteLine($"Volání 3 - PridatOsobuDoPripadu:    ret: {ret},  cid: {cid1}, pid: {pid_aktivni}  (duplicita)");

            ret = TransactionsDao.PridatOsobuDoPripadu_sp(null, cid1, pid_closed, roid, autor);
            Console.WriteLine($"Volání 4 - PridatOsobuDoPripadu_sp: ret: {ret},  cid: {cid1}, pid: {pid_closed}  (neaktivní)");

            Console.WriteLine("\n✅ Hotovo!");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"❌ CHYBA: {ex.Message}");
            if (ex.InnerException != null)
                Console.Error.WriteLine($"   Příčina: {ex.InnerException.Message}");
            Console.Error.WriteLine($"\n{ex.StackTrace}");
        }
    }
}
