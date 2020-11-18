import { TestBed } from '@angular/core/testing';

import { SalaryPaymentService } from './salary-payment.service';

describe('SalaryPaymentService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SalaryPaymentService = TestBed.get(SalaryPaymentService);
    expect(service).toBeTruthy();
  });
});
