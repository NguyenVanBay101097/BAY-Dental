import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportLocationFilterComponent } from './partner-report-location-filter.component';

describe('PartnerReportLocationFilterComponent', () => {
  let component: PartnerReportLocationFilterComponent;
  let fixture: ComponentFixture<PartnerReportLocationFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportLocationFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportLocationFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
