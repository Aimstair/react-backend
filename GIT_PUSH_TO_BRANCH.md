# How to Push Changes to Another Branch

## Option 1: Create a New Branch and Push (Recommended)

### Step 1: Create and Switch to a New Branch
```powershell
# Create a new branch with a descriptive name
git checkout -b feature/backend-api-improvements

# Or if you prefer a different name:
# git checkout -b feature/booking-api-fixes
# git checkout -b feature/cors-and-database-setup
```

### Step 2: Add Your Changes
```powershell
# Add all modified and new files
git add .

# Or add specific files:
# git add ASI.Basecode.WebApp/Controllers/BookingController.cs
# git add seed-test-data.sql
```

### Step 3: Commit Your Changes
```powershell
# Commit with a descriptive message
git commit -m "Add booking API endpoints, CORS configuration, and database seed scripts"
```

### Step 4: Push to the New Branch
```powershell
# Push the new branch to remote
git push -u origin feature/backend-api-improvements
```

## Option 2: Push to an Existing Branch

If the branch already exists:

```powershell
# Switch to the existing branch
git checkout existing-branch-name

# Add and commit your changes
git add .
git commit -m "Your commit message"

# Push to the branch
git push origin existing-branch-name
```

## Option 3: Create Branch from Current Changes (Without Committing First)

```powershell
# Stash your current changes
git stash

# Create and switch to new branch
git checkout -b feature/your-branch-name

# Apply your stashed changes
git stash pop

# Add, commit, and push
git add .
git commit -m "Your commit message"
git push -u origin feature/your-branch-name
```

## Quick Reference Commands

```powershell
# Check current branch
git branch

# See all branches (local and remote)
git branch -a

# See what files have changed
git status

# See the differences
git diff

# Create branch without switching
git branch new-branch-name

# Switch to a branch
git checkout branch-name

# Or use the newer syntax
git switch branch-name
```

## Common Branch Naming Conventions

- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `hotfix/description` - Urgent production fixes
- `refactor/description` - Code refactoring
- `docs/description` - Documentation updates

## Example Workflow

```powershell
# 1. Create feature branch
git checkout -b feature/booking-api

# 2. Make your changes (already done)

# 3. Stage changes
git add .

# 4. Commit
git commit -m "Add booking validation endpoints and CORS configuration for frontend"

# 5. Push to remote
git push -u origin feature/booking-api
```

