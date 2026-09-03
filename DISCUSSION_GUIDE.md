# Real Estate System — Discussion Guide

Everything you need to explain your project, with the hard parts covered in depth.

---

## 1. Thirty-Second Project Summary

> "It's a real estate portal built with ASP.NET Core MVC. Agents list properties,
> customers browse and search them and book viewing appointments, and an admin
> approves listings and manages the lookup data. It uses Entity Framework Core
> with SQL Server, the Repository pattern for data access, and ASP.NET Identity
> with three roles for authentication and authorization."

Say this first if they open with "tell us about your project."

---

## 2. Architecture — How a Request Flows

```
Browser  →  Routing  →  Controller  →  Repository  →  DbContext  →  SQL Server
                             ↓
                           View  →  HTML back to the browser
```

**The rule:** each layer only talks to its neighbour.
The controller never writes a query. The repository never knows about HTTP.

### Walk through /Properties/Index

**Step 1 — Routing** (`Program.cs`)

```csharp
pattern: "{controller=Home}/{action=Index}/{id?}"
```

URL `/Properties/Index` → controller `Properties`, action `Index`.

**Step 2 — DI builds the controller**

```csharp
public PropertiesController(IPropertyRepository _propertyRepository, ...)
```

The controller asks for an *interface*. ASP.NET looks in `Program.cs`, finds
`AddScoped<IPropertyRepository, PropertyRepository>()`, creates one, and passes
it in. You never write `new` anywhere.

**Step 3 — The action**

```csharp
public IActionResult Index()
{
    List<Property> properties = propertyRepository.GetAll();
    return View(properties);
}
```

Two lines. No queries, no business rules — that is the point.

**Step 4 — The repository**

```csharp
return context.Properties
    .Include(p => p.PropertyType)
    .Include(p => p.City)
    .Include(p => p.Agent)
    .ToList();
```

**Step 5 — The view**

```csharp
@model List<Property>
@foreach (var item in Model) { <td>@item.Title</td> }
```

---

## 3. AUTHORIZATION (the hard part — study this most)

### Authentication vs Authorization

| | Question it answers |
|---|---|
| **Authentication** | *Who are you?* — checking email + password, issuing the cookie |
| **Authorization** | *What are you allowed to do?* — checking your role against `[Authorize]` |

Authentication happens **once** at login. Authorization happens on **every**
request.

### The three roles

| Role | Can do |
|---|---|
| **Admin** | Everything — approve properties, manage cities, types, agents |
| **Agent** | Create / edit / delete properties and images, manage appointments |
| **Customer** | Browse, search, book a viewing |

### How roles were built — 4 steps

**Step 1 — Turn the role system on** (`Program.cs`)

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { ... })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
```

The **second type argument `IdentityRole`** is what enables roles. It registers
`RoleManager` and creates the `AspNetRoles` and `AspNetUserRoles` tables.

> If they ask: `AddDefaultIdentity` without a role type gives users but **no roles**.
> `AddIdentity<TUser, TRole>` is what you need for role-based authorization.

**What is `AddDefaultTokenProviders()`?**

A *token* here is a secure one-time code that Identity generates and later
verifies — for example the long code inside a "reset your password" link, an
email-confirmation link, or a two-factor code.

This line registers the classes that generate and validate those tokens. Without
it, calling something like `GeneratePasswordResetTokenAsync` throws
`NotSupportedException` — "No IUserTwoFactorTokenProvider named 'Default'".

**Do we use it?** Not directly. We have no password-reset or email-confirmation
feature, so no token is ever generated. It is part of the standard Identity
setup and costs nothing to leave in.

> Safe answer if asked: *"It registers the providers that generate the secure
> codes used for password reset and email confirmation. We did not implement
> those features, but it is standard Identity configuration and is required if
> we add them later."*

**Step 2 — Create the roles** (`ApplicationDbContext.OnModelCreating`)

```csharp
modelBuilder.Entity<IdentityRole>().HasData(
    new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" },
    new IdentityRole { Id = "role-agent", Name = "Agent", NormalizedName = "AGENT" },
    new IdentityRole { Id = "role-customer", Name = "Customer", NormalizedName = "CUSTOMER" }
);
```

`NormalizedName` **must be uppercase** — Identity looks roles up by the
normalized value. Lowercase there means role checks silently fail.

**Step 3 — Attach a user to a role** (`AccountController.Register`)

```csharp
var result = userManager.CreateAsync(user, model.Password).Result;

if (result.Succeeded)
{
    userManager.AddToRoleAsync(user, model.Role).Wait();
    signInManager.SignInAsync(user, false).Wait();
}
```

`AddToRoleAsync` inserts one row into `AspNetUserRoles` linking user id → role id.

**Step 4 — Enforce it**

```csharp
[Authorize(Roles = "Admin")]              // whole controller
public class CitiesController : Controller

[Authorize(Roles = "Admin,Agent")]        // single action
public IActionResult Create()

[Authorize]                               // any logged-in user
public class AppointmentsController : Controller
```

**The comma means OR**, not AND. `"Admin,Agent"` = Admin **or** Agent.
An action with no attribute is public — that is why anyone can browse
`/Properties` and `/Properties/Details`.

### The runtime pipeline — a classic exam question

```csharp
app.UseRouting();
app.UseAuthentication();   // reads the cookie → "this is Ahmed, role: Agent"
app.UseAuthorization();    // checks [Authorize(Roles=...)] against that
```

**Authentication MUST come before Authorization.**

If you reverse them, authorization runs before anyone has been identified, so
every user looks anonymous and every `[Authorize]` page redirects to login.

### Hiding links is not security

```html
@if (User.IsInRole("Admin")) { <li>Cities</li> }
```

This only hides the **link**. A customer typing `/Cities` directly is stopped by
the `[Authorize]` attribute — **not** by the view.

> Be ready to say: "The view check is for user experience. The attribute is the
> actual security."

### Tables involved

| Table | Holds |
|---|---|
| `AspNetUsers` | the accounts |
| `AspNetRoles` | Admin, Agent, Customer |
| `AspNetUserRoles` | which user has which role (many-to-many) |

---

## 4. COOKIES & STATE (second hard part)

### What happens at login

```csharp
signInManager.SignInAsync(user, model.RememberMe).Wait();
```

Identity writes an **encrypted, digitally signed cookie** to the browser
containing the user id and roles. It is signed so the user cannot edit it to
give themselves the Admin role.

### On every later request

`app.UseAuthentication()` reads that cookie and rebuilds the `User` object.
That is what makes `User.IsInRole("Admin")` and `[Authorize]` work.

### The RememberMe checkbox

```csharp
signInManager.SignInAsync(user, model.RememberMe);
```

| Value | Cookie type | Lifetime |
|---|---|---|
| `false` | Session cookie | dies when the browser closes |
| `true` | Persistent cookie | survives browser restarts |

### Cookie configuration

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
```

- **LoginPath** — where to send someone who is *not logged in*
- **AccessDeniedPath** — where to send someone who *is logged in but lacks the role*

> Two different failures, two different pages. Know the difference.

### The three state options (course topic)

| Technique | Stored | Lifetime | Used for |
|---|---|---|---|
| **Cookie** | Browser | Configurable | Your login session |
| **Session** | Server | ~20–30 min | Shopping-cart style data |
| **TempData** | Server (one read) | Next request only | Post-redirect messages |

---

## 5. DEPENDENCY INJECTION (third hard part)

### The problem it solves

Without DI:

```csharp
public class PropertiesController : Controller
{
    private PropertyRepository repo = new PropertyRepository(new ApplicationDbContext(...));
```

The controller is welded to one concrete class. To test it or swap the data
source, you must edit the controller.

### With DI

```csharp
private readonly IPropertyRepository propertyRepository;

public PropertiesController(IPropertyRepository _propertyRepository)
{
    propertyRepository = _propertyRepository;
}
```

The controller asks for an **interface** and receives whatever `Program.cs`
registered:

```csharp
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
```

To change the implementation, you change **one line** in `Program.cs`.

### The three lifetimes — they WILL ask

| Lifetime | New instance | Use for |
|---|---|---|
| **Transient** | Every single time it is requested | Lightweight, stateless helpers |
| **Scoped** | Once per HTTP request | **Repositories, DbContext** ← what we use |
| **Singleton** | Once for the whole application | Configuration, caching, logging |

**Why Scoped for repositories?** They hold a `DbContext`, which tracks changes.
One per request keeps all changes in a single unit of work, and disposes it
cleanly when the response is sent. A Singleton `DbContext` would be shared
between users — a serious bug.

---

## 6. THE REPOSITORY PATTERN

### Why it exists

**Without it:** every controller writes its own LINQ. The same query is copied
in five places. Changing it means finding all five.

**With it:** queries live in one class. The controller just asks.

### The shape (matches the course material)

```csharp
public interface IPropertyRepository
{
    List<Property> GetAll();
    Property GetById(int id);
    List<Property> Search(...);
    void Add(Property property);
    void Update(Property property);
    void Delete(int id);
    void Approve(int id);
    void Save();
}
```

### Why Add and Save are separate

```csharp
public void Add(Property property)
{
    context.Properties.Add(property);   // staged in memory only
}

public void Save()
{
    context.SaveChanges();              // writes to SQL Server
}
```

`Add` marks the object "to be inserted." **Nothing reaches the database until
`SaveChanges()`.** Splitting them lets you stage several operations and commit
once:

```csharp
propertyRepository.Add(property);
propertyImageRepository.Add(image);
propertyRepository.Save();   // both written together
```

> If a Create appears to work but no row appears — you forgot `Save()`.

---

## 7. ENTITY FRAMEWORK — THE PARTS THEY PROBE

### Include — why it matters

```csharp
context.Properties
    .Include(p => p.PropertyType)
    .Include(p => p.City)
    .ToList();
```

Without `Include`, `property.City` comes back **null**. EF loads only the
`Property` row, not related tables. `Include` adds a SQL `JOIN`.

> "That is why `@item.City?.Name` would print nothing if I removed the Include."

### Deferred execution

```csharp
var query = context.Properties.Where(p => p.CityId == 3);   // no SQL yet
var list = query.ToList();                                   // SQL runs HERE
```

The query is only *built* until you call `ToList()`, `FirstOrDefault()`, or
`Count()`. This is what makes the Search filter chain work.

### DeleteBehavior.Restrict — be ready for this one

```csharp
modelBuilder.Entity<Property>()
    .HasOne(p => p.City)
    .WithMany(c => c.Properties)
    .HasForeignKey(p => p.CityId)
    .OnDelete(DeleteBehavior.Restrict);
```

Read it as: *a Property has one City; a City has many Properties; linked by
`CityId`; deleting a City that still has properties is blocked.*

**Why Restrict and not Cascade?** `Property` has **three** foreign keys
(PropertyType, City, Agent). SQL Server refuses to create a table with multiple
cascade delete paths — the migration fails with "may cause cycles or multiple
cascade paths." Restrict avoids that, and it is also correct behaviour: you
should not lose properties because someone deleted a city.

### HasData seeding

```csharp
modelBuilder.Entity<City>().HasData(new City { Id = 1, Name = "Cairo", ... });
```

Runs as part of the **migration** — the INSERT statements are generated into the
migration file. Requires explicit `Id` values.

**Why is the admin user not seeded this way?** Passwords must be hashed, and
`HasData` cannot hash. So roles are seeded in the migration, and the admin user
is created at startup through `UserManager`.

---

## 8. MODEL BINDING & VALIDATION

### Model binding

```csharp
[HttpPost]
public IActionResult Create(Property property)
```

You never parse the form. ASP.NET sees the parameter type, matches form field
names to property names (`Title` → `property.Title`), converts types, and hands
you a filled object.

### Validation

```csharp
[Required, StringLength(200)]
public string Title { get; set; }

[Range(0, 1000000000)]
public decimal Price { get; set; }
```

If Title is empty, `ModelState.IsValid` is **false automatically**. You only
react to it:

```csharp
if (ModelState.IsValid) { ...save...; return RedirectToAction("Index"); }

PopulateDropDowns(property);
return View(property);       // redisplay with the user's data + error messages
```

**Client-side vs server-side:** `_ValidationScriptsPartial` gives instant
in-browser checks. `ModelState.IsValid` re-checks on the server — because
client-side validation can be bypassed.

---

## 9. TWO PATTERNS THEY LIKE TO ASK ABOUT

### PopulateDropDowns — why it is called 4 times

```csharp
private void PopulateDropDowns(Property property = null)
{
    ViewBag.PropertyTypeId = new SelectList(propertyTypeRepository.GetAll(), "Id", "Name", property?.PropertyTypeId);
    ViewBag.CityId = new SelectList(cityRepository.GetAll(), "Id", "Name", property?.CityId);
    ViewBag.AgentId = new SelectList(agentRepository.GetAll(), "Id", "FullName", property?.AgentId);
}
```

`SelectList(source, valueField, textField, selectedValue)` builds
`<option value="1">Apartment</option>`.

**`ViewBag` does not survive the round trip** — it is rebuilt every request. So
any path that returns a view containing a dropdown must call this first. Miss it
on the validation-failure path and you get a NullReferenceException.

### PRG — Post / Redirect / Get

```csharp
if (ModelState.IsValid)
{
    ...
    return RedirectToAction("Index");   // NOT return View()
}
```

After a successful POST you **redirect**. Otherwise refreshing the page
resubmits the form and creates a duplicate record.

---

## 10. IMAGE UPLOAD

```csharp
[NotMapped]
public IFormFile ImageFile { get; set; }
```

`[NotMapped]` = do not create a database column. The file lives in memory during
the request only.

```csharp
private string SaveImage(IFormFile imageFile)
{
    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
    ...
    return "/uploads/" + fileName;
}
```

Three points worth making:

1. **`enctype="multipart/form-data"`** on the form — without it the file never
   reaches the server and `ImageFile` is silently null.
2. **Guid filename** — two agents uploading `house.jpg` would otherwise overwrite
   each other.
3. **Two different paths** — you save to the *disk* path
   (`WebRootPath\uploads`) but store the *web* path (`/uploads/abc.jpg`) in the
   database, because that is what `<img src>` needs.

---

## 11. WHY WE MADE CERTAIN DECISIONS

Have an answer ready for each — examiners like design questions.

**Why the Repository pattern?**
Keeps queries in one place, keeps controllers thin, and lets us swap the data
source by changing one line in `Program.cs`.

**Why is there an `Agents` table AND an `Agent` role?**
They are different things. The `Agents` table holds display data (name, phone,
agency) used in the property dropdown. The `Agent` role is a login account with
permission to create properties. We kept them separate to keep the model simple.

**Why synchronous instead of async?**
For a project this size the difference is not measurable, and synchronous code
is clearer. Async releases the thread during the database call, which matters at
high load — we would use it in production.

**Why is Status set in the controller, not from the form?**
```csharp
property.Status = PropertyStatus.Pending;
```
Security. If Status came from the form, an agent could approve their own listing
by editing the HTML.

**Why UnitPrice / snapshot values?**
Historical accuracy — changing a value later must not rewrite past records.

---

## 12. LIKELY QUESTIONS — QUICK ANSWERS

| Question | Answer |
|---|---|
| Difference between authentication and authorization? | Authentication = who you are (login). Authorization = what you may do (roles). |
| Why must UseAuthentication come first? | Authorization needs to know who the user is; reversed, everyone looks anonymous. |
| What is `[Authorize(Roles="Admin,Agent")]`? | Allows Admin **or** Agent. Comma = OR. |
| Difference between ViewBag and ViewModel? | ViewBag is dynamic, no compile-time checking. ViewModel is a strongly typed class — safer. |
| What does `Include` do? | Eager-loads a related entity via SQL JOIN. Without it the navigation property is null. |
| Scoped vs Singleton vs Transient? | Per request / per app / per injection. Repositories are Scoped because they hold a DbContext. |
| What is `[NotMapped]`? | Property exists in the C# class but gets no database column. |
| Why `ModelState.IsValid`? | Server-side re-check of the data annotations; client-side validation can be bypassed. |
| What is TempData used for? | A message that survives exactly one redirect — the PRG success message. |
| What does a migration do? | Translates model changes into SQL DDL and records them in `__EFMigrationsHistory`. |
| Why `DeleteBehavior.Restrict`? | Property has 3 FKs; SQL Server rejects multiple cascade paths. Also prevents losing data. |
| What is `IFormFile`? | Represents an uploaded file; requires `enctype="multipart/form-data"`. |

---

## 13. DEMO CHECKLIST — DO THIS BEFORE YOU PRESENT

- [ ] Create one **Agent** account and one **Customer** account (you only have Admin)
- [ ] Add 2–3 **appointments** — that table is empty
- [ ] Add a few **property images** — that table is empty
- [ ] Log in as each of the 3 roles once and confirm the navbar changes
- [ ] Test the full flow: Agent creates a property → Admin approves → it appears in Search
- [ ] Have the login details written down — do not fumble at the podium
- [ ] Open the site a few minutes early if demoing the hosted version (free tiers sleep)

---

## 14. IF SOMETHING BREAKS ON STAGE

Stay calm and narrate. "This is the approval workflow — the property is Pending
so it does not appear in search yet. Let me log in as Admin and approve it."

Examiners care far more about whether you **understand** the code than whether
every click works.

If you genuinely do not know an answer: *"I did not implement that part —
[teammate] did, but my understanding is..."* is far better than inventing
something.
