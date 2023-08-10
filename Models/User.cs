using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace API.Models;
public class User
{
    public int Id { get; set; }

    public string SteamId { get; set; }

    public string SteamName { get; set; }

    public string? BoughtServicesJson { get; set; }

    
    public User(string SteamId, string SteamName, string? BoughtServicesJson)
    {
        this.SteamId = SteamId;
        this.SteamName = SteamName;
        this.BoughtServicesJson = BoughtServicesJson;
    }
}