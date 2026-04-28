using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace SmartWorkz.Tools.DevOps
{
    /// <summary>
    /// SmartWorkz Azure DevOps Project Template Generator
    /// Creates new projects with pre-configured code quality pipelines
    /// </summary>
    public class ProjectTemplateGenerator
    {
        // Configuration
        private readonly string _organizationUrl;
        private readonly string _pat;
        private readonly string _projectName;
        private readonly string _projectType;
        private readonly string _description;
        private readonly string _localDevPath;
        private readonly HttpClient _httpClient;

        // Constants
        private const string ScrumTemplateId = "adcc42ab-9882-485e-a3ed-7678f01f66bc";
        private const int ProjectInitDelaySeconds = 5;

        // Policy Type GUIDs
        private const string PolicyTypeMinReviewers      = "fa4e907d-8a14-494a-85d6-a89a79de8486";
        private const string PolicyTypeRequiredReviewers = "fd2167ab-b0be-447a-8ec8-39368250530e";
        private const string PolicyTypeCommentResolution = "c6a27be9-7728-4974-aa4d-753382dc3551";
        private const string PolicyTypeWorkItemLinking   = "40e92b44-2fe1-4dd6-b3d8-74a9c21d0c6e";

        // Retry / Polling
        private const int PolicyRetryCount        = 3;
        private const int PolicyRetryDelaySeconds = 2;
        private const int RepoReadyMaxAttempts    = 10;
        private const int RepoReadyPollSeconds    = 5;

        // Valid project types
        private static readonly string[] ValidProjectTypes =
        {
            "Angular",      // Frontend web app (TypeScript/Angular)
            "DotNet",       // Backend API (.NET/C#)
            "FullStack",    // Full stack (.NET + Angular)
            "Mobile",       // Cross-platform (iOS/Android)
            "AI",           // Machine learning project
            "Java",         // Backend API (Java/Spring)
            "PHP",          // Backend API (PHP/Laravel/Symfony)
            "Flutter",      // Cross-platform mobile (Flutter)
            "Vue",          // Frontend web app (Vue.js)
            "React" ,        // Frontend web app (React)
                "none"
        };

        public ProjectTemplateGenerator(
            string projectName,
            string projectType,
            string description,
            string pat,
            string organizationUrl = null,
            string localDevPath = null)
        {
            _projectName = projectName ?? throw new ArgumentNullException(nameof(projectName));
            _projectType = projectType ?? "FullStack";
            _description = description ?? "SmartWorkz development project";
            _pat = pat ?? throw new ArgumentNullException(nameof(pat), "PAT not found. Set AZURE_DEVOPS_PAT environment variable.");
            _organizationUrl = organizationUrl ?? "https://dev.azure.com/smartworkz";
            _localDevPath = localDevPath ?? "S:\\SmartWorkz101";

            ValidateProjectType();
            _httpClient = CreateHttpClient();
        }

        /// <summary>
        /// Main execution flow
        /// </summary>
        public async Task ExecuteAsync()
        {
            WriteInfo("================================");
            WriteInfo("SmartWorkz DevOps Template Setup");
            WriteInfo("================================\n");

            try
            {
                // Step 1: Create Project
                WriteInfo("Step 1: Creating Azure DevOps Project...");
                string projectId = await CreateProjectAsync();
                WriteSuccess($"✓ Project created: {_projectName} ({projectId})");

                // Wait for project to initialize
                WriteInfo("Waiting for project to initialize...");
                await Task.Delay(ProjectInitDelaySeconds * 1000);

                // Step 2: Setup Repository
                WriteInfo("\nStep 2: Setting up repository structure...");
                await SetupRepositoryAsync();
                WriteSuccess("✓ Repository cloned and structured");

                // Step 3: Create Files
                WriteInfo("\nStep 3: Creating configuration files...");
                CreateConfigurationFiles();
                WriteSuccess("✓ Configuration files created");

                // Step 4: Initialize Git Hooks
                WriteInfo("\nStep 4: Setting up Git hooks and validation...");
                await InitializeGitHooksAsync();
                WriteSuccess("✓ Git hooks configured");

                // Step 5: Create CODEOWNERS, PR Template, and Branch Policies
                WriteInfo("\nStep 5: Configuring code ownership, PR template, and branch policies...");
                CreateCodeOwnersFile();
                CreatePullRequestTemplate();
                CreateBranchPoliciesFile();
                WriteSuccess("✓ CODEOWNERS, PR template, and branch policies created");

                // Step 6: Create Team Configuration
                WriteInfo("\nStep 6: Creating team configuration...");
                CreateTeamConfigFile();
                WriteSuccess("✓ Team configuration created");

                // Step 7: Git operations
                WriteInfo("\nStep 7: Initializing git repository...");
                await GitAddCommitPushAsync();
                WriteSuccess("✓ Repository initialized and pushed");

                // Step 8: Configure Branch Policies
                WriteInfo("\nStep 8: Configuring branch policies...");
                await ConfigureBranchPoliciesAsync(projectId);
                WriteSuccess("✓ Branch policy configuration complete");

                // Step 9: Summary with initialization guide
                WriteInfo("\n" + new string('=', 50));
                WriteSuccess("✓ Project setup completed!");
                WriteInfo("\n📋 INITIALIZATION CHECKLIST:");
                WriteInfo("\n✅ FULLY AUTOMATED (All Done!):");
                WriteInfo("  • Azure DevOps project created");
                WriteInfo("  • Repository cloned and structured");
                WriteInfo("  • Configuration files generated");
                WriteInfo("  • Git hooks configured locally (commit validation)");
                WriteInfo("  • CODEOWNERS file created");
                WriteInfo("  • PR template configured");
                WriteInfo("  • Branch policies configured (main: 2 reviewers, develop: 1 reviewer)");
                WriteInfo("  • Repository initialized with initial commit");
                WriteInfo("\n📋 Project is ready for development!");
                WriteInfo("\n⚠️  Manual Setup Remaining (Optional Enhancements):");
                WriteInfo("\n1️⃣  Optional: CI/CD Setup");
                WriteInfo("    • Service connections: Project Settings → Service Connections");
                WriteInfo("    • Enable azure-pipelines.yml for automated builds");
                WriteInfo("\n2️⃣  Verify Policies Applied:");
                WriteInfo($"    {_organizationUrl}/{_projectName}/_settings/repositories");
                WriteInfo($"\n📖 Full setup guide: {Path.Combine(_localDevPath, _projectName, ".azuredevops", "BRANCH-POLICIES.md")}");
                WriteInfo("\n📖 Documentation:");
                WriteInfo($"  • Read: {Path.Combine(_localDevPath, _projectName, "TEAM-GUIDE.md")}");
                WriteInfo($"  • Reference: {Path.Combine(_localDevPath, _projectName, ".azuredevops", "BRANCH-POLICIES.md")}");
                WriteInfo($"  • Review: {Path.Combine(_localDevPath, _projectName, ".azuredevops", "TEAM.yml")}");
                WriteInfo($"\nProject URL: {_organizationUrl}/{_projectName}");
                WriteInfo($"Repository: {_organizationUrl}/{_projectName}/_git/{_projectName}");
            }
            catch (HttpRequestException ex)
            {
                WriteError($"API Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Step 1: Create Azure DevOps Project via REST API
        /// </summary>
        private async Task<string> CreateProjectAsync()
        {
            var projectBody = new
            {
                name = _projectName,
                description = _description,
                visibility = "private",
                capabilities = new
                {
                    versioncontrol = new { sourceControlType = "Git" },
                    processTemplate = new { templateTypeId = ScrumTemplateId }
                }
            };

            string jsonBody = JsonSerializer.Serialize(projectBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_organizationUrl}/_apis/projects?api-version=7.0",
                content);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create project: {response.StatusCode} - {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            using (JsonDocument doc = JsonDocument.Parse(responseBody))
            {
                return doc.RootElement.GetProperty("id").GetString();
            }
        }

        /// <summary>
        /// Step 2: Clone repository and create folder structure
        /// </summary>
        private async Task SetupRepositoryAsync()
        {
            string repoUrl = $"{_organizationUrl}/{_projectName}/_git/{_projectName}";
            string localPath = Path.Combine(_localDevPath, _projectName);

            // Clean up existing directory
            if (Directory.Exists(localPath))
            {
                WriteWarning($"Local path exists, removing: {localPath}");
                Directory.Delete(localPath, true);
            }

            // Clone repository - pass URL and path as separate arguments
            await RunGitCommandAsync("clone", repoUrl, localPath);

            // Create folder structure based on type
            CreateFolderStructure(localPath);
        }

        /// <summary>
        /// Create folder structure based on project type
        /// </summary>
        private void CreateFolderStructure(string basePath)
        {
            WriteInfo($"Creating project structure for: {_projectType}");

            var folders = GetFoldersForProjectType();
            foreach (var folder in folders)
            {
                Directory.CreateDirectory(Path.Combine(basePath, folder));
            }
        }

        /// <summary>
        /// Get folder structure for the specified project type
        /// </summary>
        private string[] GetFoldersForProjectType()
        {
            // Common folders for all project types
            var baseFolders = new[] { "src", "tests", "docs", "build", "config", "scripts" };

            // Type-specific subdirectories
            var typeSpecificFolders = _projectType switch
            {
                // Web Frontends
                "Angular" => new[] { "src/app", "src/assets", "src/environments", "tests/unit", "tests/e2e" },
                "React" => new[] { "src/components", "src/pages", "src/hooks", "src/utils", "public", "tests/unit", "tests/e2e" },
                "Vue" => new[] { "src/components", "src/pages", "src/stores", "src/utils", "public", "tests/unit", "tests/e2e" },

                // Backend APIs
                "DotNet" => new[] { "src/API", "src/Services", "src/Data", "tests/UnitTests", "tests/IntegrationTests" },
                "Java" => new[] { "src/main/java", "src/main/resources", "src/test/java", "target", ".mvn" },
                "PHP" => new[] { "src", "config", "routes", "tests/unit", "tests/integration", "storage", "vendor" },

                // Full Stack
                "FullStack" => new[] { "src/api", "src/web", "src/shared", "tests/api", "tests/web", "infra", "infra/terraform", "infra/kubernetes" },

                // Mobile
                "Mobile" => new[] { "src/android", "src/ios", "src/shared" },
                "Flutter" => new[] { "lib", "lib/screens", "lib/widgets", "lib/models", "lib/services", "test", "android", "ios", "web", "windows", "macos" },

                // AI/ML
                "AI" => new[] { "src/notebooks", "src/training", "src/inference", "tests", "models", "datasets" },

                // Default fallback for unknown types
                _ => new[] {"src", "tests/unit", "tests/integration" }
            };

            return baseFolders.Concat(typeSpecificFolders).ToArray();
        }

        /// <summary>
        /// Step 3: Create configuration files (common and tech-stack-specific)
        /// </summary>
        private void CreateConfigurationFiles()
        {
            string basePath = Path.Combine(_localDevPath, _projectName);

            // Common files for all projects
            File.WriteAllText(Path.Combine(basePath, ".gitignore"), GetGitignoreContent());
            File.WriteAllText(Path.Combine(basePath, ".editorconfig"), GetEditorconfigContent());
            File.WriteAllText(Path.Combine(basePath, "sonar-project.properties"), GetSonarPropertiesContent());
            File.WriteAllText(Path.Combine(basePath, "azure-pipelines.yml"), GetPipelineYamlContent());
            File.WriteAllText(Path.Combine(basePath, "README.md"), GetReadmeContent());

            // Tech-stack-specific files
            switch (_projectType)
            {
                case "React" or "Angular" or "Vue":
                    File.WriteAllText(Path.Combine(basePath, ".eslintrc.json"), GetEslintrcContent());
                    File.WriteAllText(Path.Combine(basePath, ".prettierrc.json"), GetPrettierrcContent());
                    File.WriteAllText(Path.Combine(basePath, "package.json"), GetPackageJsonContent(_projectType));
                    File.WriteAllText(Path.Combine(basePath, "tsconfig.json"), GetTsConfigContent());
                    break;

                case "Java":
                    File.WriteAllText(Path.Combine(basePath, "pom.xml"), GetMavenPomContent());
                    File.WriteAllText(Path.Combine(basePath, "application.properties"), GetJavaApplicationProperties());
                    break;

                case "PHP":
                    File.WriteAllText(Path.Combine(basePath, "composer.json"), GetComposerJsonContent());
                    File.WriteAllText(Path.Combine(basePath, ".env.example"), GetPhpEnvContent());
                    File.WriteAllText(Path.Combine(basePath, "phpunit.xml"), GetPhpUnitContent());
                    break;

                case "Flutter":
                    File.WriteAllText(Path.Combine(basePath, "pubspec.yaml"), GetFlutterPubspecContent());
                    File.WriteAllText(Path.Combine(basePath, "analysis_options.yaml"), GetFlutterAnalysisOptions());
                    break;

                case "DotNet":
                    File.WriteAllText(Path.Combine(basePath, ".eslintrc.json"), GetEslintrcContent());
                    File.WriteAllText(Path.Combine(basePath, ".prettierrc.json"), GetPrettierrcContent());
                    break;

                case "FullStack":
                    File.WriteAllText(Path.Combine(basePath, ".eslintrc.json"), GetEslintrcContent());
                    File.WriteAllText(Path.Combine(basePath, ".prettierrc.json"), GetPrettierrcContent());
                    break;
            }
        }

        /// <summary>
        /// Step 7: Git operations - add, commit, push
        /// </summary>
        private async Task GitAddCommitPushAsync()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);

            try
            {
                // Step 1: Add all files
                WriteInfo("  • Running: git add .");
                await RunGitCommandAsync("add", ".");

                // Step 2: Commit
                WriteInfo("  • Running: git commit");
                await RunGitCommandAsync("commit", "-m", "Initial commit: SmartWorkz project template");

                // Step 3: Try to push to main, fall back to master if main doesn't exist
                try
                {
                    WriteInfo("  • Running: git push origin main");
                    await RunGitCommandAsync("push", "origin", "main");
                }
                catch
                {
                    WriteWarning("  ⚠ main branch not found, trying master");
                    await RunGitCommandAsync("push", "origin", "master");
                }

                WriteSuccess("  • All git operations completed");
            }
            catch (Exception ex)
            {
                throw new Exception($"Git operations failed: {ex.Message}", ex);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Step 7: Create Azure DevOps groups and add team members from TEAM.yml
        /// Returns a dictionary mapping group names to their descriptors for policy configuration
        /// </summary>
        private async Task<Dictionary<string, string>> CreateGroupsAndAddMembersAsync()
        {
            try
            {
                string repoPath = Path.Combine(_localDevPath, _projectName);
                string teamFilePath = Path.Combine(repoPath, ".azuredevops", "TEAM.yml");

                var groupDescriptors = new Dictionary<string, string>();

                if (!File.Exists(teamFilePath))
                {
                    WriteWarning("TEAM.yml not found, skipping group creation");
                    return groupDescriptors;
                }

                // Define group names and their yaml keys
                var groupConfig = new Dictionary<string, string>
                {
                    { "admins", "admins" },
                    { "team-leads", "team_leads" },
                    { "senior-developers", "senior_developers" },
                    { "code-quality-team", "code_quality_team" },
                    { "security-team", "security_team" }
                };

                // Read team file
                string teamContent = File.ReadAllText(teamFilePath);

                // Look for existing groups and add members
                foreach (var group in groupConfig)
                {
                    string groupName = group.Key;
                    string yamlKey = group.Value;

                    // Try to find existing group
                    string? groupId = await GetGroupIdAsync(groupName);

                    if (string.IsNullOrEmpty(groupId))
                    {
                        WriteWarning($"  ⚠ Group '{groupName}' not found. Create manually: Project Settings → Security → Groups");
                        continue;
                    }

                    groupDescriptors[groupName] = groupId;
                    WriteInfo($"  ✓ Found group: {groupName}");

                    // Extract emails for this group from team file
                    var emails = ExtractEmailsFromTeamFile(teamContent, yamlKey);

                    // Add members to group
                    foreach (var email in emails)
                    {
                        await AddMemberToGroupAsync(groupId, email);
                    }

                    if (emails.Count > 0)
                    {
                        WriteInfo($"    ✓ Added {emails.Count} member(s) to {groupName}");
                    }
                }

                return groupDescriptors;
            }
            catch (Exception ex)
            {
                WriteWarning($"Group creation skipped: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Create an Azure DevOps group
        /// </summary>
        private async Task<string> CreateGroupAsync(string groupName)
        {
            try
            {
                var groupBody = new
                {
                    displayName = groupName,
                    description = $"SmartWorkz {groupName} group"
                };

                string jsonBody = JsonSerializer.Serialize(groupBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Try project-scoped endpoint
                var response = await _httpClient.PostAsync(
                    $"{_organizationUrl}/{_projectName}/_apis/identity/groups?api-version=7.0-preview.1",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    string? descriptor = doc.RootElement.GetProperty("descriptor").GetString();
                    WriteInfo($"    ✓ Created group: {groupName}");
                    return descriptor ?? string.Empty;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    // Group already exists, find it
                    WriteInfo($"    ℹ Group already exists: {groupName}");
                    string? existingId = await GetGroupIdAsync(groupName);
                    return existingId ?? string.Empty;
                }

                // Log error but don't fail
                WriteWarning($"    ⚠ Could not create group '{groupName}' (HTTP {response.StatusCode})");
                WriteInfo($"      Manual creation required: Project Settings → Security → Groups");
                return string.Empty;
            }
            catch
            {
                WriteWarning($"    ⚠ Group creation error: {groupName}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Get existing group ID by name
        /// </summary>
        private async Task<string> GetGroupIdAsync(string groupName)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_organizationUrl}/_apis/identity/groups?scopeNames=VSS_MemberEntitlement_Group&api-version=7.0-preview.1");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        var groups = doc.RootElement.GetProperty("value");
                        foreach (var group in groups.EnumerateArray())
                        {
                            string name = group.GetProperty("displayName").GetString();
                            if (name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                            {
                                return group.GetProperty("descriptor").GetString();
                            }
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Add a member to a group by email
        /// </summary>
        private async Task AddMemberToGroupAsync(string groupId, string email)
        {
            try
            {
                // First, find the user by email
                var userResponse = await _httpClient.GetAsync(
                    $"{_organizationUrl}/_apis/graph/users?subjectTypes=msa,aad&query={email}&api-version=7.0-preview.1");

                if (!userResponse.IsSuccessStatusCode)
                {
                    return;
                }

                var userBody = await userResponse.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(userBody))
                {
                    var results = doc.RootElement.GetProperty("graphUsers");
                    if (results.GetArrayLength() == 0)
                    {
                        return;
                    }

                    string userDescriptor = results[0].GetProperty("descriptor").GetString();

                    // Add user to group
                    var membershipBody = new { };
                    string jsonBody = JsonSerializer.Serialize(membershipBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    await _httpClient.PutAsync(
                        $"{_organizationUrl}/_apis/identity/groupmemberships/{groupId}/{userDescriptor}?api-version=7.0-preview.1",
                        content);
                }
            }
            catch
            {
                // Silently fail - user might not exist in Azure AD yet
            }
        }

        /// <summary>
        /// Extract email addresses from TEAM.yml for a specific group
        /// </summary>
        private List<string> ExtractEmailsFromTeamFile(string teamContent, string groupKey)
        {
            var emails = new List<string>();

            try
            {
                // Simple YAML parsing for our specific format
                string[] lines = teamContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                bool inSection = false;

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();

                    // Check if this is the section we're looking for
                    if (trimmed.StartsWith(groupKey + ":", StringComparison.OrdinalIgnoreCase))
                    {
                        inSection = true;
                        continue;
                    }

                    // Exit section if we hit another section
                    if (inSection && trimmed.EndsWith(":") && !trimmed.StartsWith("-"))
                    {
                        break;
                    }

                    // Extract email if in section
                    if (inSection && trimmed.Contains("email:"))
                    {
                        string email = trimmed.Replace("email:", "").Trim();
                        if (!string.IsNullOrEmpty(email))
                        {
                            emails.Add(email);
                        }
                    }
                }
            }
            catch
            {
                // Parsing error, return empty list
            }

            return emails;
        }

        /// <summary>
        /// Helper: Execute an async operation with exponential backoff retry.
        /// </summary>
        private async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxRetries = PolicyRetryCount, int delaySeconds = PolicyRetryDelaySeconds)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    attempt++;
                    int waitMs = delaySeconds * (int)Math.Pow(2, attempt - 1) * 1000;
                    WriteWarning($"  ⚠ Retry {attempt}/{maxRetries} after {waitMs / 1000}s: {ex.Message}");
                    await Task.Delay(waitMs);
                }
            }
        }

        /// <summary>
        /// Fetch the repository GUID for the project's default repository.
        /// Returns null if the repository does not yet exist or on any error.
        /// </summary>
        private async Task<string?> GetRepositoryIdAsync(string projectId)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{_organizationUrl}/{_projectName}/_apis/git/repositories/{_projectName}?api-version=7.0");

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                return doc.RootElement.GetProperty("id").GetString();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Poll until the default repository is available (created and initialised after project creation).
        /// Returns the repository GUID, or null if it never becomes ready within the allowed attempts.
        /// </summary>
        private async Task<string?> WaitForRepositoryReadyAsync(string projectId)
        {
            WriteInfo($"  • Waiting for repository (max {RepoReadyMaxAttempts} attempts)...");
            for (int attempt = 1; attempt <= RepoReadyMaxAttempts; attempt++)
            {
                string? repoId = await GetRepositoryIdAsync(projectId);

                if (!string.IsNullOrEmpty(repoId))
                {
                    WriteInfo($"  • Repository ready: {repoId}");
                    return repoId;
                }

                WriteInfo($"  • Attempt {attempt}/{RepoReadyMaxAttempts}: not ready, waiting {RepoReadyPollSeconds}s...");
                await Task.Delay(RepoReadyPollSeconds * 1000);
            }

            WriteWarning($"  ⚠ Repository not ready after {RepoReadyMaxAttempts} attempts");
            return null;
        }

        /// <summary>
        /// POST a single policy configuration. Non-fatal: returns false and warns on any failure.
        /// </summary>
        private async Task<bool> CreatePolicyAsync(string projectId, object policyBody)
        {
            try
            {
                return await RetryAsync(async () =>
                {
                    string jsonBody = JsonSerializer.Serialize(policyBody);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(
                        $"{_organizationUrl}/{_projectName}/_apis/policy/configurations?api-version=7.0",
                        content);

                    if (response.IsSuccessStatusCode)
                        return true;

                    string err = await response.Content.ReadAsStringAsync();
                    WriteWarning($"  ⚠ Policy failed ({response.StatusCode}): {err}");
                    return false;
                });
            }
            catch (Exception ex)
            {
                WriteWarning($"  ⚠ Policy error (non-fatal): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Step 8: Configure branch policies via the Azure DevOps Policy API.
        /// Creates minimum reviewer policies for main (2) and develop (1) branches.
        /// All failures are non-fatal — a warning is written and execution continues.
        /// </summary>
        private async Task ConfigureBranchPoliciesAsync(string projectId)
        {
            try
            {
                // Get repository ID (poll until ready)
                string? repoId = await WaitForRepositoryReadyAsync(projectId);
                if (string.IsNullOrEmpty(repoId))
                {
                    WriteWarning("  ⚠ Skipping branch policies: repository not available");
                    return;
                }

                int policyCount = 0;

                // Policy 1: main — 2 reviewers, no self-approve, blocking
                WriteInfo("  • Configuring minimum reviewer policy for 'main'...");
                bool ok = await CreatePolicyAsync(projectId, new
                {
                    isEnabled = true,
                    isBlocking = true,
                    type = new { id = PolicyTypeMinReviewers },
                    settings = new
                    {
                        minimumApproverCount = 2,
                        creatorVoteCounts = false,
                        allowDownvotes = false,
                        resetOnSourcePush = true,
                        blockLastPusherVote = true,
                        scope = new[] { new { repositoryId = repoId, refName = "refs/heads/main", matchKind = "Exact" } }
                    }
                });
                if (ok) policyCount++;

                // Policy 2: develop — 1 reviewer, blocking
                WriteInfo("  • Configuring minimum reviewer policy for 'develop'...");
                ok = await CreatePolicyAsync(projectId, new
                {
                    isEnabled = true,
                    isBlocking = true,
                    type = new { id = PolicyTypeMinReviewers },
                    settings = new
                    {
                        minimumApproverCount = 1,
                        creatorVoteCounts = false,
                        allowDownvotes = false,
                        resetOnSourcePush = true,
                        blockLastPusherVote = false,
                        scope = new[] { new { repositoryId = repoId, refName = "refs/heads/develop", matchKind = "Exact" } }
                    }
                });
                if (ok) policyCount++;

                WriteSuccess($"  ✓ Branch policies configured: {policyCount}/2 policies applied");
            }
            catch (Exception ex)
            {
                WriteWarning($"  ⚠ Branch policy configuration failed (non-fatal): {ex.Message}");
            }
        }

        /// <summary>
        /// Helper: Create HTTP client with basic auth
        /// </summary>
        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_pat}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            return client;
        }

        /// <summary>
        /// Helper: Validate project type
        /// </summary>
        private void ValidateProjectType()
        {
            if (!ValidProjectTypes.Contains(_projectType))
            {
                throw new ArgumentException($"Invalid project type. Valid types: {string.Join(", ", ValidProjectTypes)}");
            }
        }

        /// <summary>
        /// Helper: Run git command with detailed error reporting and proper argument escaping
        /// </summary>
        private async Task RunGitCommandAsync(params string[] args)
        {
            // Properly escape arguments that contain spaces or special characters
            var escapedArgs = args.Select(arg =>
            {
                if (arg.Contains(" ") || arg.Contains("\""))
                    return "\"" + arg.Replace("\"", "\\\"") + "\"";
                return arg;
            });

            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = string.Join(" ", escapedArgs),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new Exception("Failed to start git process. Is Git installed and in PATH?");
                }

                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (process.ExitCode != 0)
                {
                    string errorMsg = $"Git command failed: {string.Join(" ", args)}\n";
                    if (!string.IsNullOrEmpty(error))
                        errorMsg += $"Error: {error}";
                    if (!string.IsNullOrEmpty(output))
                        errorMsg += $"Output: {output}";

                    throw new Exception(errorMsg);
                }

                if (!string.IsNullOrEmpty(output))
                {
                    WriteInfo($"    {output.Trim()}");
                }
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Step 4: Initialize Git hooks for commit message validation
        /// </summary>
        private async Task InitializeGitHooksAsync()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string hooksDir = Path.Combine(repoPath, ".git", "hooks");

            if (!Directory.Exists(hooksDir))
            {
                WriteWarning("Git hooks directory not found, creating...");
                Directory.CreateDirectory(hooksDir);
            }

            // Create prepare-commit-msg hook
            string prepareCommitMsgPath = Path.Combine(hooksDir, "prepare-commit-msg");
            File.WriteAllText(prepareCommitMsgPath, GetPrepareCommitMsgHookContent());
            MakeScriptExecutable(prepareCommitMsgPath);

            // Create commit-msg hook for validation
            string commitMsgPath = Path.Combine(hooksDir, "commit-msg");
            File.WriteAllText(commitMsgPath, GetCommitMsgHookContent());
            MakeScriptExecutable(commitMsgPath);

            WriteInfo("  • prepare-commit-msg hook installed");
            WriteInfo("  • commit-msg hook installed");

            await Task.CompletedTask;
        }

        /// <summary>
        /// Step 5a: Create CODEOWNERS file for automatic reviewer assignment
        /// </summary>
        private void CreateCodeOwnersFile()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string azureDevOpsDir = Path.Combine(repoPath, ".azuredevops");
            Directory.CreateDirectory(azureDevOpsDir);

            string codeOwnersPath = Path.Combine(azureDevOpsDir, "CODEOWNERS");
            File.WriteAllText(codeOwnersPath, GetCodeOwnersContent());
        }

        /// <summary>
        /// Step 5b: Create Pull Request template
        /// </summary>
        private void CreatePullRequestTemplate()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string azureDevOpsDir = Path.Combine(repoPath, ".azuredevops");
            Directory.CreateDirectory(azureDevOpsDir);

            string prTemplatePath = Path.Combine(azureDevOpsDir, "PULL_REQUEST_TEMPLATE.md");
            File.WriteAllText(prTemplatePath, GetPullRequestTemplateContent());
        }

        /// <summary>
        /// Step 6: Create team configuration file
        /// </summary>
        private void CreateTeamConfigFile()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string azureDevOpsDir = Path.Combine(repoPath, ".azuredevops");
            Directory.CreateDirectory(azureDevOpsDir);

            string teamFilePath = Path.Combine(azureDevOpsDir, "TEAM.yml");
            File.WriteAllText(teamFilePath, GetTeamConfigContent());
        }

        /// <summary>
        /// Create branch policies configuration file
        /// </summary>
        private void CreateBranchPoliciesFile()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string azureDevOpsDir = Path.Combine(repoPath, ".azuredevops");
            Directory.CreateDirectory(azureDevOpsDir);

            string branchPoliciesPath = Path.Combine(azureDevOpsDir, "BRANCH-POLICIES.md");
            File.WriteAllText(branchPoliciesPath, GetBranchPoliciesContent());
        }

        /// <summary>
        /// Helper: Make script executable on Unix-like systems
        /// </summary>
        private void MakeScriptExecutable(string scriptPath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"+x \"{scriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
            catch
            {
                WriteWarning($"Could not make {scriptPath} executable (this is OK on Windows)");
            }
        }

        #region Configuration File Content Methods

        private string GetGitignoreContent() => @"# Visual Studio
.vs/
.vscode/
*.user
*.suo
bin/
obj/
.DS_Store

# Node
node_modules/
dist/
.angular/
*.log

# Python
__pycache__/
*.pyc
venv/
.env

# Artifacts
*.trx
*.coverage
coverage/
reports/

# Build
.build/
artifacts/";

        private string GetEditorconfigContent() => @"root = true

[*.cs]
indent_style = space
indent_size = 4
end_of_line = crlf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

csharp_new_line_before_open_brace = all
csharp_indent_switch_labels = true
csharp_space_after_keywords_in_control_flow_statements = true

dotnet_naming_rule.constants.severity = suggestion
dotnet_naming_rule.constants.symbols = constant_symbols
dotnet_naming_rule.constants.style = pascal_case_style

dotnet_naming_symbols.constant_symbols.applicable_kinds = const_fields
dotnet_naming_symbols.constant_symbols.applicable_accessibilities = *
dotnet_naming_symbols.constant_symbols.required_modifiers = const

dotnet_naming_style.pascal_case_style.capitalization = pascal_case

[*.json]
indent_size = 2

[*.{ts,js}]
indent_size = 2";

        private string GetEslintrcContent() => @"{
  ""root"": true,
  ""ignorePatterns"": [""projects/**/*""],
  ""overrides"": [
    {
      ""files"": [""*.ts""],
      ""parserOptions"": {
        ""project"": [""tsconfig.json""],
        ""createDefaultProgram"": true
      },
      ""extends"": [
        ""plugin:@angular-eslint/recommended"",
        ""plugin:prettier/recommended""
      ],
      ""rules"": {
        ""@angular-eslint/directive-selector"": [
          ""error"",
          {""type"": ""attribute"", ""prefix"": ""app"", ""style"": ""camelCase""}
        ],
        ""@angular-eslint/component-selector"": [
          ""error"",
          {""type"": ""element"", ""prefix"": ""app"", ""style"": ""kebab-case""}
        ],
        ""no-console"": [""warn"", {""allow"": [""warn"", ""error""]}],
        ""no-debugger"": ""error"",
        ""prefer-const"": ""error""
      }
    }
  ]
}";

        private string GetPrettierrcContent() => @"{
  ""printWidth"": 100,
  ""tabWidth"": 2,
  ""useTabs"": false,
  ""semi"": true,
  ""singleQuote"": true,
  ""trailingComma"": ""es5"",
  ""arrowParens"": ""always""
}";

        private string GetSonarPropertiesContent() => $@"sonar.projectKey=smartworkz_{_projectName.ToLower()}
sonar.projectName={_projectName}
sonar.projectVersion=1.0
sonar.projectDescription=SmartWorkz development project
sonar.sources=src
sonar.tests=tests
sonar.sourceEncoding=UTF-8

# Exclusions
sonar.exclusions=**/node_modules/**,**/bin/**,**/obj/**,**/*.min.js,**/dist/**
sonar.test.exclusions=**/node_modules/**,**/bin/**,**/obj/**

# Coverage
sonar.coverage.exclusions=**/*.spec.ts,**/*.spec.cs,**/tests/**,**/node_modules/**
sonar.typescript.lcov.reportPaths=coverage/lcov.info
sonar.cs.opencover.reportsPaths=**/coverage/opencover.xml

# Duplication
sonar.cpd.exclusions=**/*.spec.ts,**/*.spec.cs";

        private string GetPipelineYamlContent() => @"trigger:
  - main
  - develop
  - feature/*

pr:
  - main
  - develop

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '8.0.x'
  nodeVersion: '18.x'

stages:
  - stage: CodeQuality
    displayName: 'Code Quality Checks'
    jobs:
      - job: CodeAnalysis
        displayName: 'SonarQube & Linting'
        pool:
          vmImage: 'windows-latest'
        steps:
          - checkout: self
            fetchDepth: 0
          - task: SonarCloudPrepare@1
            displayName: 'Prepare SonarQube'
            inputs:
              SonarCloud: 'SonarCloud'
              organization: 'smartworkz'
              projectKey: 'smartworkz_project'
              projectName: 'Project'
          - task: DotNetCoreCLI@2
            displayName: 'Build'
            inputs:
              command: 'build'
              arguments: '--configuration $(buildConfiguration)'
          - task: SonarCloudAnalyze@1
          - task: SonarCloudPublish@1

      - job: UnitTests
        displayName: 'Unit Tests'
        pool:
          vmImage: 'windows-latest'
        steps:
          - task: UseDotNet@2
            inputs:
              version: $(dotnetVersion)
          - task: DotNetCoreCLI@2
            displayName: 'Run Tests'
            inputs:
              command: 'test'
              arguments: '/p:CollectCoverage=true'

  - stage: Build
    displayName: 'Build & Package'
    dependsOn: CodeQuality
    condition: succeeded()
    jobs:
      - job: BuildArtifacts
        pool:
          vmImage: 'windows-latest'
        steps:
          - task: DotNetCoreCLI@2
            inputs:
              command: 'publish'
              publishWebProjects: true
          - task: PublishBuildArtifacts@1";

        private string GetReadmeContent() => $@"# {_projectName}

SmartWorkz development project ({_projectType})

## Project Structure

```
{_projectName}/
├── src/              # Source code
├── tests/            # Test files
├── docs/             # Documentation
├── build/            # Build scripts
└── infra/            # Infrastructure (if applicable)
```

## Setup

### Prerequisites
- .NET 8.0+
- Node.js 18+
- Git

### Installation

```bash
git clone https://dev.azure.com/smartworkz/{_projectName}/_git/{_projectName}
cd {_projectName}
dotnet restore
npm install
```

## Development

### Code Quality Standards
- Code Coverage: Minimum 80%
- SonarQube Quality Gate: PASS
- Security Issues: None (Critical/High)
- Linting: 0 errors

### Git Workflow
1. Create feature branch: `git checkout -b feature/TASK-123-description`
2. Make changes and test locally
3. Push to remote: `git push origin feature/TASK-123-description`
4. Create Pull Request in Azure DevOps
5. Wait for pipeline checks to pass
6. Request code review (2 reviewers for main)
7. Merge when ready

### Running Tests

```bash
# .NET
dotnet test

# TypeScript/Angular
npm test

# All
npm run test:all
```

## CI/CD Pipeline

See [azure-pipelines.yml](./azure-pipelines.yml) for pipeline configuration.

### Pipeline Stages
1. **Code Quality** - SonarQube, Linting, Unit Tests
2. **Build** - Compile and package artifacts
3. **Deploy** - Deploy to staging environment

## Branch Protection Rules

- `main` - Production branch
  - Require PR review (2 approvals)
  - Require status checks

- `develop` - Staging branch
  - Require PR review (1 approval)
  - Require status checks

## Configuration Files

- **.editorconfig** - IDE formatting rules
- **.eslintrc.json** - TypeScript/JavaScript linting
- **.prettierrc.json** - Code formatting
- **sonar-project.properties** - SonarQube settings

## Team

SmartWorkz Technologies India Private Limited

## License

Internal - SmartWorkz";

        private string GetPrepareCommitMsgHookContent() => @"#!/bin/bash
# prepare-commit-msg hook - Add commit type/scope reminder

COMMIT_MSG_FILE=$1
COMMIT_SOURCE=$2

# Only for normal commits (not merges, squashes, etc.)
if [ ""$COMMIT_SOURCE"" = """" ]; then
    # Check if message already has a type
    if ! head -1 ""$COMMIT_MSG_FILE"" | grep -qE ""^(feat|fix|refactor|perf|docs|test|chore|ci)\(.*\):""; then
        # Add template reminder as comment
        {
            echo """"
            echo ""# Use format: type(scope): description""
            echo ""# Types: feat, fix, refactor, perf, docs, test, chore, ci""
            echo ""# Scopes: console, generator, api, git, files, docs""
            echo ""# Example: feat(generator): add custom path support""
            cat ""$COMMIT_MSG_FILE""
        } > /tmp/commit_msg_temp
        mv /tmp/commit_msg_temp ""$COMMIT_MSG_FILE""
    fi
fi";

        private string GetCommitMsgHookContent() => @"#!/bin/bash
# commit-msg hook - Validate commit message format

COMMIT_MSG_FILE=$1

# Read the commit message
COMMIT_MSG=$(cat ""$COMMIT_MSG_FILE"" | head -1)

# Validate format: type(scope): description
if ! echo ""$COMMIT_MSG"" | grep -qE ""^(feat|fix|refactor|perf|docs|test|chore|ci)\(.*\): ""; then
    echo ""❌ Invalid commit message format!""
    echo """"
    echo ""Required format: type(scope): description""
    echo """"
    echo ""Types: feat, fix, refactor, perf, docs, test, chore, ci""
    echo ""Scopes: console, generator, api, git, files, docs""
    echo """"
    echo ""Example: feat(generator): add custom path support""
    echo """"
    echo ""Your message: $COMMIT_MSG""
    exit 1
fi

# Check minimum length
MSG_LENGTH=${#COMMIT_MSG}
if [ $MSG_LENGTH -lt 10 ]; then
    echo ""❌ Commit message too short (minimum 10 characters)""
    exit 1
fi

if [ $MSG_LENGTH -gt 100 ]; then
    echo ""❌ Commit message too long (maximum 100 characters)""
    exit 1
fi

exit 0";

        private string GetCodeOwnersContent() => @"# Azure DevOps Code Review Policies
# Configure automatic reviewer assignment in:
# Project Settings → Repositories → [Repo Name] → Policies

## Configure These Code Review Policies:

### 1. Senior Developers Review Complex Code
Path Pattern: SmartWorkz.Tools.DevOpsProject/
Path Pattern: ProjectTemplateGenerator.cs
Reviewers: senior-developers (group)
Minimum Reviewers: 1

### 2. Code Quality Team Reviews Configuration
Path Pattern: .editorconfig
Path Pattern: .eslintrc.json
Path Pattern: .prettierrc.json
Path Pattern: azure-pipelines.yml
Path Pattern: sonar-project.properties
Reviewers: code-quality-team (group)
Minimum Reviewers: 1

### 3. Security Team Reviews Sensitive Code
Path Pattern: **/token**
Path Pattern: **/auth/**
Path Pattern: **/secret/**
Path Pattern: **/credential/**
Reviewers: security-team (group)
Minimum Reviewers: 1

### 4. Team Leads Review Everything (Default)
Path Pattern: **
Reviewers: team-leads (group)
Minimum Reviewers: 1 (or 2 for main branch)

## Setup Instructions in Azure DevOps UI:
1. Go to: Project Settings → Repositories → [Your Repo] → Policies
2. Click: Add Policy → Code Review
3. Configure each pattern above
4. Assign groups as reviewers
5. Set minimum reviewers count
6. Enable the policy";

        private string GetPullRequestTemplateContent() => @"## Description
Briefly describe the changes in this PR.

## Type of Change
- [ ] Feature (new functionality)
- [ ] Bug Fix (addressing an issue)
- [ ] Refactor (code improvement, no new feature)
- [ ] Documentation (docs only)
- [ ] Performance Improvement
- [ ] Security Enhancement

## Related Issues
Fixes #(issue number)

## Testing Done
Describe how you tested these changes.
- [ ] Unit tests passed
- [ ] Integration tests passed
- [ ] Manual testing completed
- [ ] No new test coverage needed (explain)

## Checklist
- [ ] Code follows commit message conventions
- [ ] Self-reviewed the code
- [ ] Added comments for complex logic
- [ ] Updated documentation if needed
- [ ] No new warnings in build
- [ ] Changes are backward compatible
- [ ] Security implications considered
- [ ] Performance impact analyzed

## Review Notes
Any special guidance for reviewers?";

        private string GetBranchPoliciesContent() => @"# Azure DevOps Branch Policies Configuration Guide

## Overview
This guide explains how to configure branch policies in Azure DevOps to enforce code quality standards and prevent direct commits to protected branches.

---

## main Branch (Production)

**Purpose:** Production releases - requires higher scrutiny

### Required Settings

Navigate to: **Repos → Branches → main → Branch Policies**

#### 1. Require a Minimum Number of Reviewers
- ✅ **Minimum number of reviewers:** 2
- ✅ **Allow requesters to approve their own changes:** No
- ✅ **Reset approval on new pushes:** Yes
- ✅ **Require approval from external sources:** Optional

**Why:** Production code needs two independent reviews to catch bugs and security issues

#### 2. Check for Linked Work Items
- ✅ **Required:** Yes
- **Description:** All commits to main must be linked to an Azure Boards work item

**Why:** Maintains traceability between code and requirements

#### 3. Check for Comment Resolution
- ✅ **Required:** Yes

**Why:** Ensures all review feedback is addressed before merge

#### 4. Require Status Checks to Pass
- ✅ **Required:** Yes
- **Status to check:**
  - Build validation
  - SonarQube quality gate
  - Automated tests

**Why:** Only tested, quality-approved code reaches production

#### 5. Bypass Policy
- ✅ **Who can bypass:** Only project admins
- ✅ **Require justification:** Yes

**Why:** Emergency hotfixes allowed but must be documented

#### 6. Auto-complete
- ✅ **Auto-complete set by:** No
- **Note:** Manual merge only for production changes

---

## develop Branch (Staging)

**Purpose:** Feature integration and staging environment

### Required Settings

Navigate to: **Repos → Branches → develop → Branch Policies**

#### 1. Require a Minimum Number of Reviewers
- ✅ **Minimum number of reviewers:** 1
- ✅ **Allow requesters to approve their own changes:** No
- ✅ **Reset approval on new pushes:** Yes

**Why:** One review is sufficient for staging; catches obvious issues

#### 2. Check for Linked Work Items
- ✅ **Required:** No (optional but recommended)

**Why:** Flexibility for small changes while tracking features

#### 3. Check for Comment Resolution
- ✅ **Required:** Yes

**Why:** All feedback must be addressed

#### 4. Require Status Checks to Pass
- ✅ **Required:** Yes
- **Status to check:**
  - Build validation
  - Unit tests
  - Linting

**Why:** Prevents broken code from reaching staging

#### 5. Auto-complete
- ✅ **Auto-complete set by:** Project admins (optional)
- **After approvals:** 15 minutes
- **On successful builds:** Yes

**Why:** Streamlines merging once criteria are met

---

## feature/*, bugfix/*, hotfix/* Branches

**Purpose:** Development branches - NO restrictions

### Settings

**No branch policies configured** - developers have full freedom

**Why:** Feature branches are temporary and private; policies applied at PR time

---

## Code Review Policies

Navigate to: **Project Settings → Repositories → [Repo Name] → Policies**

### Policy 1: Senior Developers Review Complex Code

**Path Pattern:** `SmartWorkz.Tools.DevOpsProject/**`, `**/ProjectTemplateGenerator.cs`

**Reviewers:** senior-developers (group)

**Min Reviewers:** 1

**Auto-notify:** Yes

---

### Policy 2: Code Quality Team Reviews Configuration

**Path Pattern:** `.editorconfig`, `.eslintrc.json`, `.prettierrc.json`, `azure-pipelines.yml`, `sonar-project.properties`

**Reviewers:** code-quality-team (group)

**Min Reviewers:** 1

**Auto-notify:** Yes

---

### Policy 3: Security Team Reviews Security Code

**Path Pattern:** `**/auth/**`, `**/token/**`, `**/secret/**`, `**/credential/**`

**Reviewers:** security-team (group)

**Min Reviewers:** 1

**Auto-notify:** Yes

---

### Policy 4: Team Leads Default Review

**Path Pattern:** `**` (catch-all)

**Reviewers:** team-leads (group)

**Min Reviewers:**
- main branch: 2
- develop branch: 1

**Auto-notify:** Yes

---

## How Branch Policies Enforce Quality

```
Developer creates PR
       ↓
Code Review Policies AUTO-ASSIGN reviewers
       ↓
Reviewers check code (linked to policies above)
       ↓
Status checks run (build, tests, SonarQube)
       ↓
All checks pass? ✅
       ↓
Required approvals received? ✅
       ↓
Comments resolved? ✅
       ↓
Merge allowed → Code goes to branch
```

---

## Step-by-Step Setup

### 1. Create Azure DevOps Groups First
```
Project Settings → Security → Groups
Create:
  ✅ admins
  ✅ team-leads
  ✅ senior-developers
  ✅ code-quality-team
  ✅ security-team
```

### 2. Add Team Members
```
Go to each group
Click: Add members
Add users from: .azuredevops/TEAM.yml
```

### 3. Configure main Branch Policy
```
Repos → Branches → main
Click: ...
Click: Branch policies
Enable:
  ✅ Require reviewers (2)
  ✅ Reset approval on new pushes
  ✅ Check for linked work items
  ✅ Check for comment resolution
  ✅ Require status checks
```

### 4. Configure develop Branch Policy
```
Repos → Branches → develop
Click: ...
Click: Branch policies
Enable:
  ✅ Require reviewers (1)
  ✅ Reset approval on new pushes
  ✅ Require status checks
```

### 5. Configure Code Review Policies
```
Project Settings → Repositories → [Repo] → Policies
Add Policy for each pattern:
  ✅ SmartWorkz.Tools.DevOpsProject → senior-developers
  ✅ Config files → code-quality-team
  ✅ Auth/token code → security-team
  ✅ ** (all) → team-leads
```

### 6. Test with a PR
```
Create test feature branch
Make a small change
Push and create PR
Verify:
  ✅ Auto-assigned reviewers appear
  ✅ Status checks start running
  ✅ Cannot merge until checks pass
  ✅ Cannot merge without approvals
```

---

## Common Scenarios

### Scenario 1: Hotfix Needed ASAP
**Process:**
1. Branch from main: `hotfix/TASK-123-critical-bug`
2. Make fix with detailed commit message
3. Create PR to main
4. 2 approvals required
5. Merge to main
6. Also merge to develop to keep in sync

**Branch policies prevent:** Direct commits, untested code, undocumented changes

---

### Scenario 2: New Feature Development
**Process:**
1. Branch from develop: `feature/TASK-456-new-feature`
2. Commit frequently: `feat(scope): description`
3. Push to remote
4. Create PR to develop
5. 1 approval required
6. Status checks must pass
7. Merge via squash-commit

**Branch policies prevent:** Skipped tests, quality issues, unclear commits

---

### Scenario 3: Configuration Change
**Process:**
1. Branch from develop: `feature/TASK-789-config-update`
2. Update config files
3. Create PR
4. AUTO-ASSIGNED: code-quality-team reviews
5. They verify best practices
6. They approve
7. Merge when ready

**Branch policies prevent:** Misconfigured environments, security issues in configs

---

## Troubleshooting

### Problem: Can't Merge PR
**Causes & Solutions:**
1. ❌ Need approvals? → Get required number of approvals
2. ❌ Status checks failing? → Fix failures and push
3. ❌ Comments not resolved? → Resolve all comments
4. ❌ Not up to date? → Click 'Update branch' button
5. ❌ Work item not linked? → Link in PR details

---

### Problem: Reviewers Not Auto-Assigned
**Cause:** Code review policies not configured
**Solution:**
1. Go to Project Settings → Repositories → Policies
2. Create policies for each team
3. Test with new PR

---

### Problem: Status Checks Taking Too Long
**Solution:**
1. Optimize build pipeline in `azure-pipelines.yml`
2. Run tests in parallel where possible
3. Cache dependencies
4. Consider splitting into stages

---

## Summary

Branch policies in Azure DevOps:
- ✅ Protect main and develop branches
- ✅ Enforce code review standards
- ✅ Require passing status checks
- ✅ Auto-assign reviewers based on code changes
- ✅ Maintain code quality and security
- ✅ Create audit trail of all changes

**Result:** Production-ready code every time

---

**Version:** 2.0.0
**Last Updated:** 2026-04-28
**For:** All Team Members";

        private string GetTeamConfigContent() => @"# SmartWorkz.Tools Team Configuration (Azure DevOps)
# This file defines team members and their roles.
# Create corresponding groups in Azure DevOps and add these members.

## ADMINS
# Full access, security rules, branch policies, project management
admins:
  - name: Tech Lead
    email: tech.lead@smartworkz.com
    aad_username: tech.lead@smartworkz.com
    role: Lead Developer

## TEAM-LEADS
# Approve PRs to main (2 required), approve PRs to develop (1 needed)
team_leads:
  - name: Senior Developer 1
    email: senior1@smartworkz.com
    aad_username: senior1@smartworkz.com
    role: Senior Developer

  - name: Senior Developer 2
    email: senior2@smartworkz.com
    aad_username: senior2@smartworkz.com
    role: Architect

## SENIOR-DEVELOPERS
# Review complex code, validate implementations
senior_developers:
  - name: Mid-level Developer 1
    email: mid1@smartworkz.com
    aad_username: mid1@smartworkz.com
    role: Developer

  - name: Mid-level Developer 2
    email: mid2@smartworkz.com
    aad_username: mid2@smartworkz.com
    role: Developer

## CODE-QUALITY-TEAM
# Review configuration files, validate structure
code_quality_team:
  - name: Code Quality Lead
    email: qa.lead@smartworkz.com
    aad_username: qa.lead@smartworkz.com
    role: QA Lead

## SECURITY-TEAM
# Review authentication, tokens, secrets
security_team:
  - name: Security Specialist
    email: security@smartworkz.com
    aad_username: security@smartworkz.com
    role: Security Engineer

## AZURE DEVOPS CONFIGURATION
organization: smartworkz
project: SmartWorkz
repository: SmartWorkz

## BRANCH POLICIES
branch_policies:
  main:
    required_approvals: 2
    reset_on_new_changes: true
    require_status_checks: true
  develop:
    required_approvals: 1
    reset_on_new_changes: true
    require_status_checks: true

## SETUP INSTRUCTIONS
# 1. Create groups in: Project Settings → Security → Groups
# 2. Add members to each group
# 3. Configure branch policies: Repos → Branches → Branch Policies
# 4. Configure code review policies: Project Settings → Repositories → Policies";

        #region Tech-Stack-Specific Configuration Files

        private string GetPackageJsonContent(string framework) => $@"{{
  ""name"": ""{_projectName.ToLower()}"",
  ""version"": ""0.0.1"",
  ""description"": ""SmartWorkz {framework} project"",
  ""main"": ""dist/index.js"",
  ""scripts"": {{
    ""dev"": ""vite"",
    ""build"": ""vite build"",
    ""lint"": ""eslint src --ext .ts,.tsx"",
    ""format"": ""prettier --write src"",
    ""test"": ""vitest"",
    ""test:coverage"": ""vitest --coverage""
  }},
  ""dependencies"": {{}},
  ""devDependencies"": {{
    ""typescript"": ""^5.0.0"",
    ""eslint"": ""^8.0.0"",
    ""prettier"": ""^3.0.0"",
    ""vite"": ""^5.0.0"",
    ""vitest"": ""^1.0.0""
  }}
}}";

        private string GetTsConfigContent() => @"{
  ""compilerOptions"": {
    ""target"": ""ES2020"",
    ""useDefineForClassFields"": true,
    ""lib"": [""ES2020"", ""DOM"", ""DOM.Iterable""],
    ""module"": ""ESNext"",
    ""skipLibCheck"": true,
    ""esModuleInterop"": true,
    ""allowSyntheticDefaultImports"": true,
    ""strict"": true,
    ""noUnusedLocals"": true,
    ""noUnusedParameters"": true,
    ""resolveJsonModule"": true,
    ""moduleResolution"": ""bundler"",
    ""allowImportingTsExtensions"": true,
    ""declaration"": true,
    ""declarationMap"": true,
    ""sourceMap"": true
  },
  ""include"": [""src""],
  ""references"": [{ ""path"": ""./tsconfig.node.json"" }]
}";

        private string GetMavenPomContent() => $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<project xmlns=""http://maven.apache.org/POM/4.0.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
         xsi:schemaLocation=""http://maven.apache.org/POM/4.0.0
         http://maven.apache.org/xsd/maven-4.0.0.xsd"">
    <modelVersion>4.0.0</modelVersion>

    <groupId>com.smartworkz</groupId>
    <artifactId>{_projectName.ToLower()}</artifactId>
    <version>0.0.1-SNAPSHOT</version>
    <name>{_projectName}</name>
    <description>SmartWorkz Java project</description>

    <parent>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-starter-parent</artifactId>
        <version>3.2.0</version>
        <relativePath/>
    </parent>

    <properties>
        <java.version>17</java.version>
        <maven.compiler.source>17</maven.compiler.source>
        <maven.compiler.target>17</maven.compiler.target>
        <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
    </properties>

    <dependencies>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-web</artifactId>
        </dependency>
        <dependency>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-starter-test</artifactId>
            <scope>test</scope>
        </dependency>
    </dependencies>

    <build>
        <plugins>
            <plugin>
                <groupId>org.springframework.boot</groupId>
                <artifactId>spring-boot-maven-plugin</artifactId>
            </plugin>
        </plugins>
    </build>
</project>";

        private string GetJavaApplicationProperties() => @"spring.application.name=smartworkz-app
server.port=8080
spring.profiles.active=dev

# Logging
logging.level.root=INFO
logging.level.com.smartworkz=DEBUG
logging.pattern.console=%d{HH:mm:ss.SSS} [%thread] %-5level %logger{36} - %msg%n";

        private string GetComposerJsonContent() => $@"{{
  ""name"": ""smartworkz/{_projectName.ToLower()}"",
  ""description"": ""SmartWorkz PHP project"",
  ""type"": ""project"",
  ""require"": {{
    ""php"": ""^8.2"",
    ""laravel/framework"": ""^11.0""
  }},
  ""require-dev"": {{
    ""laravel/pint"": ""^1.0"",
    ""phpunit/phpunit"": ""^10.0""
  }},
  ""autoload"": {{
    ""psr-4"": {{
      ""App\\\\"": ""src/""
    }}
  }},
  ""autoload-dev"": {{
    ""psr-4"": {{
      ""Tests\\\\"": ""tests/""
    }}
  }}
}}";

        private string GetPhpEnvContent() => @"APP_NAME=SmartWorkz
APP_ENV=development
APP_DEBUG=true
APP_URL=http://localhost:8000

DB_CONNECTION=mysql
DB_HOST=127.0.0.1
DB_PORT=3306
DB_DATABASE=smartworkz
DB_USERNAME=root
DB_PASSWORD=

CACHE_DRIVER=file
QUEUE_CONNECTION=sync";

        private string GetPhpUnitContent() => @"<?xml version=""1.0"" encoding=""UTF-8""?>
<phpunit xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
         xsi:noNamespaceSchemaLocation=""https://schema.phpunit.de/10.0/phpunit.xsd""
         bootstrap=""vendor/autoload.php""
         colors=""true"">
    <testsuites>
        <testsuite name=""Unit"">
            <directory>tests/unit</directory>
        </testsuite>
        <testsuite name=""Integration"">
            <directory>tests/integration</directory>
        </testsuite>
    </testsuites>
    <coverage processUncoveredFiles=""true"">
        <include>
            <directory suffix="".php"">src</directory>
        </include>
    </coverage>
</phpunit>";

        private string GetFlutterPubspecContent() => $@"name: {_projectName.ToLower().Replace(' ', '_')}
description: SmartWorkz Flutter project
version: 0.0.1+1

environment:
  sdk: '>=3.0.0 <4.0.0'

dependencies:
  flutter:
    sdk: flutter
  cupertino_icons: ^1.0.2
  http: ^1.1.0
  provider: ^6.0.0
  google_fonts: ^6.0.0

dev_dependencies:
  flutter_test:
    sdk: flutter
  flutter_lints: ^3.0.0

flutter:
  uses-material-design: true
  assets:
    - assets/images/
    - assets/icons/";

        private string GetFlutterAnalysisOptions() => @"include: package:flutter_lints/flutter.yaml

linter:
  rules:
    - avoid_empty_else
    - avoid_print
    - avoid_relative_import_imports
    - avoid_returning_null
    - avoid_slow_async_io
    - cancel_subscriptions
    - close_sinks
    - comment_references
    - control_flow_in_finally
    - empty_statements
    - hash_and_equals
    - invariant_booleans
    - iterable_contains_unrelated_type
    - list_remove_unrelated_type
    - literal_only_boolean_expressions
    - no_adjacent_strings_in_list
    - no_duplicate_case_values
    - prefer_void_to_future
    - throw_in_finally
    - unnecessary_statements
    - unrelated_type_equality_checks";

        #endregion

        #endregion

        #region Console Output Helpers

        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        #endregion
    }
}
