import * as core from '@actions/core';

import { CoverageReport } from './CoverageReport';
import { CoverageVerifier } from './CoverageVerifier';

async function run() {
  try {
    const json_summary_path = core.getInput('json_summary_path', { required: true });
    const changed_files_list = core.getInput('changed_files_list', { required: true });
    const required_branches_coverage = core.getInput('required_branches_coverage', { required: true });

    const report = await CoverageReport.fromSummaryJson(json_summary_path);
    const coverageVerifier = new CoverageVerifier({
      report,
      requiredStats: {
        branches: parseInt(required_branches_coverage),
      },
    });

    const coverageVerifierReport = coverageVerifier.verify(changed_files_list.split(','));
    
    if (coverageVerifierReport.isAllCovered()) {
      core.info('All coverage is covered');
    } else {
      core.setFailed(coverageVerifierReport.getErrorMessage());
    }
  } catch (error) {
    core.setFailed(error.message)
  }
}

run();