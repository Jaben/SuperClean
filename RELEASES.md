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

### 2. Merge `develop` into `master`

```bash
git checkout master
git pull origin master
git merge --no-ff develop
git push origin master
```

This push triggers the GitHub Actions workflow (`.github/workflows/build.yml`) which will:

1. Calculate the version via GitVersion.
2. Build and pack the dotnet tool.
3. Publish the package to nuget.org (`dotnet tool install -g SuperClean`).
4. Publish the standalone single-file `SuperClean.exe`.
5. Create/update the GitHub Release for the version tag with both attached.

### 3. Bumping major/minor versions

GitVersion increments the patch version by default. To bump major or minor, tag `master` explicitly as part of the release merge:

```bash
git tag X.Y.0
git push origin master --tags
```

(Tags use no `v` prefix, e.g. `2.1.0` — matching existing tags.)

## CI/CD Details

- **Workflow**: `.github/workflows/build.yml`
- **Triggers**: All pushes and pull requests (build + pack). NuGet publish and GitHub Release only on push to `master`.
- **Versioning**: GitVersion with `ContinuousDelivery` mode (`GitVersion.yml`).
- **NuGet API key**: Stored as `NUGET_API_KEY` repository secret.
