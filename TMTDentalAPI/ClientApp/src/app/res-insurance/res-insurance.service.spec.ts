import { TestBed } from '@angular/core/testing';

import { ResInsuranceService } from './res-insurance.service';

describe('ResInsuranceService', () => {
  let service: ResInsuranceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResInsuranceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
