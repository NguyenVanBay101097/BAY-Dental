import { TestBed } from '@angular/core/testing';

import { PartnerTitleService } from './partner-title.service';

describe('PartnerTitleService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PartnerTitleService = TestBed.get(PartnerTitleService);
    expect(service).toBeTruthy();
  });
});
