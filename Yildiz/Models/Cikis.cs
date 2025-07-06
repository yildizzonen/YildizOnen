namespace Yildiz.Models;

public class Cikis
{
    public int Id { get; set; }
    public int? SanatEseriId { get; set; }
    public SanatEseri? SanatEseri { get; set; }
    public DateTime CikisTarihi { get; set; }
}
