import { CoverageStatName } from "./types";

interface UnmatchedStatInfo {
  fileName: string;
  statName: CoverageStatName;
  required: number;
  actual: number;
}

export class CoverageVerifierReport {
  private readonly unmatchedStatInfoList: UnmatchedStatInfo[];

  constructor() {
    this.unmatchedStatInfoList = [];
  }

  addUnmatchedStat(statInfo: UnmatchedStatInfo): void {
    this.unmatchedStatInfoList.push(statInfo);
  }

  isAllCovered(): boolean {
    return !this.unmatchedStatInfoList.length;
  }

  getErrorMessage(): string {
    return this.unmatchedStatInfoList.map((report) => {
      return `Required ${report.statName} coverage for file "${report.fileName}": ${report.required}, actual: ${report.actual}`;
    }).join('\n');
  }

  static merge(reports: CoverageVerifierReport[]): CoverageVerifierReport {
    const report = new CoverageVerifierReport();

    for (const itemReport of reports) {
      report.unmatchedStatInfoList.push(...itemReport.unmatchedStatInfoList);
    }

    return report;
  }
}