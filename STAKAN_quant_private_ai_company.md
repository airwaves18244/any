# STAKAN — The On-Device Quant Analyst (Path B, hardened)

Path B grilled through a 4-model adversarial pass (Fable 5 vision · Opus red-team · Opus
architecture · Sonnet GTM). Interactive dossier:
https://claude.ai/code/artifact/609751d0-26cf-4aa7-8379-9001c4c3f1c4

## Grill verdict
As originally drawn — four products, retail beachhead, "AI narration" as the hook, "private" as
the thesis — this is a **$50–150k/yr lifestyle business that decays** as its customers blow up their
accounts. Three pivots make it real: buyer = **survivors + prop desks**, not fresh gamblers; AI = a
**grounded detector that physically can't invent a price**, not a storyteller; sell an **instrument,
never alpha**. Local-first is a moat for the always-on private layer — a trap the moment it's the
whole identity. The **deterministic Rust engine is the product**; the LLM is a grounded voice on top.

## Five reframes the grill forced
| Was | Now |
|---|---|
| An AI order-flow terminal (fights ATAS/Bookmap/Quantower) | **The Sentinel — a companion, not a terminal**; sits beside the terminal you already use |
| The LLM narrates the tape (confident-BS generator) | **Deterministic Rust detectors own every number**; the LLM only verbalizes confirmed events (0% invented levels) |
| Sell to retail scalpers (self-liquidating) | **Survivors + prop desks / small funds** — $300–1,500/seat, an expense line that doesn't rage-quit |
| "Private" is the thesis (a GPU-budget rationalization) | **Sell speed & zero marginal inference cost**; privacy is the closer. Always-on tape-watching is only viable local |
| Four products at once | **One Sentinel.** VolDesk = module #2 · SignalSieve = Phase-3 weapon · QuantLoop = internal tool |

## The three killers → neutralized
1. **Is the AI alpha or a gimmick? (HIGH).** An LLM narrating microstructure is a confident-BS generator; flow traders mock it ("I can read my own footprint"); confident-wrong narration manufactures conviction and costs money. → **LLM is a translation shell, never the engine.** Rust detectors compute every event; grammar-constrained decoding means it **physically cannot type a price the engine didn't emit** (0% numeric hallucination — a published eval gate). Detectors alone justify the price. Prove the edge in a spreadsheet in month 1.
2. **Market size × WTP (HIGH).** RU/CIS serious order-flow traders ≈ 1,000–5,000 ever; retail derivatives traders self-liquidate (median account life = months); $50/mo × ~5-mo retention = $250 LTV; ceiling ~$600k/yr and decaying. → **Reprice for prop firms / funds / serious full-timers** ($300–1,500/seat, annual, low churn). Churn is the #1 KPI — journaling, community, pause-not-cancel push 15%→9% (2× LTV). Prove 5 desk pilots in 90 days.
3. **The honest-trader paradox (HIGH).** "If it gave edge you'd trade it, not sell a $50 sub" pre-filters your best customers and sorts your base toward the naive/high-churn/scam-susceptible cohort. → **Sell an instrument, measurement, time saved — never alpha.** Trade your own calls publicly with an audited record. **SignalSieve scores everyone else's track record**, immune to the paradox and a viral funnel.

## Hardened thesis
- **Category:** the on-device quant analyst — AI that reads markets runs entirely on your hardware, watches continuously, trained on data no cloud company can collect. Internal name **Часовой** (the sentinel).
- **10× insight:** always-on tick inference is **economically impossible in the cloud** (thousands of $/user/mo, forever) but a fixed cost locally — cloud AI is *structurally priced out* of "a machine that watches the tape all day," not just trust-disqualified. And a fine-tuned 8B beats a frontier model at narrating a pre-computed feature struct.
- **Wedge:** the Sentinel watches raw feed locally, detects absorption/divergence/stop-runs/iceberg with deterministic Rust, pushes one terse Telegram line; you decide on your own chart. ~80% built from Market-flow + Market-terminal-V3 + TG-SUMMARY-PLAN + LOCAL-LLM-TUNING-PLAN. 90-day scope: MOEX futures + crypto perps, 5 detectors, one fine-tuned model.
- **Local = moat or trap?** Rule: **continuous inference on the trader's own context → local, always** (economics + trust); **episodic analysis of public data → any compute** (SignalSieve runs in the cloud, proudly). Sell speed + zero-marginal-cost; privacy is a free closer for the top decile (80% of revenue).
- **Vision:** a private quant department on every serious trader's machine, compounding into one fine-tuned model family that reads markets better than any generalist AI. The privacy architecture *is* the data-collection license.

## Architecture — deterministic eyes, local voice
Hard boundary enforced by constrained decoding, not prompting: the LLM never sees raw tape and never emits a number the engine didn't compute.
- **Feature engine + regime FSM (Rust, deterministic):** trade classification, delta/CVD, footprint ladder, imbalance, absorption/iceberg/exhaustion as typed events; a rule-based FSM (not the LLM) declares the regime and owns every money-relevant claim. Runs at full tick speed; never waits on the model.
- **Local narrator (llama.cpp, Qwen2.5-7B fine-tuned):** turns a typed `NarrationContext` into one terse sentence. **Event-triggered + cadenced, never tick-by-tick** (~1–2s — the honest answer). Three enforcement layers: grounded input contract · GBNF grammar whitelisting only the struct's numbers · post-gen validator dropping unsupported claims. "No evidence, no sentence."
- **The label ledger (moat):** one-tap 👍/👎/"traded it" = a profit-motivated trader labeling a microstructure event. Two-layer split — private (positions/reads) never leaves; shareable (public-feed fingerprint + verdict) opt-in, off by default. Quarterly fine-tune ships to every machine.
- **Feeds/persistence/privacy:** one `FeedAdapter` trait, crypto WS first (free/unlicensed), MOEX later; Parquet/DuckDB store with live & replay sharing one code path; **zero telemetry, published egress allowlist, in-app network panel** — verifiable with a firewall.
- **Eval gate (blocks every release):** numeric_grounding = 100% (grammar makes invented levels impossible), claim_grounding > 98%. Marketable fact: "the AI physically cannot show you a level our engine didn't compute."
- **GPU reality:** the terminal needs no GPU (deterministic, CPU); the narrator degrades across a tier ladder (14B/8B GPU → 3B/CPU on-demand → consent-based cloud of features only). Stack: Rust/tokio, egui/wgpu UI, llama.cpp GGUF Q4_K_M, unsloth QLoRA, teloxide, Ed25519 offline licensing, billing out of process.

## Compounding moat
Deterministic events → one-tap verdicts (a labeling workforce nobody can hire) → two-layer data split (trust compounds *with* the dataset) → quarterly fine-tune to every machine. Open weights commoditize; the ledger doesn't. The benchmark video ("our 8B vs a frontier model on live tape") is the marketing dept.

## GTM
Beachhead = RU/CIS order-flow futures scalpers who are **ATAS/Bookmap refugees** (survivors, replace-this-tool sale, proven WTP). Expansion = prop desks / small funds. Never lead with crypto — open it as the parallel English wedge once RU retention is proven. First 10: free lifetime Pro to educators + co-branded videos, post as the builder. Scale: educator affiliate rev-share + the SignalSieve leaderboard as viral top-of-funnel. Pricing: free terminal (distribution) · 990₽/mo Telegram micro-sub · **4,900₽/mo Pro** (core) · $170–450/seat prop B2B. Churn 15%→9% doubles LTV ($237→$395). Rails: YooKassa + USDT (RU), Stripe/crypto (global). Path: $10k MRR ≈ 280 subs (mo 5–7); $50k MRR ≈ 1,400 subs + 2–4 desk deals (mo 14–20).

## 18-month arc
Wk1–6 Telegram alert bot, no LLM (build the replay harness first, validate the engine on a week of captured data before any alert) → M2–6 local narrator + desktop Sentinel + fine-tune pipeline + grounding eval → M5–9 VolDesk module + first benchmark + first desk deal → M7–12 SignalSieve public leaderboard (safe now, cloud, public-data only) + DPO flywheel → M13–18 prop-desk tier (audited build, air-gapped) + English/crypto turn.

## Standing no-list
No charting-terminal war · no execution/auto-trading · no signals (observations only) · no cloud-SaaS Sentinel · no "zero data ever leaves" as absolute copy (say "your data never leaves; your verdicts, if you choose, teach the model everyone gets") · no four products at once.

## Three things that must be true
1. The core signal has a measurable, backtested forward edge — provable in a spreadsheet in month 1, independent of any LLM.
2. A non-self-liquidating buyer exists and pays $300+/mo — prove it with 5 signed pilots before product #2.
3. Ship exactly one product; obsess over churn, not signups.

**Positioning:** For serious futures & crypto traders whose edge is their read of order flow, Stakan Sentinel is an on-device AI analyst that watches every tick and alerts you the moment structure changes — runs entirely on your machine, and doesn't need your eyes to work.
