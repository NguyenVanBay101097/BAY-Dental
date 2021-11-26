import { TestBed } from '@angular/core/testing';

import { ResInsurancePaymentService } from './res-insurance-payment.service';

describe('ResInsurancePaymentService', () => {
  let service: ResInsurancePaymentService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResInsurancePaymentService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
