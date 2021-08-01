import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsPartnerListDialogComponent } from './sms-partner-list-dialog.component';

describe('SmsPartnerListDialogComponent', () => {
  let component: SmsPartnerListDialogComponent;
  let fixture: ComponentFixture<SmsPartnerListDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsPartnerListDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsPartnerListDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
