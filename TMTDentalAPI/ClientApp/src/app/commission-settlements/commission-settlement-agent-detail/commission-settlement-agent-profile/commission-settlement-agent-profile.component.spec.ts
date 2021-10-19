import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementAgentProfileComponent } from './commission-settlement-agent-profile.component';

describe('CommissionSettlementAgentProfileComponent', () => {
  let component: CommissionSettlementAgentProfileComponent;
  let fixture: ComponentFixture<CommissionSettlementAgentProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementAgentProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementAgentProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
