import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardReportFormComponent } from './sale-dashboard-report-form.component';

describe('SaleDashboardReportFormComponent', () => {
  let component: SaleDashboardReportFormComponent;
  let fixture: ComponentFixture<SaleDashboardReportFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardReportFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardReportFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
