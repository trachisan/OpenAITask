# OpenAI Web App

A minimal web application that integrates with OpenAI to perform text operations: **Summarize**, **Rephrase**, **Extract JSON**, and **Classify sentiment**.  

**Tech stack:**  
- Backend: **ASP.NET Web API**  
- Frontend: **HTML + CSS + JavaScript**

---

## Features

- Single-page web app
- Four modes:
  - **Summarize** – generate a concise summary
  - **Rephrase** – change tone (`casual | professional | friendly`)
  - **Extract JSON** – produce structured JSON output
  - **Classify** – sentiment analysis (`positive | neutral | negative`)
- Shows token usage (prompt, completion, total)
- Copy result button
- Pretty-printed JSON for `extract_json` mode
- Human-readable error messages

---

## Requirements

### Functional
- Accept user text (1–5000 characters)
- Mode selection with tone option for rephrase
- Calls backend endpoint (ASP.NET API) only
- Displays results in a result panel
- Shows token usage
- No external database required

### Non-Functional
- OpenAI API key stored only on server
- Single-page layout

---

## UI Controls

- **Mode** (select dropdown)
- **Tone** (select dropdown, visible only for rephrase)
- **Text area** (paste input)
- **Run button**
- **Copy result button**
- **Result panel** (monospaced for JSON mode)
- Footer showing token usage

---

## Setup

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd <repo-folder>

2. Copy and set your OpenAI API key in appsettings.json:
   ```bash
   "API-KEY" : "Your api from open ai"

3. Build and run the ASP.NET backend:
   ```bash
    dotnet build
    dotnet run --project OpenAIApp

4. Open index.html in a browser (frontend communicates with backend API).

## Screenshots

<img width="600" height="800" alt="image" src="https://github.com/user-attachments/assets/54ae1eb4-18e0-427d-8c14-fad3ee78e27f" />

<img width="600" height="800" alt="image" src="https://github.com/user-attachments/assets/76a6d12c-e4d4-422f-afd0-003643f5882c" />

