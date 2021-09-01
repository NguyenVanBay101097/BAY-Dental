import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportOverviewComponent } from './partner-report-overview.component';

describe('PartnerReportOverviewComponent', () => {
  let component: PartnerReportOverviewComponent;
  let fixture: ComponentFixture<PartnerReportOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
