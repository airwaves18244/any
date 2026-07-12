# KLAMMER — The Facade Compiler (Path A, hardened)

Path A grilled through a 4-model adversarial pass (Fable 5 vision · Opus red-team · Opus
architecture, grounded in the live RevitNvf codebase · Sonnet GTM). Interactive dossier:
https://claude.ai/code/artifact/4da8cc09-80ba-4438-ba5a-86ba20edb5fc

## Grill verdict
As originally written, Path A was a **$300k feature dressed as a $10M company** — aimed at a
shrinking platform, sold through the wrong wallet, requiring a model nobody builds. One pivot
fixes all three: strip it to the priced **ведомость / паспорт расчёта**, make the Core **outlive
Revit**, and sell **win-rate to the manufacturers who already pay for facade engineering**. The
clean Core/adapter split you already built *is* the company — not the AI.

## Five reframes the grill forced
| Was | Now |
|---|---|
| "AI Revit copilot" | **The Facade Compiler** — LLM parses chaos, the deterministic Core is the product & moat |
| Consume an LOD-350 model (doesn't exist in RU) | **Generate** substructure from an LOD-200 surface + system choice; 3D is a byproduct |
| Save engineer-hours (cost pitch to hour-billers) | **Sell win-rate** — quote turnaround 2–3 wks → 1 hr, priced against contract value |
| Revit-only (sanctioned, receding in RU) | **CAD-agnostic Core** — Revit is adapter #1; IFC + Renga/nanoCAD follow |
| Five products | **One compiler** + two features (audit, citations) + one internal pipeline + one emergent dataset |

## The three killers → neutralized
1. **Workflow reality (HIGH).** RU НВФ substructure is done in Excel/AutoCAD/PDF, often free by the manufacturer's техотдел; the architect's model stops at the cladding surface. → **Invert:** input = LOD-200 surface + openings + system; output = full substructure + tessellation + **priced ведомость**. Create the reason to model, don't consume the model.
2. **Platform risk (HIGH, long fuse).** Revit 2024 is sanctioned/frozen/withdrawn in RU; импортозамещение pushes firms to Renga/nanoCAD; buyer pool shrinks ~10–20%/yr. → **The netstandard2.0 Core is the company; the adapter is disposable.** No geometry/norm logic ever in an adapter. IFC front-end by month 9; Renga adapter = a partnership, not a rewrite.
3. **TAM / capture / focus (MED–HIGH).** Pure-НВФ-on-Revit ≈ $0.7–1.3M, ~150–400 seats; per-seat caps it; 5 products dilute the moat; manufacturer-funding risks capture. → **Value pricing** (per-project ₽10–40k + manufacturer win-rate deals); **non-exclusive multi-system by month 6**; collapse to one engine; geography (KZ → Turkey → GCC) is a norm-pack data change.

## Hardened thesis
- **Category:** norm-native engineering compilation — national codes + the manufacturer's альбом as *executable constraints*; output is a legally defensible artifact (раскладка, ведомость, паспорт расчёта), deterministic and auditable — what no pure-LLM entrant can claim.
- **10× insight:** in НВФ the **manufacturer** pays for facade engineering as a pre-sales cost to win the supply contract. Sell **win-rate** (quote 10× more objects, never lose a tender to a slow техотдел), which moves price from ₽50k/seat to ₽1–5M/yr + per-object fees.
- **Why now (2026):** agentic LLMs finally solve dirty-model intake; local models serve air-gapped RU institutes cloud can't; Autodesk vacated + BIM mandates + no incumbent able/allowed to serve the facade layer. A rare arbitrage that decays.
- **Wedge:** paid pilot (₽1–3M) inside **one anchor manufacturer's техотдел** — five expert users who want it to work, whose garbage-model corpus is your training ground. Flat-wall 80%, one encoded system, deliverable = the паспорт расчёта.
- **Vision:** every CIS facade system becomes a compilable **digital system passport**; architects spec via a free front-end; manufacturers pay for compilability + win-rate; the company takes a per-project fee where design intent becomes a purchase order. Own the format, own the market.

## Architecture — LLM proposes, Core disposes
Five layers: **L5** thin net48 Revit adapter (extract→ModelSnapshot DTO, materialise PlacementPlan in a Transaction, mm↔ft at the boundary — IFC/Renga are just more adapters) · **L4** agent orchestrator (parses intent→typed JobSpec, generate→validate→iterate loop, narrates from structured results; never emits a coordinate or clause number; model-agnostic via `ILlmClient`) · **L3** deterministic kernel (existing `BracketLayout`/`GuideLayout`/`PanelLayout`/`FacadeTakeoff` + NormEngine/ClashEngine/SystemConstraintValidator; pure netstandard2.0 — **model quality decoupled from correctness**) · **L2** the moat: versioned `*.fsys.json` system definitions (everything a min/max/default constraint, each bound carrying its `normRef` + `provenance`) · **L1** golden-project eval + property tests + run-ledger as the trust product.

Invariant enforced mechanically: every LLM field is a closed-enum member, a `UniqueId` in the snapshot, or free text routed only to `narration`. Two SKUs one codebase: Claude cloud for commercial; local 14–32B on-prem for air-gapped institutes — viable *because* geometry is deterministic. Cloud/Windows split: promote Norm/Clash/Takeoff/Catalog into netstandard2.0; replay recorded ModelSnapshots in cloud CI; RTF on a nightly Windows self-hosted runner covers the two Revit shims; defer APS worker pool until web demand proves it.

## Compounding moat (fastest first)
Edge-case ledger (every project → a unit-tested Core rule) · executable norm suite (СП/ГОСТ as tested, cited code) · manufacturer system encodings (two-sided lock: tenders inherit specified profiles) · pricing/tender flow (emergent dataset = TenderPilot, not a build).

## GTM
Wedge buyer = anchor manufacturer техотдел (money already flows); expansion = mid-size facade subcontractors, per-project. Manufacturers stay catalog suppliers/advertisers, never product owner. Pricing: per-project ₽10–40k (primary) · seat ₽240–360k/yr (upgrade after ~20 runs) · manufacturer ₽1–5M/yr · on-prem ₽0.5–1.5M/yr. Path: $500k ARR ≈ 5 manufacturer deals + ~250 per-project accounts (18–24 mo); $2M adds CAD-independence + KZ/Turkey + NRR 140–160%.

## 18-month arc
M0–3 anchor pilot (ship deterministic one-click first, no LLM) → M4–6 systems #2–3 non-exclusive + edge-case engine + hire a facade engineer → M7–9 IFC front-end + ModelAudit as a feature → M10–12 free architect front-end flywheel + internal FamilyForge → M13–18 the KLAMMER Benchmark + geographic expansion.

## Standing no-list
NormaGPT/TenderPilot as standalone products · free-to-architects at t=0 · per-seat entry pricing · exclusivity at any price · Revit-only · TenderPilot as a build.

## Three things that must be true
1. The paid deliverable is the priced ведомость/паспорт расчёта, generated from LOD-200 input.
2. The Core is genuinely portable to Renga/nanoCAD/IFC in months — bet the company on verifying it.
3. You reach ~20–30 paying firms via founder-led per-project sales without a manufacturer's blessing.

**Positioning:** "Stripe for facades" — the deterministic compilation layer between architectural intent and a facade purchase order, in a market the incumbents abandoned.
