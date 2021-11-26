import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderInsurancePaymentDialogComponent } from './sale-order-insurance-payment-dialog.component';

describe('SaleOrderInsurancePaymentDialogComponent', () => {
  let component: SaleOrderInsurancePaymentDialogComponent;
  let fixture: ComponentFixture<SaleOrderInsurancePaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderInsurancePaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderInsurancePaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
