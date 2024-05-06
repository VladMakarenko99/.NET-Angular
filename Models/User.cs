namespace API.Models;
public class User
{
    public User(string steamId, string steamName, string? boughtServicesJson, double balance) =>
    (SteamId, SteamName, BoughtServicesJson, Balance) = (steamId, steamName, boughtServicesJson, balance);

    public int Id { get; set; }

    public string SteamId { get; set; }

    public string SteamName { get; set; }

    public string? BoughtServicesJson { get; set; }

    public double Balance { get; set; }
}