# Smart Health and Fitness Assistant

AI-enabled mini project — Visual Studio 2022, C#, .NET Framework, ASP.NET Web Forms, Ollama.

**Flow:** Wellness goals & preferences → C# validation & calculation → Ollama (local AI) → wellness suggestions displayed on the Web Form.

---

## 1. Files in this folder

| File | What it is |
|---|---|
| `Default.aspx` | The Web Form (dashboard UI) |
| `Default.aspx.cs` | Code-behind: validation, calculation, AI call |
| `Models/UserProfile.cs` | **Classes, Properties, Validation** |
| `Models/WellnessReport.cs` | Result object (properties only) |
| `Models/CalorieBar.cs` | Tiny helper class for the progress graphic |
| `Services/WellnessCalculator.cs` | **Methods** — BMI, BMR, TDEE, macros, heart-rate zone |
| `Services/OllamaService.cs` | **AI feature** — builds the prompt, calls Ollama |
| `Content/site.css` | Dashboard styling |
| `Web.config` | Ollama URL + model name |

---

## 2. Install and run Ollama (Option A — no API key)

1. Download from **https://ollama.com/download** and install.
2. Open Command Prompt and pull a model:
   ```
   ollama pull llama3.2
   ```
   (On a low-RAM laptop use `ollama pull llama3.2:1b` or `ollama pull qwen2.5:1.5b` instead, and change `OllamaModel` in `Web.config` to match.)
3. Make sure the server is running:
   ```
   ollama serve
   ```
   Ollama usually starts automatically after install. Check it by opening
   **http://localhost:11434** in a browser — it should say *Ollama is running*.

---

## 3. Create the project in Visual Studio 2022

1. **File → New → Project**
2. Choose **ASP.NET Web Application (.NET Framework)** — C#
3. Name it exactly **`SmartHealthFitnessAssistant`** (this matters — the namespace in the code files depends on it)
4. Framework: **.NET Framework 4.8**
5. Template: **Empty**, and tick **Web Forms** under "Add folders and core references"
6. Click **Create**

---

## 4. Add the files

1. In **Solution Explorer**, right-click the project → **Add → New Folder** → name it `Models`. Repeat for `Services` and `Content`.
2. Right-click each folder → **Add → Existing Item…** → browse to the matching file from this pack → **Add**.
3. For `Default.aspx`: right-click the project → **Add → Existing Item…** → select **both** `Default.aspx` and `Default.aspx.cs`.
4. Replace the generated `Web.config` with the one from this pack (or just copy the `<appSettings>` block into yours and keep your own `targetFramework` value).

### Required references
Right-click **References → Add Reference → Assemblies → Framework**, and tick:

- `System.Net.Http`
- `System.Web.Extensions`
- `System.Configuration`

(These are usually already present in a Web Forms project; add any that are missing.)

---

## 5. Run

1. Right-click `Default.aspx` → **Set As Start Page**
2. Press **Ctrl + F5**
3. Fill in the form and click **Generate my wellness suggestions**

The first AI response can take 20–60 seconds while the model loads into memory. Later responses are much faster.

---

## 6. How each required concept is covered

| Required concept | Where to point during your viva |
|---|---|
| **Classes** | `UserProfile`, `WellnessReport`, `CalorieBar`, `OllamaService`, static class `WellnessCalculator` |
| **Properties** | `UserProfile` — full properties with private backing fields (`FullName`, `Age`, `HeightCm`…), auto-properties (`Gender`, `Goal`), and read-only computed properties (`HeightMeters`, `GoalDisplayName`, `CalorieBarPercent`) |
| **Methods** | `WellnessCalculator.CalculateBmi()` (overloaded), `CalculateBmr()`, `CalculateTdee()`, `CalculateTargetCalories()`, `BuildReport()`, plus private helpers |
| **Validation** | Three layers: (1) ASP.NET validators on the form — `RequiredFieldValidator`, `RangeValidator`, `RegularExpressionValidator`, `CustomValidator`, `ValidationSummary`; (2) property setters that throw on bad values; (3) `UserProfile.Validate()` which returns a list of all errors |
| **AI feature (Ollama)** | `OllamaService.BuildPrompt()` builds the prompt from the user data **plus the C#-calculated figures**, `SendPromptAsync()` POSTs to `http://localhost:11434/api/generate`, and `Default.aspx.cs` displays the reply in the "AI Wellness Suggestions" card |

The page also has a collapsible **"Show the exact prompt sent to Ollama"** section — useful to demonstrate the data flow to your examiner.

---

## 7. Troubleshooting

| Problem | Fix |
|---|---|
| "Could not reach Ollama" | Run `ollama serve`, or open http://localhost:11434 to confirm it is up |
| "Ollama replied with status 404" | The model isn't installed — run `ollama pull llama3.2` |
| Request times out | Use a smaller model (`llama3.2:1b`) and change `OllamaModel` in `Web.config` |
| `The type or namespace 'Models' could not be found` | Your project name/namespace isn't `SmartHealthFitnessAssistant` — either rename the project or change the `namespace` line at the top of each `.cs` file |
| `HttpClient` not found | Add the `System.Net.Http` reference |
| `JavaScriptSerializer` not found | Add the `System.Web.Extensions` reference |

---

## 8. Note

The application displays **general wellness information for educational purposes only**. It is not medical advice, it applies a minimum calorie floor so it never suggests an unsafe intake, and the AI prompt explicitly instructs the model to avoid extreme diets and to recommend seeing a doctor for medical concerns. Keep that disclaimer in your report.

Ollama Local Host Path :- 
http://localhost:11434
