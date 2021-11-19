import { TestBed } from '@angular/core/testing';

import { HrJobService } from './hr-job.service';

describe('HrJobService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: HrJobService = TestBed.get(HrJobService);
    expect(service).toBeTruthy();
  });
});
