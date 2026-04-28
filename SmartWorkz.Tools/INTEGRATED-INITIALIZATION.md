# ProjectTemplateGenerator - Integrated Initialization System

**Version**: 2.0.0  
**Status**: Initialization automation now integrated into ProjectTemplateGenerator

---

## Overview

The `ProjectTemplateGenerator` class now includes **complete initialization automation** as part of the project creation workflow. When you create a new Azure DevOps project, the generator automatically:

1. ✅ Creates the project in Azure DevOps
2. ✅ Clones the repository locally
3. ✅ Creates project structure
4. ✅ Generates configuration files
5. ✅ **Installs Git hooks** (commit message validation)
6. ✅ **Creates CODEOWNERS file** (automatic reviewer assignment)
7. ✅ **Creates PR template** (standardized pull requests)
8. ✅ **Creates team configuration** (team roles and definitions)
9. ✅ Commits and pushes everything

---

## What Gets Created Automatically

### 1. Git Hooks (.git/hooks/)

**Files Created:**
- `prepare-commit-msg` - Displays commit format reminder
- `commit-msg` - Validates commit message format

**What They Do:**
- Require format: `type(scope): description`
- Valid types: `feat, fix, refactor, perf, docs, test, chore, ci`
- Valid scopes: `console, generator, api, git, files, docs`
- Length validation: 10-100 characters
- Automatic rejection of invalid commits

**Example Valid Commits:**
```
feat(generator): add custom path support
fix(console): handle null reference exception
refactor(api): simplify request processing
docs(readme): update setup instructions
```

### 2. CODEOWNERS File (.github/CODEOWNERS)

**Purpose:** Automatically request code review from appropriate teams

**Teams Configured:**
```
* @smartworkz/team-leads                          # Everyone
SmartWorkz.Tools.DevOpsProject/ @smartworkz/senior-developers
.editorconfig @smartworkz/code-quality-team
**/token* @smartworkz/security-team
**/auth* @smartworkz/security-team
```

**How It Works:**
- When PR changes files in a pattern, matching team is auto-requested
- Example: PR touches `ProjectTemplateGenerator.cs` → `@smartworkz/senior-developers` gets requested
- Example: PR modifies `.editorconfig` → `@smartworkz/code-quality-team` gets requested

### 3. Pull Request Template (.github/PULL_REQUEST_TEMPLATE.md)

**Purpose:** Standardizes pull request format

**Template Includes:**
- Description field
- Type of Change checkbox (Feature, Bug, Refactor, Docs, Performance, Security)
- Related Issues section
- Testing checklist
- Review checklist

**What Developers See:**
When creating a PR, the template automatically appears and guides completion of:
- PR description
- Testing verification
- Code review checklist
- Commit conventions compliance

### 4. Team Configuration File (.github/TEAM.yml)

**Purpose:** Defines team structure and roles

**Sections:**
```yaml
admins:          # Admin access, full repository control
team_leads:      # Approve PRs to main (2 required)
senior_developers: # Review complex code
code_quality_team: # Review config and architecture
security_team:   # Review security-related code
```

**What's Included:**
- Team member names and emails
- GitHub usernames
- Role descriptions
- Branch protection settings
- Approval requirements per branch

---

## Execution Flow

When you run the console app:

```bash
dotnet run -- --name "MyProject" --type "DotNet"
```

The execution flow is now:

```
1. ✅ Create Azure DevOps Project
   └─ Via REST API call

2. ✅ Wait for initialization
   └─ 5-second delay

3. ✅ Setup Repository
   └─ Clone to local path

4. ✅ Create Configuration Files
   └─ .gitignore, .editorconfig, .eslintrc.json, etc.

5. ✅ Initialize Git Hooks          [NEW]
   └─ Install prepare-commit-msg and commit-msg

6. ✅ Configure Code Ownership       [NEW]
   └─ Create CODEOWNERS file
   └─ Create PULL_REQUEST_TEMPLATE.md

7. ✅ Create Team Configuration      [NEW]
   └─ Generate .github/TEAM.yml

8. ✅ Git Operations
   └─ git add .
   └─ git commit -m "Initial commit"
   └─ git push origin main

9. ✅ Display Completion Summary
   └─ List created files
   └─ Show next steps
```

---

## Output Example

When ProjectTemplateGenerator completes, you'll see:

```
================================
SmartWorkz DevOps Template Setup
================================

Step 1: Creating Azure DevOps Project...
✓ Project created: MyProject (abc-123-def)

Step 2: Setting up repository structure...
✓ Repository cloned and structured

Step 3: Creating configuration files...
✓ Configuration files created

Step 4: Setting up Git hooks and validation...
✓ Git hooks configured
  • prepare-commit-msg hook installed
  • commit-msg hook installed

Step 5: Configuring code ownership and PR template...
✓ CODEOWNERS and PR template created

Step 6: Creating team configuration...
✓ Team configuration created

Step 7: Initializing git repository...
✓ Repository initialized and pushed

==================================================
✓ Project setup completed!

📋 INITIALIZATION CHECKLIST:

✓ Completed:
  • Azure DevOps project created
  • Repository cloned and structured
  • Configuration files generated
  • Git hooks configured locally
  • CODEOWNERS file created
  • PR template configured
  • Team roles defined

⚠️  Manual Next Steps (Admin Only):
  1. Create GitHub teams: https://github.com/orgs/smartworkz/teams
     Teams needed: admins, team-leads, senior-developers, code-quality-team, security-team
  2. Set up GitHub branch protection (main: 2 approvals, develop: 1 approval)
  3. Add team members to GitHub teams
  4. Configure service connections
  5. Set up SonarCloud connection

📖 Documentation:
  • Read: S:\SmartWorkz101\MyProject\COLLABORATION.md
  • Read: S:\SmartWorkz101\MyProject\.github\SETUP.md
  • Review: S:\SmartWorkz101\MyProject\.github\TEAM.yml

Project URL: https://dev.azure.com/smartworkz/MyProject
Repository: https://dev.azure.com/smartworkz/MyProject/_git/MyProject
```

---

## Project Structure After Generation

```
MyProject/
├── src/                           # Source code
├── tests/                         # Tests
├── docs/                          # Documentation
├── build/                         # Build scripts
├── .git/
│   └── hooks/
│       ├── prepare-commit-msg     [NEW] ✅ Git hook
│       └── commit-msg             [NEW] ✅ Git hook
├── .github/
│   ├── CODEOWNERS                 [NEW] ✅ Reviewer auto-assignment
│   ├── PULL_REQUEST_TEMPLATE.md   [NEW] ✅ PR standardization
│   └── TEAM.yml                   [NEW] ✅ Team definitions
├── .gitignore                     ✅ Version control exclusions
├── .editorconfig                  ✅ IDE formatting
├── .eslintrc.json                 ✅ Linting rules
├── .prettierrc.json               ✅ Code formatting
├── sonar-project.properties       ✅ SonarQube config
├── azure-pipelines.yml            ✅ CI/CD pipeline
└── README.md                      ✅ Project documentation
```

---

## Git Hooks Behavior

### When You Commit

```bash
git commit -m "feat(generator): add feature"
```

**Valid Commit:**
- Message matches format ✓
- Length is 10-100 chars ✓
- Type is valid ✓
- **Result:** Commit accepted ✅

**Invalid Commit:**
```bash
git commit -m "add feature"
```
- Missing type and scope
- **Result:** Commit rejected ❌
```
❌ Invalid commit message format!

Required format: type(scope): description

Types: feat, fix, refactor, perf, docs, test, chore, ci
Scopes: console, generator, api, git, files, docs

Example: feat(generator): add custom path support

Your message: add feature
```

---

## CODEOWNERS Behavior

### When PR Touches Specific Files

**Example 1: PR modifies ProjectTemplateGenerator.cs**
```
Files changed:
  - SmartWorkz.Tools.DevOpsProject/ProjectTemplateGenerator.cs

Action: GitHub auto-requests review from @smartworkz/senior-developers
```

**Example 2: PR modifies .editorconfig**
```
Files changed:
  - .editorconfig

Action: GitHub auto-requests review from @smartworkz/code-quality-team
```

**Example 3: PR adds auth token handling**
```
Files changed:
  - src/auth/TokenHandler.cs

Action: GitHub auto-requests review from @smartworkz/security-team
         (pattern: **/auth*)
```

---

## Team Configuration

### What Developers Need to Do

1. **Update .github/TEAM.yml** with actual team members
   ```yaml
   admins:
     - name: "Your Name"
       email: "your.email@smartworkz.com"
       github: "your-github-username"
       role: "Role"
   ```

2. **Share COLLABORATION.md** with team
   - Location: `Project/COLLABORATION.md`
   - Explains branching strategy
   - Shows commit message format
   - Describes code review process

3. **Share SETUP.md** with admins
   - Location: `Project/.github/SETUP.md`
   - Step-by-step GitHub setup
   - Branch protection configuration
   - Team creation instructions

### What Admins Need to Do

**GitHub Setup (requires admin access):**

1. **Create teams** at https://github.com/orgs/smartworkz/teams
   - admins
   - team-leads
   - senior-developers
   - code-quality-team
   - security-team

2. **Add team members**
   - Use GitHub usernames from TEAM.yml
   - Assign to appropriate teams

3. **Set branch protection rules**
   - main: 2 approvals required
   - develop: 1 approval required
   - Status checks required
   - Dismiss stale reviews

4. **Verify CODEOWNERS is working**
   - Create test PR
   - Verify correct teams are auto-requested

---

## Integration with Existing Process

### Before This Update

1. Developer runs: `pwsh .github/init-project.ps1`
2. Admin runs: `bash .github/setup-protection.sh`
3. Manual creation of CODEOWNERS
4. Manual creation of PR template
5. Manual creation of team config

### After This Update

1. **Everything happens automatically** during project creation
2. No need for separate init scripts for each project
3. Consistent setup across all new projects
4. Git hooks working immediately
5. CODEOWNERS ready for team assignment

---

## Differences from Manual Setup

| Aspect | Manual Init | ProjectTemplateGenerator |
|--------|------------|------------------------|
| **Git Hooks** | Run init-project.ps1 | Automatic |
| **CODEOWNERS** | Create manually | Auto-generated |
| **PR Template** | Create manually | Auto-generated |
| **Team Config** | Edit TEAM.yml manually | Auto-generated with defaults |
| **Time Required** | 30+ minutes | Included in project creation (seconds) |
| **Consistency** | Varies per developer | Always identical |
| **Documentation** | Separate files | Integrated guide |

---

## Troubleshooting

### Git Hooks Not Working

**Issue:** Commit validation not happening

**Cause:** Hooks are bash scripts; Windows needs Git Bash or WSL

**Solution:**
1. Install Git Bash (comes with Git on Windows)
2. Or use WSL (Windows Subsystem for Linux)
3. Or use PowerShell pre-commit hooks (advanced)

**Verify:**
```bash
# Check if hooks exist
ls -la .git/hooks/

# Check if executable
git config core.hooksPath
```

### CODEOWNERS Not Assigning Reviewers

**Issue:** PRs don't auto-request team reviews

**Cause:** Teams don't exist on GitHub yet

**Solution:**
1. Create teams in GitHub: https://github.com/orgs/smartworkz/teams
2. Use exact names: `team-leads`, `senior-developers`, etc.
3. Add team members
4. Verify with test PR

### PR Template Not Showing

**Issue:** PR template doesn't appear in GitHub

**Cause:** File not in correct location or named incorrectly

**Solution:**
1. Verify file exists: `.github/PULL_REQUEST_TEMPLATE.md`
2. Verify file name (exact match, case-sensitive on some systems)
3. Refresh browser cache
4. Try creating new PR

---

## Extending the Initialization

### Adding More Commit Scopes

Edit the hook validation in ProjectTemplateGenerator:

```csharp
private string GetCommitMsgHookContent() => @"#!/bin/bash
# Change this line to add more scopes
if ! echo ""$COMMIT_MSG"" | grep -qE ""^(feat|fix|refactor|perf|docs|test|chore|ci)\(.*\): ""
```

Add scope to the regex pattern: `(feat|fix|refactor|perf|docs|test|chore|ci|newscope)`

### Adding More CODEOWNERS Patterns

Edit the CODEOWNERS content method:

```csharp
private string GetCodeOwnersContent() => @"
# Add your custom pattern:
src/newfeature/ @smartworkz/new-team
```

### Adding More Team Members

Edit the TEAM.yml generator:

```csharp
private string GetTeamConfigContent() => @"
new_team:
  - name: ""Developer Name""
    email: ""dev@smartworkz.com""
    github: ""dev-username""
    role: ""Role""
```

---

## Key Enhancements

### ✅ What's New

1. **Integrated Git Hooks**
   - No separate script execution needed
   - Hooks installed automatically
   - Commit validation immediate

2. **Automatic CODEOWNERS**
   - Teams auto-assigned to PRs
   - Pattern-based assignment
   - Reduces manual reviewer selection

3. **PR Template Included**
   - Standardized PR format
   - Built-in checklist
   - Type of change selection

4. **Team Configuration**
   - Pre-populated with best practices
   - Easy to customize
   - Clear role definitions

5. **Unified Setup**
   - Single command creates everything
   - No separate admin/developer setup
   - One-time initialization per project

---

## Quick Reference

### Create New Project

```bash
cd SmartWorkz.Tools\SmartWorkz.Tools.Console
dotnet run -- --name "NewProject" --type "DotNet"
```

**What happens:**
- ✅ Azure DevOps project created
- ✅ Repository cloned
- ✅ Git hooks installed
- ✅ CODEOWNERS created
- ✅ PR template created
- ✅ Team config created
- ✅ Everything committed and pushed

### After Project Creation

```bash
# 1. Review team config
cd S:\SmartWorkz101\NewProject
cat .github\TEAM.yml

# 2. Update with your team members
# (edit file as needed)

# 3. Create GitHub teams
# https://github.com/orgs/smartworkz/teams

# 4. Test Git hooks
git checkout -b feature/TEST-001
git commit -m "feat(console): test commit"  # ✅ Should succeed

# 5. Test invalid commit
git commit -m "invalid message"  # ❌ Should fail
```

---

## Support & Questions

**For questions about:**
- **Git hooks:** See Troubleshooting section above
- **CODEOWNERS:** Check .github/CODEOWNERS in your project
- **Team setup:** Review .github/TEAM.yml
- **PR template:** Check .github/PULL_REQUEST_TEMPLATE.md
- **Development guide:** Read CLAUDE.md in project root

---

**Version**: 2.0.0  
**Status**: Production Ready  
**Last Updated**: 2026-04-28
