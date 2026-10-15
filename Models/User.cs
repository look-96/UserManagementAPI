namespace UserManagementAPI.Models;

public sealed record User(
    int Id,
    string Name,
    string Email,
    string Department);
