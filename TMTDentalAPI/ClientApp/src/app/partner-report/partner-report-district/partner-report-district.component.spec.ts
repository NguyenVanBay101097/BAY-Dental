import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportDistrictComponent } from './partner-report-district.component';

describe('PartnerReportDistrictComponent', () => {
  let component: PartnerReportDistrictComponent;
  let fixture: ComponentFixture<PartnerReportDistrictComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportDistrictComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportDistrictComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
