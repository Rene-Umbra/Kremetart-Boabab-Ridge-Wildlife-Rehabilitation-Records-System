# (Kremetart) Boabab Ridge Wildlife Rehabilitation Records System ~ Animal Management System

A C# Windows Forms desktop application that records every animal in care at the Baobab Ridge Wildlife Rehabilitation Centre, calculating each animal's status and housing unit from the vet's Recovery Score, and produces a Summary Report for the Centre manager.

Data is stored in plain text files only - no database or storage-related NuGet packages are used.

**GitHub Repository:** https://github.com/Rene-Umbra/Kremetart-Boabab-Ridge-Wildlife-Rehabilitation-Records-System

---
## 1. Team Members
|   Name   |    Student Number  |
|----------|--------------------|
|   René van der Walt  | 604356 |
|   Mongezi Mahlangu   | 605039 |
|   Tshiamo Kwakwa     | 600315 |
|   Tsireletso Poo     | 604186 |
|   Patience Omphile Kamva Makgothi | 603498 |

---
## 2. How to Build and Run

### Prerequisites
- Windows 10/11
- Visual Studio 2022 (or newer) with the **.NET desktop development** workload
- .NET Framework 4.7.2 targeting pack (which is inlcuded in the aforementioned workload)
- Git

## Steps
1. Clone Repository into an empty folder:
    ```bash
    git clone https://github.com/Rene-Umbra/Kremetart-Boabab-Ridge-Wildlife-Rehabilitation-Records-System.git
    cd Kremetart-Boabab-Ridge-Wildlife-Rehabilitation-Records-System
2. Open Kremetart Ridge WinForm Project.sln (or the .csproj) in Visual Studio.
3. Build the Solution (Build → Build Solution, or Ctrl+Shift+B).
4. Run the program (Debug → Start Without Debugging, or Ctrl+F5).

The animals.txt file is loaded from and saved to the same folder as the running .exe (bin\Debug\ or bin\Release\). No absolute file paths are hard-coded, so the project builds and runs after a fresh clone on any machine.

---
## 3. Features
### Feature 1 — Add a new animal

Labelled inputs for Animal ID, Name, Species, Age and Recovery Score, plus an Add button.

Validates every field (trimmed first):

**Animal ID:** required, format WR- + exactly 4 digits, converted to upper case, must be unique, cannot be changed after saving.

**Name:** required, 1–30 characters, no | character.

**Species:** required, 1–30 characters, no | character.

**Age:** required, whole number 0–100 (decimals and text rejected).

**Recovery Score:** required, whole number 0–100 (decimals and text rejected).

**On failure**, nothing is saved, a message names the field and explains the problem, and focus returns to that field.

**On success**, status and housing unit are calculated, one line is appended to animals.txt, a confirmation shows the calculated status and housing unit, the grid refreshes, and inputs clear.

### Feature 2 — View all animals
animals.txt is loaded on startup and every valid record is shown in a read-only DataGridView.

Columns in order: Animal ID, Name, Species, Age, Recovery Score, Status, Housing Unit.

Grid is read-only (no cell edits, no add/delete rows); clicking a cell selects the whole row.

Records appear in the same order as in the file.

The grid refreshes automatically after every add, update and delete — no duplicates, no restart.

If animals.txt does not exist, an empty one is created and an empty grid is shown without error.

Malformed lines (wrong field count, non-numeric age/score, invalid ID) are skipped on load, and the user is told once how many lines were skipped.

### Feature 3 — Update an animal
A record can be loaded for editing by clicking a row or by typing an Animal ID in the Search box and clicking Search (exact match, case-insensitive). Not found → user is told, nothing changes.

In edit mode: Animal ID is read-only, Add is disabled, Save Changes is enabled, and a Clear button returns to Add mode with empty boxes.

Save Changes applies the same validation as Add (no uniqueness re-check), and recalculates status and housing unit from the score even if the score is unchanged.

Saving rewrites animals.txt so only the edited record changes — no other record is modified, lost or duplicated.

After a successful save, the grid refreshes and the form returns to Add mode.

### Feature 4 — Delete an animal
Select a row and click Delete. No selection → a message asks the user to select one.

A Yes/No confirmation shows the animal's ID and name, e.g. "Delete WR-0007 (Naledi)? This cannot be undone." Choosing No changes nothing.

Choosing Yes removes exactly that record from animals.txt; all other records stay exactly as they were. The grid refreshes, and if the deleted animal was loaded in the form, the form is cleared.

One record is deleted at a time.

### Feature 5 — Generate a summary report
Generate Summary calculates from the current animals.txt: total animals, average age, average recovery score, and the count for each of the five statuses.

Results are displayed on the form in labels / read-only text boxes (not only in a message box).

Results are saved to summary.txt beside the .exe using the required layout (see below).

Empty data is handled without crashing: total = 0, both averages shown as N/A, all five statuses listed with a count of 0.

---

## Classification rules (Recovery Score → Status → Housing Unit)
|Recovery Score|Status	|Housing Unit|
|--------|------------|------------|
|0–19|	Critical|	Intensive Care Unit|
|20–39|	Serious|	High-Dependency Ward|
|40–59|	Stable	|Recovery Ward|
|60–79|	Recovering|	Outdoor Enclosure|
|80–100|Release-Ready|	Pre-Release Camp|

Status and housing unit are never typed by the user. The classification logic lives in a single method called by both Add and Update — it is not duplicated.

---
## 4. File Formats
## 4.1 animals.txt
One animal per line, seven fields separated by the pipe character |, in this order: ID|Name|Species|Age|Score|Status|Housing Unit

No header line, no blank lines. UTF-8 encoding.

Stored in the same folder as the running .exe.

Malformed lines are skipped on load (see Feature 2).

Example lines:

---
WR-0001|Thabo|African Penguin|4|85|Release-Ready|Pre-Release Camp

WR-0002|Naledi|Serval|3|62|Recovering|Outdoor Enclosure

WR-0004|Zola|Cape Vulture|12|19|Critical|Intensive Care Unit

---

### 4.2 summary.txt
Created in the same folder as the .exe each time the summary is generated (overwritten, never appended).

Exact layout:

---
BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE

ANIMAL SUMMARY REPORT

Generated: 2026-10-14 09:32

Total animals: 12

Average age (years): 7.75

Average recovery score: 54.00

Animals per status:

Critical: 2

Serious: 2

Stable: 2

Recovering: 3

Release-Ready: 3

---

Timestamp format: yyyy-MM-dd HH:mm.

Averages use two decimal places with a full stop decimal separator, regardless of the computer's regional settings.

All five statuses are always listed, including those with a count of 0.

If there are no animals, both averages are shown as N/A and all counts are 0.

summary.txt is generated at runtime and is not committed to Git (see .gitignore).

---
## 5. Error handling

All file operations (reading, appending, rewriting and writing the summary) are wrapped in error handling. At a minimum the program handles:

A missing file or folder (creates an empty animals.txt on startup; empty grid, no error).

A file in use by another program (IOException) — friendly message, program keeps running.

Permission problems (UnauthorizedAccessException), e.g. animals.txt set to read-only — friendly message, program keeps running.

The program never shows a stack trace to the user, and never crashes with an unhandled exception during normal use — including empty fields, letters in numeric boxes, an empty file, or clicking Delete/Save with nothing selected.

---
## 6. Code structure

An Animal class (or record) holds the seven fields.

A single classification method maps a Recovery Score to (Status, Housing Unit) and is called by both Add and Update.

Every method has a comment above it explaining what it does; non-obvious logic has inline comments.

Controls and forms use meaningful names (e.g. btnAdd, dgvAnimals, txtAnimalId, lblStatus).

Old commented-out code has been removed before submission.

---
## 7. Git and GitHub

The repository was initialised before any application code was written.

The first commit contains only the blank Windows Forms project, a Visual Studio/.NET .gitignore, and this README.md.

main is the stable branch and always builds. Each feature was developed on its own branch and merged into main via a GitHub pull request:

feature/add-animal

feature/view-animals

feature/update-animal

feature/delete-animal

feature/summary-report

Commit messages start with a verb and describe the change (e.g. "Add validation for animal ID format", "Fix duplicate rows after update").

bin/, obj/, .vs/ and generated files such as summary.txt are excluded via .gitignore.

The repository is public 

---
## 8. Known limitations

The project targets .NET Framework 4.7.2 rather than .NET 8.0 as stated in the brief; this follows the supplied Visual Studio scaffold. Behaviour is otherwise identical to the brief.

animals.txt is loaded once on startup and reloaded after each add/update/delete; external edits made to the file while the program is running are not picked up until the next reload action.

The summary is generated on demand via the Generate Summary button; it is not regenerated automatically when data changes.

Only one animal can be deleted at a time (as required).

The grid does not support sorting/filtering beyond the Search box; records are shown in file order.

There is no undo for delete or update (the confirmation dialog is the only safeguard).


