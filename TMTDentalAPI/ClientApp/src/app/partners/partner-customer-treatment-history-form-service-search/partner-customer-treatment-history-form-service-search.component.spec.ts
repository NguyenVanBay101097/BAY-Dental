import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentHistoryFormServiceSearchComponent } from './partner-customer-treatment-history-form-service-search.component';

describe('PartnerCustomerTreatmentHistoryFormServiceSearchComponent', () => {
  let component: PartnerCustomerTreatmentHistoryFormServiceSearchComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentHistoryFormServiceSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentHistoryFormServiceSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentHistoryFormServiceSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
