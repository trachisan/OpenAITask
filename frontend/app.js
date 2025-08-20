const modeSelect = document.getElementById("mode");
const toneContainer = document.getElementById("tone-container");
const toneSelect = document.getElementById("tone");
const runBtn = document.getElementById("runBtn");
const inputText = document.getElementById("inputText");
const resultPanel = document.getElementById("result-panel");
const resultBox = document.getElementById("result");
const copyBtn = document.getElementById("copyBtn");
const usageBox = document.getElementById("usage");

modeSelect.addEventListener("change", () => {
  toneContainer.style.display = modeSelect.value === "rephrase" ? "block" : "none";
});

runBtn.addEventListener("click", async () => {
  const mode = modeSelect.value;
  const text = inputText.value.trim();
  const tone = toneSelect.value;

  if (!text || text.length > 5000) {
    alert("Text must be between 1 and 5000 characters.");
    return;
  }

  const payload = { mode, text };
  if (mode === "rephrase") payload.tone = tone;

  try {
    const res = await fetch("http://localhost:5158/openai/send", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });

    const data = await res.json();
    console.log(data.output)

if (!res.ok || !data.usage) {
  resultBox.textContent = data.error || "Something went wrong.";
  resultPanel.style.display = "block";
  usageBox.textContent = "";
  return;
}

const usage = data.usage || { inputTokens: 0, outputTokens: 0, totalTokens: 0 };
console.log(usage)

if (mode === "extract_json") {
  try {
    resultBox.textContent = JSON.stringify(JSON.parse(data.output), null, 2);
  } catch {
    resultBox.textContent = data.output;
  }
} else {
  resultBox.textContent = data.output;
}

resultPanel.style.display = "block";
usageBox.textContent = `Prompt: ${usage.inputTokens}, Completion: ${usage.outputTokens}, Total: ${usage.totalTokens}`;

  } catch (err) {
    resultBox.textContent = "Network error: " + err.message;
    resultPanel.style.display = "block";
  }
});

copyBtn.addEventListener("click", () => {
  navigator.clipboard.writeText(resultBox.textContent);
});
