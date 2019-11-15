import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleReportOverviewComponent } from './sale-report-overview.component';

describe('SaleReportOverviewComponent', () => {
  let component: SaleReportOverviewComponent;
  let fixture: ComponentFixture<SaleReportOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleReportOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleReportOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
