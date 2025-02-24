import * as core from '@actions/core';
import { CoverageVerificationReportOutput } from './CoverageVerificationReportOutput';
import { UnmatchedStatInfo } from '../types';

export class CoverageVerificationReportGithubOutput extends CoverageVerificationReportOutput {
  success(): void {
    core.info('All files are covered');
  }

  failure(unmatchedStatInfoList: UnmatchedStatInfo[]): void {
    const errorMessages = this.getErrorMessages(unmatchedStatInfoList);

    errorMessages.forEach((errorMessage) => {
      core.error(errorMessage);
    });
    
    core.setFailed('Some files are not covered');
  }

  private getErrorMessages(unmatchedStatInfoList: UnmatchedStatInfo[]): string[] {
    const errorMessages = unmatchedStatInfoList.map((errorStat) => {
      return `File: ${errorStat.fileName}; Stat: ${errorStat.statName}; Required: ${errorStat.required}; Actual: ${errorStat.actual}`;
    });

    return errorMessages;
  }
}