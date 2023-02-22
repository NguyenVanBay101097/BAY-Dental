import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTreatmentReportFilterPopupComponent } from './partner-treatment-report-filter-popup.component';

describe('PartnerTreatmentReportFilterPopupComponent', () => {
  let component: PartnerTreatmentReportFilterPopupComponent;
  let fixture: ComponentFixture<PartnerTreatmentReportFilterPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerTreatmentReportFilterPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTreatmentReportFilterPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
