import { UnmatchedStatInfo } from "../types";

export abstract class CoverageVerificationReportOutput {
  abstract success(): void;

  abstract failure(unmatchedStatInfoList: UnmatchedStatInfo[]): void;
}
