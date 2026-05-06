# DOCUMENTATION for DSII_PROJEKT

Datum: 2026-05-06

Stručný přehled
----------------
Tento projekt je konzolová C# aplikace (net8.0) pro demonstrační scénář správy případů (policejní systém) zaměřená na cvičení transakcí a uložené procedury. Architektura: 3-vrstvá (DTO / DAO / Database) bez použití ORM — pouze ADO.NET (Oracle.ManagedDataAccess.Core).

Co projekt dělá
---------------
- Připojuje se k Oracle DB přes connection string v App.config.
- Implementuje transakční logiku v C# (TransactionsDao.PridatOsobuDoPripadu) i ekvivalent v PL/SQL (files/PridatOsobuDoPripadu.sql).
- Provádí jednoduché testy z Program.cs: 4 volání (C# a SP verze) ověřující vložení osoby do případu a chybové stavy.

Jak projekt ovládat
-------------------
1. Naplnit databázi testovacími daty (tabulky PRIPAD, CLOVEK, ROLE, CLOVEK_PRIPAD). Projekt neobsahuje DDL.
2. Sestavit: dotnet build
3. Spustit: dotnet run
4. Výstup v konzoli zobrazí diagnostiku, výsledky 4 testů a případné chyby Oracle.

Seznam důležitých souborů (relativní k projektu DSII/)
------------------------------------------------------
- DSII.csproj
- App.config
- Database.cs
- Program.cs
- orm/dto/ClovekDto.cs
- orm/dto/PripadDto.cs
- orm/dto/ClovekPripadDto.cs
- orm/dao/ClovekDao.cs
- orm/dao/PripadDao.cs
- orm/dao/ClovekPripadDao.cs
- orm/dao/TransactionsDao.cs
- files/PridatOsobuDoPripadu.sql

Podrobné vysvětlení jednotlivých souborů
---------------------------------------
Poznámka: následují bloky s původním kódem a vysvětlením. Každý blok je označen jako C# kód pro lepší čitelnost.

---

## DSII.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Oracle.ManagedDataAccess.Core" Version="23.4.0" />
  </ItemGroup>

</Project>
```
Vysvětlení:
- Project Sdk: standardní šablona .NET 8 konzole.
- TargetFramework: net8.0 — projekt je řešený pro .NET 8.
- ImplicitUsings/Nullable: moderní komfort nastavení.
- PackageReference: přidává Oracle.ManagedDataAccess.Core (nutné pro Oracle ADO.NET klienta). Bez tohoto balíčku nebude existovat OracleConnection.

---

## App.config
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="ConnectionStringOracle"
         connectionString="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=bayer.cs.vsb.cz)(PORT=1521))(CONNECT_DATA=(SID=oracle)));User Id=HEJ0094;Password=H84J9Ej8KdsJYzL5;"
         providerName="Oracle.ManagedDataAccess.Client" />
  </connectionStrings>

  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
  </startup>
</configuration>
```
Vysvětlení (řádek po řádku):
- Řádky 1-2: XML hlavička a root element.
- connectionStrings: sekce, kde jsou uloženy připojovací řetězce.
- add name=...: klíčová položka s ConnectionStringOracle — používá se v Database.Connect.
  - connectionString: specifikuje host, port, SID (oracle) a přihlašovací údaje HEJ0094.
  - providerName: určuje Oracle ADO.NET provider.
- startup sekce není kritická pro běh, ale zachová kompatibilitu s některými hostovacími prostředími.

Bez přesného connection stringu nebude spojení fungovat.

---

## Database.cs
```C#
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
```
Vysvětlení (řádek po řádku / myšlenkový postup):
- using ...: nutné namespace pro konfiguraci a Oracle klienta.
- třída Database: obal (wrapper) nad OracleConnection a OracleTransaction.
- privátní pole:
  - _connection: samotné spojení.
  - _transaction: aktuální transakce (pokud nějaká běží).
  - _ownsConnection: flag, zda instance vlastní spojení (pomáhá sdílení spojení mezi DAO voláními).
- Konstruktor je privátní: navrženo, aby se používala statická metoda Connect.
- Connect(pDb = null):
  - Pokud je předán existující Database instance, nastaví _ownsConnection=false a vrátí ji. To umožňuje sdílet spojení a transakci mezi DAO metodami (pattern Connect/Close pDb).
  - Pokud není, vytvoří nové připojení z connection stringu v App.config a otevře ho. Nastaví _ownsConnection=true.
- Close: zavře připojení pouze pokud instance vlastní spojení; jinak nic nedělá (umožňuje, že volající, který vytvořil spojení, ho zavře).
- BeginTransaction: zahájí transakci. Důležité: isolation level je ReadCommitted (po experimentech — Serializable způsoboval ORA-08177 při testech).
- EndTransaction/Rollback: commit/rollback a nastavení transaction = null.
- CreateCommand(sql): vytvoří OracleCommand, zapne BindByName (doporučeno pro Oracle), při existenci transakce přiřadí command.Transaction tak, aby příkazy byly součástí transakce.
- ExecuteScalar: spouští ExecuteScalar a vrací 0 pokud je výsledek null/DB null jinak Convert.ToInt32.
  - Poznámka: pro Oracle NUMBER může vrátit OracleDecimal v jiných scénářích; ale zde se očekává, že ExecuteScalar vrátí .NET typ (nejčastěji long/int).
- ExecuteNonQuery a Select: jednoduché proxované volání.

Designové rozhodnutí a důvody:
- Connect/Close pattern umožňuje pohodlné sdílení transakce mezi DAO metodami bez nutnosti předávat explicitně OracleTransaction.
- BindByName=true zajišťuje, že pojmenované parametry (např. :pid) jsou mapovány podle jména, ne podle pozice.
- ReadCommitted: z důvodu ORA-08177 (serializace konflikty) bylo zvoleno ReadCommitted — pro cvičení to stačí.

---

## Program.cs
```C#
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
            
            var cmd = db.CreateCommand("SELECT COUNT(*) FROM PRIPAD");
            int countPripad = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - PRIPAD: {countPripad} řádků");
            
            cmd = db.CreateCommand("SELECT COUNT(*) FROM CLOVEK");
            int countClovek = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - CLOVEK: {countClovek} řádků");
            
            cmd = db.CreateCommand("SELECT COUNT(*) FROM ROLE");
            int countRole = db.ExecuteScalar(cmd);
            Console.WriteLine($"   - ROLE: {countRole} řádků");
            
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
```
Vysvětlení (řádek/část po části):
- Hlavička: using ... — import potřebných namespaces.
- Diagnostika (řádky ~10-47): otevře spojení a zjistí počet řádků v klíčových tabulkách. Pokud některá tabulka prázdná, vypíše chybu a ukončí se — to pomáhá zabránit testům na prázdné DB.
- Zjištění stavu: SELECT DISTINCT STAV — vypíše možné stavy případu.
- Hledání aktivního případu: vybírá první případ s STAV IN ('open','running'). Pokud žádný, ukončí.
- Zjištění CID/ROID: získá dva CID a jeden ROID pro testovací volání.
- Testovací volání: 4 volání kombinující C# transakci a SP volání — ukazuje očekávané chování (sukces, duplicita, neaktivní).
- Chytání výjimek: vypíše chybu + stack trace pro debugging.

Myšlenkový postup při tvorbě Program.cs:
- Chceme zaručit, že testy neběží na prázdné DB.
- Diagnostika pomůže rychle identifikovat chybějící data nebo špatný connection string.
- Testy volají obě implementace transakce, aby se porovnalo chování a zachytily odlišnosti.

---

## orm/dto/*.cs
Každá DTO třída mapuje jeden záznam tabulky. Používáme snake_case názvy vlastností, aby se přímo shodovaly s názvy sloupců v DB (to usnadňuje mapování a čtení dat pomocí reader.GetX).

### ClovekPripadDto.cs
```C#
namespace DSII.orm.dto;

public class ClovekPripadDto
{
    public int?  cid  { get; set; }
    public int   pid  { get; set; }
    public int   roid { get; set; }
}
```
- cid je nullable int: v DB může být CID NULL při některých operacích, proto je typ int?.

### PripadDto.cs (stručně)
- Vlastnosti odpovídají sloupcům tabulky PRIPAD: pid, cislo_pripadu, datum_zahajeni, datum_ukonceni (nullable), stav, priorita, typ_pripadu, popis, atd.

### ClovekDto.cs (stručně)
- Reprezentuje tabulku CLOVEK se všemi poli (rodne_cislo, jmeno, prijmeni, datum_narozeni, apod.). Některé textové pole jsou nullable.

Poznámka k snake_case: není standard C#, ale záměrně zachováno kvůli přímému mapování.

---

## orm/dao/*.cs
DAO třídy obsahují SQL dotazy a metody pracující s Database wrapperem.

### ClovekPripadDao.cs
```C#
private static string SqlExists =
    "SELECT COUNT(*) FROM CLOVEK_PRIPAD WHERE CID = :cid AND PID = :pid";

public static bool Exists(Database pDb, int cid, int pid)
{ ... }

private static string SqlInsert =
    "INSERT INTO CLOVEK_PRIPAD (CID, PID, ROID) VALUES (:cid, :pid, :roid)";

public static void Insert(Database pDb, ClovekPripadDto cp) { ... }
```
Vysvětlení:
- Exists: kontroluje, zda vazba osoba-případ už existuje.
- Insert: vloží záznam. Parametry se přidávají pomocí OracleParameter. Pokud cp.cid == null vložíme DBNull.Value.

DŮLEŽITÉ: primární klíč tabulky CLOVEK_PRIPAD je v DB definovaný jinak (v některých instalacích může být PK na kombinaci PID, ROID) — proto testy musí používat různá ROID pro různé osoby ve stejném případu.

### PripadDao.cs
Hlavní metody:
- GetStav(Database pDb, int pid): vrací stav případu nebo null. Zvláštní pozornost na IsDBNull a string.IsNullOrWhiteSpace — to zabraňuje chybám, pokud je ve sloupci prázdný řetězec.
- CountAktivniPripady(Database pDb, int cid): spočítá aktivní případy (join s PRIPAD, where stav in ('open','running')).

### ClovekDao.cs
- IncrementPocetPripadu(Database pDb, int cid, string autor): aktualizuje POCET_PRIPADU a metadata (POSLEDNI_AKTUALIZACE, AUTOR_POSLEDNI_AKTUALIZACE).

Všechny DAO metody používají Database.Connect(pDb) a Database.Close(pDb, db) — to umožňuje, že pokud je transakce spuštěna volajícím (db vzniklá v Connect(null)), DAO metody připojí příkazy do stejné transakce.

---

## orm/dao/TransactionsDao.cs
```C#
public static bool PridatOsobuDoPripadu(Database pDb, int p_cid, int p_pid, int p_roid, string p_autor)
{
    Database db  = Database.Connect(pDb);
    bool     ret = true;

    try
    {
        db.BeginTransaction();

        string stav = PripadDao.GetStav(db, p_pid);
        if (stav == null || (stav != "open" && stav != "running"))
            throw new InvalidOperationException(...);

        if (ClovekPripadDao.Exists(db, p_cid, p_pid))
            throw new InvalidOperationException(...);

        int aktivni = PripadDao.CountAktivniPripady(db, p_cid);
        if (aktivni >= 10)
            throw new InvalidOperationException(...);

        ClovekPripadDto vazba = new ClovekPripadDto { cid = p_cid, pid = p_pid, roid = p_roid };
        ClovekPripadDao.Insert(db, vazba);

        ClovekDao.IncrementPocetPripadu(db, p_cid, p_autor);

        db.EndTransaction();
    }
    catch (OracleException ex) { db.Rollback(); ret=false; }
    catch (InvalidOperationException ex) { db.Rollback(); ret=false; }

    Database.Close(pDb, db);
    return ret;
}
```
Vysvětlení a myšlenkový postup:
- Transakce je definována v C# s rozumným pořadím kontrol (nejlevnější kontroly první):
  1) existuje a je aktivní PRIPAD (GetStav) — relativně levné select 1 row
  2) duplicita v CLOVEK_PRIPAD (Exists) — select count
  3) počet aktivních případů osoby (CountAktivniPripady) — může být nejdražší
  4) insert vazby + update počtu případů
- Při jakékoliv poruše je volána Rollback().
- Výjimky jsou rozdílně logovány: OracleException vypisuje text databázové chyby, InvalidOperationException vypisuje obchodní (domain) chybu.

### PridatOsobuDoPripadu_sp
- Volání uložené procedury v Oracle:
  - Vytvoří command s CommandType.StoredProcedure.
  - Přidá vstupní parametry p_cid, p_pid, p_roid, p_autor.
  - Předpokládá, že procedura vrací p_ret OUT NUMBER (1 = success), které je Oracle DECIMAL — v C# je čteno jako OracleDecimal a převedeno pomocí ToInt32().

Učení/poznámky: Oracle vrací NUMBER jako OracleDecimal -> musel být použit speciální převod (oracleDecimal.ToInt32()).

---

## files/PridatOsobuDoPripadu.sql
```sql
CREATE OR REPLACE PROCEDURE PridatOsobuDoPripadu_sp (
    p_cid   IN  NUMBER,
    p_pid   IN  NUMBER,
    p_roid  IN  NUMBER,
    p_autor IN  VARCHAR2,
    p_ret   OUT NUMBER
) AS
...
BEGIN
    -- načtení stavu případu FOR UPDATE
    -- kontrola stavu (open/running)
    -- kontrola duplicity
    -- kontrola počtu aktivních případů
    -- insert do CLOVEK_PRIPAD
    -- update CLOVEK
    COMMIT;
    p_ret := 1;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_ret := 0;
END;
/
```
- Procedura reimplementuje stejnou logiku jako C# transakce. Klíčové body:
  - SELECT ... FOR UPDATE: zajišťuje exkluzivní zámek řádku PRIPAD, aby se zabránilo souběžným konfliktům.
  - Při selhání se provede ROLLBACK a p_ret=0.
  - Na konci COMMIT a p_ret=1.

---

Důležitá varování a časté chyby
--------------------------------
- ORA-12899 (value too large): při vkládání CISLO_PRIPADU bedlivě zkontrolujte délku sloupce ve schématu. Příklady testovacích dat mohou mít jiný formát než DDL očekává.
- ORA-08177: problém s izolací Serializable v některých scénářích. Při testování byla změněna izolace na ReadCommitted.
- OracleDecimal: výstup OUT NUMBER z procedury nemusí být .NET int; použít Oracle.ManagedDataAccess.Types.OracleDecimal a ToInt32().
- PK/CU kombinace: v některých schématech je PK CLOVEK_PRIPAD definována na (PID, ROID) apod. Testovací data musí odpovídat očekávanému chování (více osob do jedné případu vyžaduje různé ROID).

Jak rozšířit projekt
--------------------
- Přidání nové transakce: vytvořit DTO pro zúčastněné tabulky, DAO metody pro operace, a implementovat transakci v TransactionsDao.cs podle vzoru (nejprve kontroly, pak modifikace, commit/rollback).
- Přidání testů: lze vytvořit jednoduché integrační testy, které připojí testovací schéma, vloží seed data a spustí metody TransactionsDao.

Závěr a návrhy na zlepšení
-------------------------
- Robustnější mapování: uvažovat o mapperu (ručně) namísto předpokladu, že pořadí sloupců bude vždy stejné.
- Lepší logování: aktuální konzolové výpisy jsou dostačující pro cvičení, ale pro reálnou aplikaci použít např. Microsoft.Extensions.Logging.
- Testovací DDL a seed skripty: přidat příklad DDL a SQL seed v /files, aby bylo možné snadno replikovat testovací prostředí.

---

Konec dokumentace
-----------------
Soubor vytvořen automaticky. Pokud chcete, mohu provést:
- doplnit úplné line-by-line rozebrání i u všech DAO a DTO (je částečně shrnuto),
- přidat příklad DDL a seed SQL pro plné otestování,
- nebo přeložit dokumentaci do angličtiny.


