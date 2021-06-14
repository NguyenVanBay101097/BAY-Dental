import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsMessageStatisticComponent } from './sms-message-statistic.component';

describe('SmsMessageStatisticComponent', () => {
  let component: SmsMessageStatisticComponent;
  let fixture: ComponentFixture<SmsMessageStatisticComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsMessageStatisticComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsMessageStatisticComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
