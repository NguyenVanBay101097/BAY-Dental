import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentReportOverviewComponent } from './commission-settlement-agent-report-overview.component';

describe('CommissionSettlementAgentReportOverviewComponent', () => {
  let component: CommissionSettlementAgentReportOverviewComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentReportOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentReportOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentReportOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
