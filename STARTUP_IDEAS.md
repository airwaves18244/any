# Venture Dossier — 10 Startup Ideas from `airwaves18244`'s Repos

Synthesised from three parallel model passes (Fable 5, Opus 4.8, Sonnet) over all 18
repositories. Ranked by risk-adjusted conviction. Interactive version:
https://claude.ai/code/artifact/624210c7-77d9-49ac-a84f-571a3b101f8e

## Portfolio read

| Cluster | Repos | Role |
|---|---|---|
| **BIM / Facade** | `any` (RevitNvf), `REVIT-PLUGIN` | Rarest, least-copyable asset |
| **Markets / Trading** | `Market-flow`, `Market-terminal-V3`, `Trading-strategies`, `OPTIONS-TRADING`, `Options` | Paying, tool-hungry audience |
| **LLM / Agentic** | `LOOP-PLAN`, `LOCAL-LLM-TUNING-PLAN`, `REASERCH-PLAN`, `FABLE-5`, `wo-FABLE-5` | Connective tissue |
| **Productivity / Comms** | `TG-SUMMARY-PLAN`, `Star-club`, `HEALTH`, `STARTUP_PLAN`, `PERSONAL-PLAN` | Distribution & side revenue |

The moat is at the **intersections** (BIM × agentic; order-flow × local-LLM), not in any single cluster.

---

## The ten, ranked

### 1. FasadOS — the AI facade engineer · Path A · **Conviction 5/5**
Agentic Revit copilot: raw model → fully engineered, norm-compliant НВФ package (layout, families, layer-cake, priced ведомость) in hours vs engineer-weeks.
- **For / pain:** Facade bureaus & НВФ subcontractors (RU/CIS, Gulf, C. Asia). Detailing is 2–6 engineer-weeks of manual bracket/rail/panel placement + spec counting; redone whenever a window moves.
- **Edge:** RevitNvf *is* the deterministic core — layout algorithms isolated in `netstandard2.0` from the Revit adapter, plus family + norm skills. ~50 people worldwide hold this НВФ × Revit-API combo.
- **Architecture:** LLM agent (LOOP-PLAN pattern) reads surfaces/openings → `RevitNvf.Core` computes geometry → adapter materialises families in a `Transaction` (mm↔ft at boundary) → Core emits BOM → Excel/schedule. Facade systems as **data** (JSON per СП 426), not code.
- **MVP (8–12 wk):** One system end-to-end: rect wall → bracket+guide layout at legal step → cladding tessellation w/ corner fillers → Excel BOM. Ship signed `.addin` + installer.
- **Moat:** Encoded НВФ normativity + vendor family libs + compounding system-definition catalog.
- **GTM / pricing:** Sell to **НВФ system manufacturers** (Краспан/U-kon class) — branded plugin, free-to-architect, paid-by-vendor = distribution to hundreds of bureaus. Direct: ₽40–120k/seat/yr. Reach via isicad, forum.dwg.ru, "Revit для профи".

### 2. NormaGPT — building-code answers, on-prem · Path A · **4/5**
Air-gapped Q&A + compliance over СП/ГОСТ/СНиП with forced clause citations, running on the firm's own hardware.
- **For / pain:** Design institutes, экспертиза, нормоконтроль depts. Cloud LLMs disqualified by confidentiality + citation liability — *that is the wedge*.
- **Edge:** nvf-domain skill already turns СП 426 / ГОСТ Р 58154 prose into machine-checkable constraints; REASERCH-PLAN + LOCAL-LLM-TUNING-PLAN supply retrieval + on-prem serving.
- **Architecture:** Clause-hierarchy chunker → local embeddings → Qdrant → RAG over local 7–14B with **cite-or-abstain** (must quote retrieved span; never free-generate clause numbers). Optional LoRA. Docker Compose bundle.
- **MVP:** Facade + thermal norms only (the corpus you own). RAG-only, eval on held-out clause-lookup benchmark.
- **Moat / GTM:** Verified clause-structured corpus + credibly-offline deploy. Add-on to FasadOS accounts, ₽150–500k/yr per-project.

### 3. FlowNarrator — local-LLM order-flow analyst · Path B · **4/5**
Native Rust terminal where a locally fine-tuned model narrates microstructure in plain language; Telegram bot pushes regime alerts. Zero data leaves the machine.
- **For / pain:** Order-flow traders (Bookmap/ATAS crowd; MOEX scalpers). Pathologically privacy-sensitive → cloud AI structurally out, local is the only form.
- **Edge:** `Market-flow` (Rust viz) substrate + `LOCAL-LLM-TUNING-PLAN` (the missing layer) + `Market-terminal-V3` plumbing + `TG-SUMMARY-PLAN` delivery.
- **Architecture:** Rust ingests L2/tape → deterministic feature stream (delta/imbalance/absorption) → 7–8B fine-tuned on feature→commentary pairs, on-device → viz + narration side-by-side (wgpu).
- **MVP:** Ship the Telegram alert bot first (2–4 wk, immediate revenue), then terminal + AI narrator as paid tier.
- **Moat / GTM:** Labeled-microstructure dataset nobody has + local-first vs all cloud rivals. Free terminal, narrator $40–80/mo. Launch RU trading Telegram → crypto.

### 4. ModelAudit — automated нормоконтроль for Revit · Path A · **4/5**
Plugin audits a BIM model vs domain rules + norms; report explains *why* each flag matters with code ref + click-back to element.
- **Edge:** Reuses `RevitNvf.Core` + same system-definition JSON as FasadOS. QA follows layout — second product in one suite.
- **Architecture:** Deterministic rule engine consumes model DTOs (FilteredElementCollector); rules as data; LLM **only narrates** findings (never decides pass/fail) — kills false-positive fatigue.
- **MVP:** 10–15 НВФ rules (fastener-step, bracket→guide connectivity, corner completeness, schedule vs model count) + report.
- **GTM:** Cross-sell into FasadOS budget, ₽60–150k/yr.

### 5. SignalSieve — Telegram alpha, scored · Path B · **3.5/5**
Ingest RU trading-Telegram, extract structured signals, retroactively score every channel's real track record vs market data.
- **Edge:** `TG-SUMMARY-PLAN` + `Market-terminal-V3` + trader-slang NLP (a real barrier to outsiders).
- **Architecture:** MTProto ingest → LLM extract typed signals → **deterministic OHLC verification join** → per-channel hit-rate/R-multiple/drawdown → leaderboard + digest bot.
- **MVP (4–6 wk):** Free monthly "Топ-50 сигнальных каналов" leaderboard — viral + adversarial = distribution.
- **Moat / GTM:** Longitudinal accuracy DB (can't fake the past). Personal bot ₽500–900/mo. *Watch Telegram ToS.*

### 6. FamilyForge — catalog → Revit families · Path A · **3/5**
Manufacturer uploads a catalog → agent pipeline emits production-quality parametric families with correct shared params.
- **For / pain:** Building-product makers pay agencies $50–200/family and wait months; most RU/CIS makers have none.
- **Edge:** revit-family-generation skill encodes the conventions that make families *usable*; RevitNvf placement code is the test harness.
- **Architecture:** Catalog → typed schema → **template-family** approach (API sets params + type catalogs, not geometry — cuts hardest 80%) → headless-Revit QA validates schedules.
- **GTM:** Bundle with vendor deals; each catalog is a channel back into FasadOS. ₽100–300k/catalog.

### 7. TenderPilot — from BIM spec to смета · Path A · **3/5**
One click from facade model to смета + procurement BOM + tender docs.
- **Edge:** FasadOS output *is* the input; you know what quantities mean physically (pure-LLM startups get this wrong).
- **Architecture:** Deterministic takeoff → element→расценка mapping (human-confirmable) → ГрандСмета-XML + narrative agents.
- **Reality:** расценки world is gnarly, сметчики conservative — **year-2 expansion**, not a start.

### 8. VolDesk — options vol & positioning · Path B · **3/5**
IV surfaces, greeks aggregation, gamma exposure, scenario P&L for a trader's book — with the **MOEX coverage Western tools lack**.
- **Edge:** `OPTIONS-TRADING` + `Options` solver/models + Market-terminal plumbing.
- **Architecture:** Chain ingest → IV solver (Newton/Brent on BS/Bjerksund-Stensland) → surface fit (SVI/SABR) → greeks + exposure → dashboard; positions via broker API/CSV.
- **Moat:** Thin (quant correctness commoditises) — competes on cheaper/cleaner + RU coverage. ₽2–5k/mo. Portfolio hedge.

### 9. RepoDiligence — local codebase due-diligence · Cross · **3/5**
Point at a private/target codebase → structured tech-diligence report (architecture, debt hotspots, deps/license, bus-factor) + FABLE opportunity mining. Runs locally — code never leaves.
- **For / pain:** Acquirers, micro-PE, CTOs vetting shops. CodeScene/Sourcegraph priced for enterprise, not the one-off M&A moment.
- **Edge:** `FABLE-5`/`wo-FABLE-5` pipeline + `LOOP-PLAN` harness + local serving.
- **Architecture:** Repo crawl → AST/dep graph (tree-sitter) → retrieval → local LLM multi-step loop w/ tool calls → **evidence-grounded** report (every claim cites file:line, deterministic re-check).
- **GTM:** $500–5k/audit or agency sub; MicroAcquire/Flippa/Indie Hackers distribution.

### 10. QuantLoop — agentic strategy research (open-core) · Path B · **3/5**
Self-driving loop: agents generate hypotheses, backtest, mutate survivors, return only statistically honest candidates. **Anti-overfitting guardrails are the product.**
- **Edge:** `Trading-strategies` + `OPTIONS-TRADING` corpus + `LOOP-PLAN` template + `REASERCH-PLAN`. You're the target user.
- **Architecture:** Orchestrator runs generate→code→backtest→critique; backtester enforces walk-forward / deflated-Sharpe / regime-split the agent **cannot skip**; lineage DB audits true out-of-sample record.
- **Moat / GTM:** Weak-medium — best as an **open-core reputation engine** feeding #3 and #5. "30-day audited agent log" as marketing.

---

## Two companies, not nine features

**Path A — FasadOS** (vertical AI-BIM): `FasadOS → NormaGPT → ModelAudit → FamilyForge → TenderPilot`. One roadmap; the deterministic RevitNvf core is the moat, LLM agents are the interface; the domain corpus deepens each project. **Wins on defensibility.**

**Path B — Flow Studio** (local-first trading tools): `FlowNarrator → SignalSieve → VolDesk → QuantLoop`. Monetise the RU/CIS trader community first. **Wins on speed to revenue**; lower moat, so lead with local-LLM privacy + RU-native coverage.

### Bottom line
All three models independently ranked **FasadOS #1** — the codebase already exists (~60%), the buyer/pain is quantified in engineer-weeks, and the moat (НВФ domain × Revit API) is a barrier almost no generic dev clears. Build **Path A** as the company; run **SignalSieve** as the fast, self-marketing cash bet alongside. Don't ship all ten — the portfolio's whole story is that intersections beat breadth.
