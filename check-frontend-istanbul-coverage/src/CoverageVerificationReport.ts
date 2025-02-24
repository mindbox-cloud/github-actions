import { CoverageVerificationReportOutput } from "./coverage-verification-report-output";
import { UnmatchedStatInfo } from "./types";

export class CoverageVerificationReport {
  private readonly unmatchedStatInfoList: UnmatchedStatInfo[];

  constructor() {
    this.unmatchedStatInfoList = [];
  }

  addUnmatchedStat(statInfo: UnmatchedStatInfo): void {
    this.unmatchedStatInfoList.push(statInfo);
  }

  applyOutput(output: CoverageVerificationReportOutput): void {
    if (this.isAllCovered()) {
      output.success();
    }

    output.failure(this.unmatchedStatInfoList);
  }

  static merge(reports: CoverageVerificationReport[]): CoverageVerificationReport {
    const report = new CoverageVerificationReport();

    for (const itemReport of reports) {
      report.unmatchedStatInfoList.push(...itemReport.unmatchedStatInfoList);
    }

    return report;
  }

  private isAllCovered(): boolean {
    return !this.unmatchedStatInfoList.length;
  }
}