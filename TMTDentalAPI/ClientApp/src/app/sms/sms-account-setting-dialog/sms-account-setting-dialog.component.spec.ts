import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsAccountSettingDialogComponent } from './sms-account-setting-dialog.component';

describe('SmsAccountSettingDialogComponent', () => {
  let component: SmsAccountSettingDialogComponent;
  let fixture: ComponentFixture<SmsAccountSettingDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsAccountSettingDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsAccountSettingDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
