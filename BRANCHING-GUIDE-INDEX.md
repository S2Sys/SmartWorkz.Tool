# Branching Strategy & Azure DevOps Setup - Quick Index

**Complete guides for implementing Git Flow with Azure DevOps**

---

## 📚 Documents

### 1. **BRANCHING-STRATEGY.md** 
**What it covers:**
- Overview of Git Flow branching model
- 6 branch types (main, develop, feature, bugfix, hotfix, release)
- Branch naming conventions with examples
- Step-by-step workflow examples for each task
- Merge strategies (squash vs merge commit)
- Protection rules and best practices
- Commit message format (conventional commits)
- Troubleshooting common issues

**Read this when:**
- Learning how to create branches
- Understanding workflow for features/bugs/hotfixes
- Need branch naming conventions
- Want to know commit message format
- Troubleshooting merge issues

**Time to read**: 20-30 minutes

---

### 2. **AZURE-DEVOPS-SETUP.md**
**What it covers:**
- Prerequisites and checklist
- Creating 5 security groups
- Configuring main branch protection (2 approvals)
- Configuring develop branch protection (1 approval, auto-complete)
- Setting up code review policies (auto-assign reviewers)
- Configuring CI/CD pipelines
- Team configuration files
- Complete verification checklist
- Troubleshooting common setup issues

**Read this when:**
- Setting up a new Azure DevOps project
- Configuring branch policies
- Creating groups and assigning permissions
- Setting up code review policies
- Troubleshooting policy issues

**Time to read**: 30-45 minutes (+ 30 minutes to set up)

---

## 🚀 Quick Start (5 minutes)

### For Developers

```bash
# Create a feature
git checkout develop && git pull
git checkout -b feature/TASK-123-name
# Make changes
git commit -m "feat(scope): description"
git push -u origin feature/TASK-123-name
# Create PR in Azure DevOps
```

### For Project Admins

1. Create 5 groups (Project Administrators, Team Leads, Senior Developers, Code Quality Team, Security Team)
2. Configure main branch (2 approvals, build validation, merge commit)
3. Configure develop branch (1 approval, auto-complete, squash merge)
4. Create code review policies for auto-assignment
5. Set up build pipeline

---

## 📋 Branch Types at a Glance

| Branch | From | To | Reviews | Merge | When |
|--------|------|----|---------|----|------|
| `main` | - | - | 2 | Merge commit | Production releases |
| `develop` | - | - | 1 | Squash | Integration |
| `feature/*` | develop | develop | 1 | Squash | New features |
| `bugfix/*` | develop | develop | 1 | Squash | Non-critical bugs |
| `hotfix/*` | main | main + develop | 2 | Merge commit | Production emergencies |
| `release/*` | develop | main + develop | 2 | Merge commit | Version releases |

---

## 🎯 Scenarios

### "I want to add a new feature"
→ Read: [BRANCHING-STRATEGY.md - Creating a New Feature](./BRANCHING-STRATEGY.md#creating-a-new-feature)

### "I need to fix a production bug"
→ Read: [BRANCHING-STRATEGY.md - Hotfix for Production](./BRANCHING-STRATEGY.md#hotfix-for-production)

### "I'm setting up Azure DevOps policies"
→ Read: [AZURE-DEVOPS-SETUP.md - Step by Step](./AZURE-DEVOPS-SETUP.md)

### "My PR has merge conflicts"
→ Read: [BRANCHING-STRATEGY.md - Troubleshooting](./BRANCHING-STRATEGY.md#troubleshooting)

### "I need to configure code review policies"
→ Read: [AZURE-DEVOPS-SETUP.md - Step 4](./AZURE-DEVOPS-SETUP.md#step-4-configure-code-review-policies)

### "I don't know what branch naming to use"
→ Read: [BRANCHING-STRATEGY.md - Branch Naming Conventions](./BRANCHING-STRATEGY.md#branch-naming-conventions)

---

## ✅ Verification

After setup, verify:

1. **Can I create a feature branch?**
   ```bash
   git checkout develop
   git checkout -b feature/TASK-123-test
   ```
   Should succeed ✅

2. **Do policies work?**
   Create a PR to develop - should require 1 approval ✅

3. **Do reviewers auto-assign?**
   Create a PR - should auto-assign from groups ✅

4. **Can I force-push to feature branches?**
   Should work (no protection) ✅

5. **Can I push directly to develop?**
   Should fail - requires PR ❌

6. **Can I merge without approval?**
   Should fail - policy enforced ❌

---

## 📖 Related Documents

These guides work with existing SmartWorkz.Tools documentation:

- **[TEAM-GUIDE.md](./SmartWorkz.Tools/TEAM-GUIDE.md)** — Complete Azure DevOps team guide
- **[CLAUDE.md](./SmartWorkz.Tools/CLAUDE.md)** — Development guide for team
- **[COLLABORATION.md](./SmartWorkz.Tools/COLLABORATION.md)** — Collaboration guidelines

---

## 🔑 Key Concepts

### Git Flow
A branching model that separates production (main), staging (develop), and feature work into distinct branches.

### Branch Policies
Azure DevOps rules that enforce:
- Minimum number of approvals
- Build validation requirements
- Status checks
- Comment resolution
- Merge strategy (squash vs merge)

### Code Review Policies
Azure DevOps rules that automatically assign reviewers based on:
- Files changed
- Path patterns
- Group membership

### Conventional Commits
A commit message standard:
```
<type>(<scope>): <description>

feat(auth): add oauth integration
fix(api): handle null reference
refactor(cache): optimize queries
```

---

## ⚡ Common Commands

```bash
# Create feature from develop
git checkout develop && git pull
git checkout -b feature/TASK-123-name

# Update branch with latest develop changes
git fetch origin
git rebase origin/develop

# Force push (only on feature branches!)
git push -f origin feature/TASK-123-name

# Create merge commit
git merge --no-ff feature/TASK-123-name

# Create squash commit
git merge --squash feature/TASK-123-name

# Tag a release
git tag -a v1.1.0 -m "Release v1.1.0"
git push origin v1.1.0

# Delete merged branch
git branch -d feature/TASK-123-name
git push origin --delete feature/TASK-123-name
```

---

## 🆘 Getting Help

### Setup Issues
→ See [AZURE-DEVOPS-SETUP.md - Troubleshooting](./AZURE-DEVOPS-SETUP.md#troubleshooting)

### Git/Workflow Issues
→ See [BRANCHING-STRATEGY.md - Troubleshooting](./BRANCHING-STRATEGY.md#troubleshooting)

### Team/Process Questions
→ See [TEAM-GUIDE.md](./SmartWorkz.Tools/TEAM-GUIDE.md)

### Development Standards
→ See [CLAUDE.md](./SmartWorkz.Tools/CLAUDE.md)

---

## 📞 Support

**For questions about:**
- **Branching workflow** → Check BRANCHING-STRATEGY.md
- **Azure DevOps setup** → Check AZURE-DEVOPS-SETUP.md
- **Team roles/process** → Check TEAM-GUIDE.md
- **Code standards** → Check CLAUDE.md

**Still stuck?**
- Consult with your project admin
- Check Azure DevOps official documentation
- Review existing PRs for examples

---

## 📊 Stats

- **Branch types**: 6 (main, develop, feature, bugfix, hotfix, release)
- **Protection rules**: 15+ per branch type
- **Code review policies**: 4+ recommended
- **Setup time**: 30-45 minutes
- **Learning time**: 20-30 minutes

---

**Version**: 1.0.0  
**Created**: 2026-04-29  
**For**: SmartWorkz Development Teams

Start with **BRANCHING-STRATEGY.md** to learn the concepts, then use **AZURE-DEVOPS-SETUP.md** to implement.

