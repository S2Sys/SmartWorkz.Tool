# Azure DevOps Setup Guide for Branching Strategy

**Version**: 1.0.0  
**Last Updated**: 2026-04-29  
**Estimated Setup Time**: 30-45 minutes

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Initial Setup Checklist](#initial-setup-checklist)
3. [Step 1: Create Groups](#step-1-create-groups)
4. [Step 2: Configure main Branch](#step-2-configure-main-branch)
5. [Step 3: Configure develop Branch](#step-3-configure-develop-branch)
6. [Step 4: Configure Code Review Policies](#step-4-configure-code-review-policies)
7. [Step 5: Configure Build Pipelines](#step-5-configure-build-pipelines)
8. [Step 6: Team Configuration](#step-6-team-configuration)
9. [Verification Checklist](#verification-checklist)
10. [Troubleshooting](#troubleshooting)

---

## Prerequisites

**Required Permissions:**
- ✅ Project admin access (to configure settings)
- ✅ Repository admin access (to configure branch policies)
- ✅ Build definition admin (to configure pipelines)

**Required Knowledge:**
- Git and branching concepts
- Azure DevOps navigation
- YAML pipeline basics (optional but helpful)

**Team Members Ready:**
- List of team members with Azure AD emails
- Team structure defined (leads, developers, QA)

---

## Initial Setup Checklist

Before you start, verify you have:

- [ ] Azure DevOps organization created
- [ ] Project created (if not using SmartWorkz.Tools)
- [ ] Repository initialized with `main` and `develop` branches
- [ ] Project admin access
- [ ] Team member list with Azure AD emails
- [ ] CI/CD pipeline configured (azure-pipelines.yml)

---

## Step 1: Create Groups

Groups allow you to assign reviewers and permissions in bulk.

### Navigate to Groups

1. Go to your Azure DevOps project
2. Click **Project Settings** (bottom left)
3. Click **Security** → **Groups**

### Create Default Groups

Create these 5 groups:

#### Group 1: Project Administrators

1. Click **New group**
2. **Group name:** `Project Administrators`
3. **Description:** `Project admins with full access`
4. Click **Create**
5. Add members: Senior leads, project managers
6. Go to **Permissions** tab:
   - ✅ Edit project-level information: **Allow**
   - ✅ Create repositories: **Allow**
   - ✅ Delete repositories: **Allow**
   - ✅ Manage branch policies: **Allow**

#### Group 2: Team Leads

1. Click **New group**
2. **Group name:** `Team Leads`
3. **Description:** `Team leads and senior developers`
4. Add members: Team leads
5. Permissions:
   - ✅ Contribute to pull requests: **Allow**
   - ✅ Approve pull requests: **Allow**
   - ✅ Override policy: **Allow**

#### Group 3: Senior Developers

1. Click **New group**
2. **Group name:** `Senior Developers`
3. **Description:** `Experienced developers for architecture review`
4. Add members: Senior developers
5. Permissions:
   - ✅ Contribute to pull requests: **Allow**
   - ✅ Approve pull requests: **Allow**

#### Group 4: Code Quality Team

1. Click **New group**
2. **Group name:** `Code Quality Team`
3. **Description:** `Code quality, linting, and config reviews`
4. Add members: DevOps engineers, code quality leads
5. Same permissions as Senior Developers

#### Group 5: Security Team

1. Click **New group**
2. **Group name:** `Security Team`
3. **Description:** `Security reviews for auth and sensitive code`
4. Add members: Security engineers, leads
5. Same permissions as Senior Developers

### Result

You now have 5 groups that can be used for reviewer assignments and permissions.

---

## Step 2: Configure main Branch

The `main` branch is production-ready code. It requires strict protection.

### Navigate to Branch Policies

1. Go to your project
2. Click **Repos** → **Branches**
3. Find **main** branch
4. Click **...** → **Branch policies**

### Configure Protection Rules

#### Minimum Reviewers

1. Toggle **Require a minimum number of reviewers** → **ON**
2. **Minimum reviewers:** `2`
3. **When new changes are pushed:** Check **Reset approval**
4. **Allow requesters to approve their own changes:** Uncheck
5. **Require all reviewers to sign off:** Uncheck
6. **Block last pusher's vote:** Check

#### Build Verification

1. Toggle **Build validation** → **ON**
2. **Build pipeline:** Select your CI pipeline
3. **Build expiration:** `1 day` (policies expire after 1 day)
4. **Display name:** `Continuous Integration`

#### Status Checks (Optional)

If you have external status checks (SonarQube, etc.):

1. Toggle **Require status checks to pass** → **ON**
2. Add status checks from your pipeline

#### Work Item Linking

1. Toggle **Require a valid work item to link** → **ON**
2. **Message:** "Please link a work item to track this change"

#### Comment Resolution

1. Toggle **Require all comments addressed** → **ON**
2. Ensures all review comments are resolved before merge

#### Merge Type

1. Toggle **Enforce a merge strategy** → **ON**
2. **Merge type:** `Create a merge commit`
3. Message: "Production releases require full history"

#### Security

1. **Bypass branch policies:** Select **Project Administrators**
2. **Limit approval scope:** Check (only approve own changes initially)

### Final Configuration

When complete, your main branch should show:

```
✅ Require a minimum number of reviewers (2)
✅ Reset approval on changes
✅ Build validation enabled
✅ Status checks required
✅ Require valid work item link
✅ Require all comments addressed
✅ Merge strategy: Create merge commit
✅ Bypass policy: Project Administrators only
```

---

## Step 3: Configure develop Branch

The `develop` branch is for integration. Less strict than main, but still protected.

### Navigate to Branch Policies

1. Go to **Repos** → **Branches**
2. Find **develop** branch
3. Click **...** → **Branch policies**

### Configure Protection Rules

#### Minimum Reviewers

1. Toggle **Require a minimum number of reviewers** → **ON**
2. **Minimum reviewers:** `1`
3. **When new changes are pushed:** Check **Reset approval**
4. **Allow requesters to approve:** Uncheck
5. **Block last pusher's vote:** Uncheck

#### Build Verification

1. Toggle **Build validation** → **ON**
2. **Build pipeline:** Same as main
3. **Build expiration:** `1 day`

#### Auto-Complete

1. Toggle **Auto-complete set by default** → **ON**
2. **Auto-complete pending merges:** Check
3. Message: "PR will auto-complete when approved and checks pass"

#### Comment Resolution

1. Toggle **Require all comments addressed** → **ON**

#### Merge Type

1. Toggle **Enforce a merge strategy** → **ON**
2. **Merge type:** `Squash commit`
3. Message: "Development uses squash merges for clean history"

#### Security

1. **Bypass branch policies:** Select **Project Administrators**

### Final Configuration

```
✅ Require a minimum number of reviewers (1)
✅ Reset approval on changes
✅ Build validation enabled
✅ Auto-complete enabled
✅ Require all comments addressed
✅ Merge strategy: Squash commit
✅ Bypass policy: Project Administrators only
```

---

## Step 4: Configure Code Review Policies

Code review policies automatically assign reviewers based on files changed.

### Navigate to Code Review Policies

1. Go to **Project Settings** (bottom left)
2. Click **Repositories**
3. Select your repository
4. Click **Policies**

### Create Review Policy 1: Team Leads (Default)

1. Click **Create policy**
2. **Minimum reviewers:** `1`
3. **Pattern:** `/` (root - applies to all files)
4. **Reviewers:** `Team Leads` group
5. **Allow downgrade:** Check
6. Click **Save**

### Create Review Policy 2: Senior Developers (Core Code)

1. Click **Create policy**
2. **Minimum reviewers:** `1`
3. **Pattern:** `SmartWorkz.Tools.DevOpsProject/**` (or your core project folder)
4. **Reviewers:** `Senior Developers` group
5. Click **Save**

### Create Review Policy 3: Code Quality Team (Config Files)

1. Click **Create policy**
2. **Minimum reviewers:** `1`
3. **Pattern:** `/azure-pipelines.yml`, `/.editorconfig`, `/.eslintrc.json`, `/.prettierrc.json`, `/sonar-project.properties`
4. **Reviewers:** `Code Quality Team` group
5. Click **Save**

### Create Review Policy 4: Security Team (Auth Code)

1. Click **Create policy**
2. **Minimum reviewers:** `1`
3. **Pattern:** `**/auth/**`, `**/token/**`, `**/secret/**`
4. **Reviewers:** `Security Team` group
5. Click **Save**

### Result

Now when developers create PRs:

- **Files in core project** → Auto-assign Senior Developers
- **Config files changed** → Auto-assign Code Quality Team
- **Auth files changed** → Auto-assign Security Team
- **Other files** → Auto-assign Team Leads (default)

---

## Step 5: Configure Build Pipelines

Build pipelines run tests and checks on every PR.

### Option A: Using azure-pipelines.yml (Recommended)

If you have `azure-pipelines.yml` in your repo:

1. Go to **Pipelines** → **Pipelines**
2. Click **New pipeline**
3. Select **Azure Repos Git**
4. Select your repository
5. Click **Existing Azure Pipelines YAML file**
6. **Branch:** `develop`
7. **Path:** `/azure-pipelines.yml`
8. Click **Continue**
9. Click **Save and run**

### Option B: Create a Basic Pipeline

1. Go to **Pipelines** → **Pipelines**
2. Click **New pipeline**
3. Select your repository
4. Choose **Starter pipeline**
5. Add these steps:

```yaml
trigger:
  - main
  - develop

pool:
  vmImage: 'windows-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.0.x'
  
  - task: DotNetCoreCLI@2
    inputs:
      command: 'build'
      arguments: '--configuration Release'
  
  - task: DotNetCoreCLI@2
    inputs:
      command: 'test'
      arguments: '--configuration Release'
```

6. Click **Save and run**

### Verify Pipeline

1. Make a test commit on a feature branch
2. Create a PR to develop
3. Verify pipeline runs automatically
4. PR shows ✅ or ❌ based on pipeline result

---

## Step 6: Team Configuration

Set up your team structure for communication and tracking.

### Create TEAM.yml File

In your repository root or `.azuredevops/` folder, create `TEAM.yml`:

```yaml
# SmartWorkz Development Team Configuration
# Version: 1.0.0

admins:
  - name: Admin Name
    email: admin@company.com
    aad_username: admin@company.com
    role: Project Admin

team_leads:
  - name: Lead Name 1
    email: lead1@company.com
    aad_username: lead1@company.com
    role: Team Lead / Senior Developer
    responsibility: Architecture review, approve PRs to main

  - name: Lead Name 2
    email: lead2@company.com
    aad_username: lead2@company.com
    role: Team Lead / Senior Developer
    responsibility: Feature review, approve PRs to develop

senior_developers:
  - name: Senior Dev 1
    email: seniordev1@company.com
    aad_username: seniordev1@company.com
    role: Senior Developer
    responsibility: Code quality, architecture

developers:
  - name: Developer 1
    email: dev1@company.com
    aad_username: dev1@company.com
    role: Developer
    responsibility: Feature development

  - name: Developer 2
    email: dev2@company.com
    aad_username: dev2@company.com
    role: Developer
    responsibility: Feature development

code_quality_team:
  - name: DevOps Engineer
    email: devops@company.com
    aad_username: devops@company.com
    role: DevOps / Code Quality
    responsibility: CI/CD, code quality standards

security_team:
  - name: Security Engineer
    email: security@company.com
    aad_username: security@company.com
    role: Security
    responsibility: Authentication, data protection

# Review requirements
review_requirements:
  main_branch: "2 approvals required (team leads)"
  develop_branch: "1 approval required (any senior)"
  feature_branches: "Follow code review policies"

# Communication
slack_channel: "#development"
standups: "Daily 10:00 AM EST"
sprint_planning: "First Monday of sprint, 2:00 PM EST"
retros: "Last Friday of sprint, 3:00 PM EST"

# Documentation
docs_location: "https://dev.azure.com/smartworkz/_git/projectname/docs"
onboarding_guide: ".azuredevops/ONBOARDING.md"
coding_standards: ".azuredevops/CODING-STANDARDS.md"
```

### Create CODEOWNERS File

In `.azuredevops/` folder, create `CODEOWNERS`:

```
# SmartWorkz Code Owners
# https://docs.microsoft.com/en-us/azure/devops/repos/git/codeowners

# All files - Team Leads by default
* @team-leads

# Core generator code - Senior Developers
SmartWorkz.Tools.DevOpsProject/ @senior-developers

# Configuration files - Code Quality Team
/.editorconfig @code-quality-team
/.eslintrc.json @code-quality-team
/azure-pipelines.yml @code-quality-team
/sonar-project.properties @code-quality-team

# Authentication code - Security Team
**/auth/** @security-team
**/token/** @security-team
**/secret/** @security-team

# Documentation - Anyone can edit
*.md @team-leads
```

---

## Verification Checklist

After completing all steps, verify everything is working:

### ✅ Groups Created

- [ ] Project Administrators group exists
- [ ] Team Leads group exists
- [ ] Senior Developers group exists
- [ ] Code Quality Team group exists
- [ ] Security Team group exists
- [ ] All groups have appropriate members

### ✅ Branch Policies Configured

- [ ] main branch requires 2 approvals
- [ ] main branch requires build validation
- [ ] develop branch requires 1 approval
- [ ] develop branch has auto-complete enabled
- [ ] All branches have comment resolution enabled

### ✅ Code Review Policies

- [ ] Team Leads policy created
- [ ] Senior Developers policy created
- [ ] Code Quality Team policy created
- [ ] Security Team policy created

### ✅ Build Pipeline

- [ ] Pipeline created and enabled
- [ ] Pipeline runs on PRs automatically
- [ ] Pipeline shows in branch policies as required check

### ✅ Test with Real PR

1. Create a test feature branch
2. Make a small change
3. Create PR to develop
4. Verify:
   - [ ] PR template loads
   - [ ] Reviewers auto-assigned
   - [ ] Build pipeline triggered
   - [ ] Status shows required checks

### ✅ Test Branch Protection

1. Try to push directly to develop
   - Should fail (push requires PR)
2. Try to merge without approval
   - Should fail (policy enforced)
3. Try to merge with 1 approval
   - Should succeed (policy satisfied)

---

## Troubleshooting

### Issue: Groups Not Showing in Policies

**Problem**: Can't find group when assigning reviewers

**Solution:**
1. Check group was created in Project Settings → Security → Groups
2. Wait 5 minutes for Azure DevOps to sync
3. Refresh the page
4. Try typing group name in the policy reviewers field

### Issue: Build Pipeline Not Triggering

**Problem**: PR created but pipeline doesn't run

**Solution:**
1. Check pipeline is enabled: **Pipelines** → Select pipeline → **More options** → Verify enabled
2. Check triggers: Edit pipeline → Triggers → Check `Pull request validation` is enabled
3. Check branch filter includes your branches
4. Check build definition status checks are configured in branch policies

### Issue: Auto-Assign Not Working

**Problem**: PR created but reviewers aren't auto-assigned

**Solution:**
1. Check code review policies are created: **Project Settings** → **Repositories** → **Policies**
2. Verify pattern matches your files
3. Check groups have members
4. Try closing and reopening PR
5. Manually assign reviewer to test it works

### Issue: Can't Merge PR Despite Approvals

**Problem**: Merge button is disabled even with approvals

**Reasons:**
1. **Status checks failing** → Fix failing tests
2. **Comments not resolved** → Resolve all comments
3. **Branch not up to date** → Click "Update branch"
4. **Work item not linked** → Link to Azure Boards item
5. **Build expired** → Re-run pipeline

### Issue: Getting Merge Conflict

**Problem**: Can't merge due to conflicts

**Solution:**
```bash
git fetch origin
git checkout feature/your-branch
git merge origin/develop
# Resolve conflicts in editor
git add .
git commit -m "merge: resolve conflicts"
git push
```

### Issue: Accidentally Pushed to main

**Problem**: Committed directly to main instead of creating PR

**Solution** (if not yet pushed):
```bash
git reset --soft HEAD~1
git checkout -b feature/TASK-123-fix
git commit -m "feat: description"
```

**Solution** (if already pushed):
Contact your project admin - may need to revert commit

---

## Quick Reference

### Create Feature

```bash
git checkout develop
git pull origin develop
git checkout -b feature/TASK-123-name
# Make changes
git commit -m "feat(scope): description"
git push -u origin feature/TASK-123-name
# Create PR in Azure DevOps
```

### Fix Bug

```bash
git checkout develop
git pull origin develop
git checkout -b bugfix/TASK-456-name
# Make changes
git commit -m "fix(scope): description"
git push -u origin bugfix/TASK-456-name
# Create PR in Azure DevOps
```

### Hotfix Production

```bash
git checkout main
git pull origin main
git checkout -b hotfix/TASK-789-name
# Make changes
git commit -m "fix(scope): critical fix"
git push -u origin hotfix/TASK-789-name
# Create PR to main (needs 2 approvals)
# After merged, create PR from main to develop
```

### Release Version

```bash
git checkout develop
git pull origin develop
git checkout -b release/v1.1.0
# Update version numbers
git commit -m "chore(release): v1.1.0"
git push -u origin release/v1.1.0
# Create PR to main
# After approved, merge and tag
git tag -a v1.1.0 -m "Release v1.1.0"
git push origin v1.1.0
```

---

## Support

**Questions about:**
- **Branching**: See [BRANCHING-STRATEGY.md](./BRANCHING-STRATEGY.md)
- **Team workflows**: See [TEAM-GUIDE.md](./SmartWorkz.Tools/TEAM-GUIDE.md)
- **Commit messages**: See [CLAUDE.md](./SmartWorkz.Tools/CLAUDE.md)
- **Development**: See [CLAUDE.md](./SmartWorkz.Tools/CLAUDE.md)

**Need help?**
- Contact your project admin
- Check Azure DevOps documentation
- Review pull request history for examples

---

**Version**: 1.0.0  
**Last Updated**: 2026-04-29  
**Owner**: SmartWorkz Development Team

