#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;
using SamplerService.SystemHelpers;

namespace SamplerService.Workers.PromouteRegistration.WorkerServices;

public class RegistrationServiceSettings
{
    public static readonly string Format = "dd.MM.yyyy";
    public int TryCont { get; set; }
    [Required]
    public string UserToken { get; set; }
    [Required]
    public string UserPhone { get; set; }
    public string UserTrevalDate { get; set; }
}
