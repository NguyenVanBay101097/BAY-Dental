import { TestBed } from '@angular/core/testing';

import { EmpCategoryService } from './emp-category.service';

describe('EmpCategoryService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: EmpCategoryService = TestBed.get(EmpCategoryService);
    expect(service).toBeTruthy();
  });
});
