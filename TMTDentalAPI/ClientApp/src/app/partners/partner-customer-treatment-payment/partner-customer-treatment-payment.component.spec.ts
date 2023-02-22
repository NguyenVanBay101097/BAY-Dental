import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentPaymentComponent } from './partner-customer-treatment-payment.component';

describe('PartnerCustomerTreatmentPaymentComponent', () => {
  let component: PartnerCustomerTreatmentPaymentComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentPaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentPaymentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
