import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResInsuranceDebtPaymentDialogComponent } from './res-insurance-debt-payment-dialog.component';

describe('ResInsuranceDebtPaymentDialogComponent', () => {
  let component: ResInsuranceDebtPaymentDialogComponent;
  let fixture: ComponentFixture<ResInsuranceDebtPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResInsuranceDebtPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResInsuranceDebtPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
