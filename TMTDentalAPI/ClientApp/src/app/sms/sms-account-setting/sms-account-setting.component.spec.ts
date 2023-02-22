import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsAccountSettingComponent } from './sms-account-setting.component';

describe('SmsAccountSettingComponent', () => {
  let component: SmsAccountSettingComponent;
  let fixture: ComponentFixture<SmsAccountSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsAccountSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsAccountSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
