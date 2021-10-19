import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentDetailComponent } from './commission-settlement-agent-detail.component';

describe('CommissionSettlementAgentDetailComponent', () => {
  let component: CommissionSettlementAgentDetailComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
