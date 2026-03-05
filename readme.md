# Bank Transfer

## Context

This component is used in an internal transfer system.
 It applies fees depending on the context and delegates execution to the banking core.
 Historically, priority was given to delivery speed rather than to design robustness.

## Issues observed in production

- Negative amounts detected too late
- Anomalies when introducing new fee policies
- Inconsistent behaviors depending on the chosen implementation
- Difficulty reasoning about business invariants

## Kata objective

- Identify a violation of the Liskov Substitution Principle
- Separate fee calculation from banking execution
- Make business rules explicit
- Write tests capable of securing refactors

## Rules

- ❌ Classes marked with `DoNotModify` must not be touched
- ✅ The code must become testable
- ✅ Invariants must be clarified through tests
- ❌ No full rewrite
