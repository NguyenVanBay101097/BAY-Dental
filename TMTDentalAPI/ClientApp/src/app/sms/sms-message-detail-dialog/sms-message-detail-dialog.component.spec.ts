import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsMessageDetailDialogComponent } from './sms-message-detail-dialog.component';

describe('SmsMessageDetailDialogComponent', () => {
  let component: SmsMessageDetailDialogComponent;
  let fixture: ComponentFixture<SmsMessageDetailDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsMessageDetailDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsMessageDetailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
