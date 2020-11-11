import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentHistoryFormComponent } from './partner-customer-treatment-history-form.component';

describe('PartnerCustomerTreatmentHistoryFormComponent', () => {
  let component: PartnerCustomerTreatmentHistoryFormComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentHistoryFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentHistoryFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentHistoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
