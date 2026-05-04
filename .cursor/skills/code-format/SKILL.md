---
name: code-format
description: >-
  Formats C# type bodies in the skill’s project root: adds XML /// documentation where missing, wraps members in
  ordered #region blocks (Fields, Events, Ctors, Utils, Methods, Properties, Operators, Nested), orders properties in
  a single Properties region using tier sequence (scalar → computed → navigation → collections) with optional blank
  lines between non-empty tiers (no nested #region under Properties), places explicit interface implementations in
  trailing #region {Interface} Members blocks, and sorts members alphabetically within each tier and other groupings.
  Use when the user asks for code-format, regions, member ordering, or XML summaries within that subfolder only
  (not sibling monorepo projects).
---

# Code format (XML docs + regions + ordering)

## Project root boundary (mandatory)

Same rule as before: **only** edit files under **`<ProjectRoot>`**, where this file is `.../<ProjectRoot>/.cursor/skills/code-format/SKILL.md`. Do not scan or change files outside `ProjectRoot`.

Excluded under `ProjectRoot`: `bin/`, `obj/`, generated files, `*.Designer.cs`, third-party verbatim copies.

## Goals (in order)

1. **`///` XML documentation**: `<summary>` and related tags for members missing or empty docs (see rules below).
2. **`#region` grouping**: Use the region names and order below; omit regions that would be empty.
3. **Alphabetical order**: Within each standard region bucket (except **Properties** tier handling below) and **within each Properties tier**, sort members **A–Z** by **member name** (case-insensitive, invariant culture), unless the user specifies a different collation. For **Properties**, **tier order** (below) defines block sequence; **do not** use nested `#region` / `#endregion` under **Properties**; sort **within** each non-empty tier only.

## Region blocks (syntax)

Use this shape (blank line after `#region` is optional; match the project’s existing style if already present):

```csharp
#region Methods

public override string ToString() => "";

#endregion Methods
```

- **Standard** region names: **exact** `Fields`, `Events`, `Ctors`, `Utils`, `Methods`, `Properties`, `Operators`, `Nested` (Pascal case after `#region` / `#endregion`).
- **Standard region order** (top to bottom): **Fields → Events → Ctors → Utils → Methods → Properties → Operators → Nested**.
- Include **only** standard regions that contain at least one member after classification.
- **After** all standard regions that apply, append **explicit interface implementation** regions (see next section). Nothing follows them except the type’s closing `}`.

## Explicit interface implementation regions (mandatory when present)

Use this pattern for members declared as **`ReturnType IInterface<...>.MemberName`** (methods), **`T IInterface<...>.Property`**, or explicit **`event`** implementations:

```csharp
#region IRequestHandler Members

Task<Unit> IRequestHandler<TCommand, Unit>.HandleAsync(TCommand command, CancellationToken cancellationToken)
{
    return HandleAsync(command, cancellationToken);
}

#endregion IRequestHandler Members
```

Rules:

1. **Do not** place these members in **Utils**, **Methods**, or **Properties**; they live only in their **interface** region.
2. **Region label**: `#region {InterfaceName} Members` / `#endregion {InterfaceName} Members`, where **`InterfaceName`** is the **simple** interface type name **without** the generic arity list (e.g. `IRequestHandler<TCommand, Unit>` → **`IRequestHandler`**; `System.IDisposable` → **`IDisposable`**).
3. **Position**: **last** in the type body—after **Nested** (or after whichever is the last **non-empty** standard region if **Nested** is omitted).
4. **Several interfaces**: use **one region per distinct `InterfaceName`**. Order those regions **alphabetically** by `InterfaceName` (e.g. `IDisposable` before `IRequestHandler`).
5. **Several members** for the same interface: keep them in **one** region; sort **alphabetically** by member name inside the region.

## Member classification

| Region | Contents |
|--------|----------|
| **Fields** | `private` instance/static **fields** and **`const`** (including `private const`). |
| **Events** | **Event**-like surface: `event` declarations; **`public`** delegates / multicast types that are clearly intended as event callbacks (`Action`/`Func`/`EventHandler` fields used as events, custom `delegate` types when `public`). If unsure, prefer **`event`** keyword here. |
| **Ctors** | **Constructors** only (instance and `static` `.cctor`). **Finalizers** (`~Type()`) are not constructors → **Utils**. |
| **Utils** | Members that are **not** `public` and are **not** explicit interface implementations: `private` / `internal` / `protected` / `protected internal` **methods**. |
| **Methods** | **`public` methods** (including `public override`, `public virtual`, **public** interface implementations). |
| **Properties** | **Properties** and **indexers** (`this[...]`) that are **not** explicit interface implementations — see **[Properties tier order](#properties-tier-order-single-region-no-sub-regions)**. |
| **Operators** | **User-defined operators** (`operator +`, `implicit`/`explicit` conversions). |
| **Nested** | **Nested** `class`, `struct`, `record`, `interface`, `enum` declarations. |

### Everything else

- **`public` / `protected` / `internal` `const` or `static readonly` fields** (not `private`): place in **Properties** if they read like API constants; otherwise **Methods** only when they are clearly ancillary to behavior—prefer **Properties** for stable named values exposed to callers.
- **Static local / file-level** members: normal C# has no file-level in classic classes; **file-scoped types** are out of scope for inner regions.
- **Records** with primary constructor: **do not** tear apart the primary constructor line from the type declaration; apply regions only to **additional** members in the type body.
- **Interfaces**: regions optional; if used, only regions that exist and **no** forced **Fields**/**Ctors** unless applicable.

## Properties tier order (single region, no sub-regions)

When a type declares one or more **Properties** region members (not explicit interface implementations), use **one** `#region Properties` … `#endregion Properties` block only. **Do not** add nested `#region` / `#endregion` inside **Properties** (no `#region Scalar properties`, etc.).

**Tier sequence** (top to bottom): emit members in this order, **skipping empty tiers**. Between **two adjacent non-empty tiers**, insert **one blank line** for readability.

| Tier (conceptual order only — no `#region` name) | Include |
|--------------------------------------------------|---------|
| **Scalar** | **CLR primitives**, **`string`** / **`string?`**, **`decimal`**, **enums**, date/time APIs (`DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`), **`Guid`**, **`TimeSpan`**, **nullable** versions of those, **foreign-key scalars** (e.g. `int WorkerId`), **`byte[]`**, arrays whose elements are primitives or enums (e.g. `float[]?`), immutable **embedded value components** reused as fields (types under `*.ValueTypes` such as **`Money`**, **`Contact`**, **`Address`** when used like a scalar column/component). |
| **Computed** | Read-only APIs whose **backing is not** a simple backing field surfaced 1:1 — typically **`=>` expression-bodied `get`** (e.g. `public bool IsCurrent => EndDate is null`), or getters that derive from other in-memory state. **Do not** put plain `{ get; private set; }` auto-properties here unless they shadow another member in an unusual way. |
| **Navigation** | Single references to another **persisted aggregate/entity/root** exposed as a property (usual EF pattern: **`public virtual`** navigation to **`EntityBase`/`DeletableEntityBase` descendants**, `JobCategory? Parent`, …). Exclude `string` FK-only ids (those stay in **Scalar**). |
| **Collection** | `IEnumerable` / `IReadOnlyCollection` / `IReadOnlyList` / `ICollection` / `List<>` surfaces, or anything clearly representing **many** related entities/read models tied to one parent — including **backing-field projected** `.AsReadOnly()` lists. **Do not** place primitive/value arrays here (those stay **Scalar**). Indexers (`this[…]`) belong in **Collection** if they expose a sequence/map; otherwise **Scalar**. |

Sorting: **within each tier**, alphabetical by property **name** (case-insensitive, invariant culture). **Across** tiers, the sequence **Scalar → Computed → Navigation → Collection** always wins.

DTO / command types with **only scalar-like** members: one contiguous block, **A–Z** by name, no extra blank lines unless the user’s file already uses them consistently.

```csharp
#region Properties

/// <summary>
/// Full description shown to applicants.
/// </summary>
public string Description { get; set; }

/// <summary>
/// Number of positions requested for the shift.
/// </summary>
public int HeadCount { get; set; }

/// <summary>
/// Identifier of the job posting to update.
/// </summary>
public int JobPostingId { get; set; }

/// <summary>
/// Calendar date for the shift.
/// </summary>
public DateOnly ShiftDate { get; set; }

/// <summary>
/// End time for the shift.
/// </summary>
public TimeOnly ShiftEndTime { get; set; }

/// <summary>
/// Start time for the shift.
/// </summary>
public TimeOnly ShiftStartTime { get; set; }

/// <summary>
/// Short title of the posting.
/// </summary>
public string Title { get; set; }

/// <summary>
/// Monetary amount component of the wage.
/// </summary>
public decimal WageAmount { get; set; }

/// <summary>
/// ISO currency code for the wage.
/// </summary>
public string WageCurrency { get; set; }

#endregion Properties
```

Entity-style mix (tiers separated by a single blank line, still **no** sub-regions):

```csharp
#region Properties

public DateTimeOffset CreatedAt { get; private set; }
public int WorkerId { get; private set; }

public bool IsCurrent => EndDate is null;

public virtual Worker Worker { get; private set; }

public virtual IReadOnlyList<WorkerSkill> Skills => _skills.AsReadOnly();

#endregion Properties
```

## XML documentation (unchanged intent)

- Prioritize **`public`** / **`protected`**; **`internal`** when central; **`private`** only if unusually complex or requested.
- Do not replace good existing `///` text; fill gaps and placeholders only.
- Doc text **English** unless user/workspace asks otherwise.
- **No** drive-by refactors beyond ordering, regions, and docs.

## Work order

1. Confirm paths ⊆ `ProjectRoot`.
2. Classify each member → standard region **or** explicit-interface region (by interface simple name).
3. For **Properties**, classify each member into **[tiers](#properties-tier-order-single-region-no-sub-regions)**; emit **one** `#region Properties` block (no nested property sub-regions); omit empty tiers; use one blank line between adjacent non-empty tiers; sort alphabetically inside each tier.
4. Sort within every other region alphabetically by name.
5. Emit standard regions in the fixed order; **then** emit explicit-interface regions (alphabetically by interface name).
6. Add missing `///` where appropriate.
7. Verify compile-oriented sanity (no duplicate `#endregion` labels; **Properties** has at most one pair `#region Properties` / `#endregion Properties` with no nested `#region` inside it).

## Conflicts

If alphabetical order would break a **required** semantic order (rare: `#if` interleaving), keep **alphabetical** within logically contiguous members and document the exception in a single-line comment only if the user already uses that pattern.
