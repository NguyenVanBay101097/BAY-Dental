import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsMessageDialogComponent } from './sms-message-dialog.component';

describe('SmsMessageDialogComponent', () => {
  let component: SmsMessageDialogComponent;
  let fixture: ComponentFixture<SmsMessageDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsMessageDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsMessageDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
