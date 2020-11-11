import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent } from './partner-customer-treatment-history-form-add-service-dialog.component';

describe('PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent', () => {
  let component: PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent;
  let fixture: ComponentFixture<PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
