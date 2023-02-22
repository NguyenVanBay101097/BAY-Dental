import { TestBed } from '@angular/core/testing';

import { WorkEntryTypeService } from './work-entry-type.service';

describe('WorkEntryTypeService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: WorkEntryTypeService = TestBed.get(WorkEntryTypeService);
    expect(service).toBeTruthy();
  });
});
