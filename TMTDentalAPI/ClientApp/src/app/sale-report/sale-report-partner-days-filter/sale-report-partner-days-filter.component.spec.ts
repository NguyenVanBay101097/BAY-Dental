import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleReportPartnerDaysFilterComponent } from './sale-report-partner-days-filter.component';

describe('SaleReportPartnerDaysFilterComponent', () => {
  let component: SaleReportPartnerDaysFilterComponent;
  let fixture: ComponentFixture<SaleReportPartnerDaysFilterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleReportPartnerDaysFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleReportPartnerDaysFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
