namespace AuctionService.DTOs;

public class UpdateAuctionDto
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; } = string.Empty;
    public int? Mileage { get; set; }
}
