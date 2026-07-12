# KLAMMER Engine — Best-in-World Automated НВФ Modeling (super-grill)

Goal: automatically model ventilated facades in Revit across **every cladding type** and **every level of
architectural complexity**. Grilled through 5 model passes (capability ladder, geometry red-team, engine
architecture, competitive benchmark, and the 1-click intake spec), grounded in the live `RevitNvf.Core`.
Interactive dossier: https://claude.ai/code/artifact/f61c2ec4-25e1-4afa-a24a-637fffa4a8d6

## Verdict
The current engine solves the **one case that never occurs in practice** — a blank rectangular wall with no
openings, no corners, no neighbours. Its data model (`Substrate = Width×Height`, layout = Cartesian product)
actively **prevents** the extensions that matter. Reaching "best in the world" is **not a refactor; it's a
restart of the Core's input contracts** — keeping only the pure-Core/thin-adapter split and the deterministic
discipline. "All claddings × all complexity" is a decade of edge cases; the honest first claim is **hidden/
visible-klammer keramogranit on planar orthogonal facades with real openings + corners**, with norm-validation,
diff-based recompute, and auto-BOM. Narrow, hard, valuable, and nobody else does it.

## Current state (Tier 0)
- `Substrate(Width,Height)` from a `PlanarFace` UV bounding box — ignores the real outline **and** all holes.
- `BracketLayout` uniform grid; `GuideLayout` one vertical per column; `PanelLayout` rect grid + joint + start mode + optional edge filler.
- `FacadeSystem` = bracket steps only; `Cladding` = panel W×H + joint + start mode.
- No openings, corners, curves, inclination, multi-face, thermal, wind-zoning, cladding-type specifics.

## The complexity ladder
- **T0 Flat rectangle (done).** Clean, deterministic — right foundation, wrong abstraction to extend.
- **T1 True boundary + openings (first real tier).** Polygon-with-holes; bracket exclusion zones (~100mm off openings) + perimeter densification at jambs/lintels; guides split above/below openings; panels clipped; auto reveals (откос/отлив/наличник) with depth from the layer stackup; joints snap to jambs. *Where every Dynamo script dies.*
- **T2 Multi-face + corners.** Face-adjacency graph, dihedral classification, course continuity → a **global building datum** (per-face layout is wrong past here); corner elements as SKUs (bent L-cassette unfold, corner profiles); corner-zone bracket doubling.
- **T3 Full-building engineering.** Parapet caps/цоколь/deformation joints; thermal fixed/sliding runs (one fixed per ≤3–6m, splice gaps); wind densification — bracket step becomes a **field, not a scalar** (corner strips ≈ b/10, parapet & height bands); fire окантовка; weak-substrate → slab-line attachment. *Converts a picture into рабочая документация — pricing power lives here.*
- **T4 Inclined/faceted/cylindrical/soffit.** `ISurface` abstraction; developable surfaces unroll & panelize like flat; freeform facets to chord tolerance (δ=c²/8R); refuse uncertified overhead. Different program.
- **T5 Free-form/double-curved.** Deferred; different product generation.

## Cladding matrix — why "one grid fits all" is a lie
Families must be **data rows, not code branches**, but four axes fracture the abstraction and the schema must express all four:
1. **Module authority:** catalog-fixed (keramogranit, terracotta) vs made-to-order (cassette, ACP) → solver runs in two modes (fit fixed modules vs derive from wall).
2. **Joint authority:** parametric (keramogranit, FC) vs tooling-fixed (cassette folds, terracotta profiles).
3. **Attachment is relational:** рядовой klammer belongs to two courses; interlocking cassettes impose install order; FC imposes hole patterns with one fixed point. Output needs attachment entities linking panels↔guides.
4. **Corner strategy enum** (open-joint+profile / mitre / bent-SKU-unfold / vendor extrusion) + **cut policy** (free / factory-only / refabricate) which changes what the optimizer minimizes.

**Acid test:** linear/planken has **no vertical grid**. If the panelization strategy expresses "courses only" without hacking the rectangular panelizer, the abstraction is right — so the grid must be a list of independent courses (1D or 2D cells).

## Eight ways it breaks (red-team, ranked)
1. **Substrate lie (HIGH).** A UV bbox invents area over gables, ignores openings, origin isn't on a non-rect face, UV basis isn't guaranteed horizontal → tilted/mirrored grids; "the wall" is often many segments/masses/curtain/links. → hole-aware polygon-with-holes in a **world-anchored frame (gravity-derived, not face UV)** from real EdgeLoops. Fixing `PlanarFaceSubstrate.GetBoundingBox()` is the **#1 correctness debt**.
2. **Openings & corners (HIGH).** Hole vs regular grid is incompatible; corners couple faces (courses must align turning the corner) → boolean-subtract openings with clearance + cell classifier with min-panel rule (never emit slivers); building **graph** with shared corner edges; courses on a global datum.
3. **Associativity (HIGH).** Regenerate-from-scratch changes every ElementId → tags/dims/overrides/schedules detach; one window move = full re-issue. → **diff-and-reconcile** with stable keys (face+course+position), key→UniqueId in Extensible Storage, delta-only apply, override preservation at key granularity (never freeform geometry merge).
4. **Correctness trap (MED, CRITICAL).** Plausible-but-wrong: anchors on mortar joints, unmanaged thermal, 20mm slivers, uniform fastening under wind suction. → a **validation layer that fails loudly** (min panel width, max thermal run, wind-zone density, anchor-into-valid-material) from СП 426/ГОСТ 58154-55 as constraints.
Plus: Revit scale (20–60k elements → DirectShape + batching), curved/inclined (scope out of v1), cladding combinatorics (shared kernel + per-family strategies, not flags).

## Engine architecture — LLM proposes, Core disposes
**Restart the substrate:** `Substrate(W,H)` → `PlanarRegion(Outer ring + Holes + FaceFrame)` → `BuildingEnvelope(Faces + CornerEdges + LevelBands)` → `ISurface` (curved/faceted). World-anchored frame from gravity + face normal, not face UV.

**Pure Core pipeline (7 stages, immutable DTOs):** SubstrateExtraction → RegionModel → SubstructureLayout → Panelization → CornerOpeningResolve → ThermalWindAssign → Takeoff. DTO contract: `GeometryInput` in, `LayoutResult` out — geometry-in, layout-out, no Revit types.

**Cladding-as-data:** one `SystemDefinition` selects `IPanelizationStrategy` / `ICornerStrategy` / `IAttachmentStrategy`; 90% of new systems ship as a data row. **Materialization:** DirectShape for cut panels, FamilyInstance for repeated panels/brackets/guides, Adaptive Components on curves; **point-placed not face-hosted** (hosting fights the diff engine); one TransactionGroup, batch creates, single regen, activate symbols once, shared parameters → schedule agrees with BOM by construction.

**Eval:** golden-building fixtures (hand-authored `GeometryInput` JSON + canonical expected snapshot), determinism gate (run twice → byte-identical; shuffle face order → identical), property invariants (area conservation, no overlaps, one fixed point per run, no slivers), metamorphic tests (move window ±ε → only local keys change). Provable on Linux CI before Revit opens; RTF Windows smoke only for the two shims.

## The 10× differentiators
- **D1 Identity-stable recompute (existential).** Stable keys + diff-apply preserve IDs/tags/overrides. Everyone can generate; almost no one can re-generate. Design into Core output contracts **now** — can't retrofit.
- **D2 Corner + opening узлы (existential).** ~80% of facade-engineering labour is details (откосы, отливы, corner cassettes, parapet caps, fire окантовка). Emitting correct cladding-specific detail elements replaces the expensive human work.
- **D3 Wind/zone parameter-fields (wins deals).** Every scalar becomes a piecewise field over the facade; without it, output is un-submittable above ~10m.
- **D4 SKU minimization + cut optimization (wins deals).** Quantize cut panels, nest into stock; report waste % and SKU count — a number to sell ("12%→6%").
- **D5 Thermal/movement solver (wins deals).** Fixed/sliding assignment, splice gaps, deformation breaks; invariants reviewers respect.
- **D6 Norm-annotated provenance (wins institutions).** Every element carries rule + constraint + clause + pass/fail vs СП 426/ГОСТ 58154 → экспертиза becomes the sales channel.
- **D7 Tolerance-driven curved panelization (wins landmarks).** Facet width from radius + joint tolerance; with D1+D4 on a curved tower, unmatched.

D1, D2, D6 are the durable moats (deep model architecture, not copyable algorithms).

## The "1 click" promise, made honest
**1 click = facade or diagnosis. No silent decisions.** Formula: 1 intent + K confirmations + 0 surprises, K measured & shown (target ≤5/facade, 1 mandatory = the layout datum). The first click always yields a completed artifact — a full draft facade + BOM + паспорт, OR an audit with площади, per-m² estimate, and a concrete fix recipe. Inside a compiler, outside a review (pins in 3D closed by a click).

**Intake agent — LLM translates, Core measures.** Hard rule: the LLM never emits a coordinate, dimension, or count. It classifies ambiguity, groups scope, prioritizes questions, and phrases in the engineer's language. Confidence = **provenance tier** (green auto / yellow assume / red refuse), not LLM self-rating. Question budget harness-forced; over-budget → yellow assumption logged in the паспорт, never silent.

**Dirty-model failure catalog (each = detect + repair + question/refuse + a regression fixture):** F1 split walls (auto-merge), F2 linked model (read-only, place in host), F3 void-openings (batched confirm), F4 non-vertical (≤0.5° project / 0.5–3° ask / >3° refuse), F5 sweeps/rusts (absorb or ask), F6 stacked walls (band split), F7 curtain-as-substrate (refuse + junction node), F8 mass/DirectShape (refuse → audit path), F9 mirrored families (truth = the wall cut), F10 no level datum (the one un-removable question). A catalog entry without a `.rvt` fixture counts as unimplemented.

**Trust architecture:** provenance on every element (ruleId + version + input face + params + normRef + assumption ancestors + input hash); паспорт расчёта byte-reproducible ("пересчитайте — получите то же"); verify 20k elements in 15 min via 4 lenses (assumptions list, violations panel ≤5 groups, statistical fingerprint histograms, guided sample walk). Never touch the architect's elements — an invariant, not a setting.

## Proving "best in the world" — the benchmark
Public, reproducible **40-cell matrix (5 tiers × 8 claddings)**, each a downloadable reference building + expected ranges (an MLPerf/SWE-bench for facade modeling). Scored per-cell, never one blended number. World-class targets: coverage 100% in the claimed slice; norm-compliance 100% (0 violations); 0 clashes; ≥98% panels ≥ min-width; waste ≤5–8% (T0–2)/≤12% (T3), unique-SKU ≤15%; recompute <5s (500m²)/<30s (full building); manual-touch ≤1/100m² (T0–1)/≤3 (T2–3); building-scale generate <3min (~5000m²); BOM accuracy ≥99%. **Proof motion:** same building run three ways — KLAMMER vs Revit-native (expert) vs Dynamo expert — raw files published; the benchmark is the marketing.
*Assumption: target numbers are engineering orientations to calibrate on the first real benchmark run, not measurements of the current Core.*

## Sequencing to a truthful #1
- **Phase 1 — T1 openings × keramogranit (visible klammer) + metal cassette.** The two families span both module modes and both attachment modes → forces the SystemDefinition schema to be real. Restart Substrate now. Build D1 (identity+diff) here — an architectural invariant. Start D6 provenance. **First claim:** "best-in-world НВФ engine for keramogranit on facades with openings — norm-checked, diff-recompute, auto-BOM."
- **Phase 2 — T2 corners + fibre-cement/HPL.** Face graph, global datums, corner strategies; FC/HPL ride nearly free. First D4. Most direct RU-tool competition — worth the most to win.
- **Phase 3 — T3 full-building.** Zone-fields (D3), thermal solver (D5), parapets/цоколь/deformation/fire, slab-line mode. Converts "layout tool" → "рабочка tool."
- **Phase 4 — T4 + linear/terracotta.** Faceting (D7), 1D cutting-stock.

## The v1 no-list
All claddings × all complexity · curved/inclined >3°/masses (→ red zone + audit) · face-hosted placement · structural/wind/thermal *verification* calc (norms as constraints only; boundary on page 1 of the паспорт) · auto-selecting the facade system (pre-fill, engineer presses the button) · generative/stochastic layout (determinism is a feature D1 depends on).

## The three hardest problems that MUST be solved
1. **Hole-aware, world-oriented, multi-face substrate extraction** from messy real Revit models — everything is downstream, and it hasn't been started.
2. **Editable associativity via diff-and-reconcile** preserving IDs, tags, overrides, schedules — the most-skipped requirement in the industry; it's what makes it a modeler, not a generator.
3. **Engineering-correctness validation** (thermal runs, wind density, min-panel, anchor-into-valid-material) that makes wrong output fail loudly instead of rendering plausibly.

**Files this evolves:** `src/RevitNvf.Core/Model/Substrate.cs` (→ PlanarRegion/BuildingEnvelope), `src/RevitNvf.Core/Model/FacadeSystem.cs` (→ SystemDefinition + SubstructureSpec), `src/RevitNvf.Core/Layout/PanelLayout.cs` (→ opening-aware Panelization + strategies; AxisCells survives as the inner strip kernel), `src/RevitNvf.Revit/Geometry/PlanarFaceSubstrate.cs` (→ real edge-loop/hole extraction, the #1 debt), `src/RevitNvf.Core/Reporting/FacadeTakeoff.cs` (→ per-SKU/per-zone BillOfMaterials).
