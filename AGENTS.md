# Assistant rules for Mommy's World

Public credit is always Astra. Read [AI-USE.md](AI-USE.md) and [PROVENANCE.md](PROVENANCE.md) first: they are this
world's rules.

Seeded with Claude Opus 5.5 (`claude-opus-5-5`), 2026-10-19. Record the model and version whenever another assistant
changes this repository.

## Right now: design first, no Unity

- Astra, 2026-10-19: "don't do anything to make, put anything in the Unity file yet." Don't create or change a Unity
  project here until she says go.
- When she does, start from VRChat Creator Companion's World template. Never start from a copy of another world
  (Astra's Infinite Pole, Liminal Doors, Piranesi House). Take only the pieces Astra names.
- Build with Unity Unicorn's tools ([`docs/unity-unicorn-tools-20261019.md`](docs/unity-unicorn-tools-20261019.md)).
  - A tool that could serve any world is written in Unity Unicorn first.
  - A tool only for this world lives here under `Tools/`, and is also listed on Unity Unicorn's page.
- Drive an open editor through this world's agent queue. Never run Unity in batch mode while the project is open.

## Every change

- Work on `claude/<topic>-<date>` or `codex/<topic>-<date>` branches. `main` changes only by pull request, with Astra's
  OK.
- Name the model in `Co-Authored-By` and in the handoff.
- Write the asset's ledger row before the asset goes in. Add a decision-log row when Astra decides something. An
  assistant may write a decision down; it never makes one.
- Ask before any image generation, download or purchase, giving the name, source and size.
- Keep these out of Git: paid assets, receipts, account details, logs, and club members' private information.
- New descriptive files get lowercase names with Astra's America/Denver date before the extension
  (`world-brief-20261019.md`). Standard files keep their usual names.
- Leave a dated handoff in `docs/` (`handoff-<assistant>-<topic>-<date>.md`, date last, per Astra's naming rule).
