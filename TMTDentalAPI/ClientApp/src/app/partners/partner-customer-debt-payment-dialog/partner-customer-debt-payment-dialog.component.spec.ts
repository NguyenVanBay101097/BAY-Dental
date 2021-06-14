import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerDebtPaymentDialogComponent } from './partner-customer-debt-payment-dialog.component';

describe('PartnerCustomerDebtPaymentDialogComponent', () => {
  let component: PartnerCustomerDebtPaymentDialogComponent;
  let fixture: ComponentFixture<PartnerCustomerDebtPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerDebtPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerDebtPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
