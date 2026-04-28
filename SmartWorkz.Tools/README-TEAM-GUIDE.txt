================================================================================
SMARTWORKZ.TOOLS - TEAM GUIDE REFERENCE
================================================================================

📖 READ THIS FIRST: TEAM-GUIDE.md

This is your complete reference for everything:
✅ Creating new projects
✅ Git workflow (Feature, Bugfix, Hotfix)
✅ Commit message format (Conventional Commits)
✅ Code review process
✅ Team roles and responsibilities
✅ Branch protection rules
✅ Automatic reviewer assignment
✅ Troubleshooting

================================================================================
QUICK START (5 MINUTES)
================================================================================

FOR DEVELOPERS:
  1. Create project:
     dotnet run -- --name "MyProject" --type "DotNet"

  2. Update team members:
     code S:\SmartWorkz101\MyProject\.github\TEAM.yml

  3. Start developing:
     git checkout develop
     git checkout -b feature/TASK-123-name
     git commit -m "feat(scope): description"

FOR ADMINS:
  1. Create GitHub teams at https://github.com/orgs/smartworkz/teams
  2. Add team members
  3. Set branch protection (main: 2 approvals, develop: 1 approval)
  4. Test with a PR

================================================================================
KEY FEATURES
================================================================================

✅ ONE COMMAND PROJECT CREATION
   All automation happens automatically in 10 seconds
   - Azure DevOps project
   - Git repository
   - Configuration files
   - Git hooks (commit validation)
   - CODEOWNERS (auto reviewer assignment)
   - PR template
   - Team configuration

✅ GIT HOOKS VALIDATION
   Commits must follow format: type(scope): description
   Examples: feat(console): new feature
            fix(api): bug fix
            refactor(generator): improve code

✅ AUTOMATIC REVIEWER ASSIGNMENT
   GitHub automatically assigns reviewers based on:
   - Files changed
   - Code ownership patterns
   - CODEOWNERS rules

✅ STANDARDIZED CODE REVIEW
   PR template guides developers through:
   - Description
   - Type of change
   - Testing done
   - Review checklist

✅ CLEAR TEAM ROLES
   - Admins: Full access, setup, releases
   - Team Leads: Approve main (2), develop (1)
   - Senior Developers: Review complex code
   - Code Quality Team: Review configuration
   - Security Team: Review auth/security code

================================================================================
DOCUMENTATION STRUCTURE
================================================================================

TEAM-GUIDE.md (THIS IS YOUR REFERENCE)
├─ Table of Contents
├─ Quick Start (5 minutes)
├─ Creating a New Project
├─ Git Workflow
├─ Commit Message Format
├─ Code Review Process
├─ Team Roles
├─ Branch Protection Rules
├─ Files Created
├─ Troubleshooting
└─ Quick Reference

All team members should read TEAM-GUIDE.md

================================================================================
WORKFLOW SUMMARY
================================================================================

DEVELOPER WORKFLOW:
  1. Create project (admin runs setup)
  2. Create feature branch
  3. Make changes (git hook validates commits)
  4. Push and create PR (GitHub auto-assigns reviewers)
  5. Address feedback
  6. Merge when approved

ADMIN WORKFLOW:
  1. Create GitHub teams
  2. Add team members
  3. Set branch protection rules
  4. Verify with test PR

REVIEWER WORKFLOW:
  1. Receive auto-assignment (via CODEOWNERS)
  2. Review code against checklist
  3. Approve or request changes
  4. Code merged when approved

================================================================================
FILES AUTO-CREATED
================================================================================

Configuration Files (7):
  .gitignore
  .editorconfig
  .eslintrc.json
  .prettierrc.json
  sonar-project.properties
  azure-pipelines.yml
  README.md

Team Collaboration Files (5):
  .git/hooks/prepare-commit-msg
  .git/hooks/commit-msg
  .github/CODEOWNERS
  .github/PULL_REQUEST_TEMPLATE.md
  .github/TEAM.yml

Total: 12 files created automatically

================================================================================
BRANCH STRUCTURE
================================================================================

main (PRODUCTION)
  ├─ 2 approvals required
  ├─ Status checks required
  ├─ Force push disabled
  └─ For: releases and hotfixes

develop (STAGING)
  ├─ 1 approval required
  ├─ Status checks required
  ├─ Force push disabled
  └─ For: feature integration

feature/TASK-###-name (FEATURE)
  ├─ Created from: develop
  ├─ Approval: 1 (via PR)
  ├─ Force push allowed
  └─ Merged via: Squash merge

================================================================================
COMMIT MESSAGE EXAMPLES
================================================================================

VALID ✅:
  feat(console): add new command
  fix(api): handle null exception
  refactor(generator): simplify logic
  perf(console): optimize processing
  docs(readme): update instructions
  test(generator): add unit tests
  chore(deps): update dependencies
  ci(pipeline): add quality checks

INVALID ❌:
  add feature                           (missing type/scope)
  Fixed bug                             (needs lowercase)
  This is way too long and exceeds...   (too long)
  commit without format                 (wrong format)

================================================================================
TEAM ROLES AT A GLANCE
================================================================================

ADMINS:
  - Create projects
  - Set up GitHub
  - Configure branches
  - Release management

TEAM LEADS:
  - Approve PRs to main (2 required)
  - Approve PRs to develop (1 needed)
  - Merge PRs
  - Enforce standards

SENIOR DEVELOPERS:
  - Review complex code
  - Validate architecture
  - Mentor team

CODE QUALITY TEAM:
  - Review configuration files
  - Check project structure
  - Validate CI/CD

SECURITY TEAM:
  - Review auth/security code
  - Check for secrets
  - Validate best practices

================================================================================
TROUBLESHOOTING QUICK REFERENCE
================================================================================

Problem: Git hooks not working
  → Install Git Bash or use WSL

Problem: Reviewers not assigned
  → Create GitHub teams first, then test PR

Problem: Can't merge PR
  → Check: approvals, status checks, up to date

Problem: Commit rejected by hook
  → Check format: type(scope): description

Problem: PR template not showing
  → Verify: .github/PULL_REQUEST_TEMPLATE.md exists

Full troubleshooting guide: See TEAM-GUIDE.md

================================================================================
SUMMARY
================================================================================

Everything is automated and integrated.

ONE COMMAND creates complete project with:
  ✅ All configuration files
  ✅ Git hooks validation
  ✅ CODEOWNERS setup
  ✅ PR template
  ✅ Team configuration
  ✅ Ready to code

Time saved: 1-2 hours per project → 10 seconds

All team members should:
  1. Read TEAM-GUIDE.md
  2. Understand their role
  3. Follow the workflow
  4. Use commit format
  5. Request code reviews

No manual setup needed anymore.
Everything is automated.

================================================================================
QUESTIONS?
================================================================================

Read TEAM-GUIDE.md - it has everything you need

TEAM-GUIDE.md structure:
  - Quick Start
  - Git Workflow
  - Commit Format
  - Code Review
  - Team Roles
  - Branch Rules
  - Troubleshooting
  - Quick Reference

================================================================================
Version: 2.0.0
Date: 2026-04-28
Status: Complete ✅
