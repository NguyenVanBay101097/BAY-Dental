import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentPaymentDialogComponent } from './commission-settlement-agent-payment-dialog.component';

describe('CommissionSettlementAgentPaymentDialogComponent', () => {
  let component: CommissionSettlementAgentPaymentDialogComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
