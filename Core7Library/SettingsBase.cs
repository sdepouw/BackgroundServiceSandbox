using System.ComponentModel.DataAnnotations;

namespace Core7Library;

public abstract class SettingsBase
{
    [Required]
    public EnvironmentName EnvironmentName { get; set; }
}