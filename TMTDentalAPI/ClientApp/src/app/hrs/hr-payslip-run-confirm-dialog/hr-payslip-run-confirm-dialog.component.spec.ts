import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrPayslipRunConfirmDialogComponent } from './hr-payslip-run-confirm-dialog.component';

describe('HrPayslipRunConfirmDialogComponent', () => {
  let component: HrPayslipRunConfirmDialogComponent;
  let fixture: ComponentFixture<HrPayslipRunConfirmDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrPayslipRunConfirmDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrPayslipRunConfirmDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
