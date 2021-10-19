import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentComponent } from './commission-settlement-agent.component';

describe('CommissionSettlementAgentComponent', () => {
  let component: CommissionSettlementAgentComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
