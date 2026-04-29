# Complete Branching Strategy Guide

**Version**: 1.0.0  
**Last Updated**: 2026-04-29  
**For**: SmartWorkz Development Teams

---

## Table of Contents

1. [Overview](#overview)
2. [Branch Types](#branch-types)
3. [Branch Naming Conventions](#branch-naming-conventions)
4. [Workflow Examples](#workflow-examples)
5. [Merge Strategies](#merge-strategies)
6. [Protection Rules](#protection-rules)
7. [Best Practices](#best-practices)
8. [Troubleshooting](#troubleshooting)

---

## Overview

We use **Git Flow** branching strategy with Azure DevOps. This strategy ensures:
- ✅ Stable production code on `main`
- ✅ Development integration on `develop`
- ✅ Feature isolation on feature branches
- ✅ Quick hotfixes for production issues
- ✅ Clear version tracking with releases

### Branch Hierarchy

```
main (Production - v1.0.0, v1.1.0, etc.)
  ↑
  └─ hotfix/* (Emergency fixes from main)
  
develop (Staging - next release)
  ↑
  └─ feature/* (New features)
  └─ bugfix/* (Bug fixes)
  └─ release/* (Release preparation)
```

---

## Branch Types

### 1. **main** (Production Branch)
- **Purpose**: Production-ready code, tagged with version numbers
- **Who can push**: Admins only (via approved PRs)
- **Protection level**: Maximum
- **Merges from**: `release/*` and `hotfix/*` branches
- **Tag pattern**: `v1.0.0`, `v1.1.0`, `v2.0.0`

**Characteristics:**
- Every commit is a release
- Always deployable
- Requires 2 approvals minimum
- Status checks must pass
- Force push forbidden

### 2. **develop** (Staging/Integration Branch)
- **Purpose**: Integration branch for features, next release candidate
- **Who can push**: Via approved PRs only
- **Protection level**: High
- **Merges from**: `feature/*`, `bugfix/*`, `release/*`
- **Merged to**: `main` (via release)

**Characteristics:**
- Contains integrated features for next release
- Should be stable (all tests passing)
- Requires 1 approval minimum
- Status checks must pass
- Can be rebased/squashed

### 3. **feature/\*** (Feature Branches)
- **Purpose**: Develop new features in isolation
- **Naming**: `feature/TASK-123-feature-name`
- **Created from**: `develop`
- **Merged back to**: `develop`
- **Deleted after**: Merge complete
- **Protection level**: None (direct commits allowed)

**Characteristics:**
- One feature per branch
- Short-lived (days to weeks)
- Can be force-pushed
- Developer has full control

### 4. **bugfix/\*** (Bug Fix Branches)
- **Purpose**: Fix bugs that don't need hotfixes
- **Naming**: `bugfix/TASK-456-bug-name`
- **Created from**: `develop`
- **Merged back to**: `develop`
- **Deleted after**: Merge complete
- **Protection level**: None

**Characteristics:**
- Bug fix for next release
- Same as feature branches
- Follow same review process

### 5. **hotfix/\*** (Production Hotfix Branches)
- **Purpose**: Critical fixes for production bugs
- **Naming**: `hotfix/TASK-789-critical-issue`
- **Created from**: `main`
- **Merged to**: `main` AND `develop`
- **Deleted after**: Merge complete
- **Protection level**: None initially

**Characteristics:**
- For critical production issues only
- Requires 2 approvals (same as main)
- Merged directly to main, then back to develop
- Requires immediate release

### 6. **release/\*** (Release Preparation Branches)
- **Purpose**: Prepare release (version bumps, release notes)
- **Naming**: `release/v1.1.0` or `release/2.0.0`
- **Created from**: `develop`
- **Merged to**: `main` (tagged) and back to `develop`
- **Deleted after**: Merge complete

**Characteristics:**
- Only documentation and version changes
- No new features
- Allows final testing
- Tagged after merge to main

---

## Branch Naming Conventions

### Pattern

```
<type>/<TASK-###>-<description>
```

### Types

| Type | Use Case | Example |
|------|----------|---------|
| `feature/` | New feature | `feature/TASK-123-user-authentication` |
| `bugfix/` | Bug fix | `bugfix/TASK-456-fix-login-error` |
| `hotfix/` | Production emergency | `hotfix/TASK-789-security-patch` |
| `release/` | Release preparation | `release/v1.1.0` |
| `refactor/` | Code refactoring | `refactor/TASK-234-optimize-queries` |
| `docs/` | Documentation only | `docs/TASK-567-api-documentation` |
| `test/` | Test additions | `test/TASK-890-add-unit-tests` |

### Naming Rules

- **Lowercase**: `feature/TASK-123-name` (not `Feature/`)
- **Hyphens for spaces**: `feature/TASK-123-user-auth` (not `feature/TASK-123-user auth`)
- **No special characters**: Only letters, numbers, hyphens
- **Descriptive**: `feature/TASK-123-add-oauth` (not `feature/TASK-123-stuff`)
- **Max length**: 64 characters total
- **TASK number**: Always include if available

### Valid Examples ✅

```
feature/TASK-123-oauth-integration
bugfix/TASK-456-null-reference-exception
hotfix/TASK-789-payment-processing-bug
release/v2.1.0
docs/TASK-234-api-documentation
test/TASK-567-unit-test-coverage
```

### Invalid Examples ❌

```
my-feature              # Missing type
FEATURE/TASK-123        # Wrong case
feature/123             # Missing TASK- prefix
feature/TASK-123 auth   # Space instead of hyphen
feature/TASK-123_name   # Underscore instead of hyphen
```

---

## Workflow Examples

### Creating a New Feature

**Step 1: Create and checkout feature branch**
```bash
git checkout develop
git pull origin develop
git checkout -b feature/TASK-123-user-dashboard
```

**Step 2: Make changes and commit**
```bash
# Edit files...
git add .
git commit -m "feat(dashboard): add user profile widget"
# Git hook validates commit message automatically
```

**Step 3: Push and create PR**
```bash
git push -u origin feature/TASK-123-user-dashboard
# Go to Azure DevOps → Repos → Pull Requests → New Pull Request
```

**Step 4: Fill PR details**
- Title: Clear and concise
- Description: What changed and why
- Link work item: Select TASK-123 from Azure Boards
- Reviewers: Auto-assigned based on policies

**Step 5: Address review feedback**
```bash
# Make requested changes
git add .
git commit -m "refactor(dashboard): improve accessibility"
git push
# PR automatically updates
```

**Step 6: Merge (when approved)**
- Click "Complete" in Azure DevOps
- Merge type: **Squash commit** (for develop)
- Auto-delete branch: Yes
- Link work items: Leave checked

---

### Fixing a Bug

**Step 1: Create bugfix branch from develop**
```bash
git checkout develop
git pull origin develop
git checkout -b bugfix/TASK-456-login-timeout
```

**Step 2: Fix and test**
```bash
# Make changes
git commit -m "fix(auth): increase login timeout to 30 minutes"
```

**Step 3: Push and create PR**
```bash
git push -u origin bugfix/TASK-456-login-timeout
```

**Step 4: Same review and merge as features**
- Requires 1 approval
- Squash merge to develop

---

### Hotfix for Production

**CRITICAL: Only for urgent production issues**

**Step 1: Create hotfix from main**
```bash
git checkout main
git pull origin main
git checkout -b hotfix/TASK-789-payment-bug
```

**Step 2: Fix critical issue**
```bash
git commit -m "fix(payments): resolve charge duplication"
```

**Step 3: Push and create PR to main**
```bash
git push -u origin hotfix/TASK-789-payment-bug
# Create PR to main branch
```

**Step 4: Get 2 approvals (urgent)**
- Get 2 senior developer approvals
- Status checks must pass
- Merge to main immediately

**Step 5: Tag the release**
```bash
git checkout main
git pull origin main
git tag -a v1.0.1 -m "Hotfix: Payment charge duplication"
git push origin v1.0.1
```

**Step 6: Merge back to develop**
```bash
git checkout develop
git pull origin develop
git merge --no-ff main
git push origin develop
```

---

### Release Preparation

**Step 1: Create release branch from develop**
```bash
git checkout develop
git pull origin develop
git checkout -b release/v1.1.0
```

**Step 2: Update version numbers**
```bash
# Update version in .csproj files, package.json, etc.
# Update CHANGELOG.md with release notes
git commit -m "chore(release): bump version to 1.1.0"
```

**Step 3: Push and create PR to main**
```bash
git push -u origin release/v1.1.0
# Create PR to main branch
```

**Step 4: Get 2 approvals**
- Code review (final check)
- Merge to main with merge commit

**Step 5: Tag the release**
```bash
git checkout main
git pull origin main
git tag -a v1.1.0 -m "Release: Version 1.1.0"
git push origin v1.1.0
```

**Step 6: Merge back to develop**
```bash
git checkout develop
git merge --no-ff main
git push origin develop
```

**Step 7: Delete release branch**
```bash
git branch -d release/v1.1.0
git push origin --delete release/v1.1.0
```

---

## Merge Strategies

### For develop branch: **Squash Merge**

**Why**: Keeps develop history clean, one PR = one commit

**Example:**
```bash
git checkout develop
git merge --squash feature/TASK-123-auth
git commit -m "feat(auth): add oauth integration"
git push
```

**Result**: Single clean commit on develop

### For main branch: **Merge Commit**

**Why**: Preserves full history, clear release points

**Example:**
```bash
git checkout main
git merge --no-ff release/v1.1.0
git push
```

**Result**: Merge commit showing entire feature history

### For hotfix: **Merge Commit** to both branches

**Example:**
```bash
# Merge to main
git checkout main
git merge --no-ff hotfix/TASK-789-bug
git tag v1.0.1
git push

# Merge back to develop
git checkout develop
git merge --no-ff main
git push
```

---

## Protection Rules

### main Branch Protection

**Minimum code review:**
- ✅ Require minimum reviewers: **2 reviewers**
- ✅ Reset approval on changes: **Yes**
- ✅ Allow requesters to approve: **No**
- ✅ Block last pusher vote: **Yes**

**Build and testing:**
- ✅ Require status checks to pass: **Yes**
- ✅ Status checks**: 
  - `continuous-integration/azure-pipelines`
  - Code coverage checks (if configured)

**Admin overrides:**
- ❌ Allow force push: **No**
- ❌ Allow delete: **No**
- ✅ Enforce for admins: **Yes** (admins must also follow rules)

**Work items:**
- ✅ Require work item linking: **Optional** (but recommended)

### develop Branch Protection

**Minimum code review:**
- ✅ Require minimum reviewers: **1 reviewer**
- ✅ Reset approval on changes: **Yes**
- ✅ Allow requesters to approve: **No**

**Build and testing:**
- ✅ Require status checks to pass: **Yes**

**Admin overrides:**
- ✅ Allow force push: **No**
- ✅ Allow delete: **No**

**Auto-complete:**
- ✅ Auto-complete PR: **Yes** (when approved, auto-merge)

### feature/* Branch Protection

**No restrictions:**
- ❌ Can commit directly
- ✅ Can force push
- ✅ Can delete branch

**Note**: Only protection is PR review requirement to merge to develop

---

## Commit Message Format

### Pattern

```
<type>(<scope>): <description>

<optional body>

<optional footer>
```

### Types

| Type | When |
|------|------|
| `feat` | New feature |
| `fix` | Bug fix |
| `refactor` | Code improvement (no feature) |
| `perf` | Performance improvement |
| `docs` | Documentation only |
| `test` | Add/update tests |
| `chore` | Maintenance tasks |
| `ci` | CI/CD pipeline changes |
| `style` | Formatting/style only |

### Scopes

| Scope | What it covers |
|-------|----------------|
| `api` | API endpoints |
| `auth` | Authentication/authorization |
| `ui` | User interface |
| `db` | Database changes |
| `cache` | Caching logic |
| `config` | Configuration files |
| `docs` | Documentation |

### Examples

✅ **Valid:**
```
feat(auth): add oauth2 integration
fix(api): handle null reference in user endpoint
refactor(cache): optimize redis queries
docs(readme): update setup instructions
test(auth): add unit tests for login
perf(db): add index to user_id column
```

❌ **Invalid:**
```
add feature              # Missing type/scope
Fixed bug               # Wrong case
updated code            # Too vague
feat(auth) add oauth    # Missing colon and space
```

---

## Best Practices

### ✅ DO

- Create short-lived branches (max 2 weeks)
- Commit frequently (daily)
- Push daily to backup changes
- Write clear commit messages
- Review your own changes first
- Address all review comments
- Keep PRs focused (one feature per PR)
- Test locally before pushing
- Link work items to PRs
- Use conventional commits
- Rebase feature branches before merge
- Delete merged branches immediately

### ❌ DON'T

- Commit directly to main or develop
- Create branches from main for features (always from develop)
- Use `git push --force` on shared branches
- Merge unreviewed code
- Mix features in one PR
- Commit secrets or credentials
- Leave TODO comments without context
- Ignore status check failures
- Merge with approval from auto-requester
- Merge PR before all conversations resolved
- Keep long-lived feature branches
- Commit large binaries or build artifacts

---

## Troubleshooting

### My branch is out of date with develop

**Problem**: `develop` has new commits, your branch is behind

**Solution:**
```bash
git fetch origin
git rebase origin/develop
# Or merge (less preferred)
git merge origin/develop
```

### I accidentally committed to main

**Problem**: Made a commit directly on main instead of feature branch

**Solution** (if not pushed):
```bash
git reset --soft HEAD~1  # Undo commit, keep changes
git checkout -b feature/TASK-123-fix
git commit -m "feat(scope): description"
```

**Solution** (if already pushed):
Contact your admin - may need to revert commit

### PR has merge conflicts

**Problem**: develop has changed, your branch needs update

**Solution:**
```bash
git fetch origin
git rebase origin/develop
# Fix conflicts in editor
git add .
git rebase --continue
git push -f origin feature/TASK-123-name
```

### Status checks are failing

**Problem**: Tests or build checks failed on PR

**Solution:**
```bash
# Fix the issue locally
git add .
git commit -m "fix: resolve failing test"
git push
# PR automatically updates and re-runs checks
```

### How to delete a merged branch locally and remotely

**Solution:**
```bash
# Delete locally
git branch -d feature/TASK-123-name

# Delete on remote
git push origin --delete feature/TASK-123-name

# Or in Azure DevOps UI: PR → Complete → Auto-delete
```

---

## Summary

| Branch | From | To | Approvals | Merge Type | Notes |
|--------|------|----|-----------|-----------|----|
| `main` | - | - | 2 | Merge commit | Production, tagged |
| `develop` | - | - | 1 | Squash | Integration, staging |
| `feature/*` | develop | develop | 1 | Squash | Features |
| `bugfix/*` | develop | develop | 1 | Squash | Non-critical fixes |
| `hotfix/*` | main | main + develop | 2 | Merge commit | Critical production |
| `release/*` | develop | main (tag) + develop | 2 | Merge commit | Release preparation |

---

**Questions?** See [TEAM-GUIDE.md](./SmartWorkz.Tools/TEAM-GUIDE.md) and [CLAUDE.md](./SmartWorkz.Tools/CLAUDE.md)

