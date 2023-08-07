using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace API.Models;
public class User
{

    public int Id { get; set; }

    public string? Email { get; set; }

    public User()
    {
    }

    public User(string email)
    {
        Email = email;
    }
}


