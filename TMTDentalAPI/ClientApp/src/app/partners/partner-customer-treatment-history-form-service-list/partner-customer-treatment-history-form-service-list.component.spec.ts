import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentHistoryFormServiceListComponent } from './partner-customer-treatment-history-form-service-list.component';

describe('PartnerCustomerTreatmentHistoryFormServiceListComponent', () => {
  let component: PartnerCustomerTreatmentHistoryFormServiceListComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentHistoryFormServiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentHistoryFormServiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentHistoryFormServiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
