# Release & Hotfix Flow Diagrams - Visual Guide

**Complete visual representation of how releases and hotfixes work in Git Flow**

---

## 📊 Complete Git Flow Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         GIT FLOW COMPLETE CYCLE                         │
└─────────────────────────────────────────────────────────────────────────┘

                                 PRODUCTION
                                    ▼
        ┌──────────────────────────────────────────────────────────┐
        │                  main (Production)                        │
        │              ✓ Always deployable                          │
        │              ✓ Tagged: v1.0.0, v1.1.0, v2.0.0           │
        │              ✓ 2 approvals required                       │
        │              ✓ Merge commits only                         │
        └──────────────────┬──────────────────────────────────────┘
                           │
                ┌──────────┴──────────┐
                ▼                     ▼
         ┌──────────────┐      ┌─────────────┐
         │  release/*   │      │  hotfix/*   │
         │  v1.1.0      │      │  emergency  │
         └──────────────┘      └─────────────┘
                │                      │
                │ (1) Create from      │ (1) Create from
                │     develop          │     main
                │                      │
                │ (2) Update version   │ (2) Fix bug
                │     files            │
                │                      │
                │ (3) Merge to main    │ (3) Merge to main
                │     + tag            │     + tag
                │                      │
                └──────────┬───────────┘
                           │
                    (4) Merge back to
                    develop (optional)
                           │
        ┌──────────────────┴──────────────────┐
        ▼                                      ▼
    ┌─────────────────────────────────────────────────┐
    │            develop (Staging/Integration)         │
    │        ✓ Next release candidate                  │
    │        ✓ 1 approval required                     │
    │        ✓ Squash merges                          │
    │        ✓ Auto-complete enabled                   │
    └─────────────────────────────────────────────────┘
                           ▲
                ┌──────────┴──────────┐
                │                     │
         ┌──────────────┐    ┌────────────────┐
         │ feature/*    │    │   bugfix/*     │
         │ New features │    │   Non-critical │
         │              │    │     bugs       │
         └──────────────┘    └────────────────┘

    Features & Bugfixes are short-lived branches
    that merge into develop via PRs
```

---

## 🎯 Release Workflow - Step by Step

```
┌────────────────────────────────────────────────────────────────────────┐
│                      RELEASE WORKFLOW (v1.1.0)                          │
└────────────────────────────────────────────────────────────────────────┘

STEP 1: Features integrated into develop
┌─────────────────────────────────────────────────────────┐
│  develop branch                                          │
│  ├─ Feature A merged (oauth integration)                │
│  ├─ Feature B merged (new dashboard)                    │
│  ├─ Bugfix C merged (performance issue)                │
│  └─ All tests passing, ready for release                │
└─────────────────────────────────────────────────────────┘
                           │
                      Create branch
                           │
                           ▼
STEP 2: Create release branch from develop
┌─────────────────────────────────────────────────────────┐
│  $ git checkout develop                                 │
│  $ git pull origin develop                              │
│  $ git checkout -b release/v1.1.0                       │
│                                                         │
│  release/v1.1.0 branch created                          │
│  (Copy of develop at this moment)                       │
└─────────────────────────────────────────────────────────┘
                           │
                      Prepare release
                           │
                           ▼
STEP 3: Update version numbers and documentation
┌─────────────────────────────────────────────────────────┐
│  release/v1.1.0 branch                                  │
│  ├─ Update version in .csproj: 1.0.0 → 1.1.0           │
│  ├─ Update package.json version                         │
│  ├─ Update CHANGELOG.md with release notes              │
│  ├─ Update README.md if needed                          │
│  └─ Commit: "chore(release): bump version to 1.1.0"    │
│                                                         │
│  Push to Azure DevOps                                   │
└─────────────────────────────────────────────────────────┘
                           │
                     Create PR to main
                           │
                           ▼
STEP 4: Create Pull Request to main branch
┌─────────────────────────────────────────────────────────┐
│  PR: release/v1.1.0 → main                              │
│  ├─ Title: "Release v1.1.0"                             │
│  ├─ Description: What's included                        │
│  ├─ Link to Release ticket                              │
│  └─ Auto-assigned to: Team Leads                        │
│                                                         │
│  Status: WAITING FOR APPROVALS                          │
└─────────────────────────────────────────────────────────┘
                           │
                    Get 2 approvals
                           │
                           ▼
STEP 5: Code Review & Approval
┌─────────────────────────────────────────────────────────┐
│  Team Lead 1: Reviews version changes ✅ APPROVES       │
│  Team Lead 2: Reviews changelog ✅ APPROVES             │
│  Build pipeline: ✅ PASSES                              │
│                                                         │
│  Status: READY TO MERGE                                 │
└─────────────────────────────────────────────────────────┘
                           │
                       Merge PR
                           │
                           ▼
STEP 6: Merge to main with merge commit
┌─────────────────────────────────────────────────────────┐
│  $ git checkout main                                    │
│  $ git merge --no-ff release/v1.1.0                     │
│  $ git push origin main                                 │
│                                                         │
│  Merge commit created (preserves history)               │
│  Release branch automatically deleted                   │
└─────────────────────────────────────────────────────────┘
                           │
                      Tag the release
                           │
                           ▼
STEP 7: Tag the release version
┌─────────────────────────────────────────────────────────┐
│  $ git checkout main                                    │
│  $ git pull origin main                                 │
│  $ git tag -a v1.1.0 -m "Release: Version 1.1.0"        │
│  $ git push origin v1.1.0                               │
│                                                         │
│  Tag created and pushed to remote                       │
│  GitHub shows this as a "Release"                       │
│                                                         │
│  main branch now at: v1.1.0                             │
└─────────────────────────────────────────────────────────┘
                           │
                    Merge back to develop
                           │
                           ▼
STEP 8: Merge release back to develop
┌─────────────────────────────────────────────────────────┐
│  $ git checkout develop                                 │
│  $ git merge --no-ff main                               │
│  $ git push origin develop                              │
│                                                         │
│  develop now has version bump commit                    │
│  develop and main are in sync                           │
│  Ready for next feature work                            │
└─────────────────────────────────────────────────────────┘
                           │
                     RELEASE COMPLETE! 🎉
                           │
        ┌──────────────────┴──────────────────┐
        ▼                                      ▼
    main: v1.1.0                          develop: v1.1.0
    Tagged and deployed                   Ready for new features

Timeline: 2-4 hours (1 hour if pre-agreed)
```

---

## 🚨 Hotfix Workflow - Step by Step

```
┌────────────────────────────────────────────────────────────────────────┐
│           HOTFIX WORKFLOW (Critical Production Bug - v1.0.1)            │
└────────────────────────────────────────────────────────────────────────┘

⚠️  CRITICAL: Production issue detected!
    Payment processing is broken - customers can't checkout
    Need fix ASAP

                           │
                    Immediate action
                           │
                           ▼
STEP 1: Create hotfix branch from main
┌─────────────────────────────────────────────────────────┐
│  $ git checkout main                                    │
│  $ git pull origin main                                 │
│  $ git checkout -b hotfix/TASK-999-payment-bug          │
│                                                         │
│  hotfix/TASK-999-payment-bug branch created             │
│  (Based on v1.0.0, not develop!)                        │
│                                                         │
│  Why from main?                                         │
│  - Don't include unreleased features from develop       │
│  - Fix only the production issue                        │
│  - Minimal risk, fast release                           │
└─────────────────────────────────────────────────────────┘
                           │
                    Fix the critical bug
                           │
                           ▼
STEP 2: Fix the bug on hotfix branch
┌─────────────────────────────────────────────────────────┐
│  hotfix/TASK-999-payment-bug branch                     │
│  ├─ Locate issue in PaymentProcessor.cs                 │
│  ├─ Fix: Add null check for transaction ID              │
│  ├─ Test locally: Verify payment works                  │
│  └─ Commit: "fix(payments): resolve charge duplication" │
│                                                         │
│  Minimal change, only fixes the bug                     │
└─────────────────────────────────────────────────────────┘
                           │
                     Create PR to main
                           │
                           ▼
STEP 3: Create PR to main (URGENT)
┌─────────────────────────────────────────────────────────┐
│  PR: hotfix/TASK-999-payment-bug → main                 │
│  ├─ Title: "HOTFIX: Payment processing bug - v1.0.1"    │
│  ├─ Priority: CRITICAL - Production down                │
│  ├─ Description: Clear explanation of fix               │
│  └─ Auto-assigned to: Team Leads (with urgency)         │
│                                                         │
│  Status: WAITING FOR 2 APPROVALS (URGENT)               │
└─────────────────────────────────────────────────────────┘
                           │
              Get 2 approvals FAST
              (Express review process)
                           │
                           ▼
STEP 4: Fast-track Review & Approval
┌─────────────────────────────────────────────────────────┐
│  Team Lead 1: Reviews code ✅ APPROVES (5 mins)         │
│  Team Lead 2: Reviews code ✅ APPROVES (5 mins)         │
│  Build pipeline: ✅ PASSES (2 mins)                     │
│                                                         │
│  Status: APPROVED - READY FOR IMMEDIATE MERGE           │
└─────────────────────────────────────────────────────────┘
                           │
                   Merge to main ASAP
                           │
                           ▼
STEP 5: Merge to main (merge commit)
┌─────────────────────────────────────────────────────────┐
│  $ git checkout main                                    │
│  $ git merge --no-ff hotfix/TASK-999-payment-bug        │
│  $ git push origin main                                 │
│                                                         │
│  Fix is now in main branch                              │
│  Production can be deployed immediately                 │
│  Release branch NOT needed (minor version bump)         │
└─────────────────────────────────────────────────────────┘
                           │
                    Tag the hotfix
                           │
                           ▼
STEP 6: Tag the hotfix version
┌─────────────────────────────────────────────────────────┐
│  $ git checkout main                                    │
│  $ git tag -a v1.0.1 -m "Hotfix: Payment processing"    │
│  $ git push origin v1.0.1                               │
│                                                         │
│  main: v1.0.0 → v1.0.1 (patch version only)             │
│  Ready for immediate production deployment              │
│                                                         │
│  Deploy to production NOW! 🚀                           │
└─────────────────────────────────────────────────────────┘
                           │
        ┌──────────────────┴──────────────────┐
        │                                     │
        │ (Production live with fix)          │
        │                                     │
        └──────────────────┬──────────────────┘
                           │
              Now merge back to develop
              (Very important!)
                           │
                           ▼
STEP 7: Merge hotfix back to develop
┌─────────────────────────────────────────────────────────┐
│  Why merge back to develop?                             │
│  ├─ develop needs the bug fix                           │
│  ├─ Future releases will include this fix               │
│  ├─ Keep main and develop in sync                       │
│  └─ Prevent regression when next release happens        │
│                                                         │
│  $ git checkout develop                                 │
│  $ git pull origin develop                              │
│  $ git merge --no-ff main                               │
│  $ git push origin develop                              │
│                                                         │
│  develop now includes the payment fix                   │
│  develop branch: v1.1.0-dev (with payment fix)          │
└─────────────────────────────────────────────────────────┘
                           │
                    HOTFIX COMPLETE! ✅
                           │
        ┌──────────────────┴──────────────────┐
        ▼                                      ▼
    main: v1.0.1                          develop: v1.1.0
    (with payment fix)                   (with payment fix)
    Production safe                      Ready for features

Timeline: 15-30 minutes (emergency response)
```

---

## 🔄 Side-by-Side: Release vs Hotfix

```
┌────────────────────────────────────────────────────────────────────────┐
│                    RELEASE vs HOTFIX COMPARISON                         │
└────────────────────────────────────────────────────────────────────────┘

RELEASE (Planned)                    │  HOTFIX (Emergency)
─────────────────────────────────────┼──────────────────────────────────
Timeline: 2-4 hours                  │  Timeline: 15-30 minutes
Planned release day                  │  Unplanned, any time
─────────────────────────────────────┼──────────────────────────────────
Created from: develop                │  Created from: main
Contains: Multiple new features      │  Contains: 1 critical bug fix
Includes: All work for release       │  Excludes: New features
─────────────────────────────────────┼──────────────────────────────────
Branch: release/v1.1.0               │  Branch: hotfix/TASK-999-name
Naming: release/VERSION              │  Naming: hotfix/TASK-###-name
─────────────────────────────────────┼──────────────────────────────────
Approvals: 2 required                │  Approvals: 2 required (fast-track)
Review time: Standard process        │  Review time: URGENT, express
Testing: Full QA cycle               │  Testing: Focused test
─────────────────────────────────────┼──────────────────────────────────
Merge to: main (merge commit)        │  Merge to: main (merge commit)
Tag: v1.1.0                          │  Tag: v1.0.1 (patch version)
─────────────────────────────────────┼──────────────────────────────────
Merge back: Yes, to develop          │  Merge back: Yes, CRITICAL!
Why back: Keep develop updated       │  Why back: Keep bug fix in develop
─────────────────────────────────────┼──────────────────────────────────
Version bump:                        │  Version bump:
  1.0.0 → 1.1.0 (minor)              │  1.0.0 → 1.0.1 (patch)
─────────────────────────────────────┼──────────────────────────────────
Deploy: Scheduled time               │  Deploy: Immediate
Impact: Multiple features            │  Impact: 1 bug fix only
Risk: Moderate                       │  Risk: Minimal (small change)
─────────────────────────────────────┼──────────────────────────────────
File changes: Many                   │  File changes: 1-3 files
Scope: Multiple systems              │  Scope: Single issue
```

---

## 📈 Complete Timeline Example

```
┌────────────────────────────────────────────────────────────────────────┐
│                      TIMELINE: 2-WEEK SPRINT CYCLE                      │
└────────────────────────────────────────────────────────────────────────┘

WEEK 1: Feature Development
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Monday:  ┌─ feature/TASK-100-auth ─────────────────┐
         │                                         │
         └─ feature/TASK-101-dashboard ──────────┐ │
                                                  │ │
         ┌─ feature/TASK-102-api ────────────────┐│ │
         │                                       ││ │
         develop ◄─ (integrate features) ─ merge ││ │
                                                  ││ │
Friday:  develop v1.1.0-dev                       ││ │
         (All features integrated & tested)        ││ │
         └──────────────────────────────────────────┘│ │
                                                      │ │
         (Features continue into next sprint)         │ │


WEEK 2: Release Preparation
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

Monday:  ┌─ release/v1.1.0 (from develop)
         │  • Update version numbers
         │  • Write release notes
         │  │
         └─► PR to main (needs 2 approvals)
             │
             ▼
         Review & Approval: 1-2 hours
             │
             ▼
Wednesday:  main ◄─── merged (merge commit)
            │
            ├─ Tag: v1.1.0 ✅
            │
            ├─ Deploy to production 🚀
            │
            └─ Merge back to develop
               │
               ▼
            develop v1.1.0 (now in sync with main)
            Ready for next sprint


EMERGENCY: Hotfix During Release
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⚠️  Tuesday 3:00 PM: CRITICAL BUG FOUND IN PRODUCTION (v1.0.0)

Tuesday:  ┌─ hotfix/TASK-999-critical (from main v1.0.0)
3:15 PM   │  • Fix payment processing
          │  • Test locally
          └─► PR to main (EXPRESS REVIEW)
              │
              ▼ (5 mins each reviewer)
          Approved ✅ Deploy 🚀
              │
3:45 PM   └─ main: v1.0.0 → v1.0.1
              │
              ├─ Tag: v1.0.1
              │
              ├─ Merge back to develop
              │
              ▼
          develop: gets payment fix
          (develop was already at v1.1.0, now with hotfix)

Result: Production safe, develop protected


TIMELINE SUMMARY:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Feature branch creation:  1 day
Feature development:      5-7 days
Feature review & merge:   1-2 hours
Release branch creation:  1 hour
Release review:           1-2 hours
Release merge & deploy:   15-30 mins
Hotfix (if needed):       15-30 mins (emergency)
```

---

## 🎯 Detailed Branch Diagram

```
┌────────────────────────────────────────────────────────────────────────┐
│                     DETAILED BRANCH RELATIONSHIPS                       │
└────────────────────────────────────────────────────────────────────────┘

                                TIME →

v1.0.0 ═══════════════════════════════════════════════════════════════════
       │                                                                   │
       │                  main (Production)                               │
       │                  Tag: v1.0.0                                     │
       │                  Status: Stable, deployed                        │
       │                                                                   │
       │                                                                   │
       └──────────────────┬──────────────────────────────────────────────┬─────
                          │                                              │
                          │ Create hotfix branch                         │
                          │ (Emergency fix)                              │
                          │                                              │
                    ┌─────┴────────────┐                                 │
                    │                  │                                 │
            hotfix/v1.0.1 ────► Fix ───┘  (20 mins)                     │
                    │                                                    │
                    └─────┬────────────────────────────────────────────────┬
                          │                                              │
                          │ Merge to main                                │
                          │ Tag: v1.0.1                                 │
                          │ Deploy to production                         │
                          │                                              │
                    ┌─────┘                                              │
                    │                                                    │
v1.0.1 ════════════●════════════════════════════════════════════════════●═════
       │           │                                                    │
       │           │ MERGE BACK TO DEVELOP (IMPORTANT!)                │
       │           │ (Bug fix now in next release)                     │
       │           │                                                    │
       │           │  develop (Staging)  ◄───────────────────────┐     │
       │           │  v1.1.0-dev                                 │     │
       │           │                                              │     │
       │    ┌──────┴──────┐                                       │     │
       │    │             │                                       │     │
       │    ▼             ▼                                       │     │
       │ feature/TASK-100 feature/TASK-101  (New features)       │     │
       │    │             │                                       │     │
       │    └──────┬──────┘                                       │     │
       │           │ Merge via PR                                 │     │
       │           │ 1 approval required                          │     │
       │           │ Squash commit                                │     │
       │           │                                              │     │
       │    ┌──────┴───────────────────────────────────────────┐ │     │
       │    │                                                  │ │     │
       │    └─────────► develop (Integration)                 │ │     │
       │               ├─ feature/100 merged                   │ │     │
       │               ├─ feature/101 merged                   │ │     │
       │               ├─ hotfix/v1.0.1 merged                │ │     │
       │               │                                       │ │     │
       │               │ Status: All tests passing              │ │     │
       │               │ Ready for release                      │ │     │
       │               │                                        │ │     │
       │               │ After 2 weeks: Ready for v1.1.0        │ │     │
       │               │                                        │ │     │
       │    ┌──────────┴───────────────┐                        │ │     │
       │    │                          │                        │ │     │
       │    ▼                          ▼                        │ │     │
       │ release/v1.1.0              release/v1.2.0            │ │     │
       │ (Version bump)              (Next release)            │ │     │
       │ (Release notes)                                        │ │     │
       │ │                                                      │ │     │
       │ └──► PR to main (2 approvals)                         │ │     │
       │      │                                                 │ │     │
       │      ▼                                                 │ │     │
       │   Merge & Tag: v1.1.0 ─────────────────────────────┐  │ │     │
       │   Deploy to production 🚀                           │  │ │     │
       │                                                     │  │ │     │
       │                                                     │  │ │     │
v1.1.0 ═════════════════════════════════════════════════════●──●─┘     │
       │                                                     │           │
       │ (merge back to develop)  ◄─────────────────────────┘           │
       │                                                                 │
       │ develop v1.1.0 (in sync with main)                            │
       │ Ready for next sprint features                                │
       │                                                                 │
       └────────────────────────────────────────────────────────────────┘

KEY POINTS:
───────────
1. main only receives merges from release/* and hotfix/*
2. develop receives merges from feature/*, bugfix/*, release/*, hotfix/*
3. hotfix/* MUST merge back to develop (critical!)
4. release/* updates version then merges to main
5. Feature branches are deleted after merge
6. Every commit to main is tagged with version
```

---

## 🔄 Hotfix Loop - Why Merge Back to Develop?

```
┌────────────────────────────────────────────────────────────────────────┐
│           HOTFIX MERGE-BACK: Why It's CRITICAL                         │
└────────────────────────────────────────────────────────────────────────┘

SCENARIO: Bug found in v1.0.0 production, but develop is already at v1.1.0

Before hotfix merge-back (❌ WRONG):
─────────────────────────────────────

main:          v1.0.0 ──► v1.0.1 (with bug fix)
               ↓
               ● Deployed to production
               ● Has: Bug fix

develop:       v1.1.0-dev ──────► v1.1.0 → v2.0.0
               ↓
               ● Doesn't have bug fix from v1.0.1!
               ● When v1.1.0 released, BUG RETURNS! 😱

RESULT: The bug fix is LOST in the next release!


After hotfix merge-back (✅ CORRECT):
──────────────────────────────────────

main:          v1.0.0 ──► v1.0.1 (with bug fix)
               ↓
               ● Deployed to production
               ● Has: Bug fix

               MERGE BACK TO DEVELOP ↓

develop:       v1.1.0-dev ──────┐
               (gets bug fix)   │
                                ├─► v1.1.0 (Release)
                                │   ● Has: Bug fix from v1.0.1
                                │   ● Has: New features
                                │
                                └─► v2.0.0 (Future)
                                    ● Has: Bug fix
                                    ● Has: New features

RESULT: Bug fix is protected in ALL future releases! ✅


TIMELINE COMPARISON:
───────────────────

WITHOUT merge-back:
  v1.0.0 ──► v1.0.1 ──► v1.1.0 (bug returns!) ──► v2.0.0 (bug still there!)
             (fix)      (lost fix)                (lost fix)

WITH merge-back:
  v1.0.0 ──► v1.0.1 ──► v1.1.0 (fix protected) ──► v2.0.0 (fix protected)
             (fix)      (fix preserved)          (fix preserved)


THE MERGE-BACK PROCESS:
──────────────────────

1. Fix merged to main & deployed
   $ git merge --no-ff hotfix/TASK-999-name
   $ git tag v1.0.1

2. Immediately merge back to develop
   $ git checkout develop
   $ git merge --no-ff main    ◄─── CRITICAL!
   $ git push origin develop

3. develop now has:
   ├─ Ongoing work (features)
   ├─ Bug fix from v1.0.1
   └─ All future releases protected


VISUAL: The "Hotfix Loop"
────────────────────────

       ┌──────────────────────────┐
       │   main (v1.0.0)          │
       │   Production             │
       └──────────┬───────────────┘
                  │
                  │ BUG FOUND!
                  │ Create hotfix
                  ▼
        ┌─────────────────┐
        │ hotfix/TASK-999 │
        │ (Fix the bug)   │
        └────────┬────────┘
                 │
                 ▼
        ┌─────────────────────────┐
        │ main (v1.0.1)           │
        │ With bug fix            │
        │ Deployed to production  │
        │ ✅ BUG IS FIXED         │
        └──────────┬──────────────┘
                   │
                   │ IMPORTANT: Merge back
                   │ to protect future releases
                   │
                   ▼
        ┌──────────────────────────┐
        │ develop (v1.1.0-dev)     │
        │ Gets bug fix merged      │
        │ ✅ BUG FIXED IN DEV      │
        │ ✅ FUTURE RELEASES SAFE  │
        └──────────┬───────────────┘
                   │
                   │ Continue development
                   │
                   ▼
        ┌──────────────────────────┐
        │ v1.1.0, v2.0.0 (future)  │
        │ All have the bug fix     │
        │ ✅ PROTECTED FOREVER     │
        └──────────────────────────┘
```

---

## 📋 Quick Command Reference

```
┌────────────────────────────────────────────────────────────────────────┐
│                    GIT COMMANDS FOR RELEASE & HOTFIX                    │
└────────────────────────────────────────────────────────────────────────┘

RELEASE PROCESS:
────────────────

# 1. Create release branch from develop
$ git checkout develop
$ git pull origin develop
$ git checkout -b release/v1.1.0

# 2. Update version files
#    • Update .csproj, package.json
#    • Update CHANGELOG.md
#    • Update version constants

# 3. Commit version changes
$ git add .
$ git commit -m "chore(release): bump version to 1.1.0"

# 4. Push release branch
$ git push -u origin release/v1.1.0
#    (Create PR in Azure DevOps)

# 5. After 2 approvals, merge to main
$ git checkout main
$ git pull origin main
$ git merge --no-ff release/v1.1.0
$ git push origin main

# 6. Tag the release
$ git tag -a v1.1.0 -m "Release: Version 1.1.0"
$ git push origin v1.1.0

# 7. Merge back to develop
$ git checkout develop
$ git pull origin develop
$ git merge --no-ff main
$ git push origin develop

# 8. Delete release branch (auto-deleted by Azure DevOps)
$ git branch -d release/v1.1.0
$ git push origin --delete release/v1.1.0


HOTFIX PROCESS:
───────────────

# 1. Create hotfix branch from main (NOT develop!)
$ git checkout main
$ git pull origin main
$ git checkout -b hotfix/TASK-999-critical-bug

# 2. Fix the bug
#    • Edit affected files
#    • Test locally to verify fix
#    • Keep changes minimal

# 3. Commit the fix
$ git add .
$ git commit -m "fix(payments): resolve charge duplication"

# 4. Push hotfix branch
$ git push -u origin hotfix/TASK-999-critical-bug
#    (Create PR in Azure DevOps → main)

# 5. After 2 FAST approvals, merge to main
$ git checkout main
$ git pull origin main
$ git merge --no-ff hotfix/TASK-999-critical-bug
$ git push origin main

# 6. Tag the patch version
$ git tag -a v1.0.1 -m "Hotfix: Payment processing bug"
$ git push origin v1.0.1
#    Deploy to production IMMEDIATELY! 🚀

# 7. CRITICAL: Merge back to develop
$ git checkout develop
$ git pull origin develop
$ git merge --no-ff main           ◄─── THIS IS CRITICAL!
$ git push origin develop
#    Now develop has the bug fix

# 8. Delete hotfix branch
$ git branch -d hotfix/TASK-999-critical-bug
$ git push origin --delete hotfix/TASK-999-critical-bug
```

---

## ✅ Verification Checklist

**After Release:**
- [ ] Tag created on main (v1.1.0)
- [ ] Merge commit shows in main history
- [ ] develop has merge commit from main
- [ ] Versions match in main and develop
- [ ] Release branch deleted
- [ ] Deployed to production

**After Hotfix:**
- [ ] Tag created on main (v1.0.1)
- [ ] Merge commit shows in main history
- [ ] **develop has bug fix merged (CRITICAL!)**
- [ ] Hotfix branch deleted
- [ ] Deployed to production
- [ ] Next release will include bug fix

---

**Version**: 1.0.0  
**Created**: 2026-04-29  
**For**: SmartWorkz Development Teams

