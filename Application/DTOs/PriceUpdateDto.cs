using System.ComponentModel.DataAnnotations;

public class PriceUpdateDto
{
    public List<string> CategoryIds { get; set; }
    public decimal UpdateFactor { get; set; }
    public bool IsPercentage { get; set; }
}
