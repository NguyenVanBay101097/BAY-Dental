import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsComfirmDialogComponent } from './sms-comfirm-dialog.component';

describe('SmsComfirmDialogComponent', () => {
  let component: SmsComfirmDialogComponent;
  let fixture: ComponentFixture<SmsComfirmDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsComfirmDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsComfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
