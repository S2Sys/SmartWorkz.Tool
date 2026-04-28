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

        // Valid project types
        private static readonly string[] ValidProjectTypes = { "Angular", "DotNet", "FullStack", "Mobile", "AI" };

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

                // Step 5: Create CODEOWNERS and PR Template
                WriteInfo("\nStep 5: Configuring code ownership and PR template...");
                CreateCodeOwnersFile();
                CreatePullRequestTemplate();
                WriteSuccess("✓ CODEOWNERS and PR template created");

                // Step 6: Create Team Configuration
                WriteInfo("\nStep 6: Creating team configuration...");
                CreateTeamConfigFile();
                WriteSuccess("✓ Team configuration created");

                // Step 7: Git operations
                WriteInfo("\nStep 7: Initializing git repository...");
                await GitAddCommitPushAsync();
                WriteSuccess("✓ Repository initialized and pushed");

                // Step 8: Summary with initialization guide
                WriteInfo("\n" + new string('=', 50));
                WriteSuccess("✓ Project setup completed!");
                WriteInfo("\n📋 INITIALIZATION CHECKLIST:");
                WriteInfo("\n✓ Completed:");
                WriteInfo("  • Azure DevOps project created");
                WriteInfo("  • Repository cloned and structured");
                WriteInfo("  • Configuration files generated");
                WriteInfo("  • Git hooks configured locally");
                WriteInfo("  • CODEOWNERS file created");
                WriteInfo("  • PR template configured");
                WriteInfo("  • Team roles defined");
                WriteInfo("\n⚠️  Manual Next Steps (Admin Only):");
                WriteInfo("  1. Create GitHub teams: https://github.com/orgs/smartworkz/teams");
                WriteInfo("     Teams needed: admins, team-leads, senior-developers, code-quality-team, security-team");
                WriteInfo("  2. Set up GitHub branch protection (main: 2 approvals, develop: 1 approval)");
                WriteInfo("  3. Add team members to GitHub teams");
                WriteInfo("  4. Configure service connections");
                WriteInfo("  5. Set up SonarCloud connection");
                WriteInfo("\n📖 Documentation:");
                WriteInfo($"  • Read: {Path.Combine(_localDevPath, _projectName, "COLLABORATION.md")}");
                WriteInfo($"  • Read: {Path.Combine(_localDevPath, _projectName, ".github", "SETUP.md")}");
                WriteInfo($"  • Review: {Path.Combine(_localDevPath, _projectName, ".github", "TEAM.yml")}");
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

            // Clone repository
            await RunGitCommandAsync("clone", $"{repoUrl} \"{localPath}\"");

            // Create folder structure based on type
            CreateFolderStructure(localPath);
        }

        /// <summary>
        /// Create folder structure based on project type
        /// </summary>
        private void CreateFolderStructure(string basePath)
        {
            WriteInfo($"Creating project structure for: {_projectType}");

            var folders = _projectType switch
            {
                "DotNet" => new[]
                {
                    "src", "src/API", "src/Services", "src/Data",
                    "tests", "tests/UnitTests", "tests/IntegrationTests",
                    "docs", "build"
                },
                "Angular" => new[]
                {
                    "src", "src/app", "src/assets", "src/environments",
                    "tests", "tests/unit", "tests/e2e",
                    "docs", "build"
                },
                "FullStack" => new[]
                {
                    "src", "src/api", "src/web", "src/shared",
                    "tests", "tests/api", "tests/web",
                    "docs", "build", "infra", "infra/terraform", "infra/kubernetes"
                },
                "Mobile" => new[]
                {
                    "src", "src/android", "src/ios", "src/shared",
                    "tests", "docs", "build"
                },
                "AI" => new[]
                {
                    "src", "src/notebooks", "src/training", "src/inference",
                    "tests", "docs", "models", "datasets"
                },
                _ => new[] { "src", "tests", "docs", "build" }
            };

            foreach (var folder in folders)
            {
                string folderPath = Path.Combine(basePath, folder);
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// Step 3: Create configuration files
        /// </summary>
        private void CreateConfigurationFiles()
        {
            string basePath = Path.Combine(_localDevPath, _projectName);

            // .gitignore
            File.WriteAllText(
                Path.Combine(basePath, ".gitignore"),
                GetGitignoreContent());

            // .editorconfig
            File.WriteAllText(
                Path.Combine(basePath, ".editorconfig"),
                GetEditorconfigContent());

            // .eslintrc.json
            File.WriteAllText(
                Path.Combine(basePath, ".eslintrc.json"),
                GetEslintrcContent());

            // .prettierrc.json
            File.WriteAllText(
                Path.Combine(basePath, ".prettierrc.json"),
                GetPrettierrcContent());

            // sonar-project.properties
            File.WriteAllText(
                Path.Combine(basePath, "sonar-project.properties"),
                GetSonarPropertiesContent());

            // azure-pipelines.yml
            File.WriteAllText(
                Path.Combine(basePath, "azure-pipelines.yml"),
                GetPipelineYamlContent());

            // README.md
            File.WriteAllText(
                Path.Combine(basePath, "README.md"),
                GetReadmeContent());
        }

        /// <summary>
        /// Step 4: Git operations - add, commit, push
        /// </summary>
        private async Task GitAddCommitPushAsync()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);

            // Change to repo directory and execute git commands
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c cd {repoPath} && git add . && git commit -m \"Initial commit: SmartWorkz project template\" && git push origin main",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception("Git operations failed");
                }
            }

            await Task.CompletedTask;
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
        /// Helper: Run git command
        /// </summary>
        private async Task RunGitCommandAsync(params string[] args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = string.Join(" ", args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception($"Git command failed: {error}");
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
            string gitHubDir = Path.Combine(repoPath, ".github");
            Directory.CreateDirectory(gitHubDir);

            string codeOwnersPath = Path.Combine(gitHubDir, "CODEOWNERS");
            File.WriteAllText(codeOwnersPath, GetCodeOwnersContent());
        }

        /// <summary>
        /// Step 5b: Create Pull Request template
        /// </summary>
        private void CreatePullRequestTemplate()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string gitHubDir = Path.Combine(repoPath, ".github");
            Directory.CreateDirectory(gitHubDir);

            string prTemplatePath = Path.Combine(gitHubDir, "PULL_REQUEST_TEMPLATE.md");
            File.WriteAllText(prTemplatePath, GetPullRequestTemplateContent());
        }

        /// <summary>
        /// Step 6: Create team configuration file
        /// </summary>
        private void CreateTeamConfigFile()
        {
            string repoPath = Path.Combine(_localDevPath, _projectName);
            string gitHubDir = Path.Combine(repoPath, ".github");
            Directory.CreateDirectory(gitHubDir);

            string teamFilePath = Path.Combine(gitHubDir, "TEAM.yml");
            File.WriteAllText(teamFilePath, GetTeamConfigContent());
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

        private string GetCodeOwnersContent() => @"# SmartWorkz.Tools CODEOWNERS
# Automatically request code review from appropriate teams

# Default: Team leads review everything
* @smartworkz/team-leads

# Senior developers review complex code
SmartWorkz.Tools.DevOpsProject/ @smartworkz/senior-developers
ProjectTemplateGenerator.cs @smartworkz/senior-developers

# Code quality team reviews configuration and structure
.editorconfig @smartworkz/code-quality-team
.eslintrc.json @smartworkz/code-quality-team
.prettierrc.json @smartworkz/code-quality-team
azure-pipelines.yml @smartworkz/code-quality-team
sonar-project.properties @smartworkz/code-quality-team

# Security team reviews authentication and secrets
**/token* @smartworkz/security-team
**/auth* @smartworkz/security-team
**/secret* @smartworkz/security-team
**/credential* @smartworkz/security-team

# Documentation review
*.md @smartworkz/team-leads
COLLABORATION.md @smartworkz/team-leads
INITIALIZATION.md @smartworkz/team-leads
CLAUDE.md @smartworkz/team-leads";

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

        private string GetTeamConfigContent() => @"# SmartWorkz.Tools Team Configuration
#
# This file defines team members and their roles.
# Update this file when team members are added or removed.

## Administrators
# Full access, security rules, branch protection
admins:
  - name: ""Tech Lead""
    email: ""tech.lead@smartworkz.com""
    github: ""tech-lead-username""
    role: ""Lead Developer""

## Team Leads
# Approve critical changes to main branch
team_leads:
  - name: ""Senior Developer 1""
    email: ""senior1@smartworkz.com""
    github: ""senior1-username""
    role: ""Senior Developer""

  - name: ""Senior Developer 2""
    email: ""senior2@smartworkz.com""
    github: ""senior2-username""
    role: ""Architect""

## Senior Developers
# Review complex code
senior_developers:
  - name: ""Mid-level Developer 1""
    email: ""mid1@smartworkz.com""
    github: ""mid1-username""
    role: ""Developer""

  - name: ""Mid-level Developer 2""
    email: ""mid2@smartworkz.com""
    github: ""mid2-username""
    role: ""Developer""

## Code Quality Team
# Review configuration and architecture
code_quality_team:
  - name: ""Code Quality Lead""
    email: ""qa.lead@smartworkz.com""
    github: ""qa-lead-username""
    role: ""QA Lead""

## Security Team
# Review security-related code
security_team:
  - name: ""Security Specialist""
    email: ""security@smartworkz.com""
    github: ""security-username""
    role: ""Security Engineer""

## Configuration
repository: ""smartworkz/{_projectName}""
branch_protection:
  main:
    required_approvals: 2
    enforce_admins: true
  develop:
    required_approvals: 1
    enforce_admins: false";

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
