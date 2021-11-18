import { TestBed } from '@angular/core/testing';

import { ProductPriceListItemsService } from './product-price-list-items.service';

describe('ProductPriceListItemsService', () => {
  let service: ProductPriceListItemsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProductPriceListItemsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
