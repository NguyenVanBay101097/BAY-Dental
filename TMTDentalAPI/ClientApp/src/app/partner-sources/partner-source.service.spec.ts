import { TestBed } from '@angular/core/testing';

import { PartnerSourceService } from './partner-source.service';

describe('PartnerSourceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PartnerSourceService = TestBed.get(PartnerSourceService);
    expect(service).toBeTruthy();
  });
});
