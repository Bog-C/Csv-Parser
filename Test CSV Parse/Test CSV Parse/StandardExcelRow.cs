
internal class StandardExcelRow
{
    public int RowNo { get; set; }
    public int? DetailNo { get; set; } = null; 
    public string? SchemeTransactionID { get; set; } = string.Empty; 
    public string? CustomField1 { get; set; } = string.Empty; 
    public DateOnly? TradeDate { get; set; } = null!; 
    public string? FXMPTradeID { get; set; } = string.Empty; 
    public string? NatWestSchemeID { get; set; } = string.Empty; 
    public string? NatWestSchemeName { get; set; } = string.Empty; 
    public string? GFXCounterparty { get; set; } = string.Empty; 
    public DateOnly? ValueDate { get; set; } = null!; 
    public string? CCY { get; set; } = string.Empty; 
    public decimal Amount { get; set; }
    public decimal? Rate { get; set; }  
    public string? PaymentDirection { get; set; } = string.Empty; 
    public string? DealType { get; set; } = string.Empty; 
    public DateOnly? RBSReceiptDate { get; set; } = null!; 
    public string? SubmittedCashflowID { get; set; } = string.Empty; 
    public DateTime? Submitted { get; set; } = null!; 
    public string? RBSPaymentRef { get; set; } = string.Empty; 
}
