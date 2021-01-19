import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierFormDebitPaymentDialogComponent } from './partner-supplier-form-debit-payment-dialog.component';

describe('PartnerSupplierFormDebitPaymentDialogComponent', () => {
  let component: PartnerSupplierFormDebitPaymentDialogComponent;
  let fixture: ComponentFixture<PartnerSupplierFormDebitPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierFormDebitPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierFormDebitPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
