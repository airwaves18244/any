# KLAMMER Engine — Best-in-the-World Automated Revit Modeling of НВФ

Super-grill of the modeling engine itself: 4 models (Fable 5 capability ladder · Opus red-team
geometry autopsy · Opus engine architecture · Sonnet competitive benchmark), each grounded in the
live `RevitNvf.Core` source. Goal: **best in the world at automated Revit modeling of ventilated
facades across all cladding types and all levels of building-architecture complexity.**

---

## 0. The verdict on today's engine

The current Core solves **Tier 0** cleanly — and Tier 0 is the one case that never occurs in
practice: a blank rectangular wall with no openings, no corners, no neighbors.

The lie is in the type system: `Substrate` (src/RevitNvf.Core/Model/Substrate.cs) holds exactly two
doubles. `PlanarFaceSubstrate.ToSubstrate()` collapses a Revit face to its **UV bounding box** —
losing the real outline (gables, cut corners), all window/door holes, and world orientation
(face UV is not guaranteed gravity-aligned; the bbox origin may not even lie on the face).
`BracketLayout`/`PanelLayout` are Cartesian products that cannot express a hole.

**Every algorithm written against the bare rectangle is rework.** This is not a refactor; it is a
restart of the Core's data contracts — keeping the two things that are genuinely right: the
pure-deterministic-Core / thin-adapter split, and the idempotence discipline. Those are the correct
bones; the flesh must be rebuilt on a polygon-with-holes substrate in a world-anchored frame.

## 1. The complexity ladder (what "all complexity" decomposes into)

| Tier | Scope | New problems that must be solved |
|---|---|---|
| **T0** | Flat rectangle | DONE — axis subdivision, start modes, edge fillers, takeoff |
| **T1** | True boundary + openings | Polygon-with-holes region; anchor exclusion zones + edge-distance minima (~100 mm); jamb/lintel bracket densification; guide splitting at openings; panel clipping; **reveal generation (откос/отлив/lintel) with depth derived from LayerStackup**; joint-to-jamb snapping. *Where every Dynamo script dies. Minimum bar for a real building.* |
| **T2** | Multi-face + corners | Face-adjacency graph with dihedral classification; **building-global datums** (courses align across faces — per-face solving is conceptually wrong from here); corner strategies (wrap / miter / L-element / trim, per cladding); corner bracket doubling + thermal gap; corner SKUs with unfold |
| **T3** | Full-building engineering | Parapet caps, цоколь, deformation joints; **thermal fixed/sliding runs** (exactly one несущий per segment, splice gaps, max run length); **wind-zone densification as parameter fields** (corner strips ≈ b/10, parapet bands, height bands — zone maps as data, no verification calc); fire рассечки/окантовка (АКП); weak-substrate mode (газобетон → attachment at slab lines = a layout-topology switch). *Converts output from "a picture" to рабочая документация — pricing power lives here.* |
| **T4** | Inclined / faceted / cylindrical / soffits | Developed-surface panelization (cylinder unrolls exactly); chord-tolerance faceting (`δ = c²/8R ≤ δ_allow`); gravity ≠ face-up (fixed/sliding and admissibility change); engine must **refuse correctly** where systems aren't certified overhead |
| **T5** | Free-form / double-curved | Mesh planarization, per-panel fabrication, scan-to-BIM absorption. **A different product generation — deliberately not promised.** |

## 2. The three existential problems (red-team consensus)

1. **Hole-aware, world-oriented, multi-face substrate extraction** from messy real models (joined
   wall segments, sweeps, masses, curtain systems, links). Everything is downstream of this.
   `Substrate = W×H` actively prevents it.
2. **Editable associativity via diff-and-reconcile.** "Regenerate from scratch" fails in practice:
   every ElementId churns → every tag/dimension/override detaches → schedules diff as
   delete-all/add-all → one window move = full facade re-issue. This is what separates a *modeler*
   from a one-shot screenshot generator, and it **cannot be retrofitted** — element identity must be
   designed into Core output contracts now.
3. **Engineering-correctness validation that fails loudly.** Plausible-but-wrong output passes
   visual review and fails экспертиза/site: anchors on mortar joints, zero fixed/sliding logic,
   20 mm slivers at corners, uniform grid under-fastening the wind-corner strips. Encode СП 426 /
   ГОСТ Р 58154-55 constraints as a validation layer that refuses or flags — never renders silently.

(Corners — coupled-face course alignment — is the close 4th, and is really a facet of #1's data model.)

## 3. Engine architecture (the rebuild)

### Substrate model — three layers, all pure Core types
- **`PlanarRegion`** = outer `Ring` + hole `Ring`s + `FaceFrame` (world origin + gravity-aligned
  U/V + normal). Built by the adapter from **real `EdgeLoops`** (never `GetBoundingBox`), coplanar
  neighbor faces unioned first. Coordinates snapped to a fixed grid (0.1 µm) for exact, deterministic
  boolean ops; one shared `Tol` policy.
- **`BuildingEnvelope`** = faces + `CornerEdge` half-edge adjacency (dihedral → ExternalL / Internal /
  Coplanar / Parapet / Plinth / Expansion) + `LevelBand` shared datums (the coordination backbone:
  courses, splices, wind bands all reference building levels so joints align across faces).
- **`ISurface`** abstraction: Planar / Cylindrical / Conical evaluate as analytic pure structs
  (developable → unroll, panelize in developed 2D unchanged); freeform arrives pre-faceted by the
  adapter under an explicit `FacetingPolicy { MaxChordMm, MaxWarpDeg, FlatFacet|ColdBend|Reject }`.

### Pipeline — seven pure stages behind immutable DTOs
```
GeometryInput → [1] SubstrateExtraction → [2] RegionModel → [3] SubstructureLayout
→ [4] Panelization → [5] CornerOpeningResolve → [6] ThermalWindAssign → [7] Takeoff
→ LayoutResult
```
Each stage independently golden-tested and swappable. `GeometryInput` (faces/edges/levels/system
spec) in; `LayoutResult` (brackets, guide segments with splices+fixed/sliding, panel cells with
footprint rings + SKUs, trims, BOM) out. The adapter is a geometry courier + diff materializer.

### Opening-aware panelization
Grid per strategy (start modes generalized to `LayoutDatum { Anchor, BondPattern, CourseSource }` —
running bond = per-row phase; courses-from-levels hits floor lines) → polygon-clip each nominal cell
against outline+holes → classify `Full / CutByOpening / CutByEdge / Reveal / CornerFiller / Sliver`
→ **never emit a Sliver** (absorb into neighbor or convert to filler via `ResolutionPolicy` with
`minPanelFraction`) → generate reveal assemblies per hole. SKU assignment v1 = deterministic
quantize-and-count (5 mm buckets), histogram in takeoff; global optimization deferred as a swappable
stage-7 consumer.

### Substructure with wind/thermal correctness
`SubstructureSpec` carries `WindZone[]` as **declarative predicates** (CornerStrip(w) /
ParapetBand(h) / HeightAbove(z) / OpeningMargin(d)) evaluated to masks from the envelope graph —
densify inside masks, snap brackets to guide columns, subtract opening margins. Guides segmented by
max run / floor lines; **exactly one fixed bracket per segment**, rest sliding, splice gaps emitted —
an assignment solver with invariants, directly BOM-countable.

### Cladding-system-as-data — the polymorphism boundary
One engine, strategy interfaces resolved from a data-driven `SystemDefinition`:
`IPanelizationStrategy` (module authority: catalog-fixed vs made-to-order; grid = list of courses so
**linear/planken with no vertical grid is expressible** — the acid test), `ICornerStrategy`
(open-joint profile / bent-SKU-with-unfold / miter / vendor extrusion), `IAttachmentStrategy`
(klammer rows shared between courses / cassette interlock imposing install order / rivet patterns
with one fixed hole; also yields `PreferredGuideStep`). ~90% of new systems = a data row over shared
strategies. Load-bearing schema facts: module authority, joint authority (parametric vs
tooling-fixed), **attachment is relational** (entities linking panels↔guides, not per-panel flags),
cut policy (free / factory-only / refabricate) drives what the optimizer even minimizes.

### Associativity — the biggest moat
- **Stable keys relative to datums**: `PanelKey { FaceId, CourseIndex, PositionIndex, SystemId }` —
  moving a window changes keys only for touched cells; every untouched panel keeps its key and its
  Revit element.
- **`MaterializationRecord` in Extensible Storage**: domain key → `UniqueId` (never ElementId) +
  `OverrideFlags` + `InputHash` (identical input ⇒ true no-op).
- **Diff-apply in one TransactionGroup**: Added/Removed/Changed/Unchanged; unchanged elements are
  never touched; manual overrides tracked at **key granularity** (replace/keep — no freeform
  geometry 3-way merge, ever) and conflicts flagged, not clobbered.

### Revit materialization at scale (20–60k elements)
- Cut/non-rect panels → **DirectShape** (solids pre-built outside the transaction); repeated
  rectangular panels → parametric FamilyInstance; curved → adaptive components; brackets/guides →
  point-placed FamilyInstances. **Own the placement — no face-hosting** (hosting fights the diff
  engine and shatters on host edits).
- Batch everything; **zero `Regenerate` in loops** — one regen at commit; symbols activated once;
  transactions split per element class under one TransactionGroup; fasteners are **counted, never
  instanced**.
- Shared parameters (`NVF_System, NVF_PanelSku, NVF_BracketType, NVF_FixedOrSliding, NVF_WindZone…`)
  bound at startup → schedules agree with `BillOfMaterials` by construction.

### Eval harness (cross-platform, no Revit needed)
- **Golden buildings**: hand-authored `GeometryInput` JSON fixtures spanning tier × cladding;
  canonicalized snapshot approval tests.
- **Determinism gates**: run twice → byte-identical; shuffle input face order → identical.
- **Property invariants** (CsCheck): area conservation (panels+joints+openings == region), no
  overlaps, exactly one fixed point per run, every bracket on a guide, zero slivers.
- **Metamorphic**: move a window ±ε → only local keys change (bounds the associativity blast
  radius); scale face → SKU-count invariant; mirror corner → mirrored resolution.
- Windows-only smoke: materialize a golden into a doc, element counts == BOM, schedule totals == takeoff.

## 4. The 10x differentiators (ranked)

| # | Differentiator | Why make-or-break |
|---|---|---|
| **D1** | Identity-stable associative recompute | Everyone can generate; almost no one can *re*-generate into a documented model. Existential. |
| **D2** | Corner + opening intelligence (узлы, not field) | ~80% of facade labor is details — откосы, отливы, corner cassettes, parapets, fire окантовка. Where customers decide the tool is real. Existential. |
| **D3** | Parameters-as-fields (wind/height/zone densification) | Real drawings never have one bracket step; absence = un-submittable above ~10 m. Wins deals. |
| **D4** | SKU minimization + cutting stock | A number to sell: "waste 12% → 6%." Wins deals. |
| **D5** | Thermal/movement constraint solver | One violated rule and the facade oil-cans in the first winter. Relational logic scripts can't express. Wins deals. |
| **D6** | Explainable, norm-annotated output | Per-element provenance + СП/ГОСТ pass/fail audit → converts экспертиза into a sales channel. Wins institutions. |
| **D7** | Tolerance-driven curved panelization | Known math — narrow moat alone; unbeatable combined with D1+D4. Wins landmark projects. |

## 5. The benchmark — making "best in the world" measurable

Public **NVF Facade Benchmark**: 5 tiers × 8 claddings = **40 cells**, each a downloadable reference
building (RVT/IFC) + golden solution. Score published per-cell (0 = unsupported / 1 = passes with
violations / 2 = full pass) — aggregates can't hide weak cells, and gaps are visible by design so
expanding claims never requires hiding anything.

| Metric | World-class target |
|---|---|
| Norm-compliance (parameter constraints) | 100% elements in допуск (0 violations / ≥500 elements) |
| Clash-free (pie layers, ПОК, openings) | 0 clashes on reference buildings |
| Min-panel-width discipline | ≥98% panels ≥ min; remainder = flagged доборные only |
| Waste % / unique-SKU ratio | ≤5–8% waste T0–T2 (≤12% T3+); SKUs ≤15% of panel count |
| Recompute after an edit | <5 s @ 500 m² facade; <30 s full building |
| Manual-touch rate | ≤1 fix/100 m² (T0–T1), ≤3/100 m² (T2–T3) |
| Full generation @ 5,000 m² | <3 min incl. Revit materialization |
| BOM accuracy vs golden | ≥99% by count/type |

**Proof motion:** side-by-side runs — same building: this engine vs Revit-native curtain system by an
expert vs a Dynamo expert with LunchBox/PanelingTools — publishing time-to-model+BOM, manual edits,
and post-hoc norm compliance, **with the raw files downloadable**. The benchmark repo is the
category-defining artifact: first publisher defines the terms of comparison; competitors either play
by the rubric (legitimizing it) or stay unmeasured.

**The white space nobody occupies:** norm-correct ПОК + all-cladding + high geometric complexity +
auto-BOM + associativity + **operable by a normal facade engineer, not a computational-design
expert**. Revit native has geometry but no ПОК domain and no norms; Dynamo/GH has geometry but is
norm-blind, non-reusable, expert-only; vendor configurators are deep but single-catalog; RU tools are
near the right user but stop at flat facades without full ПОК/BOM. Incentives keep it empty: Autodesk
monetizes the platform, vendors monetize their hardware, consultants monetize the expertise the tool
would erase.

## 6. Conquest sequence

**Phase 1 — T1 × керамогранит (кляммер).** The volume core of the RU market and the first
honestly-winnable claim. Replace `Substrate` with `PlanarRegion` **now**; ship opening exclusion +
perimeter densification, guide splitting, panel clipping, reveal generation, joint-to-jamb snapping.
**D1 (stable keys + diff adapter) and D6 (provenance) are built in this phase — architectural
invariants, not features.** Claim on the public benchmark: *best in the world at automated
ПОК+cladding modeling for keramogranit facades with openings, norm-compliant, auto-BOM.* Narrow,
verifiable, true.

**Phase 2 — T2 + металлокассета (+ АКП).** Face graph, building frame, global datums, corner
strategies incl. bent-cassette unfold. Cassettes force the schema to be real (made-to-order module
mode, install-order interlock). Fibrocement/HPL ride along nearly free. First SKU minimization
(needs whole-building coordinates).

**Phase 3 — T3.** Zone-map fields (D3), fixed/sliding solver (D5), parapets/цоколь/деформационные
швы, fire окантовка, slab-line attachment mode. This converts users from "layout tool" to "рабочка
tool" — pricing power.

**Phase 4 — T4 + реечные/терракота.** Inclined/soffit admissibility, faceting solver (D7), 1D
cutting-stock maturity, vendor-constrained corner SKU logic.

**Deliberately deferred, said out loud:** T5 free-form; natural stone & glass (per-panel structural
liability collides with the no-verification-calc doctrine); FEM/wind *calculation* (stay
parameters-per-СП; partner); scan-to-BIM (keep bracket adjustment range as a data field so the door
stays open); generative/stochastic layout search (determinism is a feature D1 depends on — don't
trade it before the constraint model is complete).

## 7. Immediate file-level implications

| Today | Becomes |
|---|---|
| `src/RevitNvf.Core/Model/Substrate.cs` (W×H) | `PlanarRegion` + `BuildingEnvelope` + `ISurface` |
| `src/RevitNvf.Revit/Geometry/PlanarFaceSubstrate.cs` (`GetBoundingBox` — the #1 correctness debt) | Real `EdgeLoops` + hole extraction + gravity-aligned `FaceFrame`, coplanar union |
| `src/RevitNvf.Core/Model/FacadeSystem.cs` (4 scalars) | `SystemDefinition` + `SubstructureSpec` (constraints/zones as data) |
| `src/RevitNvf.Core/Layout/PanelLayout.cs` | `AxisCells` survives as the inner strip kernel; top-level API becomes strategy-driven opening-aware `Panelization` |
| `src/RevitNvf.Core/Reporting/FacadeTakeoff.cs` | Per-SKU / per-zone `BillOfMaterials` with provenance |

**Honesty clauses:** "norm-compliance" is always stated as *conformance to the norms' parametric
constraints*, never as engineering load verification. "All claddings × all complexity" is a decade
of edge cases — the benchmark matrix is published with its gaps visible, and the claim grows
cell-by-cell, never ahead of the evidence.
