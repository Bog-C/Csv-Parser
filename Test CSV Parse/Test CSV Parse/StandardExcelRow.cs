// See https://aka.ms/new-console-template for more information
//using nietras.SeparatedValues;

internal class StandardExcelRow
{
    public string? RowNo { get; set; } = null!;
    public string? DetailNo { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? SchemeTransactionID { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? CustomField1 { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? TradeDate { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? FXMPTradeID { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? NatWestSchemeID { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? NatWestSchemeName { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? GFXCounterparty { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? ValueDate { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? CCY { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? Amount { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? Rate { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? PaymentDirection { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? DealType { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? RBSReceiptDate { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? SubmittedCashflowID { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? Submitted { get; set; } = string.Empty; // Fix: Initialize with a default value
    public string? RBSPaymentRef { get; set; } = string.Empty; // Fix: Initialize with a default value
}
