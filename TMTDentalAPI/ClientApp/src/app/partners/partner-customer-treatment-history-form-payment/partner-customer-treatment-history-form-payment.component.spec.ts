import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentHistoryFormPaymentComponent } from './partner-customer-treatment-history-form-payment.component';

describe('PartnerCustomerTreatmentHistoryFormPaymentComponent', () => {
  let component: PartnerCustomerTreatmentHistoryFormPaymentComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentHistoryFormPaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentHistoryFormPaymentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentHistoryFormPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
