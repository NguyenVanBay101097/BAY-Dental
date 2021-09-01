import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDashboardApCrChartComponent } from './sale-dashboard-ap-cr-chart.component';

describe('SaleDashboardApCrChartComponent', () => {
  let component: SaleDashboardApCrChartComponent;
  let fixture: ComponentFixture<SaleDashboardApCrChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleDashboardApCrChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleDashboardApCrChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
