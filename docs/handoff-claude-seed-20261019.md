status: open (waiting on Astra: lobby sketch, round 3 clicks, three short answers, repo visibility, merges)
for: any
from: Claude Opus 5.5 (claude-opus-5-5), 2026-10-19; updated 2026-10-20
needs: AGENTS.md, AI-USE.md, PROVENANCE.md, docs/world-brief-20261019.md, docs/decision-log-20261019.md, docs/questionnaire-answers-20261020.md, design/spaces/
---
## Task

Seed Mommy's World: folders, plans and the rules for AI use and provenance, with no Unity project yet. Astra asked
for it on 2026-10-19: "start by creating the files, but don't ... put anything in the Unity file yet." Then gather her
design decisions. On 2026-10-20 she said "end chat and leave note we will pick up later", so this card is that note.

## Ownership and state

State: paused, waiting on Astra.
Implementer: released. The seed branch `claude/seed-20261019` (pull request #1) has no active writer.
Reviewer: Astra, who reviews pull request #1. Next action: whoever picks this up reads this card, then the open
items below.

## What I did

- **Repo:** `findastra/mommys-world`. `main` holds one commit, a short README (`4e9c007`). The seed is on
  `claude/seed-20261019`, pull request #1, open and not merged. Its latest commit is the one that adds this card.
- **Visibility:** the repo was created **public** (Astra's rule for every new world). It was **private** when checked
  on 2026-10-20, probably from that day's sweep of most repos to private. That breaks two things:
  - the profile link (findastra/findastra#10);
  - the "Who made what" link in the AI line for the VRChat description.

  Astra decides: make it public again, or hold the profile link.
- **The first world-spawn-tool steps** (Unity Unicorn): the repo, and the profile link findastra/findastra#10 (open).
  - The tool's Unity step was not run.
  - unity-unicorn#24 (open, checks green) makes that step start from VCC's World template.
- **The seed:**
  - README, AGENTS.md, CLAUDE.md and SECURITY.md;
  - AI-USE.md, which Astra approved on 2026-10-20 (decision 18);
  - PROVENANCE.md and CREDITS.md;
  - the `design/` folders;
  - in `docs/`: the brief, the decision log, an empty asset ledger, and the Unity Unicorn tools list.
- **Decisions 1–19** are in `docs/decision-log-20261019.md`.
  - The headline is decision 10: the world is Astra's September draft, five floors on an island with one pole
    through the middle.
  - Every answer is in `docs/questionnaire-answers-20261020.md` (66 of them).
- **One folder per level** in `design/spaces/`: space, heaven, rave-house (the roof), dance-academy (4F),
  avatar-academy (3F), vip-locker-room (2F), lobby (1F), hell-arcade, and the-pole. Each brief says what is decided
  and what is open.
- **The September draft page:** a Claude-made HTML page from 2026-09-06, kept on Astra's PC, never on GitHub. Its
  location is in the world brief.
  - It contains club members' avatar photos, prices and 18+ notes. Decision 19: it becomes the club website, in a
    public repo, only once those are taken out until each one is OK'd.
  - Not started.
- **The questionnaire** is a private claude.ai artifact (link in `docs/questionnaire-answers-20261020.md`).
  - Round 3 holds six click questions, still unanswered as of 2026-10-20: night or daylight, and where the stage,
    backstage, lounge, photo studio and info hall go.
  - Its answers are stored in the artifact's own store, collection `answers`, one document per question id. Read
    them there, then add decision-log rows.
- **Tests:** every relative link in the repo's 25 Markdown files resolves, and the asset ledger JSON parses. Nothing
  else is testable yet: there is no Unity project and no page.

## Open items for Astra (in the order they unblock work)

1. **A sketch of the lobby (1F).** It is designed first (decision 17), from her own sketches only. Save the
   original in `design/sketches/`.
2. **Round 3 in the questionnaire:** six clicks.
3. **Three short answers** (she answered "something else"): what the pole is; how the club's name and logo show in
   the world; when the club website starts.
4. **Repo visibility:** public again, or hold findastra/findastra#10.
5. **Merges:** mommys-world#1 (the seed), findastra/findastra#10 (the profile link) and unity-unicorn#24
   (world-spawn-tool from the VCC template). None of them blocks design work.
6. **Naming:** AI-USE.md, PROVENANCE.md and CREDITS.md use uppercase names without dates, like the hub's
   PROVENANCE.md. Astra's naming rule says lowercase with a date. Keep them as contract files, or rename them?

## Also noted (not this repo)

Astra's feedback on 2026-10-20, about the hub pet program pages' chat window: "no round edges!!", and indicator
lights go to the left of a tool's name, "always, even in the border line". The session started for that fix was
archived before it made anything. Pick it up only if Astra asks.

## Done when

The seed is merged with Astra's OK and the open items are answered. Then the lobby design starts from her sketch.
The Unity project waits for her go, from VCC's World template, built with Unity Unicorn's tools.

## Kept for the world (2026-10-29)

Astra, 2026-10-29: "keep those files we are about to build Mommy's world". The world is no longer paused: she is
about to build it. Keep these, and never delete them:

| What | Where |
| --- | --- |
| Three plant collages (A, B, C) | findastra/moodglass `docs/images/moodglass-plants-collage-{a,b,c}-20261029.jpg` (moodglass#1, draft), and findastra/findastra.github.io `art/projects/moodglass-plants-collage-{a,b,c}-20261029.webp` (#27) |
| The plant pictures they come from | `%USERPROFILE%\Downloads\moodglass\plants` on her desktop PC: the pictures Moodglass posted in her Discord plants channel |

Where those pictures first came from isn't recorded yet, so check before any of them goes into the world itself
(see AI-USE.md and PROVENANCE.md). No README work for this repo until the world is built.

## Return

Record the pickup here: who, which commit was read, and the next action. Astra's new answers go into the decision
log.

## Receipt and review

Pending. No one has picked this up yet.
