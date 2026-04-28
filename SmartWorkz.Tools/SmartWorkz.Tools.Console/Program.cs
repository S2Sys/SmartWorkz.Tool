using SmartWorkz.Tools.DevOps;

try
{
    // ============================================================================
    // DEBUG MODE: Uncomment to test in Visual Studio without command-line args
    // ============================================================================
    // IMPORTANT: Comment out this section before committing to git!
    string[] debugArgs = new[]
    {
        "--name", "TestProject",           // Project name
        "--type", "none",                 // Project type (Angular, DotNet, FullStack, Mobile, AI)
        "--description", "Test project for debugging"  // Optional description
    };

    // Use debug args or command-line args
    bool useDebugMode = true; // Change to false for production
    args = useDebugMode ? debugArgs : args;
    // ============================================================================

    // Parse arguments
    var options = ParseArguments(args);

    if (string.IsNullOrEmpty(options["name"]))
    {
        ProjectTemplateGenerator.WriteError("ERROR: Project name is required. Use --name \"ProjectName\"");
        Environment.Exit(1);
    }

    // Get PAT from environment variable or command-line argument
    string? pat = Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT");
    if (string.IsNullOrEmpty(pat) && options.TryGetValue("pat", out var cmdPat))
    {
        pat = cmdPat;
    }

    if (string.IsNullOrEmpty(pat))
    {
        ProjectTemplateGenerator.WriteError("ERROR: Azure DevOps PAT not found. Set AZURE_DEVOPS_PAT environment variable.");
        Environment.Exit(1);
    }

    // Create generator and run
    var generator = new ProjectTemplateGenerator(
        projectName: options["name"],
        projectType: options.ContainsKey("type") ? options["type"] : "FullStack",
        description: options.ContainsKey("description") ? options["description"] : "SmartWorkz development project",
        pat: pat,
        organizationUrl: options.ContainsKey("org") ? options["org"] : null,
        localDevPath: options.ContainsKey("path") ? options["path"] : null
    );

    await generator.ExecuteAsync();
}
catch (Exception ex)
{
    ProjectTemplateGenerator.WriteError($"FATAL ERROR: {ex.Message}");
    Environment.Exit(1);
}

static Dictionary<string, string> ParseArguments(string[] args)
{
    var options = new Dictionary<string, string>();

    for (int i = 0; i < args.Length; i++)
    {
        if (args[i].StartsWith("--") && i + 1 < args.Length)
        {
            string key = args[i].Substring(2).ToLower();
            string value = args[i + 1];
            options[key] = value;
            i++;
        }
    }

    return options;
}
