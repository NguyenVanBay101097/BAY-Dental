import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentReportDetailComponent } from './commission-settlement-agent-report-detail.component';

describe('CommissionSettlementAgentReportDetailComponent', () => {
  let component: CommissionSettlementAgentReportDetailComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
