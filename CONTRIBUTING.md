# Contributing Guide

This repository incorporates [GitLab Flow](https://about.gitlab.com/2014/09/29/gitlab-flow/).
For the releations among branches, please see [here](https://about.gitlab.com/2014/09/29/gitlab-flow/#release-branches-with-gitlab-flow).

In short, "upstream first" policy:

1. `master` branch always contains the latest edit.
2. Release branches stay untouched unless there are serious bug fixes. In this case, changes are cherry-picked from `master` branch.

---

To make a pull request to fix a bug or to add features:

1. Fork or clone the repo.
2. Add a new feature branch in your repo.
3. Switch to the new branch, and do the hacking.
4. Initiate a pull request from the new branch.
5. Feature merged! Yay!
