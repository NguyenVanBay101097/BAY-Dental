import { TestBed } from '@angular/core/testing';

import { MedicineOrderService } from './medicine-order.service';

describe('MedicineOrderService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MedicineOrderService = TestBed.get(MedicineOrderService);
    expect(service).toBeTruthy();
  });
});
