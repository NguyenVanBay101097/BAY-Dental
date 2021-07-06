import { TestBed } from '@angular/core/testing';

import { CustomerReceiptService } from './customer-receipt.service';

describe('CustomerReceiptService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CustomerReceiptService = TestBed.get(CustomerReceiptService);
    expect(service).toBeTruthy();
  });
});
