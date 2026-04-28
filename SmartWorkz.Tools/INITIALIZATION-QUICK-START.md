# SmartWorkz.Tools - Initialization Quick Start

**Everything you need to create a new project with complete automation setup**

---

## The One-Command Solution

```bash
cd SmartWorkz.Tools\SmartWorkz.Tools.Console
dotnet run -- --name "MyProject" --type "DotNet"
```

**That's it.** The ProjectTemplateGenerator does everything:

✅ Creates Azure DevOps project  
✅ Clones repository  
✅ Creates folder structure  
✅ Generates config files  
✅ **Installs Git hooks** (new!)  
✅ **Creates CODEOWNERS** (new!)  
✅ **Creates PR template** (new!)  
✅ **Creates team config** (new!)  
✅ Commits and pushes  

---

## What Gets Created

### Automatic Files

| File | Purpose | Location |
|------|---------|----------|
| **prepare-commit-msg** | Commit format reminder | `.git/hooks/` |
| **commit-msg** | Commit validation | `.git/hooks/` |
| **CODEOWNERS** | Auto-assign reviewers | `.github/` |
| **PULL_REQUEST_TEMPLATE.md** | Standardized PRs | `.github/` |
| **TEAM.yml** | Team definitions | `.github/` |
| **.gitignore** | Version control exclusions | Root |
| **.editorconfig** | Formatting rules | Root |
| **.eslintrc.json** | Linting config | Root |
| **.prettierrc.json** | Code formatting | Root |
| **sonar-project.properties** | SonarQube config | Root |
| **azure-pipelines.yml** | CI/CD pipeline | Root |
| **README.md** | Project documentation | Root |

---

## After Project Creation

### Step 1: Update Team Members (5 minutes)

```bash
# Open the team config file
cd S:\SmartWorkz101\MyProject
code .github\TEAM.yml
```

Update with your actual team members:
```yaml
admins:
  - name: "Your Name"
    email: "your.email@smartworkz.com"
    github: "your-github-username"
    role: "Lead Developer"
```

### Step 2: Create GitHub Teams (10 minutes) - ADMIN ONLY

Go to: https://github.com/orgs/smartworkz/teams

Create these teams:
- ✅ admins
- ✅ team-leads
- ✅ senior-developers
- ✅ code-quality-team
- ✅ security-team

Add your team members to each team.

### Step 3: Setup GitHub Branch Protection (5 minutes) - ADMIN ONLY

Go to: https://github.com/smartworkz/MyProject/settings/branches

For **main** branch:
- ✅ Require pull request reviews
- ✅ Require 2 approvals
- ✅ Require status checks
- ✅ Require branches to be up to date
- ✅ Auto-delete head branches

For **develop** branch:
- ✅ Require pull request reviews
- ✅ Require 1 approval
- ✅ Require status checks
- ✅ Auto-delete head branches

### Step 4: Verify Everything Works (5 minutes)

**Test Git hooks:**
```bash
cd S:\SmartWorkz101\MyProject

# Create test branch
git checkout -b feature/TEST-001

# Try valid commit (should succeed)
git commit -m "feat(console): test commit" --allow-empty

# Try invalid commit (should fail)
git commit -m "invalid" --allow-empty
# ❌ Should show: "Invalid commit message format!"
```

**Test CODEOWNERS:**
```bash
# Create test PR on GitHub
# Verify correct teams are auto-requested
```

---

## Commit Message Format

### Valid Examples

```
feat(generator): add custom path support
fix(console): handle null reference exception
refactor(api): simplify request processing
docs(readme): update setup instructions
test(hooks): verify commit validation
chore(deps): update dependencies
perf(generator): optimize file operations
ci(pipeline): add code quality stage
```

### Invalid Examples ❌

```
add feature
Fixed bug
Updated code
commit without type/scope
This is a message that is way too long and exceeds the maximum character limit of one hundred
```

### Format Rules

**Required:** `type(scope): description`

**Types:** feat, fix, refactor, perf, docs, test, chore, ci

**Scopes:** console, generator, api, git, files, docs

**Length:** 10-100 characters

---

## Team Roles

### Admins
- Set up and manage GitHub
- Create branch protections
- Manage team access
- Handle emergency changes

### Team Leads
- Approve PRs to main (2 required)
- Approve PRs to develop (1 can approve)
- Review critical code
- Merge PRs

### Senior Developers
- Review complex code
- Validate architecture
- Check performance
- Verify security

### Code Quality Team
- Review configuration files
- Check project structure
- Validate CI/CD setup
- Review eslint, prettier configs

### Security Team
- Review authentication code
- Check token handling
- Validate input validation
- Review security settings

---

## Documentation Files

After project creation, read these (in order):

1. **COLLABORATION.md** - Git workflow and code review process
2. **.github/SETUP.md** - Step-by-step GitHub setup for admins
3. **.github/TEAM.yml** - Your team definitions
4. **CLAUDE.md** - Development guidelines
5. **README.md** - Project documentation

---

## Troubleshooting

### Git hooks not working?

**Cause:** Hooks are bash scripts, Windows needs Git Bash

**Fix:**
1. Install Git Bash (comes with Git)
2. Or use WSL (Windows Subsystem for Linux)

Verify with:
```bash
git config core.hooksPath
# Should show: .git/hooks
```

### CODEOWNERS not assigning reviewers?

**Cause:** Teams don't exist on GitHub yet

**Fix:**
1. Create teams at https://github.com/orgs/smartworkz/teams
2. Use exact names (no spaces, kebab-case)
3. Add team members
4. Test with new PR

### PR template not showing?

**Cause:** File in wrong location or named incorrectly

**Fix:**
1. Verify file: `.github/PULL_REQUEST_TEMPLATE.md`
2. Commit and push
3. Refresh browser cache
4. Create new PR

---

## Development Workflow

### Create a Feature

```bash
# 1. Update develop branch
git checkout develop
git pull origin develop

# 2. Create feature branch
git checkout -b feature/TASK-123-feature-name

# 3. Make changes
# ... edit files ...

# 4. Commit (git hook validates)
git add .
git commit -m "feat(scope): description"
# ✅ Git hook validates automatically

# 5. Push
git push -u origin feature/TASK-123-feature-name

# 6. Create PR on GitHub
# (PR template appears automatically)
# (CODEOWNERS auto-assigns reviewers)

# 7. Address feedback
# (make more commits as needed)

# 8. Merge when approved
# (squash-merge to develop)
```

### Fix a Bug

Same as feature, but use:
```bash
git checkout -b bugfix/TASK-456-bug-name
git commit -m "fix(scope): description"
```

---

## Key Commands Reference

```bash
# Verify setup
git config core.hooksPath          # Should show .git/hooks

# Test commit validation
git commit -m "feat(test): test"   # ✅ Valid
git commit -m "invalid"             # ❌ Invalid

# Create feature branch
git checkout -b feature/TASK-001-name

# View team config
cat .github/TEAM.yml

# View CODEOWNERS rules
cat .github/CODEOWNERS

# View PR template
cat .github/PULL_REQUEST_TEMPLATE.md
```

---

## Success Checklist

After following this guide:

- [ ] Project created with all files
- [ ] .github/TEAM.yml updated with team members
- [ ] GitHub teams created (admins, team-leads, etc.)
- [ ] GitHub teams populated with members
- [ ] Branch protection rules set (main: 2, develop: 1)
- [ ] Git hooks verified working
- [ ] Valid commit succeeded ✅
- [ ] Invalid commit rejected ❌
- [ ] CODEOWNERS verified (test PR)
- [ ] PR template verified (test PR)

---

## Time Estimates

| Step | Time | Who |
|------|------|-----|
| **Create project** | 1-2 min | Developer |
| **Update TEAM.yml** | 5 min | Developer |
| **Create GitHub teams** | 10 min | Admin |
| **Add team members** | 5 min | Admin |
| **Setup branch protection** | 10 min | Admin |
| **Verify everything** | 5-10 min | Team |
| **TOTAL** | 40-50 min | - |

---

## Next Steps

After initialization:

1. Start developing with confidence
2. Follow commit format (hooks enforce it)
3. Create PRs for code review
4. Auto-assigned reviewers will review
5. Merge when approvals received

**Everything is automated. No manual review assignment needed.**

---

## Support

**Questions about:**
- **Git hooks:** Check troubleshooting above or INTEGRATED-INITIALIZATION.md
- **Team setup:** Review .github/TEAM.yml
- **Branch protection:** See .github/SETUP.md
- **Commit format:** Review COLLABORATION.md
- **Development:** Read CLAUDE.md

---

**Version**: 2.0.0 - Integrated Initialization  
**Last Updated**: 2026-04-28
