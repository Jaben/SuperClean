# Release Process

This project follows [Gitflow](https://nvie.com/posts/a-successful-git-branching-model/) and uses [GitVersion](https://gitversion.net/) (ContinuousDelivery mode) for automatic semantic versioning. NuGet publishing is handled by GitHub Actions on push to `master`.

## Branch Overview

| Branch | Purpose |
|---|---|
| `master` | Production releases. Pushes here trigger NuGet publish and a GitHub Release. |
| `develop` | Integration branch for the next release. |
| `feature/*` | Feature branches off `develop`. |

## Release Steps

### 1. Ensure `develop` is ready

- All work for the release is merged into `develop`.
- CI is green on `develop`.

### 2. Merge `develop` into `master` and tag

Tagging is required — GitVersion (ContinuousDelivery mode) produces a prerelease version (e.g. `2.0.1-8`) for untagged commits on `master`, which nuget.org rejects. The tag is what makes the release version clean.

```bash
git checkout master
git pull origin master
git merge --no-ff develop
git tag X.Y.Z
git push origin master --tags
```

This push triggers the GitHub Actions workflow (`.github/workflows/build.yml`) which will:

1. Calculate the version via GitVersion.
2. Build and pack the dotnet tool.
3. Publish the package to nuget.org (`dotnet tool install -g SuperClean`).
4. Publish the standalone single-file `SuperClean.exe`.
5. Create/update the GitHub Release for the version tag with both attached.

Tags use no `v` prefix (e.g. `2.1.0`), matching existing tags. GitVersion increments the patch by default between releases; pick the tag number to bump major/minor.

## CI/CD Details

- **Workflow**: `.github/workflows/build.yml`
- **Triggers**: All pushes and pull requests (build + pack). NuGet publish and GitHub Release only on push to `master`.
- **Versioning**: GitVersion with `ContinuousDelivery` mode (`GitVersion.yml`).
- **NuGet API key**: Stored as `NUGET_API_KEY` repository secret.
