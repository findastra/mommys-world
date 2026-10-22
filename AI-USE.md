# How AI is used in Mommy's World

**Approved by Astra on 2026-10-20** (decision 18: "Keep them as they are"). Written by Claude Opus 5.5
(`claude-opus-5-5`) on 2026-10-19, from what Astra said that day: "I want to be ethical about it. And I want to be transparent about my use of AI. I want to preserve
creativity, authenticity, and provenance."

Astra designs Mommy's World, and AI assistants help build it. This page says plainly what they do, what they don't
do, and how you can check.

## Who does what

| Who | What they do here |
|---|---|
| Astra | The ideas, the looks, the spaces, and every final yes |
| Mommy's (the club) | Wishes for the spaces, and their own work: dances, music, photos, performances |
| AI assistants (Claude, Codex) | They write and run tools, scripts and code, drive Unity through Unity Unicorn's tools, check licences and keep these records |

## Rules

1. **AI suggests, Astra decides.** No look, space or feature goes in because an AI picked it.
2. **Every AI contribution is named.** The record gives the assistant, its model and version, the date, and what it
   did. That goes in the commit (a `claude/` or `codex/` branch, with a `Co-Authored-By` line) and in the asset's
   ledger row.
3. **Every asset has a label** in [`docs/asset-ledger-20261019.json`](docs/asset-ledger-20261019.json):
   - `handmade`: made by a person, either Astra or a club member who said yes.
   - `ai-assisted`: a person directed it and an AI did part of the work. Example: a Blender script an assistant
     wrote to Astra's design.
   - `ai-generated`: an AI made it and Astra approved it. This happens only when Astra asks for it, each time.
   - `third-party`: someone else's work, used under its licence and credited.
4. **No copying.** Nothing comes from other worlds. No AI imitates a living artist's style. Inspiration is written
   down as inspiration.
5. **People's own work and likeness need their yes.** Club members' dances, music, photos, avatars and voices go in
   only with their permission, credited by the name they choose. No AI copies anyone's face, voice or avatar.
6. **The world says it too.** The VRChat description and the in-world credits say that AI assistants helped, and
   link here. The line for the description: "Designed by Astra for Mommy's. Built in Unity with help from AI
   assistants. Who made what: github.com/findastra/mommys-world."

## Check it yourself

- **Commits:** every agent commit names its model. Astra's own commits will be signed with her security key (see
  [Astra's PetOS PROVENANCE.md](https://github.com/findastra/astras-pet-os/blob/main/PROVENANCE.md)).
- **Assets:** each ledger row says who made the asset, how, and under which licence.
- **Decisions:** [`docs/decision-log-20261019.md`](docs/decision-log-20261019.md) says who made each design choice.
