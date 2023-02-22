import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrSalaryReportListComponent } from './hr-salary-report-list.component';

describe('HrSalaryReportListComponent', () => {
  let component: HrSalaryReportListComponent;
  let fixture: ComponentFixture<HrSalaryReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrSalaryReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrSalaryReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
