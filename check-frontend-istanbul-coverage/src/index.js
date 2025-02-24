import fs from 'fs';
import core from '@actions/core';

function normalizeSummaryJson(summaryJson) {
  const entries = Object.entries(summaryJson);
  const currentDir = process.cwd();

  const normalizedEntries = entries.map(([file, coverage]) => [
    file.replace(currentDir, ''),
    coverage
  ]);

  return Object.fromEntries(normalizedEntries);
}

function extractCoverageFromSummary(summaryJson, changedFilesList) {
  return changedFilesList.reduce((agg, cur) => {
    const coverageInfo = summaryJson[cur];

    if (coverageInfo) {
      agg[cur] = coverageInfo;
    }

    return agg;
  }, {});
}

function getUnmatchedCoverage(coverage, { branches }) {
  const unmatchedCoverage = {};

  Object.entries(coverage).forEach(([file, coverage]) => {
    const currentBranchesCoverage = coverage.branches.pct;

    if (currentBranchesCoverage >= branches) {
      return;
    }

    unmatchedCoverage[file] = {
      branches: currentBranchesCoverage,
    };
  });

  return unmatchedCoverage;
}

function logUnmatchedCoverage(unmatchedCoverage) {
  Object.entries(unmatchedCoverage).forEach(([file, coverage]) => {
    core.error(`Unmatched coverage for file ${file}: ${coverage.branches}`);
  });
}

async function run() {
  try {
    const json_summary_path = core.getInput('json_summary_path', { required: true })
    const changed_files_list = core.getInput('changed_files_list', { required: true })
    const required_branches_coverage = core.getInput('required_branches_coverage', { required: true })

    const jsonSummaryContent = await fs.promises.readFile(json_summary_path, 'utf-8');
    core.info('JSON summary content: ' + jsonSummaryContent);
    const jsonSummary = JSON.parse(jsonSummaryContent);
    const normalizedJsonSummary = normalizeSummaryJson(jsonSummary);
    core.info('Normalized JSON summary: ' + JSON.stringify(normalizedJsonSummary));

    const changedFilesList = JSON.parse(changed_files_list);
    core.info('Changed files list: ' + changedFilesList.joint(' '));

    const coverage = extractCoverageFromSummary(normalizedJsonSummary, changedFilesList);
    core.info('Coverage: ' + JSON.stringify(coverage));

    const unmatchedCoverage = getUnmatchedCoverage(coverage, { branches: parseInt(required_branches_coverage) });
    core.info('Unmatched coverage: ' + JSON.stringify(unmatchedCoverage));

    logUnmatchedCoverage(unmatchedCoverage);
  } catch (error) {
    core.setFailed(error.message)
  }
}

run();