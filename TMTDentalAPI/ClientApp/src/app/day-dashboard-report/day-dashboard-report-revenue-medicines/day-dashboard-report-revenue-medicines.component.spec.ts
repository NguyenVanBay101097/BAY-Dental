import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DayDashboardReportRevenueMedicinesComponent } from './day-dashboard-report-revenue-medicines.component';

describe('DayDashboardReportRevenueMedicinesComponent', () => {
  let component: DayDashboardReportRevenueMedicinesComponent;
  let fixture: ComponentFixture<DayDashboardReportRevenueMedicinesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DayDashboardReportRevenueMedicinesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DayDashboardReportRevenueMedicinesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
