import { TestBed } from '@angular/core/testing';

import { CommissionSettlementsService } from './commission-settlements.service';

describe('CommissionSettlementsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CommissionSettlementsService = TestBed.get(CommissionSettlementsService);
    expect(service).toBeTruthy();
  });
});
