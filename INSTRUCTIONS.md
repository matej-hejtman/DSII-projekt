# DOC.md — Projektová dokumentace

## Poslední aktualizace: TASK 1 — Vytvoření projektu a App.config

---

```DBML
TABLE Adresa {
    adresa varchar(255) pk [ref: > Clovek.adresa_aktualni, ref: > Clovek.adresa_trvala, ref: > Zlocin.misto_zlocinu, ref: > Dukaz.misto_ziskani]
    ulice varchar(255) [not null]
    cislo_popisne varchar(20) [not null]
    mesto varchar(255) [not null]
    psc varchar(20) [not null]
    stat varchar(255) [not null]
}

Table Clovek {
  cid int pk [not null, ref: > Dukaz.zodpovedna_osoba]
  rodne_cislo varchar(10) [not null]
  jmeno varchar(255) [not null]
  prijmeni varchar(31) [not null]
  datum_narozeni date [not null]
  misto_narozeni varchar(255) [pk, not null] 
  pohlavi char [not null]
  statni_obcanstvi varchar(255) [not null]
  adresa_trvala varchar(255) [not null]
  adresa_aktualni varchar(255) [not null]
  vyska int [not null]
  vaha int [not null]
  barva_oci varchar(31) [not null]
  barva_vlasu varchar(31) [null]
  zvlastni_znaky text [null]
  rizikovy_level int [not null]
  pocet_pripadu int [not null]
  pocet_odsouzeni int [not null]
  datum_registrace date [not null]
  posledni_aktualizace timestamp [not null]
  autor_posledni_aktualizace varchar(255) [not null]
  poznamky text [null] 
  interni_komentar text [null]
}

Table Pripad {
  pid int [pk, not null]
  cislo_pripadu varchar(10) [not null]
  datum_zahajeni date [not null]
  datum_ukonceni date [null]
  stav varchar(255) [not null]
  priorita int [not null]
  typ_pripadu varchar(255) [not null]
  popis text [null]
  poznamky text [null]
  vedouci_pripad varchar(255) [not null]
  posledni_aktualizace timestamp [not null]
  autor_posledni_aktualizace varchar(255) [not null]
}

Table Zlocin {
  zid int [pk, not null]
  zavaznost int [not null]
  nazev varchar(255) [not null]
  typ varchar(255) [not null]
  popis text [null]
  datum_zlocinu date [not null]
  misto_zlocinu varchar(255) [not null]
  trestni_stupen varchar(31) [not null]
  posledni_aktualizace timestamp [not null]
  autor_posledni_aktualizace varchar(255) [not null]
}

Table Dukaz {
  evid int [pk, not null]
  typ varchar(255) [not null]
  popis text [not null]
  datum_ziskani date [not null]
  misto_ziskani varchar(255) [not null]
  zodpovedna_osoba int [not null]
  stav varchar(255) [not null]
  datum_registrace date [not null]
  posledni_aktualizace timestamp [not null]
  autor_posledni_aktualizace varchar(255) [not null]
  poznamky text [null]
}

Table Dusledek {
  cons_id int [pk, not null]
  nazev varchar(63) [not null]
  typ varchar(63) [not null]
  delka int [null]
  podminky text [null]
  popis text [null]
  posledni_aktualizace timestamp [not null]
  autor_posledni_aktualizace varchar(255) [not null]
  poznamky text [null]
}

Table Role {
  roid int [pk, not null]
  nazev varchar(63) [not null]
  popis text [null]
  aktivni bit [not null]
  posledni_aktualizace timestamp [not null]
  autor_posledni_aktualizace varchar(255)
  poznamky text [null]
}

Table Clovek_Pripad {
  cid int [ref: > Clovek.cid, null]
  pid int [ref: > Pripad.pid, not null]
  roid int [ref: > Role.roid, not null]
}

Table Pripad_Zlocin {
  pid int [ref: > Pripad.pid, not null]
  zid int [ref: > Zlocin.zid, not null]
}

Table Pripad_Dukaz {
  pid int [ref: > Pripad.pid, not null]
  evid int [ref: > Dukaz.evid, null]
}

Table Dusledek_Pripad {
  cid int [ref: > Clovek.cid, not null]
  pid int [ref: > Pripad.pid, not null]
  cons_id int [ref: > Dusledek.cons_id, not null]
}
```

```PL/SQL
-- ============================================================
--  Kriminální databáze – Oracle DDL
--  Pořadí CREATE TABLE respektuje závislosti FK
-- ============================================================

-- ------------------------------------------------------------
-- 1. ADRESA  (na ni referencují Clovek, Zlocin, Dukaz)
-- ------------------------------------------------------------
CREATE TABLE Adresa (
    adresa          VARCHAR2(255)   NOT NULL,
    ulice           VARCHAR2(255)   NOT NULL,
    cislo_popisne   VARCHAR2(20)    NOT NULL,
    mesto           VARCHAR2(255)   NOT NULL,
    psc             VARCHAR2(20)    NOT NULL,
    stat            VARCHAR2(255)   NOT NULL,
    CONSTRAINT pk_adresa PRIMARY KEY (adresa)
);

-- ------------------------------------------------------------
-- 2. CLOVEK  (FK → Adresa)
-- ------------------------------------------------------------
CREATE TABLE Clovek (
    cid                         NUMBER(10)      NOT NULL,
    rodne_cislo                 VARCHAR2(10)    NOT NULL,
    jmeno                       VARCHAR2(255)   NOT NULL,
    prijmeni                    VARCHAR2(31)    NOT NULL,
    datum_narozeni              DATE            NOT NULL,
    misto_narozeni              VARCHAR2(255)   NOT NULL,
    pohlavi                     CHAR(1)         NOT NULL,
    statni_obcanstvi            VARCHAR2(255)   NOT NULL,
    adresa_trvala               VARCHAR2(255)   NOT NULL,
    adresa_aktualni             VARCHAR2(255)   NOT NULL,
    vyska                       NUMBER(5)       NOT NULL,
    vaha                        NUMBER(5)       NOT NULL,
    barva_oci                   VARCHAR2(31)    NOT NULL,
    barva_vlasu                 VARCHAR2(31),
    zvlastni_znaky              CLOB,
    rizikovy_level              NUMBER(3)       NOT NULL,
    pocet_pripadu               NUMBER(10)      NOT NULL,
    pocet_odsouzeni             NUMBER(10)      NOT NULL,
    datum_registrace            DATE            NOT NULL,
    posledni_aktualizace        TIMESTAMP       NOT NULL,
    autor_posledni_aktualizace  VARCHAR2(255)   NOT NULL,
    poznamky                    CLOB,
    interni_komentar            CLOB,
    CONSTRAINT pk_clovek PRIMARY KEY (cid),
    CONSTRAINT fk_clovek_adresa_trvala
        FOREIGN KEY (adresa_trvala)   REFERENCES Adresa (adresa),
    CONSTRAINT fk_clovek_adresa_aktualni
        FOREIGN KEY (adresa_aktualni) REFERENCES Adresa (adresa)
);

-- ------------------------------------------------------------
-- 3. PRIPAD  (nezávislá tabulka)
-- ------------------------------------------------------------
CREATE TABLE Pripad (
    pid                         NUMBER(10)      NOT NULL,
    cislo_pripadu               VARCHAR2(10)    NOT NULL,
    datum_zahajeni              DATE            NOT NULL,
    datum_ukonceni              DATE,
    stav                        VARCHAR2(255)   NOT NULL,
    priorita                    NUMBER(3)       NOT NULL,
    typ_pripadu                 VARCHAR2(255)   NOT NULL,
    popis                       CLOB,
    poznamky                    CLOB,
    vedouci_pripad              VARCHAR2(255)   NOT NULL,
    posledni_aktualizace        TIMESTAMP       NOT NULL,
    autor_posledni_aktualizace  VARCHAR2(255)   NOT NULL,
    CONSTRAINT pk_pripad PRIMARY KEY (pid)
);

-- ------------------------------------------------------------
-- 4. ZLOCIN  (FK → Adresa)
-- ------------------------------------------------------------
CREATE TABLE Zlocin (
    zid                         NUMBER(10)      NOT NULL,
    zavaznost                   NUMBER(3)       NOT NULL,
    nazev                       VARCHAR2(255)   NOT NULL,
    typ                         VARCHAR2(255)   NOT NULL,
    popis                       CLOB,
    datum_zlocinu               DATE            NOT NULL,
    misto_zlocinu               VARCHAR2(255)   NOT NULL,
    trestni_stupen              VARCHAR2(31)    NOT NULL,
    posledni_aktualizace        TIMESTAMP       NOT NULL,
    autor_posledni_aktualizace  VARCHAR2(255)   NOT NULL,
    CONSTRAINT pk_zlocin PRIMARY KEY (zid),
    CONSTRAINT fk_zlocin_adresa
        FOREIGN KEY (misto_zlocinu) REFERENCES Adresa (adresa)
);

-- ------------------------------------------------------------
-- 5. DUKAZ  (FK → Adresa, Clovek)
-- ------------------------------------------------------------
CREATE TABLE Dukaz (
    evid                        NUMBER(10)      NOT NULL,
    typ                         VARCHAR2(255)   NOT NULL,
    popis                       CLOB            NOT NULL,
    datum_ziskani               DATE            NOT NULL,
    misto_ziskani               VARCHAR2(255)   NOT NULL,
    zodpovedna_osoba            NUMBER(10)      NOT NULL,
    stav                        VARCHAR2(255)   NOT NULL,
    datum_registrace            DATE            NOT NULL,
    posledni_aktualizace        TIMESTAMP       NOT NULL,
    autor_posledni_aktualizace  VARCHAR2(255)   NOT NULL,
    poznamky                    CLOB,
    CONSTRAINT pk_dukaz PRIMARY KEY (evid),
    CONSTRAINT fk_dukaz_adresa
        FOREIGN KEY (misto_ziskani)     REFERENCES Adresa (adresa),
    CONSTRAINT fk_dukaz_clovek
        FOREIGN KEY (zodpovedna_osoba)  REFERENCES Clovek (cid)
);

-- ------------------------------------------------------------
-- 6. DUSLEDEK  (nezávislá tabulka)
-- ------------------------------------------------------------
CREATE TABLE Dusledek (
    cons_id                     NUMBER(10)      NOT NULL,
    nazev                       VARCHAR2(63)    NOT NULL,
    typ                         VARCHAR2(63)    NOT NULL,
    delka                       NUMBER(10),
    podminky                    CLOB,
    popis                       CLOB,
    posledni_aktualizace        TIMESTAMP       NOT NULL,
    autor_posledni_aktualizace  VARCHAR2(255)   NOT NULL,
    poznamky                    CLOB,
    CONSTRAINT pk_dusledek PRIMARY KEY (cons_id)
);

-- ------------------------------------------------------------
-- 7. ROLE  (nezávislá tabulka)
-- ------------------------------------------------------------
CREATE TABLE Role (
    roid                        NUMBER(10)      NOT NULL,
    nazev                       VARCHAR2(63)    NOT NULL,
    popis                       CLOB,
    aktivni                     NUMBER(1)       NOT NULL,   -- 0 = false, 1 = true
    posledni_aktualizace        TIMESTAMP       NOT NULL,
    autor_posledni_aktualizace  VARCHAR2(255),
    poznamky                    CLOB,
    CONSTRAINT pk_role PRIMARY KEY (roid),
    CONSTRAINT chk_role_aktivni CHECK (aktivni IN (0, 1))
);

-- ------------------------------------------------------------
-- 8. CLOVEK_PRIPAD  (vazební tabulka; FK → Clovek, Pripad, Role)
-- ------------------------------------------------------------
CREATE TABLE Clovek_Pripad (
    cid     NUMBER(10),               -- může být NULL (neznámý pachatel apod.)
    pid     NUMBER(10)  NOT NULL,
    roid    NUMBER(10)  NOT NULL,
    CONSTRAINT pk_clovek_pripad  PRIMARY KEY (pid, roid),
    CONSTRAINT fk_cp_clovek FOREIGN KEY (cid)  REFERENCES Clovek (cid),
    CONSTRAINT fk_cp_pripad FOREIGN KEY (pid)  REFERENCES Pripad (pid),
    CONSTRAINT fk_cp_role   FOREIGN KEY (roid) REFERENCES Role   (roid)
);

-- ------------------------------------------------------------
-- 9. PRIPAD_ZLOCIN  (FK → Pripad, Zlocin)
-- ------------------------------------------------------------
CREATE TABLE Pripad_Zlocin (
    pid  NUMBER(10)  NOT NULL,
    zid  NUMBER(10)  NOT NULL,
    CONSTRAINT pk_pripad_zlocin PRIMARY KEY (pid, zid),
    CONSTRAINT fk_pz_pripad FOREIGN KEY (pid) REFERENCES Pripad (pid),
    CONSTRAINT fk_pz_zlocin FOREIGN KEY (zid) REFERENCES Zlocin (zid)
);

-- ------------------------------------------------------------
-- 10. PRIPAD_DUKAZ  (FK → Pripad, Dukaz)
-- ------------------------------------------------------------
CREATE TABLE Pripad_Dukaz (
    pid   NUMBER(10)  NOT NULL,
    evid  NUMBER(10),               -- může být NULL
    CONSTRAINT pk_pripad_dukaz PRIMARY KEY (pid),
    CONSTRAINT fk_pd_pripad FOREIGN KEY (pid)  REFERENCES Pripad (pid),
    CONSTRAINT fk_pd_dukaz  FOREIGN KEY (evid) REFERENCES Dukaz  (evid)
);

-- ------------------------------------------------------------
-- 11. DUSLEDEK_PRIPAD  (FK → Clovek, Pripad, Dusledek)
-- ------------------------------------------------------------
CREATE TABLE Dusledek_Pripad (
    cid      NUMBER(10)  NOT NULL,
    pid      NUMBER(10)  NOT NULL,
    cons_id  NUMBER(10)  NOT NULL,
    CONSTRAINT pk_dusledek_pripad PRIMARY KEY (cid, pid, cons_id),
    CONSTRAINT fk_dp_clovek   FOREIGN KEY (cid)     REFERENCES Clovek   (cid),
    CONSTRAINT fk_dp_pripad   FOREIGN KEY (pid)     REFERENCES Pripad   (pid),
    CONSTRAINT fk_dp_dusledek FOREIGN KEY (cons_id) REFERENCES Dusledek (cons_id)
);

-- ============================================================
-- Hotovo – všechny tabulky a vazby jsou vytvořeny.
-- ============================================================
```

## Architektura projektu

Projekt implementuje třívrstvou arquitekturu:

1. **Database.cs** — Helper třída pro správu Oracle připojení a transakcí
   - Obsluhuje `OracleConnection` lifecycle (Connect/Close)
   - Spravuje transakce (`BeginTransaction`, `EndTransaction`, `Rollback`)
   - Zajišťuje `BindByName = true` pro všechny příkazy
   - Sdílí transakci mezi vrstvami přes parametr `pDb`

2. **DTO vrstva** (orm/dto/) — Data Transfer Objects, jen datové kontejnery bez logiky
   - `ClovekDto`, `PripadDto`, `ClovekPripadDto`
   - Property názvy odpovídají přesně sloupcům v DB (lowercase snake_case)
   - Nullable properties (`DateTime?`, `string?`, `int?`) zrcadlí `NULL` ve schématu

3. **DAO vrstva** (orm/dao/) — Data Access Objects, statické metody pro DB operace
   - `ClovekDao` — operace s osobami
   - `PripadDao` — operace s případy (čtení stavu, počítání aktivních)
   - `ClovekPripadDao` — operace s vazbou osoba-případ
   - `TransactionsDao` — komplexní transakční operace (C# a PL/SQL)
   - Všechny metody jsou **statické** — nejsou potřeba instance
   - První parametr je vždy `Database pDb` — umožňuje sdílení transakce

4. **Program.cs** — Vstupní bod, testovací volání obou implementací

---

## Soubory

### App.config
**Účel:** Konfigurační soubor .NET Framework, obsahuje Oracle connection string

**Obsah a vysvětlení:**

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <!-- connectionStrings — oddíl pro všechna připojení -->
  <connectionStrings>
    <!-- name="ConnectionStringOracle" — identifikátor, kterým se odkazujeme v kódu -->
    <add name="ConnectionStringOracle"
         <!-- 
         Oracle-specifický format:
         - (DESCRIPTION=...) — popis kako se připojit
           - (ADDRESS=...) — síťová adresa serveru
             - PROTOCOL=TCP — komunikační protokol
             - HOST=bayer.cs.vsb.cz — server
             - PORT=1521 — standardní Oracle port
           - (CONNECT_DATA=(SID=oracle)) — databáze identifikátor (SID = System ID)
         - User Id=HEJ0094 — uživatel
         - Password=... — heslo
         POZNÁMKA: Oracle používá "User Id", SQL Server by používal "User"
         -->
         connectionString="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=bayer.cs.vsb.cz)(PORT=1521))(CONNECT_DATA=(SID=oracle)));User Id=HEJ0094;Password=H84J9Ej8KdsJYzL5;"
         providerName="Oracle.ManagedDataAccess.Client" />
  </connectionStrings>
  
  <!-- startup — .NET Framework startupní konfigurace -->
  <startup>
    <!-- supportedRuntime — kterou verzi .NET Framework projekt vyžaduje (4.8) -->
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8" />
  </startup>
</configuration>
```

**Klíčové poznámky:**

- **Proč connection string v App.config a ne v kódu?**
  - Bezpečnost: hesla nejsou v source code
  - Flexibility: snadno změnit server/databázi bez rebuild
  - DevOps best practice: oddělení konfigurace od kódu

- **Proč Oracle DESCRIPTION format vs SQL Server?**
  - SQL Server: `Data Source=server;Initial Catalog=database;...`
  - Oracle: Komplexnější (ADDRESS s HOST/PORT/SID) protože historicky připojení bylo složitější
  - DESCRIPTION je syntaxa z Oracle Net Configuration

- **Proč `Oracle.ManagedDataAccess` a ne starší `Oracle.DataAccess`?**
  - "Managed" = pure .NET, není potřeba Oracle Client nainstalovaný lokálně
  - "Unmanaged" (stará verze) vyžadovala Oracle Client instalaci
  - Modernější, lépe supportovaný, Microsoft recommended

### Adresářová struktura
```
DSI_cv11/
├── App.config                          ← Oracle connection string
├── Database.cs                         ← (příště) helper třída
├── Program.cs                          ← (příště) vstupní bod
├── DOC.md                              ← (tento soubor)
├── files/
│   └── PridatOsobuDoPripadu.sql        ← (příště) PL/SQL procedura
└── orm/
    ├── dto/
    │   ├── ClovekDto.cs
    │   ├── PripadDto.cs
    │   └── ClovekPripadDto.cs
    └── dao/
        ├── ClovekDao.cs
        ├── PripadDao.cs
        ├── ClovekPripadDao.cs
        └── TransactionsDao.cs
```

---

### Database.cs
**Účel:** Unmodifiable (po vytvoření NIKDY se nemění) helper třída pro správu Oracle připojení, transakcí a příkazů

**Using statements a jejich role:**
```csharp
using System;                                  // základní třídy (Convert, DBNull)
using System.Data;                             // IsolationLevel, CommandType, ParameterDirection enumerace
using System.Configuration;                    // ConfigurationManager pro čtení App.config
using Oracle.ManagedDataAccess.Client;         // OracleConnection, OracleCommand, OracleTransaction
```

**Privátní pole:**
```csharp
private OracleConnection  _connection;         // Oracle TCP connection
private OracleTransaction _transaction;        // aktuální transakce (NULL když není)
private bool              _ownsConnection;     // flag — kdo má zodpovědnost za zavření
```

**Privátní konstruktor:**
```csharp
private Database() { }                         // nijak inicializuje — všechno je static metodami
```

**Metoda: `Connect(Database pDb = null)`**
```csharp
public static Database Connect(Database pDb = null)
{
    // Pokud pDb != null, znamená to: "Sdílej moje transakci"
    // Vrátíme pDb beze změny, pouze nastavíme _ownsConnection = false
    // → Close() pak nebude zavírat spojení (protože ho "nevlastníme")
    if (pDb != null) { pDb._ownsConnection = false; return pDb; }

    // Jinak: vytvoř nové spojení
    Database db = new Database();
    // Přečti connection string z App.config
    string cs   = ConfigurationManager
                    .ConnectionStrings["ConnectionStringOracle"]
                    .ConnectionString;
    // Vytvoř Oracle connection a otevři
    db._connection = new OracleConnection(cs);
    db._connection.Open();
    // MY jsme vlastníci → v Close() se zavře
    db._ownsConnection = true;
    return db;
}
```

**Proč `Connect(pDb)` pattern?**
- `TransactionsDao.PridatOsobuDoPripadu(null, ...)` → nové spojení, nová transakce
- `PripadDao.GetStav(db, ...)` volaný UVNITŘ transakce → `db` je předáno, sdílíme transakci
- Bezpečí: cada DAO metoda si "vezme" své spojení, ale když je ve transakci, všechny se sdílí

**Metoda: `Close(Database pDb, Database db)`**
```csharp
public static void Close(Database pDb, Database db)
{
    // Pouze pokud WE vlastní spojení (vytvořili jsme si ho), zavřeme ho
    if (db._ownsConnection) db._connection.Close();
    // Pokud _ownsConnection = false (tzn. sdílela se), necháme ho otevřené
}
```

**Metoda: `BeginTransaction()`**
```csharp
public void BeginTransaction()
{
    // Serializable = nejpřísnější izolační úroveň
    // → zabavuje "phantom read" — osoba nesmí mít najednou >= 10 a < 10 případů
    _transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
}
```

**Proč `Serializable`?**
- `ReadCommitted` (výchozí): může nastat "phantom read" — COUNT(STAV='open') by se změnil mezi Select a Insert
- `Serializable`: zamyká reader, žádné dalšíí transakce nemohou měnit data mezi našimi příkazy
- Cena: pomalejší, ale korektní

**Metoda: `EndTransaction()`**
```csharp
public void EndTransaction()
{
    // COMMIT všechny příkazy v transakci
    _transaction.Commit();
    // Vyčisti referenci
    _transaction = null;
}
```

**Metoda: `Rollback()`**
```csharp
public void Rollback()
{
    // Bezpečně: pouze pokud transakce existuje
    if (_transaction != null) { _transaction.Rollback(); _transaction = null; }
}
```

**Metoda: `CreateCommand(string sql)`**
```csharp
public OracleCommand CreateCommand(string sql)
{
    OracleCommand cmd  = new OracleCommand(sql, _connection);
    cmd.BindByName     = true;
    // ↑ KRITICKÉ: parametry se vážou jménem (`:cid`), ne pozicí
    // Bez toho by `:cid, :pid, :cid` byl bugem (pozice by selhala)
    
    if (_transaction != null) cmd.Transaction = _transaction;
    // ↑ Pokud máme transakci: připoj ji příkazu
    // Bez toho by se přikaz commitnul okamžitě (nepožadované)
    return cmd;
}
```

**Metoda: `ExecuteScalar(OracleCommand command)`**
```csharp
public int ExecuteScalar(OracleCommand command)
{
    object result = command.ExecuteScalar();
    // SELECT vracíod 1 řádek, 1 sloupec (např. COUNT)
    // Pokud NULL nebo DBNull: vrať 0 (bezpečný default)
    return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
}
```

**Metoda: `ExecuteNonQuery(OracleCommand command)`**
```csharp
public int ExecuteNonQuery(OracleCommand command)
{
    // INSERT/UPDATE/DELETE: vrací počet ovlivněných řádků
    return command.ExecuteNonQuery();
}
```

**Metoda: `Select(OracleCommand command)`**
```csharp
public OracleDataReader Select(OracleCommand command)
{
    // SELECT: vrací reader, který musí volající zavřít (reader.Close())
    return command.ExecuteReader();
}
```

---

### orm/dto/ClovekPripadDto.cs
**Účel:** DTO (Data Transfer Object) reprezentující řádek z tabulky `CLOVEK_PRIPAD` (vazba osoba↔případ↔role)

```csharp
public class ClovekPripadDto
{
    // CID — ID osoby
    // NULL povoleno: tabulka CLOVEK_PRIPAD.CID je nullable (dle DDL)
    public int?  cid  { get; set; }
    
    // PID — ID případ (NOT NULL)
    public int   pid  { get; set; }
    
    // ROID — ID role osoby v případu (NOT NULL)
    public int   roid { get; set; }
}
```

**Proč nullable `cid` ale non-nullable `pid` a `roid`?**
- DDL schéma: `CID NUMBER NULL, PID NUMBER NOT NULL, ROID NUMBER NOT NULL`
- C#: `int?` znamená nullable (zabaleno v `Nullable<int>`), `int` je vždy inicializovaný

---

### orm/dto/PripadDto.cs
**Účel:** DTO reprezentující řádek z tabulky `PRIPAD` (právní případ)

```csharp
public class PripadDto
{
    public int       pid                        { get; set; }  // PK, NOT NULL
    public string    cislo_pripadu              { get; set; }  // NOT NULL
    public DateTime  datum_zahajeni             { get; set; }  // NOT NULL, DATE
    
    // NULL povoleno: případ může být stále otevřen
    public DateTime? datum_ukonceni             { get; set; }  // DATE NULL
    
    public string    stav                       { get; set; }  // NOT NULL, VARCHAR2(255)
    public int       priorita                   { get; set; }  // NOT NULL, NUMBER
    public string    typ_pripadu                { get; set; }  // NOT NULL, VARCHAR2(255)
    
    // Nullable fieldy — popis a poznámky nejsou povinné
    public string?   popis                      { get; set; }  // VARCHAR2(4000) NULL
    public string?   poznamky                   { get; set; }  // VARCHAR2(4000) NULL
    
    public string    vedouci_pripad             { get; set; }  // NOT NULL, VARCHAR2(255)
    public DateTime  posledni_aktualizace       { get; set; }  // TIMESTAMP NOT NULL
    public string    autor_posledni_aktualizace { get; set; }  // VARCHAR2(255) NOT NULL
}
```

**Proč `DateTime?` pro `datum_ukonceni`?**
- Případ má během svého života stavy: `open`, `running`, později `closed` nebo `unsolved`
- Když je otevřen/běží, `datum_ukonceni` je NULL
- Když se uzavře, `datum_ukonceni` se vyplní
- Oracle `NULL` → C# `null` → typ musí být nullable (`DateTime?`)

---

### orm/dto/ClovekDto.cs
**Účel:** DTO reprezentující řádek z tabulky `CLOVEK` (osoba v systému)

```csharp
public class ClovekDto
{
    // Personální identifikátory
    public int      cid                        { get; set; }  // PK, NOT NULL
    public string   rodne_cislo                { get; set; }  // NOT NULL, VARCHAR2(10)
    public string   jmeno                      { get; set; }  // NOT NULL, VARCHAR2(255)
    public string   prijmeni                   { get; set; }  // NOT NULL, VARCHAR2(31)
    
    // Základní data
    public DateTime datum_narozeni             { get; set; }  // NOT NULL, DATE
    public string   misto_narozeni             { get; set; }  // NOT NULL, VARCHAR2(255)
    public char     pohlavi                    { get; set; }  // NOT NULL, CHAR(1)
    public string   statni_obcanstvi           { get; set; }  // NOT NULL, VARCHAR2(255)
    
    // Adresa
    public string   adresa_trvala              { get; set; }  // NOT NULL, VARCHAR2(255)
    public string   adresa_aktualni            { get; set; }  // NOT NULL, VARCHAR2(255)
    
    // Fyzický popis
    public int      vyska                      { get; set; }  // NOT NULL, NUMBER
    public int      vaha                       { get; set; }  // NOT NULL, NUMBER
    public string   barva_oci                  { get; set; }  // NOT NULL, VARCHAR2(31)
    public string?  barva_vlasu                { get; set; }  // NULL povoleno
    public string?  zvlastni_znaky             { get; set; }  // VARCHAR2(4000) NULL (např. tetování)
    
    // Systémová data a čítače
    public int      rizikovy_level             { get; set; }  // NOT NULL, NUMBER
    public int      pocet_pripadu              { get; set; }  // NOT NULL, počet aktuálních případů
    public int      pocet_odsouzeni            { get; set; }  // NOT NULL, počet odsouzení
    
    // Audit a poslední změna
    public DateTime datum_registrace           { get; set; }  // NOT NULL, DATE
    public DateTime posledni_aktualizace       { get; set; }  // NOT NULL, TIMESTAMP
    public string   autor_posledni_aktualizace { get; set; }  // NOT NULL, VARCHAR2(255)
    
    // Komentáře
    public string?  poznamky                   { get; set; }  // VARCHAR2(4000) NULL
    public string?  interni_komentar           { get; set; }  // VARCHAR2(4000) NULL
}
```

**Proč `string?` na `barva_vlasu`, `zvlastni_znaky`, atd.?**
- DDL: `BARVA_VLASU VARCHAR2(31) NULL` — osoba může být plešatá (NULL)
- `ZVLASTNI_ZNAKY VARCHAR2(4000) NULL` — ne každá osoba má viditelné značky

---

## Poznámky k DTOs

**Proč DTO vůbec?**
- Oddělení dat od logiky (DTO je jen container)
- Snadnější testování (DTOs bez dependencies)
- Jasné rozhraní mezi DAO a业务 logika

**Mapování typů Oracle → C#:**
- Oracle `NUMBER` → C# `int` (nebo `int?` když nullable)
- Oracle `VARCHAR2` → C# `string` (nebo `string?` když nullable)
- Oracle `DATE` / `TIMESTAMP` → C# `DateTime` (nebo `DateTime?`)
- Oracle `CHAR(1)` → C# `char`

**Proč lowercase snake_case v C#?**
- Instrukce: property názvy = přesně jak v DB (lowercase snake_case)
- Usnadňuje mapping — při čtení z `OracleDataReader` se jméno property rovná názvu sloupce
- Atypické pro C# (obvyklá konvence je PascalCase), ale specifikace projektu

---

### orm/dao/ClovekPripadDao.cs
**Účel:** DAO (Data Access Object) pro operace na vazbě osoba↔případ↔role

```csharp
public static class ClovekPripadDao
{
    // Proč private static string? 
    // - Konstantní SQL dotaz pro třídu
    // - Viditelné na jednom místě pro maintenance
    // - Ne lokální proměnná (byla by nečitelná)
    private static string SqlExists =
        "SELECT COUNT(*) FROM CLOVEK_PRIPAD WHERE CID = :cid AND PID = :pid";
        // :cid, :pid — Oracle parametry (nikoliv @cid jako SQL Server!)

    public static bool Exists(Database pDb, int cid, int pid)
    {
        // pDb = null → nové spojení, pDb != null → sdíl transakci
        Database db = Database.Connect(pDb);
        
        OracleCommand command = db.CreateCommand(SqlExists);
        
        // OracleParameter("cid", hodnota)
        // Proč ne AddWithValue? Explicitnější, lépe pracuje s Oracle driverem
        command.Parameters.Add(new OracleParameter("cid", cid));
        command.Parameters.Add(new OracleParameter("pid", pid));
        
        // ExecuteScalar = vrací jeden scalar (COUNT(*))
        int count = db.ExecuteScalar(command);
        Database.Close(pDb, db);
        
        return count > 0;
    }

    private static string SqlInsert =
        "INSERT INTO CLOVEK_PRIPAD (CID, PID, ROID) VALUES (:cid, :pid, :roid)";

    public static void Insert(Database pDb, ClovekPripadDto cp)
    {
        Database db = Database.Connect(pDb);
        OracleCommand command = db.CreateCommand(SqlInsert);
        
        // CID je nullable (int?)
        // Jak předat NULL parametru?
        // - Pokud cp.cid.HasValue: použij hodnotu
        // - Jinak: DBNull.Value (Oracle NULL)
        command.Parameters.Add(new OracleParameter("cid",
            cp.cid.HasValue ? (object)cp.cid.Value : DBNull.Value));
        
        command.Parameters.Add(new OracleParameter("pid",  cp.pid));
        command.Parameters.Add(new OracleParameter("roid", cp.roid));
        
        db.ExecuteNonQuery(command);
        Database.Close(pDb, db);
    }
}
```

**Klíčové body:**
- Obě metody jsou **statické** — nejsou potřeba instance
- `Exists()` — detektor: vrací `bool` (duplikáta check)
- `Insert()` — action: nic nevrací (`void`)
- Proč `reader.Close()` PŘED `Database.Close(pDb, db)`? Aby se reader uzavřel v kontextu conexe

---

### orm/dao/ClovekDao.cs
**Účel:** DAO pro operace na osobě

```csharp
public static class ClovekDao
{
    private static string SqlIncrementPocetPripadu =
        @"UPDATE CLOVEK
          SET POCET_PRIPADU              = POCET_PRIPADU + 1,
              POSLEDNI_AKTUALIZACE       = SYSDATE,
              AUTOR_POSLEDNI_AKTUALIZACE = :autor
          WHERE CID = :cid";
    // @ — verbatim string (povoleny newline bez escape)
    // SYSDATE — Oracle time (né DateTime.Now v C#!)
    // Proč? Databáze má vlastní čas, zajišťuje atomicitu v multi-node prostředí

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
```

**Poznámka:**
- `POCET_PRIPADU = POCET_PRIPADU + 1` — atomická operace v DB (ne SELECT, +1, UPDATE z C#)
- `SYSDATE` — aktuální server čas (garantuje konzistenci)
- `AUTOR_POSLEDNI_AKTUALIZACE` — audit trail (kdo udělal poslední změnu)

---

### orm/dao/PripadDao.cs
**Účel:** DAO pro operace na případu

```csharp
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
        // ExecuteReader() vrací OracleDataReader
        OracleDataReader reader = db.Select(command);
        
        // Přečti první řádek
        if (reader.Read())
            // Sloupec 0 = STAV
            // reader.IsDBNull(0)? — ověř NULL PŘED GetString (bezpečnost)
            stav = reader.IsDBNull(0) ? null : reader.GetString(0);
        
        // !! KRITICKÉ !! reader.Close() PŘED Database.Close()
        // Jinak by reader zůstal otevřený a zamykal by connection
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
        
        // ExecuteScalar — vrací COUNT(*) jako int
        int count = db.ExecuteScalar(command);
        Database.Close(pDb, db);
        
        return count;
    }
}
```

**Klíčové body:**
- `GetStav()` — kontrola stavu případu (je aktivní?)
- `CountAktivniPripady()` — počítač pro krok 3 transakce (má osoba >= 10 aktivních?)
- `reader.IsDBNull(0)` — VŽDY ověř NULL PŘED `GetXxx()`
- `reader.Close()` — uvolnění prostředků reader-u

---

## Sdílení transakce v DAO

**Problém:** Když voláme DAO v transakci, všechny musí sdílet stejné spojení!

**Řešení: Pattern `Connect(pDb)`**

```csharp
// Volání z TransactionsDao.PridatOsobuDoPripadu:
Database db = Database.Connect(null);
db.BeginTransaction();

// Uvnitř transakce předáváme `db`, ne `null`:
string stav = PripadDao.GetStav(db, pid);           // ← db, ne null!
bool exists = ClovekPripadDao.Exists(db, cid, pid); // ← db, ne null!
int count = PripadDao.CountAktivniPripady(db, cid); // ← db, ne null!

// Každá metoda si vezme `db`:
// → Connect(db) vrátí db beze změny
// → _ownsConnection = false
// → Close(pDb, db) se NEBUDE zavírat
```

---

### orm/dao/TransactionsDao.cs
**Účel:** DAO pro komplexní transakční operace s plnou validací v C#

```csharp
using System;
using Oracle.ManagedDataAccess.Client;

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
            // !! KRITICKÉ !! BeginTransaction MUSÍ být PRVNÍ příkaz
            // Bez něj by následující DAO volání commitnula okamžitě (nepožadované)
            db.BeginTransaction();

            // ─────────────────────────────────────────────────────────────
            // Krok 1: Případ musí existovat a být aktivní (NEJLEVNĚJŠÍ dotaz)
            // ─────────────────────────────────────────────────────────────
            string stav = PripadDao.GetStav(db, p_pid);
            // Pokud případ neexistuje: stav = null
            // Pokud případ existuje ale není "open" či "running": chyba
            if (stav == null || (stav != "open" && stav != "running"))
                throw new InvalidOperationException(
                    $"Případ {p_pid} neexistuje nebo není aktivní (stav='{stav}').");
            // Proč throw a ne return false?
            // → throw skočí do catch → zavolá se Rollback
            // → return false by si museli myslet na Rollback sami

            // ───────────────────────────────────────────────────────────────
            // Krok 2: Osoba nesmí být v případu duplicitně (LEVNÝ)
            // ───────────────────────────────────────────────────────────────
            if (ClovekPripadDao.Exists(db, p_cid, p_pid))
                throw new InvalidOperationException(
                    $"Osoba {p_cid} již v případu {p_pid} figuruje.");

            // ──────────────────────────────────────────────────────────────
            // Krok 3: Osoba nesmí mít >= 10 aktivních případů (DRAŽŠÍ dotaz)
            // ──────────────────────────────────────────────────────────────
            // Pořadí kroků! 1→2→3 = od nejlevnějšího po nejdražší
            // Proč? Když skončí s chybou, není zbytečný drag
            int aktivni = PripadDao.CountAktivniPripady(db, p_cid);
            if (aktivni >= 10)
                throw new InvalidOperationException(
                    $"Osoba {p_cid} má již {aktivni} aktivních případů (max 10).");

            // ──────────────────────────────────────────────────────────────
            // Krok 4: Vložit vazbu osoba↔případ↔role
            // ──────────────────────────────────────────────────────────────
            ClovekPripadDto vazba = new ClovekPripadDto
            {
                cid  = p_cid,
                pid  = p_pid,
                roid = p_roid
            };
            ClovekPripadDao.Insert(db, vazba);

            // ──────────────────────────────────────────────────────────────
            // Krok 5: Zvýšit čítač případů osoby, aktualizovat audit
            // ──────────────────────────────────────────────────────────────
            ClovekDao.IncrementPocetPripadu(db, p_cid, p_autor);

            // ──────────────────────────────────────────────────────────────
            // COMMIT — všechny příkazy jsou OK
            // ──────────────────────────────────────────────────────────────
            db.EndTransaction();
        }
        // CATCH 1: Oracle-specifické chyby (network error, constraint violation, atd.)
        catch (OracleException ex)
        {
            Console.Error.WriteLine($"[Oracle chyba] {ex.Message}");
            // Rollback — vrať DB do původního stavu
            db.Rollback();
            ret = false;
        }
        // CATCH 2: Naše business validace
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"[Podmínka] {ex.Message}");
            // Rollback — vrať DB do původního stavu
            db.Rollback();
            ret = false;
        }

        // !! MIMO try/catch !!
        // Vždy se musíme zavřít, bez ohledu na průběh
        Database.Close(pDb, db);
        return ret;
    }
}
```

**Klíčové body transakce:**

1. **Pořadí kroků 1→2→3** (od nejlevnějšího po nejdražší)
   - Krok 1: SELECT STAV — jeden řádek
   - Krok 2: SELECT COUNT(*) — jeden výsledek
   - Krok 3: SELECT COUNT(*) s JOIN — potenciálně mnoho řádků
   - Pokud selheme na kroku 1, není zbytečně drag ostatních

2. **`throw` vs `return false`**
   - `throw` → skočí do `catch` → `Rollback()` je automaticky zavolán
   - `return false` → nám bychom si museli myslet na `Rollback` sami (nebezpečí!)

3. **Sdílení transakce**
   - Všechna DAO volání dostávají `db`, ne `null`
   - `Database.Connect(db)` vrátí `db` beze změny
   - Všechny příkazy jsou v Serializable transakci

4. **Try/Catch/Finally pattern**
   - `OracleException` — DB-level chyby (network, constraint, permissions)
   - `InvalidOperationException` — business validace (STAV != 'open', duplicita, atd.)
   - `Database.Close()` je MIMO `try/catch` → vždy se zavolá

5. **Proč `Serializable` izolační úroveň?**
   - `ReadCommitted` (default) by měl "phantom read":
     - Transakce A: COUNT() → vrátí 5
     - Transakce B vloží nový řádek
     - Transakce A: COUNT() znova → vrátí 6!
   - `Serializable`: zámek na tabulku → žádná konkurenční úprava během našeho čtení

6. **Atomicita UPDATE v kroku 5**
   - `POCET_PRIPADU = POCET_PRIPADU + 1` v SQL (nikoliv SELECT, +1, UPDATE z C#)
   - Zabraňuje race conditions

---

### files/PridatOsobuDoPripadu.sql
**Účel:** PL/SQL stored procedure — serverová implementace stejné logiky jako C# TransactionsDao

```sql
CREATE OR REPLACE PROCEDURE PridatOsobuDoPripadu_sp (
    -- Vstupní parametry (IN)
    p_cid   IN  NUMBER,       -- ID osoby
    p_pid   IN  NUMBER,       -- ID případu
    p_roid  IN  NUMBER,       -- ID role
    p_autor IN  VARCHAR2,     -- Audit: kdo to dělá
    
    -- Výstupní parametr (OUT)
    p_ret   OUT NUMBER        -- 0 = chyba/podmínka selhala, 1 = úspěch
) AS
    -- Lokální proměnné
    v_stav      VARCHAR2(255);   -- Stav případu
    v_existuje  NUMBER;          -- Počet existujících vazbí
    v_aktivni   NUMBER;          -- Počet aktivních případů
BEGIN
    -- Bezpečnostní default: vrať chybu na začátku
    p_ret := 0;

    -- ──────────────────────────────────────────────────────────────────
    -- Krok 1: Případ musí existovat a být aktivní
    -- ──────────────────────────────────────────────────────────────────
    BEGIN
        -- SELECT INTO — Oracle způsob jak přečíst hodnotu do proměnné
        -- FOR UPDATE — zamykáme řádek (obdoba SQL Server WITH (UPDLOCK))
        -- Zabezpečuje Serializable chování
        SELECT STAV INTO v_stav
        FROM PRIPAD
        WHERE PID = p_pid
        FOR UPDATE;
    EXCEPTION
        -- NO_DATA_FOUND — SQL vrátil 0 řádků (případ neexistuje)
        WHEN NO_DATA_FOUND THEN
            ROLLBACK;
            RETURN;        -- Vrať se z procedury, p_ret zůstane 0
    END;

    -- Pokud STAV není 'open' ani 'running': chyba
    IF v_stav NOT IN ('open', 'running') THEN
        ROLLBACK;
        RETURN;
    END IF;

    -- ──────────────────────────────────────────────────────────────────
    -- Krok 2: Osoba nesmí být v případu duplicitně
    -- ──────────────────────────────────────────────────────────────────
    SELECT COUNT(*) INTO v_existuje
    FROM CLOVEK_PRIPAD
    WHERE CID = p_cid AND PID = p_pid;

    IF v_existuje > 0 THEN
        ROLLBACK;
        RETURN;
    END IF;

    -- ──────────────────────────────────────────────────────────────────
    -- Krok 3: Osoba nesmí mít >= 10 aktivních případů
    -- ──────────────────────────────────────────────────────────────────
    SELECT COUNT(*) INTO v_aktivni
    FROM CLOVEK_PRIPAD cp
    JOIN PRIPAD p ON p.PID = cp.PID
    WHERE cp.CID = p_cid
      AND p.STAV IN ('open', 'running');

    IF v_aktivni >= 10 THEN
        ROLLBACK;
        RETURN;
    END IF;

    -- ──────────────────────────────────────────────────────────────────
    -- Krok 4: Vložit novou vazbu osoba↔případ↔role
    -- ──────────────────────────────────────────────────────────────────
    INSERT INTO CLOVEK_PRIPAD (CID, PID, ROID)
    VALUES (p_cid, p_pid, p_roid);

    -- ──────────────────────────────────────────────────────────────────
    -- Krok 5: Zvýšit čítač případů osoby, aktualizovat audit
    -- ──────────────────────────────────────────────────────────────────
    UPDATE CLOVEK
    SET POCET_PRIPADU              = POCET_PRIPADU + 1,
        POSLEDNI_AKTUALIZACE       = SYSDATE,
        AUTOR_POSLEDNI_AKTUALIZACE = p_autor
    WHERE CID = p_cid;

    -- ──────────────────────────────────────────────────────────────────
    -- COMMIT — všechny příkazy jsou OK, ulož do DB
    -- ──────────────────────────────────────────────────────────────────
    COMMIT;
    p_ret := 1;             -- Vrať úspěch

-- ────────────────────────────────────────────────────────────────────────
-- EXCEPTION WHEN OTHERS — catch-all pro ALL ostatní chyby
-- ────────────────────────────────────────────────────────────────────────
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        p_ret := 0;         -- Vrať chybu
END;
/
```

**Klíčové Oracle-specifické prvky:**

1. **`CREATE OR REPLACE`**
   - Vytvoří proceduru NEBO ji nahradí, pokud už existuje
   - Bezpečné pro re-deployment
   - SQL Server by měl `CREATE OR ALTER`

2. **`IN / OUT` parametry**
   - `IN` — jen vstup (procedura nemůže měnit)
   - `OUT` — jen výstup (procedura jí nastavuje)
   - `IN OUT` — obojí (zde se nepoužívá)

3. **`p_ret OUT NUMBER`**
   - Proč `NUMBER` a ne `BOOLEAN`?
   - Oracle PL/SQL NEMÁ vestavěný BOOLEAN pro `OUT` parametry
   - `NUMBER(1)` či `NUMBER` pro 0/1 je workaround

4. **`SELECT INTO v_stav`**
   - Oracle způsob jak přečíst hodnotu do proměnné
   - SQL Server by měl `SELECT @stav = ...`
   - Vyhazuje `NO_DATA_FOUND`, když 0 řádků

5. **`FOR UPDATE` na SELECT**
   - Zamykáme řádek během čtení
   - Zabezpečuje, aby se PRIPAD nemohla měnit během naší transakce
   - Obdoba SQL Server `WITH (UPDLOCK)` nebo `WITH (SERIALIZABLE)`

6. **`EXCEPTION WHEN NO_DATA_FOUND`**
   - Speciální exception v PL/SQL
   - SELECT INTO bez výsledku automaticky vyvolá
   - WHEN OTHERS — catch-all, který chytá VŠECHNY ostatní chyby

7. **`COMMIT` je TĚSNĚ PŘED `p_ret := 1`**
   - Nejdřív všechny úpravy tabulek
   - Pak COMMIT
   - Pak signál úspěchu (`p_ret := 1`)
   - Zajišťuje, že volající dostane `p_ret = 1` IFF byla data committed

8. **`EXCEPTION WHEN OTHERS THEN ROLLBACK`**
   - Proč až na konci?
   - PL/SQL struktura: BEGIN...END, pak EXCEPTION
   - Jakákoliv neošetřená chyba skočí sem
   - ROLLBACK vrací DB do konsistentního stavu

**Srovnání C# vs PL/SQL:**

| Aspekt | C# (TransactionsDao) | PL/SQL (procedura) |
|--------|----------------------|-------------------|
| Transakce | `BeginTransaction()` | Implicitní (procedura = transakce) |
| Krok 1 kontrola | `GetStav()` + if/else | `SELECT INTO` + `FOR UPDATE` |
| Výjimky | `OracleException`, `InvalidOperationException` | `NO_DATA_FOUND`, `WHEN OTHERS` |
| Návratová hodnota | `bool` (true/false) | `NUMBER` (1/0) |
| ROLLBACK | Explicitní v catch | Explicitní v EXCEPTION |
| COMMIT | `EndTransaction()` | Explicitní `COMMIT` |

---

    public static bool PridatOsobuDoPripadu_sp(
        Database pDb,
        int      p_cid,
        int      p_pid,
        int      p_roid,
        string   p_autor)
    {
        Database db    = Database.Connect(pDb);
        bool     v_ret = false;

        // ── Vytvoř OracleCommand ──
        OracleCommand command = db.CreateCommand("PridatOsobuDoPripadu_sp");
        
        // ── Řekni, že to je procedura, ne SQL dotaz ──
        // CommandType.StoredProcedure signalizuje:
        // - Jméno = název procedury
        // - Parametry se mapují automaticky
        // - Oracle přeloží na BEGIN proc(); END; na pozadí
        command.CommandType = System.Data.CommandType.StoredProcedure;

        // ── Vstupní parametry (IN) ──
        command.Parameters.Add(new OracleParameter("p_cid",   p_cid));
        command.Parameters.Add(new OracleParameter("p_pid",   p_pid));
        command.Parameters.Add(new OracleParameter("p_roid",  p_roid));
        command.Parameters.Add(new OracleParameter("p_autor", p_autor));

        // ── Výstupní parametr (OUT) ──
        OracleParameter pRet = new OracleParameter("p_ret", OracleDbType.Int32);
        pRet.Direction = System.Data.ParameterDirection.Output;
        // ParameterDirection.Output řekne: tato proměnná je OUT
        // Bez toho by se hodnota z procedury ignorovala
        command.Parameters.Add(pRet);

        // ── Spusť proceduru ──
        db.ExecuteNonQuery(command);

        // ── Přečti výstupní parametr ──
        // pRet.Value obsahuje to, co procedura nastavila
        // Oracle vrací NUMBER jako object
        v_ret = (pRet.Value != DBNull.Value) &&          // Ověř NULL
                Convert.ToInt32(pRet.Value) == 1;        // Převeď a srovnaj s 1

        Database.Close(pDb, db);
        return v_ret;
    }
}
```

**Klíčové body SP invokace:**

1. **`CommandType.StoredProcedure`**
   - Signalizuje: "Toto je procedura, ne SQL dotaz"
   - Oracle za scén s vytvoří `BEGIN PridatOsobuDoPripadu_sp(...); END;`
   - Výchozí (bez toho) by byl `CommandType.Text`

2. **Parametry procedury**
   - Vstupní (`p_cid`, `p_pid`, `p_roid`, `p_autor`) se zadávají běžně
   - Výstupní (`p_ret`) vyžaduje `ParameterDirection.Output`

3. **`OracleDbType.Int32` pro výstupní parametr**
   - Oracle `NUMBER` se mapuje na C# `int`
   - Bez explicitního typu by mohl být probém s převodem
   - `OracleDbType` je lepší než `DbType` pro Oracle-specifika

4. **`ParameterDirection.Output`**
   - Co by se stalo bez toho?
   - Parametr by se poslal proceduře, ale hodnota se NEČETLA zpět
   - `pRet.Value` by zůstal původní (null)

5. **Proč NEMÁ `BeginTransaction()`**
   - Transakce je UVNITŘ PL/SQL procedury
   - Procedura sám zavolá `COMMIT` nebo `ROLLBACK`
   - Volající (C#) nedělá s transakcí nic

6. **Konverze výstupu**
   ```csharp
   v_ret = (pRet.Value != DBNull.Value) &&      // Nejdřív ověř non-NULL
           Convert.ToInt32(pRet.Value) == 1;    // Pak konvertuj a srovnaj
   ```
   - Proč ne `(bool)pRet.Value`?
   - Oracle nezná `bool` v OUT parametrech
   - Vrací `NUMBER` (0 nebo 1)
   - Je potřeba ručně konvertovat na `bool` logiku

**Srovnání: C# transakce vs SP procedura**

| Aspekt | C# `PridatOsobuDoPripadu` | PL/SQL `PridatOsobuDoPripadu_sp` |
|--------|---------------------------|----------------------------------|
| Kdo spravuje transakci? | C# (BeginTransaction/EndTransaction) | Procedura (BEGIN/EXCEPTION/COMMIT) |
| Kdy se COMMIT? | Když `EndTransaction()` | Procedura sama (`COMMIT;`) |
| Kdy se ROLLBACK? | Catch block zavolá `db.Rollback()` | Procedura sama (EXCEPTION / if+ROLLBACK) |
| Výstup | `bool` (true/false) | `NUMBER` (1/0) |
| Error handling | Try/catch C# exceptions | PL/SQL EXCEPTION blok |
| Volání z C# | Normalní metoda | `CommandType.StoredProcedure` + OUT |

---

### Program.cs
**Účel:** Vstupní bod konzolové aplikace, 4 testovací scénáře

```csharp
using System;

class Program
{
    static void Main(string[] args)
    {
        // Testovací data — MUSÍTE upravit podle skutečné Oracle DB!
        int    cid1        = 1;     // Zjistěte: SELECT CID FROM CLOVEK WHERE ROWNUM <= 1;
        int    cid2        = 2;     // Jiná osoba: SELECT CID FROM CLOVEK WHERE ROWNUM <= 5;
        int    pid_aktivni = 1;     // Zjistěte: SELECT PID FROM PRIPAD WHERE STAV IN ('open','running');
        int    pid_closed  = 9999;  // Neexistující PID (nebo zavřený): SELECT PID FROM PRIPAD WHERE STAV NOT IN ('open','running');
        int    roid        = 1;     // Zjistěte: SELECT ROID FROM ROLE WHERE ROWNUM <= 1;
        string autor       = "HEJ0094";

        bool ret;

        // ────────────────────────────────────────────────────────────────
        // VOLÁNÍ 1: C# transakce, nový záznam (očekáváme True)
        // ────────────────────────────────────────────────────────────────
        // Scénář: První přidání osoby `cid1` do případu `pid_aktivni`
        // - Případ existuje a je 'open' / 'running'
        // - Osoba není v případu
        // - Osoba má < 10 aktivních případů
        // Očekávaný výsledek: True (INSERT se provede, COMMIT)
        ret = TransactionsDao.PridatOsobuDoPripadu(null, cid1, pid_aktivni, roid, autor);
        Console.WriteLine($"PridatOsobuDoPripadu:    ret: {ret},  cid: {cid1}, pid: {pid_aktivni}");

        // ────────────────────────────────────────────────────────────────
        // VOLÁNÍ 2: SP, jiná osoba (očekáváme True)
        // ────────────────────────────────────────────────────────────────
        // Scénář: Přidání JINÉ osoby `cid2` do stejného případu `pid_aktivni`
        // - Případ stále existuje a je aktivní
        // - Osoba `cid2` NENÍ v případu (cid1 je, ale cid2 je jiná)
        // - Osoba `cid2` má < 10 aktivních případů
        // Očekávaný výsledek: True (INSERT se provede, COMMIT)
        // Poznámka: To, že je v `pid_aktivni` již `cid1`, nám nevadí (různé osoby)
        ret = TransactionsDao.PridatOsobuDoPripadu_sp(null, cid2, pid_aktivni, roid, autor);
        Console.WriteLine($"PridatOsobuDoPripadu_sp: ret: {ret},  cid: {cid2}, pid: {pid_aktivni}");

        // ────────────────────────────────────────────────────────────────
        // VOLÁNÍ 3: C# transakce, DUPLICITA (očekáváme False)
        // ────────────────────────────────────────────────────────────────
        // Scénář: Pokus přidat STEJNOU osobu `cid1` do STEJNÉHO případu `pid_aktivni`
        // - Případ existuje a je aktivní
        // - Osoba `cid1` JIŽ V PŘÍPADU JE (z volání 1)
        // Krok 2 selhane: ClovekPripadDao.Exists(db, cid1, pid_aktivni) → true
        // Výsledek: throw InvalidOperationException
        // → catch → Rollback
        // → return False
        // Očekávaný výsledek: False (ROLLBACK, bez INSERT)
        ret = TransactionsDao.PridatOsobuDoPripadu(null, cid1, pid_aktivni, roid, autor);
        Console.WriteLine($"PridatOsobuDoPripadu:    ret: {ret},  cid: {cid1}, pid: {pid_aktivni}  (duplicita)");

        // ────────────────────────────────────────────────────────────────
        // VOLÁNÍ 4: SP, NEAKTIVNÍ PŘÍPAD (očekáváme False)
        // ────────────────────────────────────────────────────────────────
        // Scénář: Pokus přidat osobu `cid1` do UZAVŘENÉHO/NEEXISTUJÍCÍHO případu `pid_closed`
        // - Případ `pid_closed` NEEXISTUJE nebo má STAV != 'open'/'running'
        // Krok 1 selhane: PripadDao.GetStav(db, pid_closed) → null NEBO 'closed'/'unsolved'
        // V proceduře: SELECT STAV ... → NO_DATA_FOUND NEBO v_stav NOT IN ('open','running')
        // Výsledek: IF selhane → ROLLBACK → p_ret := 0
        // Očekávaný výsledek: False (ROLLBACK, bez INSERT)
        ret = TransactionsDao.PridatOsobuDoPripadu_sp(null, cid1, pid_closed, roid, autor);
        Console.WriteLine($"PridatOsobuDoPripadu_sp: ret: {ret},  cid: {cid1}, pid: {pid_closed}  (neaktivní)");

        Console.WriteLine("\nHotovo.");
    }
}
```

**Očekávaný výstup:**
```
PridatOsobuDoPripadu:    ret: True,  cid: 1, pid: 1
PridatOsobuDoPripadu_sp: ret: True,  cid: 2, pid: 1
PridatOsobuDoPripadu:    ret: False, cid: 1, pid: 1  (duplicita)
PridatOsobuDoPripadu_sp: ret: False, cid: 1, pid: 9999  (neaktivní)
```

**Jak zjistit správné testovací hodnoty z Oracle:**

```sql
-- Zjistěte existující CID:
SELECT CID FROM CLOVEK WHERE ROWNUM <= 5;

-- Zjistěte aktivní případ (STAV = 'open' nebo 'running'):
SELECT PID, STAV FROM PRIPAD WHERE ROWNUM <= 5;

-- Zjistěte existující ROID:
SELECT ROID FROM ROLE WHERE ROWNUM <= 5;

-- Případně: Najděte PID s uzavřeným stavem (pro volání 4):
SELECT PID, STAV FROM PRIPAD WHERE STAV NOT IN ('open', 'running') AND ROWNUM <= 1;
```

**Úprava testů:**
- Upravte `cid1`, `cid2`, `pid_aktivni`, `pid_closed`, `roid` podle svých dat
- Pokud nemáte uzavřený případ, použijte neexistující PID (např. 9999)
- `autor` by měla být vaše přihlášená osoba v DB

---

---

## Odevzdávané soubory

| Soubor | Typ | Účel |
|--------|-----|------|
| `App.config` | XML Config | Oracle connection string (User Id=HEJ0094) |
| `Database.cs` | C# Class | Connection & transaction helper (unmodifiable) |
| `Program.cs` | C# Main | Entry point, 4 test calls |
| `orm/dto/ClovekDto.cs` | C# DTO | Person data container |
| `orm/dto/PripadDto.cs` | C# DTO | Case data container |
| `orm/dto/ClovekPripadDto.cs` | C# DTO | Person-Case-Role link |
| `orm/dao/ClovekDao.cs` | C# DAO | Person data access (increment count) |
| `orm/dao/PripadDao.cs` | C# DAO | Case data access (get state, count active) |
| `orm/dao/ClovekPripadDao.cs` | C# DAO | Person-Case link (exists, insert) |
| `orm/dao/TransactionsDao.cs` | C# DAO | Complex transactions (C# + SP versions) |
| `files/PridatOsobuDoPripadu.sql` | PL/SQL | Stored procedure (server-side implementation) |
| `DOC.md` | Markdown | Comprehensive documentation (this file) |

---

## Jak nasadit a spustit

### 1. Nasazení Stored Procedure
Spusťte `files/PridatOsobuDoPripadu.sql` v SQL Developer, sqlplus, nebo jiném Oracle klientu:

```bash
# Příklad sqlplus:
sqlplus HEJ0094@bayer.cs.vsb.cz:1521/oracle @files/PridatOsobuDoPripadu.sql
```

Nebo zkopírujte obsah SQL souboru a spusťte ručně v SQL Developer.

### 2. Ověřit testovací data v Oracle DB
Před spuštěním Program.cs se ujistěte, že máte správné hodnoty:

```sql
-- Zjistěte CID:
SELECT CID, JMENO, PRIJMENI FROM CLOVEK WHERE ROWNUM <= 5;

-- Zjistěte aktivní případ:
SELECT PID, CISLO_PRIPADU, STAV FROM PRIPAD 
WHERE STAV IN ('open', 'running') AND ROWNUM <= 5;

-- Zjistěte ROID:
SELECT ROID, NAZEV FROM ROLE WHERE ROWNUM <= 5;
```

### 3. Aktualizovat Program.cs testovacími hodnotami
Otevřete `Program.cs` a upravte:

```csharp
int    cid1        = 1;          // ← vaše CID
int    cid2        = 2;          // ← jiné CID
int    pid_aktivni = 1;          // ← aktivní PID
int    pid_closed  = 9999;       // ← neexistující PID
int    roid        = 1;          // ← vaše ROID
string autor       = "HEJ0094";  // vaše user
```

### 4. Sestav a spusť v Visual Studiu
1. Otevřete VS 2019+ (nebo jiný C# IDE)
2. Vytvořte nový **Console Application (.NET Framework 4.8)** projekt
3. Zkopírujte všechny soubory do `DSI_cv11/` adresáře
4. V NuGet Package Manager nainstalujte:
   ```
   Install-Package Oracle.ManagedDataAccess -Version 21.14.0
   ```
5. Build → F5 (nebo Ctrl+F5 bez debuggeru)

### 5. Ověřit výstup
Očekávaný výstup:
```
PridatOsobuDoPripadu:    ret: True,  cid: 1, pid: 1
PridatOsobuDoPripadu_sp: ret: True,  cid: 2, pid: 1
PridatOsobuDoPripadu:    ret: False, cid: 1, pid: 1  (duplicita)
PridatOsobuDoPripadu_sp: ret: False, cid: 1, pid: 9999  (neaktivní)
```

---

## Testovací scénáře

### Volání 1: C# transakce — Success path
**Metoda:** `TransactionsDao.PridatOsobuDoPripadu(null, cid1, pid_aktivni, roid, autor)`

**Podmínky:**
- Případ `pid_aktivni` existuje a má STAV = 'open' nebo 'running' ✓
- Osoba `cid1` NENÍ v případu `pid_aktivni` ✓
- Osoba `cid1` má < 10 aktivních případů ✓

**Průběh:**
1. `BeginTransaction()` → Serializable isolace
2. Krok 1: `GetStav()` → vrátí 'open' ✓
3. Krok 2: `Exists()` → vrátí false (není tam) ✓
4. Krok 3: `CountAktivniPripady()` → vrátí < 10 ✓
5. Krok 4: `Insert()` → INSERT do CLOVEK_PRIPAD ✓
6. Krok 5: `IncrementPocetPripadu()` → POCET_PRIPADU += 1 ✓
7. `EndTransaction()` → COMMIT

**Očekávaný výsledek:** ✅ `True`
**DB stav:** Osoba `cid1` je nyní v případu `pid_aktivni`

---

### Volání 2: SP — Success path (jiná osoba)
**Metoda:** `TransactionsDao.PridatOsobuDoPripadu_sp(null, cid2, pid_aktivni, roid, autor)`

**Podmínky:**
- Případ `pid_aktivni` stále existuje a je aktivní ✓
- Osoba `cid2` NENÍ v případu `pid_aktivni` (cid1 tam je, ale to jiné) ✓
- Osoba `cid2` má < 10 aktivních případů ✓

**Průběh (procedura):**
1. `p_ret := 0` — default
2. Krok 1: SELECT STAV s FOR UPDATE → vrátí 'open' ✓
3. Krok 2: COUNT vazeb → vrátí 0 ✓
4. Krok 3: COUNT aktivních případů → vrátí < 10 ✓
5. Krok 4: INSERT do CLOVEK_PRIPAD ✓
6. Krok 5: UPDATE CLOVEK (zvýšit počet) ✓
7. COMMIT
8. `p_ret := 1`

**Očekávaný výsledek:** ✅ `True`
**DB stav:** Osoba `cid2` je nyní v případu `pid_aktivni`; nyní tam jsou cid1 i cid2

---

### Volání 3: C# transakce — DUPLICITA (Failure path)
**Metoda:** `TransactionsDao.PridatOsobuDoPripadu(null, cid1, pid_aktivni, roid, autor)`

**Podmínky:**
- Případ `pid_aktivni` existuje a je aktivní ✓
- Osoba `cid1` JIŽ v případu `pid_aktivni` je (z volání 1) ✗

**Průběh:**
1. `BeginTransaction()`
2. Krok 1: `GetStav()` → OK, vrátí 'open' ✓
3. Krok 2: `Exists()` → Vrátí **true** (je tam z volání 1)
4. `throw new InvalidOperationException(...)` ← Skočí do catch
5. `catch (InvalidOperationException)`
6. `db.Rollback()` — Vrát VŠE
7. `ret = false`

**Očekávaný výsledek:** ❌ `False`
**DB stav:** ŽÁDNÉ změny (ROLLBACK); cid1 a cid2 zůstávají v pid_aktivni jen z úspěšných volání

---

### Volání 4: SP — Neaktivní případ (Failure path)
**Metoda:** `TransactionsDao.PridatOsobuDoPripadu_sp(null, cid1, pid_closed, roid, autor)`

**Podmínky:**
- Případ `pid_closed` NEEXISTUJE nebo má STAV != 'open'/'running' ✗

**Průběh (procedura):**
1. `p_ret := 0`
2. Krok 1: SELECT STAV ... WHERE PID = pid_closed
   - Pokud PID neexistuje: `NO_DATA_FOUND` → catch → ROLLBACK → RETURN
   - Pokud PID existuje ale STAV = 'closed': IF NOT IN → ROLLBACK → RETURN
3. `db.Rollback()`
4. `p_ret` zůstává 0

**Očekávaný výsledek:** ❌ `False`
**DB stav:** ŽÁDNÉ změny (ROLLBACK)

---

## Souhrn technických rozhodnutí

| Rozhodnutí | Důvod | Alternativa |
|------------|-------|-------------|
| `Serializable` isolation | Zamezení phantom read (COUNT se měnil) | `ReadCommitted` → bugs |
| `:param` místo `@param` | Oracle syntaxe (ne SQL Server) | `@param` → CHYBA na Oracle |
| `SELECT INTO FOR UPDATE` v SP | Zámek řádku během čtení | Bez zámku → race condition |
| `SYSDATE` v SQL | DB-side time (konzistentní) | `DateTime.Now` v C# → async issues |
| `BindByName = true` | Parametry se vážou jménem | Default pozice → neobjevné bugs |
| Transakce v C# i SP | Demonstrace obou přístupů | Jen jeden přístup → incompleted |
| DTO bez logiky | Čisté oddělení dat | DTO s metodami → coupling |
| Statické DAO metody | Utilities, ne objekty | Instance DAO → zbytečná state |
| `throw` + `catch` | Zajistí Rollback | `return false` → risk forget Rollback |

---

## Checklist pro odevzdání

Před odevzdáním zkontrolujte:

- [ ] ✅ Žádný `System.Data.SqlClient` — pouze `Oracle.ManagedDataAccess.Client`
- [ ] ✅ Žádný ORM (Entity Framework, Dapper, NHibernate, LINQ-to-SQL)
- [ ] ✅ Žádný `SELECT MAX(ID)` — nepoužívá se
- [ ] ✅ Všechny SQL parametry jsou `:nazev` (ne `@nazev`)
- [ ] ✅ `cmd.BindByName = true` v `Database.CreateCommand()`
- [ ] ✅ `TransactionsDao.PridatOsobuDoPripadu` má `BeginTransaction`, `EndTransaction`, `Rollback`
- [ ] ✅ `TransactionsDao.PridatOsobuDoPripadu_sp` NEMÁ `BeginTransaction` (procedura to spravuje)
- [ ] ✅ `Database.cs` je kopírován přesně (bez změn od šablony)
- [ ] ✅ `files/PridatOsobuDoPripadu.sql` má `CREATE OR REPLACE PROCEDURE`
- [ ] ✅ `App.config` obsahuje `ConnectionStringOracle` s Oracle formatem
- [ ] ✅ Program.cs se úspěšně zkompiluje a spustí (po úpravě testovacích dat)
- [ ] ✅ Výstup odpovídá očekávaným hodnotám (True/True/False/False)
- [ ] ✅ Všechna pole DTO jsou nullable (`?`) dle schéma (`NULL` ve sloupci)
- [ ] ✅ Všechny DAO metody jsou **statické**
- [ ] ✅ Všechny DAO metody mají první parametr `Database pDb`
- [ ] ✅ `DOC.md` je kompletní a aktuální

---

*Dokument byl vygenerován automaticky. Poslední aktualizace: TASK 9*
