# Provenance

This page answers three questions about every piece of Mommy's World: who made it, who changed it, and can you
prove it?

This world follows [Astra's PetOS PROVENANCE.md](https://github.com/findastra/astras-pet-os/blob/main/PROVENANCE.md)
and Unity Unicorn's
[asset ledger](https://github.com/findastra/unity-unicorn/blob/main/docs/asset-ledger-20261014.md): no row, no ship.

## What gets recorded, and where

| What | Where | When |
|---|---|---|
| Every asset (model, texture, shader, script, sound, video link, avatar piece) | a row in [`docs/asset-ledger-20261019.json`](docs/asset-ledger-20261019.json) | before it goes into the Unity project |
| Every design decision | a row in [`docs/decision-log-20261019.md`](docs/decision-log-20261019.md) | when it's made |
| Every change | a commit on a `claude/` or `codex/` branch, with `Co-Authored-By` naming the model | always |
| Every tool run | the tool's usage record under `Review/` | every run (`Review/` stays on the PC) |
| Every VRChat upload | `docs/publications-<date>.md`: the date, commit, version and world ID | at each upload |
| Astra's sketches and other originals | [`design/sketches/`](design/sketches/), named with their date | when added |

## A ledger row

A row has the asset-ledger fields (`id`, `name`, `kind`, `creator`, `source`, `license`, `attribution`,
`where_used`, `in_repo`, `public_ok`, `status`, `checked`). This world adds three more:

| Field | What it holds |
|---|---|
| `made` | `handmade`, `ai-assisted`, `ai-generated` or `third-party` (see [AI-USE.md](AI-USE.md)) |
| `ai` | Which assistant and model helped, and what it did. `null` when no AI was used. |
| `designed_by` | Whose idea and design it is |

Proof (receipts, emailed permissions) stays on the PC and never goes into Git.

## Rules

1. **No row, no ship.** The world is uploaded only when every asset in it is `cleared`.
2. **Originals are kept.** A sketch or a club photo is never overwritten. An edit is saved as a new file.
3. **Paid assets never go in the repo.** The ledger records them, and the files stay on the PC.
4. **Inspiration is declared.** A space inspired by a real place or someone's work says so.
5. **Credits are generated from the ledger.** [CREDITS.md](CREDITS.md) and the in-world credits board are built from
   its rows.
