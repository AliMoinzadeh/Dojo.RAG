#!/usr/bin/env node

import fs from "node:fs";
import os from "node:os";
import path from "node:path";
import { spawnSync } from "node:child_process";

function printHelp() {
  console.log("Usage: node scripts/build-presentation.mjs [--input <path>] [--output <path>]");
  console.log("");
  console.log("Defaults:");
  console.log("  --input  docs/presentation.de.md");
  console.log("  --output docs/presentation.de.html");
}

function parseArgs(argv) {
  const options = {
    input: "docs/presentation.de.md",
    output: "docs/presentation.de.html",
  };

  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i];

    if (arg === "--help" || arg === "-h") {
      printHelp();
      process.exit(0);
    }

    if ((arg === "--input" || arg === "-i") && argv[i + 1]) {
      options.input = argv[i + 1];
      i += 1;
      continue;
    }

    if ((arg === "--output" || arg === "-o") && argv[i + 1]) {
      options.output = argv[i + 1];
      i += 1;
      continue;
    }

    throw new Error(`Unknown argument: ${arg}`);
  }

  return options;
}

function runCommand(command, args, envOverrides = {}) {
  const result = spawnSync(command, args, {
    stdio: "inherit",
    shell: process.platform === "win32",
    env: {
      ...process.env,
      ...envOverrides,
    },
  });

  if (result.error) {
    throw result.error;
  }

  if (result.status !== 0) {
    throw new Error(`Command failed: ${command} ${args.join(" ")}`);
  }
}

function toBase64Svg(svgPath) {
  return fs.readFileSync(svgPath).toString("base64");
}

function buildMermaidConfig(configPath) {
  const config = {
    theme: "base",
    flowchart: {
      htmlLabels: true,
      curve: "basis",
    },
    themeVariables: {
      background: "#1e1e2e",
      primaryColor: "#313244",
      primaryBorderColor: "#89b4fa",
      primaryTextColor: "#cdd6f4",
      secondaryColor: "#45475a",
      tertiaryColor: "#181825",
      lineColor: "#89b4fa",
      fontFamily: "Inter, Segoe UI, Arial, sans-serif",
    },
  };

  fs.writeFileSync(configPath, JSON.stringify(config, null, 2), "utf8");
}

function run() {
  const options = parseArgs(process.argv.slice(2));
  const inputPath = path.resolve(options.input);
  const outputPath = path.resolve(options.output);

  if (!fs.existsSync(inputPath)) {
    throw new Error(`Input file not found: ${inputPath}`);
  }

  const npxCommand = "npx";
  const tempDir = fs.mkdtempSync(path.join(os.tmpdir(), "presentation-build-"));
  const commandEnv = {
    npm_config_cache: path.join(tempDir, "npm-cache"),
  };

  try {
    const mermaidConfigPath = path.join(tempDir, "mermaid.config.json");
    buildMermaidConfig(mermaidConfigPath);

    const sourceMarkdown = fs.readFileSync(inputPath, "utf8");
    const mermaidFence = /```mermaid\s*\r?\n([\s\S]*?)```/g;
    let diagramCount = 0;

    const preparedMarkdown = sourceMarkdown.replace(mermaidFence, (_, diagram) => {
      diagramCount += 1;
      const diagramName = `diagram-${String(diagramCount).padStart(2, "0")}`;
      const mermaidInputPath = path.join(tempDir, `${diagramName}.mmd`);
      const mermaidSvgPath = path.join(tempDir, `${diagramName}.svg`);

      fs.writeFileSync(mermaidInputPath, `${diagram.trim()}\n`, "utf8");

      runCommand(npxCommand, [
        "--yes",
        "@mermaid-js/mermaid-cli",
        "-i",
        mermaidInputPath,
        "-o",
        mermaidSvgPath,
        "-c",
        mermaidConfigPath,
        "-b",
        "transparent",
      ], commandEnv);

      const base64Svg = toBase64Svg(mermaidSvgPath);
      return [
        '<div class="mermaid-diagram">',
        `  <img alt="Mermaid Diagram ${diagramCount}" src="data:image/svg+xml;base64,${base64Svg}" />`,
        "</div>",
      ].join("\n");
    });

    const generatedMarkdownPath = path.join(tempDir, "presentation.generated.md");
    fs.writeFileSync(generatedMarkdownPath, preparedMarkdown, "utf8");
    fs.mkdirSync(path.dirname(outputPath), { recursive: true });

    runCommand(npxCommand, [
      "--yes",
      "@marp-team/marp-cli",
      generatedMarkdownPath,
      "-o",
      outputPath,
      "--html",
      "--allow-local-files",
    ], commandEnv);

    console.log(`Presentation generated: ${outputPath}`);
    console.log(`Mermaid diagrams rendered: ${diagramCount}`);
  } finally {
    fs.rmSync(tempDir, { recursive: true, force: true });
  }
}

try {
  run();
} catch (error) {
  const message = error instanceof Error ? error.message : String(error);
  console.error(`Presentation build failed: ${message}`);
  process.exit(1);
}
