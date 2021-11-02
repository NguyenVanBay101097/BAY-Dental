import { TestBed } from '@angular/core/testing';

import { DayDashboardReportService } from './day-dashboard-report.service';

describe('DayDashboardReportService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DayDashboardReportService = TestBed.get(DayDashboardReportService);
    expect(service).toBeTruthy();
  });
});
