import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTreatmentReportComponent } from './partner-treatment-report.component';

describe('PartnerTreatmentReportComponent', () => {
  let component: PartnerTreatmentReportComponent;
  let fixture: ComponentFixture<PartnerTreatmentReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerTreatmentReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTreatmentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
