using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public sealed class UserWriteRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[\s\S]*\S[\s\S]*$", ErrorMessage = "Name cannot contain only whitespace.")]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[\s\S]*\S[\s\S]*$", ErrorMessage = "Department cannot contain only whitespace.")]
    public string Department { get; init; } = string.Empty;
}
