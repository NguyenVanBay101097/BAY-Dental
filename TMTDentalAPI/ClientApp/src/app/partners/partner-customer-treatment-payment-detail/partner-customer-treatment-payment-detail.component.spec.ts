import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentPaymentDetailComponent } from './partner-customer-treatment-payment-detail.component';

describe('PartnerCustomerTreatmentPaymentDetailComponent', () => {
  let component: PartnerCustomerTreatmentPaymentDetailComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentPaymentDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentPaymentDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentPaymentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
