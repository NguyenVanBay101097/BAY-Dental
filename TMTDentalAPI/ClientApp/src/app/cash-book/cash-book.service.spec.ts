import { TestBed } from '@angular/core/testing';

import { CashBookService } from './cash-book.service';

describe('CashBookService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CashBookService = TestBed.get(CashBookService);
    expect(service).toBeTruthy();
  });
});
