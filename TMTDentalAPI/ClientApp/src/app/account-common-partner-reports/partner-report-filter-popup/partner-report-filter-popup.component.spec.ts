import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportFilterPopupComponent } from './partner-report-filter-popup.component';

describe('PartnerReportFilterPopupComponent', () => {
  let component: PartnerReportFilterPopupComponent;
  let fixture: ComponentFixture<PartnerReportFilterPopupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportFilterPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportFilterPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
