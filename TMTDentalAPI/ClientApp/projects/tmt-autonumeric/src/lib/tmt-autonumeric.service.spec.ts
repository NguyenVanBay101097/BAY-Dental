import { TestBed } from '@angular/core/testing';

import { TmtAutonumericService } from './tmt-autonumeric.service';

describe('TmtAutonumericService', () => {
  let service: TmtAutonumericService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TmtAutonumericService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
