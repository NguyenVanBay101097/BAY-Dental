import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardLaboOrderReportComponent } from './dashboard-labo-order-report.component';

describe('DashboardLaboOrderReportComponent', () => {
  let component: DashboardLaboOrderReportComponent;
  let fixture: ComponentFixture<DashboardLaboOrderReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardLaboOrderReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardLaboOrderReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
