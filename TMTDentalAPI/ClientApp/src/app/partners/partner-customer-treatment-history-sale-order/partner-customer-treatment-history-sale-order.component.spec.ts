import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentHistorySaleOrderComponent } from './partner-customer-treatment-history-sale-order.component';

describe('PartnerCustomerTreatmentHistorySaleOrderComponent', () => {
  let component: PartnerCustomerTreatmentHistorySaleOrderComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentHistorySaleOrderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentHistorySaleOrderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentHistorySaleOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
