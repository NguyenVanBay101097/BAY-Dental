import { TestBed } from '@angular/core/testing';

import { SmsConfigService } from './sms-config.service';

describe('SmsConfigService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SmsConfigService = TestBed.get(SmsConfigService);
    expect(service).toBeTruthy();
  });
});
