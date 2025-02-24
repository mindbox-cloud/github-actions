import fs from 'fs';
import path from 'path';

import { CoverageReportItem, CoverageReportContent } from './types';

export class CoverageReport {
  private readonly content: CoverageReportContent;

  constructor(content: CoverageReportContent) {
    this.content = content;
  }

  getCoverageForItem(name: string): CoverageReportItem | null {
    const resolved = path.resolve(name);
    return this.content[resolved] || null;
  }

  static async fromSummaryJson(path: string): Promise<CoverageReport> {
    const jsonSummaryContent = await fs.promises.readFile(path, 'utf-8');
    const jsonSummary = JSON.parse(jsonSummaryContent);

    return new CoverageReport(jsonSummary);
  }
}