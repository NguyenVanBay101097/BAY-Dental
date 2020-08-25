import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementReportDetailComponent } from './commission-settlement-report-detail.component';

describe('CommissionSettlementReportDetailComponent', () => {
  let component: CommissionSettlementReportDetailComponent;
  let fixture: ComponentFixture<CommissionSettlementReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
