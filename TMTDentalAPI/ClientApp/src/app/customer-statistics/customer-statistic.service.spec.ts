import { TestBed } from '@angular/core/testing';

import { CustomerStatisticService } from './customer-statistic.service';

describe('CustomerStatisticService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CustomerStatisticService = TestBed.get(CustomerStatisticService);
    expect(service).toBeTruthy();
  });
});
