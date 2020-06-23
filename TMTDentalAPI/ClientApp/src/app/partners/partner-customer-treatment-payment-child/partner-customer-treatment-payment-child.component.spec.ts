import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentPaymentChildComponent } from './partner-customer-treatment-payment-child.component';

describe('PartnerCustomerTreatmentPaymentChildComponent', () => {
  let component: PartnerCustomerTreatmentPaymentChildComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentPaymentChildComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentPaymentChildComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentPaymentChildComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
