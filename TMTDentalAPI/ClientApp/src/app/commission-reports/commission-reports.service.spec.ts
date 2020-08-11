import { TestBed } from '@angular/core/testing';

import { CommissionReportsService } from './commission-reports.service';

describe('CommissionReportsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CommissionReportsService = TestBed.get(CommissionReportsService);
    expect(service).toBeTruthy();
  });
});
