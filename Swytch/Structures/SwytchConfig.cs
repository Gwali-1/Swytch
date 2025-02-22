namespace Swytch.Structures;

/// <summary>
/// Custom type used to configure your swytch app.
/// </summary>
public class SwytchConfig
{
    /// <summary>
    /// Explicitly specify a different template location. Swytch by default looks for the template location in the root or where the application runs
    /// </summary>
    public string? TemplateLocation { get; set; } = null;

    /// <summary>
    /// Enable the static file serving action built into Swytch , allowing you to quickly serve files in the static directory 
    /// </summary>
    public bool EnableStaticFileServer { get; set; } = false;

    /// <summary>
    /// Specifies if you would want to precompile the templates in your web application at startup.This might cause
    /// startup time to be longer relatively but responses that render templates contents during runtime is instant 
    /// </summary>
    public bool PrecompileTemplates { get; set; } = false;

    /// <summary>
    /// Specifies the max-age directive of the Cache-control header. The value specified(in seconds)
    /// indicate how long a static served from the SwytchApp ie if you have set EnableStaticFileServer to true
    /// can be cached and reused. Default is an hour (3600)
    /// </summary>
    public string? StaticCacheMaxAge { get; set; } = null;
}