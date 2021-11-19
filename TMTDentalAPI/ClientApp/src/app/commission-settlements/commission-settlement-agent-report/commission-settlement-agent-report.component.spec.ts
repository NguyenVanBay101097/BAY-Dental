import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentReportComponent } from './commission-settlement-agent-report.component';

describe('CommissionSettlementAgentReportComponent', () => {
  let component: CommissionSettlementAgentReportComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
