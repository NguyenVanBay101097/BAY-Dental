import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrSalaryReportDetailComponent } from './hr-salary-report-detail.component';

describe('HrSalaryReportDetailComponent', () => {
  let component: HrSalaryReportDetailComponent;
  let fixture: ComponentFixture<HrSalaryReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrSalaryReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrSalaryReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
