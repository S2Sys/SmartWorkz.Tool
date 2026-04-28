# SmartWorkz.Tools - Complete Team Guide

**Everything you need to know - from project creation to code review**

---

## Table of Contents

1. [Quick Start (5 minutes)](#quick-start)
2. [Creating a New Project](#creating-a-new-project)
3. [Git Workflow](#git-workflow)
4. [Commit Message Format](#commit-message-format)
5. [Code Review Process](#code-review-process)
6. [Team Roles](#team-roles)
7. [Branch Protection Rules](#branch-protection-rules)
8. [Files Created](#files-created)
9. [Troubleshooting](#troubleshooting)

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
- ✅ CODEOWNERS configured (auto reviewer assignment)
- ✅ PR template ready
- ✅ Team roles defined

**Step 3: Update team members**
```bash
cd S:\SmartWorkz101\MyProject
code .github/TEAM.yml
# Edit with your actual team members
```

### For Admins (20 minutes)

After developers create the project:

1. **Create GitHub teams** at https://github.com/orgs/smartworkz/teams
   - admins
   - team-leads
   - senior-developers
   - code-quality-team
   - security-team

2. **Add team members** to each team

3. **Set branch protection** at https://github.com/smartworkz/MyProject/settings/branches
   - **main**: 2 approvals required, status checks required
   - **develop**: 1 approval required, status checks required

4. **Verify** with a test PR

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
6. ✅ Create CODEOWNERS
7. ✅ Create PR template
8. ✅ Create team config
9. ✅ Commit and push
```

**Time Required:** ~10 seconds

---

## Git Workflow

### Branch Strategy (Git Flow)

```
main (Production)
  ├─ Branch protection: 2 approvals required
  ├─ Status checks: Required
  └─ Force push: Disabled

develop (Staging)
  ├─ Branch protection: 1 approval required
  ├─ Status checks: Required
  └─ Force push: Disabled

feature/TASK-###-name
  ├─ Created from: develop
  ├─ Approved by: Reviewers
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
# Go to GitHub → Create Pull Request
```

**Step 4: Request review**
- GitHub auto-assigns reviewers based on CODEOWNERS
- Reviewers will see the PR template
- Wait for approvals

**Step 5: Merge**
```bash
# Once approved, merge via GitHub UI
# Use "Squash and merge" for develop
# Use "Create merge commit" for main
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
chore(deps): update dependencies
ci(pipeline): add code quality stage
```

❌ **Invalid Commits:**
```
add feature              # Missing type and scope
Fixed bug               # Type lowercase, missing scope
Updated code            # Too vague
commit without format   # Wrong format
This is a commit that is way too long and exceeds the maximum allowed character limit
```

### Rules

- **Format:** `type(scope): description` (required)
- **Length:** 10-100 characters
- **Imperative mood:** "add" not "added"
- **No period at end**
- **One logical change per commit**

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
```
❌ Invalid commit message format!

Required format: type(scope): description

Types: feat, fix, refactor, perf, docs, test, chore, ci
Scopes: console, generator, api, git, files, docs

Example: feat(generator): add custom path support
```

---

## Code Review Process

### For Developers (Creating PR)

**Step 1: Fill out PR template**

When you create a PR, this template appears:

```markdown
## Description
Briefly describe the changes.

## Type of Change
- [ ] Feature (new functionality)
- [ ] Bug Fix (addressing an issue)
- [ ] Refactor (code improvement)
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
```

**Step 2: Complete the checklist**
- Check all boxes
- Answer all questions
- Link related issues
- Describe testing

**Step 3: Request review**
- GitHub auto-assigns reviewers (from CODEOWNERS)
- No manual reviewer selection needed
- Reviewers will see PR template

**Step 4: Address feedback**
- Read reviewer comments
- Make requested changes
- Push new commits
- Re-request review if needed

**Step 5: Wait for approval**
- **develop**: 1 approval required
- **main**: 2 approvals required

### For Reviewers

**When you receive a review request:**

1. **Read PR description** - Understand what changed and why
2. **Check commit messages** - Are they in correct format?
3. **Review the code** - Check:
   - ✓ Logic correctness
   - ✓ Edge cases handled
   - ✓ Error handling appropriate
   - ✓ Code quality good
   - ✓ Performance impact
   - ✓ Security implications
   - ✓ Tests added if needed

4. **Leave comments**
   - **MUST FIX** - Blocking issue
   - **SHOULD FIX** - Strongly recommended
   - **CONSIDER** - Nice to have
   - **NITPICK** - Very minor

5. **Approve or Request Changes**
   - ✅ Approve - Code is good
   - ❌ Request Changes - Need fixes
   - 💬 Comment - Just feedback

### Review Checklist

```
□ Code follows commit conventions
□ Logic is correct
□ Edge cases handled
□ Error handling appropriate
□ No obvious bugs
□ Code quality good
□ Performance OK
□ Security OK
□ Tests added/updated
□ Documentation updated
□ Backward compatible
```

---

## Team Roles

### Admins (Project Lead, DevOps Engineer)

**Responsibilities:**
- ✅ Create Azure DevOps projects
- ✅ Set up GitHub teams
- ✅ Configure branch protection
- ✅ Manage team access
- ✅ Handle emergency changes
- ✅ Create service connections
- ✅ Approve releases to main

**Required Access:**
- Admin access to GitHub repository
- Azure DevOps organization admin
- GitHub organization admin

### Team Leads (Senior Developers)

**Responsibilities:**
- ✅ Approve PRs to main (2 required)
- ✅ Approve PRs to develop (1 can approve)
- ✅ Review critical code paths
- ✅ Merge approved PRs
- ✅ Monitor code quality
- ✅ Enforce team standards

**PR Approval Authority:**
- **main**: Can approve (2 required)
- **develop**: Can approve (1 needed)

### Senior Developers

**Responsibilities:**
- ✅ Review complex code
- ✅ Validate architectural decisions
- ✅ Check performance implications
- ✅ Mentor junior developers
- ✅ Set code standards

**Auto-assigned to PRs:**
- Files in `SmartWorkz.Tools.DevOpsProject/`
- `ProjectTemplateGenerator.cs`
- Any complex code

### Code Quality Team

**Responsibilities:**
- ✅ Review configuration files
- ✅ Check project structure
- ✅ Validate CI/CD setup
- ✅ Review linting and formatting
- ✅ Ensure consistency

**Auto-assigned to PRs:**
- `.editorconfig`
- `.eslintrc.json`
- `.prettierrc.json`
- `azure-pipelines.yml`
- `sonar-project.properties`

### Security Team

**Responsibilities:**
- ✅ Review authentication code
- ✅ Check token handling
- ✅ Validate input validation
- ✅ Check for hardcoded secrets
- ✅ Security best practices

**Auto-assigned to PRs:**
- Files containing: `token`, `auth`, `secret`, `credential`
- Any security-related changes

---

## Branch Protection Rules

### main Branch (Production)

**Requirements:**
- ✅ Pull Request required
- ✅ 2 approvals required (both team-leads)
- ✅ Status checks required
- ✅ Conversation resolution required
- ✅ Branches must be up to date
- ✅ Auto-delete head branches

**What's Blocked:**
- ❌ Direct commits (PR required)
- ❌ Force push
- ❌ Merge without 2 approvals
- ❌ Merge with failing status checks

### develop Branch (Staging)

**Requirements:**
- ✅ Pull Request required
- ✅ 1 approval required
- ✅ Status checks required
- ✅ Branches must be up to date
- ✅ Auto-delete head branches

**What's Blocked:**
- ❌ Direct commits (PR required)
- ❌ Force push
- ❌ Merge without 1 approval
- ❌ Merge with failing status checks

### feature/* Branches (Development)

**No restrictions:**
- Developers can force push
- No approval required
- Can be deleted after merge

---

## Files Created

### Configuration Files (Automatically Generated)

| File | Purpose |
|------|---------|
| `.gitignore` | Version control exclusions |
| `.editorconfig` | IDE formatting rules |
| `.eslintrc.json` | TypeScript/JavaScript linting |
| `.prettierrc.json` | Code formatting standards |
| `sonar-project.properties` | SonarQube configuration |
| `azure-pipelines.yml` | CI/CD pipeline (3-stage) |
| `README.md` | Project documentation |

### Automation Files (Team Collaboration)

| File | Location | Purpose |
|------|----------|---------|
| `prepare-commit-msg` | `.git/hooks/` | Shows commit format reminder |
| `commit-msg` | `.git/hooks/` | Validates commit format |
| `CODEOWNERS` | `.github/` | Auto-assigns reviewers |
| `PULL_REQUEST_TEMPLATE.md` | `.github/` | Standardized PR form |
| `TEAM.yml` | `.github/` | Team definitions |

### Total: 12 Files Auto-Created

---

## Automatic Reviewer Assignment (CODEOWNERS)

### How It Works

When you create a PR, GitHub automatically requests review from teams based on files changed:

### Assignment Rules

| Files Changed | Auto-assigned Team |
|---------------|-------------------|
| `SmartWorkz.Tools.DevOpsProject/*` | `@smartworkz/senior-developers` |
| `ProjectTemplateGenerator.cs` | `@smartworkz/senior-developers` |
| `.editorconfig`, `.eslintrc.json`, `.prettierrc.json` | `@smartworkz/code-quality-team` |
| `azure-pipelines.yml`, `sonar-project.properties` | `@smartworkz/code-quality-team` |
| `**/token*`, `**/auth*`, `**/secret*` | `@smartworkz/security-team` |
| `*.md` files | `@smartworkz/team-leads` |
| Everything (default) | `@smartworkz/team-leads` |

### Example

**PR touches `ProjectTemplateGenerator.cs`:**
```
Files Changed:
  - SmartWorkz.Tools.DevOpsProject/ProjectTemplateGenerator.cs

Auto-assigned:
  - @smartworkz/senior-developers
  - @smartworkz/team-leads (default)
```

---

## Troubleshooting

### Git Hooks Not Working

**Problem:** Commit validation not happening

**Cause:** Hooks are bash scripts; Windows needs Git Bash

**Solution:**
1. Install Git Bash (comes with Git for Windows)
2. Or use WSL (Windows Subsystem for Linux)
3. Verify with: `git config core.hooksPath`

### CODEOWNERS Not Assigning Reviewers

**Problem:** PRs don't auto-request team reviews

**Cause:** GitHub teams don't exist yet

**Solution:**
1. Go to: https://github.com/orgs/smartworkz/teams
2. Create teams: admins, team-leads, senior-developers, code-quality-team, security-team
3. Add team members
4. Test with a new PR

### PR Template Not Showing

**Problem:** Template doesn't appear when creating PR

**Cause:** File location or naming incorrect

**Solution:**
1. Verify file exists: `.github/PULL_REQUEST_TEMPLATE.md`
2. Verify exact file name (case-sensitive)
3. Clear browser cache
4. Try creating new PR

### Commit Hook Rejects Valid Message

**Problem:** Git hook rejects message that looks correct

**Cause:** Format issue (spacing, colon, scope)

**Solution:**
- Check exact format: `type(scope): description`
- Must have colon and space after scope
- No extra spaces
- Example: `feat(generator): add feature` ✅

### Cannot Merge PR

**Problem:** Merge button is disabled

**Cause:** Branch protection rule blocking merge

**Solutions:**
1. **Need approvals?** - Get required number of approvals
2. **Status checks failing?** - Fix failures and push new commit
3. **Conversation not resolved?** - Resolve open conversations
4. **Not up to date?** - Click "Update branch"

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

# Create PR on GitHub
# Wait for approvals
# Merge via GitHub UI

# Create bug fix
git checkout -b bugfix/TASK-456-name
git commit -m "fix(scope): description"

# Hotfix for production
git checkout main
git pull origin main
git checkout -b hotfix/TASK-789-name
git commit -m "fix(scope): critical"
```

### Commit Format Quick Check

```
✅ feat(console): add new feature
✅ fix(api): handle null exception
✅ refactor(generator): simplify logic
✅ docs(readme): update instructions

❌ add feature (missing type/scope)
❌ Fixed bug (needs lowercase and scope)
❌ This is too long and exceeds the maximum character limit
```

### Team Members File

**Location:** `.github/TEAM.yml`

**Update with:**
```yaml
team_leads:
  - name: "Your Name"
    email: "your.email@smartworkz.com"
    github: "your-github-username"
    role: "Your Role"
```

---

## Summary

### For New Developers

1. ✅ Create project (run one command)
2. ✅ Update `.github/TEAM.yml` with team
3. ✅ Create feature branch
4. ✅ Make changes
5. ✅ Commit with proper format (git hook validates)
6. ✅ Push and create PR
7. ✅ Reviewers auto-assigned
8. ✅ Address feedback
9. ✅ Merge when approved

### For Admins

1. ✅ Create GitHub teams
2. ✅ Add team members
3. ✅ Set branch protection rules
4. ✅ Verify with test PR

### For Reviewers

1. ✅ Receive auto-assignment via CODEOWNERS
2. ✅ Review code against checklist
3. ✅ Approve or request changes
4. ✅ Merged automatically when approved

---

## Support

**Questions?**
- Check "Troubleshooting" section above
- Review `.github/TEAM.yml` in your project
- Check `.github/CODEOWNERS` for assignment rules
- Read `.github/PULL_REQUEST_TEMPLATE.md` format

**Issues?**
- Git hooks not working → Check Troubleshooting
- Reviewers not assigned → Verify GitHub teams exist
- Can't merge PR → Check branch protection rules

---

**Version:** 2.0.0  
**Last Updated:** 2026-04-28  
**For:** All Team Members
