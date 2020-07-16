import { TestBed } from '@angular/core/testing';

import { ReportPartnerSourcesService } from './report-partner-sources.service';

describe('ReportPartnerSourcesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ReportPartnerSourcesService = TestBed.get(ReportPartnerSourcesService);
    expect(service).toBeTruthy();
  });
});
