public class BankCard : BaseEntity
{
    public string? CardName { get; set; }

    [MaxLength(50)]
    public string? CardType { get; set; }

    [MaxLength(25)]
    public string? CardNumber { get; set; }


}
