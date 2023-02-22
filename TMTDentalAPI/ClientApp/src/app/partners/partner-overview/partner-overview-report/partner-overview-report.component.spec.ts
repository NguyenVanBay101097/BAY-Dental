import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerOverviewReportComponent } from './partner-overview-report.component';

describe('PartnerOverviewReportComponent', () => {
  let component: PartnerOverviewReportComponent;
  let fixture: ComponentFixture<PartnerOverviewReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerOverviewReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerOverviewReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
