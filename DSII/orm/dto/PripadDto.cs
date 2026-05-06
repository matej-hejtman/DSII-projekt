namespace DSII.orm.dto;

public class PripadDto
{
    public int       pid                        { get; set; }
    public string    cislo_pripadu              { get; set; }
    public DateTime  datum_zahajeni             { get; set; }
    public DateTime? datum_ukonceni             { get; set; }
    public string    stav                       { get; set; }
    public int       priorita                   { get; set; }
    public string    typ_pripadu                { get; set; }
    public string?   popis                      { get; set; }
    public string?   poznamky                   { get; set; }
    public string    vedouci_pripad             { get; set; }
    public DateTime  posledni_aktualizace       { get; set; }
    public string    autor_posledni_aktualizace { get; set; }
}
