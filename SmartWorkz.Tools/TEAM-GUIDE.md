# SmartWorkz.Tools - Complete Team Guide (Azure DevOps)

**Everything you need to know - from project creation to code review in Azure DevOps**

---

## Quick Start

### For Developers (5 minutes)

**Step 1: Create a project**
```bash
cd SmartWorkz.Tools\SmartWorkz.Tools.Console
dotnet run -- --name "MyProject" --type "DotNet"
```

**Step 2: Everything is ready**
- ✅ Azure DevOps project created
- ✅ Repository cloned to `S:\SmartWorkz101\MyProject`
- ✅ Git hooks installed (commit validation)
- ✅ Configuration files generated
- ✅ Team roles defined
- ✅ Branch policy guide created

**Step 3: Update team members**
```bash
cd S:\SmartWorkz101\MyProject
code .azuredevops/TEAM.yml
# Edit with your actual team members (use Azure AD emails)
```

### For Admins (30 minutes)

After developers create the project:

1. **Create Azure DevOps groups**
   - Go to: Project Settings → Security → Groups
   - Create: admins, team-leads, senior-developers, code-quality-team, security-team

2. **Add team members**
   - Use Azure AD usernames (email format)
   - Add members from .azuredevops/TEAM.yml

3. **Configure branch policies**
   - Go to: Repos → Branches
   - Select main branch → Branch policies
   - Set 2 approvals, status checks required
   - Select develop branch → Branch policies
   - Set 1 approval, status checks required

4. **Configure code review policies**
   - Go to: Project Settings → Repositories → [Repo Name] → Policies
   - Add policies following .azuredevops/CODEOWNERS guide

5. **Test with a PR**
   - Create test feature branch
   - Submit PR
   - Verify policies are enforced

---

## Creating a New Project

### Command

```bash
dotnet run -- --name "ProjectName" --type ProjectType
```

### Project Types

- **DotNet**: C# backend API
- **Angular**: Frontend web app
- **FullStack**: Both API and web
- **Mobile**: iOS/Android cross-platform
- **AI**: Machine learning project

### Example

```bash
dotnet run -- --name "MyDataService" --type "DotNet"
```

### What Happens Automatically

```
1. ✅ Create Azure DevOps project
2. ✅ Clone repository locally
3. ✅ Create folder structure
4. ✅ Generate configuration files
5. ✅ Install Git hooks
6. ✅ Create CODEOWNERS guide
7. ✅ Create team configuration
8. ✅ Commit and push
```

---

## Git Workflow (Git Flow)

### Branch Strategy

```
main (Production)
  ├─ 2 approvals required
  ├─ Status checks required
  └─ For: releases, hotfixes

develop (Staging)
  ├─ 1 approval required
  ├─ Status checks required
  └─ For: feature integration

feature/TASK-###-name (Development)
  ├─ Created from: develop
  ├─ Approval: via PR
  └─ Merged via: Squash merge
```

### Creating a Feature

**Step 1: Create branch**
```bash
git checkout develop
git pull origin develop
git checkout -b feature/TASK-123-description
```

**Step 2: Make changes**
```bash
# ... edit files ...
git add .
git commit -m "feat(scope): description"
# ✅ Git hook validates automatically
```

**Step 3: Push and create PR**
```bash
git push -u origin feature/TASK-123-description
# Go to Azure DevOps → Pull Requests → New PR
```

**Step 4: Request review**
- Azure DevOps auto-assigns reviewers based on policies
- Reviewers will see the PR template
- Wait for approvals

**Step 5: Merge**
```bash
# Once approved (2 for main, 1 for develop):
# Click "Complete" in Azure DevOps UI
# Use "Squash commit" for develop
# Use "Merge commit" for main
```

### Fixing a Bug

Same as feature, but use:
```bash
git checkout -b bugfix/TASK-456-bug-name
git commit -m "fix(scope): description"
```

### Hotfix for Production

```bash
git checkout main
git pull origin main
git checkout -b hotfix/TASK-789-name
git commit -m "fix(scope): critical fix"
git push -u origin hotfix/TASK-789-name
# Create PR to main with 2 approvals
# Also merge to develop
```

---

## Commit Message Format

### Required Format

```
<type>(<scope>): <description>
```

### Valid Types

| Type | When to Use |
|------|-------------|
| **feat** | New feature |
| **fix** | Bug fix |
| **refactor** | Code improvement (no new feature) |
| **perf** | Performance improvement |
| **docs** | Documentation only |
| **test** | Add/update tests |
| **chore** | Maintenance tasks |
| **ci** | CI/CD pipeline changes |

### Valid Scopes

| Scope | What It Covers |
|-------|----------------|
| **console** | Console app |
| **generator** | ProjectTemplateGenerator |
| **api** | API endpoints |
| **git** | Git operations |
| **files** | File generation |
| **docs** | Documentation |

### Examples

✅ **Valid Commits:**
```
feat(generator): add custom path support
fix(console): handle null reference exception
refactor(api): simplify request processing
docs(readme): update setup instructions
test(generator): add unit tests
perf(console): optimize file operations
```

❌ **Invalid Commits:**
```
add feature              # Missing type and scope
Fixed bug               # Type lowercase, missing scope
Updated code            # Too vague
```

### Git Hook Behavior

**When you commit:**
```bash
git commit -m "feat(console): new feature"
```

**Valid:**
- Message matches format ✓
- Length is 10-100 chars ✓
- Type is valid ✓
- **Result:** Commit accepted ✅

**Invalid:**
```bash
git commit -m "add feature"
```
- Missing type and scope
- **Result:** Commit rejected ❌

---

## Code Review Process (Azure DevOps)

### For Developers (Creating PR)

**Step 1: Fill out PR description**

When you create a PR, add:
- Description of changes
- Type of change (feat, fix, refactor, etc.)
- Testing performed
- Related work items

**Step 2: Link work item**

- Go to: Pull Request → Link work item
- Select task/bug from Azure Boards

**Step 3: Auto-assign reviewers**

- Azure DevOps auto-assigns based on code review policies
- No manual reviewer selection needed

**Step 4: Address feedback**

- Read reviewer comments
- Make requested changes
- Push new commits
- Re-request review if needed

**Step 5: Merge when approved**

- Wait for required approvals (2 for main, 1 for develop)
- Click "Complete" button
- Choose merge type:
  - **Squash commit** for develop
  - **Merge commit** for main

### For Reviewers

**When you receive a review request:**

1. **Review the code**
   - Check logic correctness
   - Check edge cases
   - Check error handling
   - Check code quality
   - Check performance
   - Check security

2. **Leave comments**
   - **BLOCKING**: Issue that must be fixed
   - **SUGGESTION**: Recommended improvement
   - **QUESTION**: Clarification needed
   - **PRAISE**: Good practice example

3. **Approve or request changes**
   - ✅ Approve - Code is good
   - ❌ Request Changes - Need fixes before merge

### Review Checklist

```
✓ Code follows commit conventions
✓ Logic is correct
✓ Edge cases handled
✓ Error handling appropriate
✓ No obvious bugs
✓ Code quality good
✓ Performance OK
✓ Security OK
✓ Tests added/updated
✓ Documentation updated
```

---

## Team Roles (Azure DevOps)

### Admins

**Responsibilities:**
- Create Azure DevOps projects
- Create and manage groups
- Configure branch policies
- Manage team access
- Handle emergency changes
- Approve releases to main

**Required Access:**
- Project admin access
- Organization admin

### Team Leads

**Responsibilities:**
- Approve PRs to main (2 required)
- Approve PRs to develop (1 can approve)
- Review critical code
- Merge approved PRs
- Enforce team standards

**Approval Authority:**
- **main**: Can approve (2 required)
- **develop**: Can approve (1 needed)

### Senior Developers

**Responsibilities:**
- Review complex code
- Validate architectural decisions
- Check performance
- Mentor team members

**Auto-assigned to PRs:**
- `SmartWorkz.Tools.DevOpsProject/`
- `ProjectTemplateGenerator.cs`

### Code Quality Team

**Responsibilities:**
- Review configuration files
- Check project structure
- Validate CI/CD setup
- Review linting config

**Auto-assigned to PRs:**
- `.editorconfig`
- `.eslintrc.json`
- `.prettierrc.json`
- `azure-pipelines.yml`

### Security Team

**Responsibilities:**
- Review authentication code
- Check token handling
- Validate input validation
- Check for hardcoded secrets

**Auto-assigned to PRs:**
- `**/auth/**`
- `**/token/**`
- `**/secret/**`

---

## Branch Policies (Azure DevOps)

### Configure in Azure DevOps UI

Navigate to: **Repos → Branches → Select branch → Branch policies**

### main Branch (Production)

**Required Settings:**
- ✅ Require minimum reviewers: **2**
- ✅ Reset approval on changes: **Yes**
- ✅ Allow requesters to approve: **No**
- ✅ Require status checks: **Yes**
- ✅ Enforce work item linking: **Optional**
- ✅ Auto-complete: **No** (manual approval)

### develop Branch (Staging)

**Required Settings:**
- ✅ Require minimum reviewers: **1**
- ✅ Reset approval on changes: **Yes**
- ✅ Allow requesters to approve: **No**
- ✅ Require status checks: **Yes**
- ✅ Enforce work item linking: **Optional**
- ✅ Auto-complete: **Yes** (merge when ready)

### feature/* Branches

**No restrictions:**
- Direct commits allowed
- No approval required
- Force push allowed

---

## Auto Reviewer Assignment (Code Review Policies)

### How It Works

When you create a PR, Azure DevOps automatically assigns reviewers based on:
- Files changed
- Code review policies configured
- Defined path patterns

### Configure Policies

**Location:** Project Settings → Repositories → [Repo Name] → Policies

**For each code area, create a policy:**

```
Pattern: SmartWorkz.Tools.DevOpsProject/
Reviewers: senior-developers group
Min Reviewers: 1

Pattern: .editorconfig, .eslintrc.json
Reviewers: code-quality-team group
Min Reviewers: 1

Pattern: **/auth/**
Reviewers: security-team group
Min Reviewers: 1

Pattern: **
Reviewers: team-leads group (default)
Min Reviewers: 1 or 2 (for main)
```

### Example

**PR touches `ProjectTemplateGenerator.cs`:**
```
Files Changed:
  - SmartWorkz.Tools.DevOpsProject/ProjectTemplateGenerator.cs

Auto-assigned:
  - senior-developers group
  - team-leads group (fallback)
```

---

## Files Created Automatically

### Configuration Files (Root)

| File | Purpose |
|------|---------|
| `.gitignore` | Version control exclusions |
| `.editorconfig` | IDE formatting rules |
| `.eslintrc.json` | JavaScript/TypeScript linting |
| `.prettierrc.json` | Code formatting |
| `sonar-project.properties` | SonarQube configuration |
| `azure-pipelines.yml` | CI/CD pipeline |
| `README.md` | Project documentation |

### Azure DevOps Configuration Files

| File | Location | Purpose |
|------|----------|---------|
| `CODEOWNERS` | `.azuredevops/` | Code review policy guide |
| `PULL_REQUEST_TEMPLATE.md` | `.azuredevops/` | PR description template |
| `TEAM.yml` | `.azuredevops/` | Team definitions |
| `BRANCH-POLICIES.md` | `.azuredevops/` | Branch policy configuration guide |

### Total: 12 Files Auto-Created

---

## Troubleshooting

### Git Hooks Not Working

**Problem:** Commit validation not happening

**Cause:** Hooks are bash scripts; Windows needs Git Bash

**Solution:**
1. Install Git Bash (comes with Git for Windows)
2. Or use WSL (Windows Subsystem for Linux)
3. Verify with: `git config core.hooksPath`

### Reviewers Not Assigned

**Problem:** PRs don't auto-request reviewers

**Cause:** Code review policies not configured

**Solution:**
1. Create Azure DevOps groups first
2. Add members to groups
3. Configure code review policies
4. Test with new PR

### Can't Merge PR

**Problem:** Merge button disabled

**Causes & Solutions:**
1. **Need approvals?** → Get required number of approvals
2. **Status checks failing?** → Fix failures and push
3. **Comments not resolved?** → Resolve open comments
4. **Not up to date?** → Click "Update branch"
5. **Work item not linked?** → Link to Azure Boards item

### Commit Hook Rejects Valid Message

**Problem:** Git hook rejects message that looks correct

**Cause:** Format issue (spacing, colon, scope)

**Solution:**
- Check exact format: `type(scope): description`
- Must have colon and space after scope
- No extra spaces
- Example: `feat(generator): add feature` ✅

---

## Quick Reference

### Common Commands

```bash
# Create feature branch
git checkout develop
git pull origin develop
git checkout -b feature/TASK-123-name

# Make changes
git add .
git commit -m "feat(scope): description"
git push -u origin feature/TASK-123-name

# Create PR in Azure DevOps UI
# Wait for approvals
# Merge via Azure DevOps

# Hotfix
git checkout main
git pull origin main
git checkout -b hotfix/TASK-789-name
git commit -m "fix(scope): critical"
```

### Team Members File

**Location:** `.azuredevops/TEAM.yml`

**Update with:**
```yaml
admins:
  - name: Your Name
    email: your.email@smartworkz.com
    aad_username: your.email@smartworkz.com
    role: Your Role
```

### Azure DevOps Navigation

- **Groups:** Project Settings → Security → Groups
- **Branch Policies:** Repos → Branches → Branch Policies
- **Code Review Policies:** Project Settings → Repositories → Policies
- **Pull Requests:** Repos → Pull Requests
- **Work Items:** Boards → Work Items

---

## Summary

### For Developers

1. Create project (one command)
2. Update `.azuredevops/TEAM.yml`
3. Create feature branch
4. Make changes (git hook validates)
5. Push and create PR
6. Reviewers auto-assigned
7. Address feedback
8. Merge when approved

### For Admins

1. Create Azure DevOps groups
2. Add team members
3. Configure branch policies
4. Configure code review policies
5. Test with PR

### For Reviewers

1. Receive auto-assignment
2. Review code against checklist
3. Approve or request changes
4. Code merged when approved

---

## Support

**Questions?**
- Check the sections above
- Review `.azuredevops/TEAM.yml`
- Check `.azuredevops/CODEOWNERS` for assignment rules
- Read `.azuredevops/PULL_REQUEST_TEMPLATE.md`

**Issues?**
- Git hooks not working → Check Troubleshooting
- Reviewers not assigned → Verify groups and policies exist
- Can't merge PR → Check branch policies in Azure DevOps

---

**Version:** 2.0.0 - Azure DevOps  
**Last Updated:** 2026-04-28  
**For:** All Team Members
