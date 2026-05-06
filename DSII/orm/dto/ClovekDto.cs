namespace DSII.orm.dto;

public class ClovekDto
{
    public int      cid                        { get; set; }
    public string   rodne_cislo                { get; set; }
    public string   jmeno                      { get; set; }
    public string   prijmeni                   { get; set; }
    public DateTime datum_narozeni             { get; set; }
    public string   misto_narozeni             { get; set; }
    public char     pohlavi                    { get; set; }
    public string   statni_obcanstvi           { get; set; }
    public string   adresa_trvala              { get; set; }
    public string   adresa_aktualni            { get; set; }
    public int      vyska                      { get; set; }
    public int      vaha                       { get; set; }
    public string   barva_oci                  { get; set; }
    public string?  barva_vlasu                { get; set; }
    public string?  zvlastni_znaky             { get; set; }
    public int      rizikovy_level             { get; set; }
    public int      pocet_pripadu              { get; set; }
    public int      pocet_odsouzeni            { get; set; }
    public DateTime datum_registrace           { get; set; }
    public DateTime posledni_aktualizace       { get; set; }
    public string   autor_posledni_aktualizace { get; set; }
    public string?  poznamky                   { get; set; }
    public string?  interni_komentar           { get; set; }
}
