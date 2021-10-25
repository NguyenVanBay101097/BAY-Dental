import { TestBed } from '@angular/core/testing';

import { IrAttachmentService } from './ir-attachment.service';

describe('IrAttachmentService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: IrAttachmentService = TestBed.get(IrAttachmentService);
    expect(service).toBeTruthy();
  });
});
