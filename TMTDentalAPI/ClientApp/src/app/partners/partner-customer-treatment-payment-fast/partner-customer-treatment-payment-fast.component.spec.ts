import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentPaymentFastComponent } from './partner-customer-treatment-payment-fast.component';

describe('PartnerCustomerTreatmentPaymentFastComponent', () => {
  let component: PartnerCustomerTreatmentPaymentFastComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentPaymentFastComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentPaymentFastComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentPaymentFastComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
