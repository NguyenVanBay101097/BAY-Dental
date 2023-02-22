import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryPaymentDialogV2Component } from './salary-payment-dialog-v2.component';

describe('SalaryPaymentDialogV2Component', () => {
  let component: SalaryPaymentDialogV2Component;
  let fixture: ComponentFixture<SalaryPaymentDialogV2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalaryPaymentDialogV2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryPaymentDialogV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
