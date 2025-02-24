import { CoverageReport } from "./CoverageReport";
import { CoverageVerificationReport } from "./CoverageVerificationReport";
import { CoverageRequiredStats } from "./types";

interface ICoverageVerifierParams {
  report: CoverageReport;
  requiredStats?: Partial<CoverageRequiredStats>;
}

const DEFAULT_REQUIRED_STATS: CoverageRequiredStats = {
  lines: 0,
  functions: 0,
  statements: 0,
  branches: 50
};

export class CoverageVerifier {
  private readonly report: CoverageReport;

  private readonly requiredStats: CoverageRequiredStats;

  constructor({
    report,
    requiredStats = {},
  }: ICoverageVerifierParams) {
    this.report = report;
    this.requiredStats = {
      ...DEFAULT_REQUIRED_STATS,
      ...requiredStats
    };
  }

  public verify(fileNameList: string[]): CoverageVerificationReport {
    const itemReports = fileNameList.map(fileName => this.getItemReport(fileName));
    return CoverageVerificationReport.merge(itemReports);
  }

  private getItemReport(fileName: string): CoverageVerificationReport {
    const coverageItem = this.report.getCoverageForItem(fileName);
    const report = new CoverageVerificationReport();

    if (!coverageItem) {
      return report;
    }

    if (coverageItem.lines.pct < this.requiredStats.lines) {
      report.addUnmatchedStat({
        fileName,
        statName: 'lines',
        required: this.requiredStats.lines,
        actual: coverageItem.lines.pct
      });
    }

    if (coverageItem.functions.pct < this.requiredStats.functions) {
      report.addUnmatchedStat({
        fileName,
        statName: 'functions',
        required: this.requiredStats.functions,
        actual: coverageItem.functions.pct
      });
    }

    if (coverageItem.statements.pct < this.requiredStats.statements) {
      report.addUnmatchedStat({
        fileName,
        statName: 'statements',
        required: this.requiredStats.statements,
        actual: coverageItem.statements.pct
      });
    }

    if (coverageItem.branches.pct < this.requiredStats.branches) {
      report.addUnmatchedStat({
        fileName,
        statName: 'branches',
        required: this.requiredStats.lines,
        actual: coverageItem.lines.pct
      });
    }

    return report;
  }
}