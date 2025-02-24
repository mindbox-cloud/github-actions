export interface CoverageStats {
  total: number;
  covered: number;
  skipped: number;
  pct: number;
}

export type CoverageStatName = 'lines' | 'functions' | 'statements' | 'branches';

export type CoverageReportItem = Record<CoverageStatName, CoverageStats>;

export interface CoverageReportContent {
  total: CoverageReportItem;
  [key: string]: CoverageReportItem;
}

export type CoverageRequiredStats = Record<CoverageStatName, number>;

export interface UnmatchedStatInfo {
  fileName: string;
  statName: CoverageStatName;
  required: number;
  actual: number;
}