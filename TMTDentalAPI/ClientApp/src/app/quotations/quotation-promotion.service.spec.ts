import { TestBed } from '@angular/core/testing';

import { QuotationPromotionService } from './quotation-promotion.service';

describe('QuotationPromotionService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: QuotationPromotionService = TestBed.get(QuotationPromotionService);
    expect(service).toBeTruthy();
  });
});
